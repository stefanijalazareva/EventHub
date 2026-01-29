using EventHub.Domain.DTO;
using EventHub.Domain.Interfaces;
using EventHub.Service.Interface;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace EventHub.Service.Implementation;

public class WeatherService : IWeatherService
{
    private readonly IEventRepository _eventRepository;
    private readonly HttpClient _httpClient;
    private readonly ILogger<WeatherService> _logger;

    // City coordinates for Macedonian cities
    private readonly Dictionary<string, (double Lat, double Lon)> _cityCoordinates = new()
    {
        { "Skopje", (41.9973, 21.4280) },
        { "Bitola", (41.0297, 21.3347) },
        { "Ohrid", (41.1172, 20.8017) },
        { "Tetovo", (42.0089, 20.9714) },
        { "Prilep", (41.3453, 21.5547) },
        { "Kumanovo", (42.1322, 21.7144) },
        { "Veles", (41.7158, 21.7758) },
        { "Strumica", (41.4378, 22.6433) }
    };

    public WeatherService(IEventRepository eventRepository, HttpClient httpClient, ILogger<WeatherService> logger)
    {
        _eventRepository = eventRepository;
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<WeatherDto?> GetWeatherForEventAsync(int eventId)
    {
        try
        {
            var eventEntity = await _eventRepository.GetByIdAsync(eventId);
            if (eventEntity == null)
            {
                _logger.LogWarning("Event with ID {EventId} not found", eventId);
                return null;
            }

            // Get coordinates for the city
            if (!_cityCoordinates.TryGetValue(eventEntity.City, out var coordinates))
            {
                // Default to Skopje if city not found
                _logger.LogWarning("City {City} not found in coordinates, defaulting to Skopje", eventEntity.City);
                coordinates = _cityCoordinates["Skopje"];
            }

            // Call Open-Meteo API
            var url = $"https://api.open-meteo.com/v1/forecast?" +
                     $"latitude={coordinates.Lat}&longitude={coordinates.Lon}" +
                     $"&hourly=temperature_2m,relative_humidity_2m,apparent_temperature,precipitation_probability,wind_speed_10m,weather_code" +
                     $"&timezone=Europe/Skopje" +
                     $"&forecast_days=16";

            var response = await _httpClient.GetAsync(url);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to fetch weather data: {StatusCode}", response.StatusCode);
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            var weatherData = JsonSerializer.Deserialize<OpenMeteoResponse>(content, new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true 
            });

            if (weatherData?.Hourly == null)
            {
                _logger.LogError("Invalid weather data received");
                return null;
            }

            // Find the closest hour to the event start time
            var eventDateTime = eventEntity.StartDate;
            var hourIndex = FindClosestHourIndex(weatherData.Hourly.Time, eventDateTime);

            if (hourIndex == -1)
            {
                _logger.LogWarning("Could not find weather data for event date {EventDate}", eventDateTime);
                return null;
            }

            // Transform the data
            var temperature = weatherData.Hourly.Temperature2m[hourIndex];
            var feelsLike = weatherData.Hourly.ApparentTemperature[hourIndex];
            var humidity = weatherData.Hourly.RelativeHumidity2m[hourIndex];
            var precipChance = weatherData.Hourly.PrecipitationProbability[hourIndex];
            var windSpeed = weatherData.Hourly.WindSpeed10m[hourIndex];
            var weatherCode = weatherData.Hourly.WeatherCode[hourIndex];

            var weatherDto = new WeatherDto
            {
                EventName = eventEntity.Name,
                City = eventEntity.City,
                EventDate = eventDateTime,
                Temperature = Math.Round(temperature, 1),
                FeelsLike = Math.Round(feelsLike, 1),
                WeatherCondition = GetWeatherDescription(weatherCode),
                Humidity = Math.Round(humidity, 1),
                WindSpeed = Math.Round(windSpeed, 1),
                PrecipitationChance = Math.Round(precipChance, 1),
                Recommendation = GenerateRecommendation(temperature, precipChance, windSpeed, eventEntity.Name)
            };

            return weatherDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting weather for event {EventId}", eventId);
            return null;
        }
    }

    private int FindClosestHourIndex(List<string> times, DateTime targetDateTime)
    {
        for (int i = 0; i < times.Count; i++)
        {
            if (DateTime.TryParse(times[i], out var time))
            {
                if (time.Date == targetDateTime.Date && time.Hour == targetDateTime.Hour)
                {
                    return i;
                }
            }
        }

        // If exact match not found, find closest date
        for (int i = 0; i < times.Count; i++)
        {
            if (DateTime.TryParse(times[i], out var time))
            {
                if (time.Date == targetDateTime.Date)
                {
                    return i;
                }
            }
        }

        return -1;
    }

    private string GetWeatherDescription(int code)
    {
        return code switch
        {
            0 => "Clear sky",
            1 or 2 or 3 => "Partly cloudy",
            45 or 48 => "Foggy",
            51 or 53 or 55 => "Drizzle",
            61 or 63 or 65 => "Rain",
            71 or 73 or 75 => "Snow",
            77 => "Snow grains",
            80 or 81 or 82 => "Rain showers",
            85 or 86 => "Snow showers",
            95 => "Thunderstorm",
            96 or 99 => "Thunderstorm with hail",
            _ => "Unknown"
        };
    }

    private string GenerateRecommendation(double temp, double precip, double wind, string eventName)
    {
        var recommendations = new List<string>();

        if (precip > 70)
            recommendations.Add("High chance of rain - bring an umbrella!");
        else if (precip > 40)
            recommendations.Add("Possible rain - consider bringing rain protection");

        if (temp < 5)
            recommendations.Add("Very cold weather - dress warmly with layers");
        else if (temp < 15)
            recommendations.Add("Cool weather - bring a jacket");
        else if (temp > 30)
            recommendations.Add("Very hot weather - stay hydrated and use sunscreen");
        else if (temp > 25)
            recommendations.Add("Warm weather - light clothing recommended");

        if (wind > 30)
            recommendations.Add("Very windy conditions - secure loose items");
        else if (wind > 20)
            recommendations.Add("Windy conditions expected");

        if (recommendations.Count == 0)
            recommendations.Add("Perfect weather conditions for the event!");

        return string.Join(". ", recommendations) + ".";
    }

    // DTOs for Open-Meteo API response
    private class OpenMeteoResponse
    {
        public HourlyData? Hourly { get; set; }
    }

    private class HourlyData
    {
        public List<string> Time { get; set; } = new();
        public List<double> Temperature2m { get; set; } = new();
        public List<double> RelativeHumidity2m { get; set; } = new();
        public List<double> ApparentTemperature { get; set; } = new();
        public List<double> PrecipitationProbability { get; set; } = new();
        public List<double> WindSpeed10m { get; set; } = new();
        public List<int> WeatherCode { get; set; } = new();
    }
}
