using EventHub.Domain.DTO;
using EventHub.Service.Interface;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace EventHub.Repository.ExternalServices;

public class SerpApiEventService : IExternalEventService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private const string BaseUrl = "https://serpapi.com/search.json";

    public SerpApiEventService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["SerpApi:ApiKey"] ?? throw new InvalidOperationException("SerpApi:ApiKey not configured");
    }

    public async Task<CityEventsResponseDto> GetEventsByCityAsync(string city)
    {
        try
        {
            var url = $"{BaseUrl}?engine=google_events&q=Events+in+{Uri.EscapeDataString(city)}&api_key={_apiKey}";
            
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var serpApiResponse = JsonSerializer.Deserialize<SerpApiResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var cityEventsResponse = new CityEventsResponseDto
            {
                City = city,
                Events = new List<ExternalEventDto>()
            };

            if (serpApiResponse?.EventsResults != null)
            {
                foreach (var eventResult in serpApiResponse.EventsResults)
                {
                    cityEventsResponse.Events.Add(new ExternalEventDto
                    {
                        Title = eventResult.Title ?? "Untitled Event",
                        Description = eventResult.Description ?? string.Empty,
                        Date = FormatDate(eventResult.Date),
                        Location = eventResult.Address?.FirstOrDefault() ?? city,
                        Link = eventResult.Link ?? string.Empty,
                        ThumbnailUrl = eventResult.Thumbnail
                    });
                }
            }

            cityEventsResponse.TotalEvents = cityEventsResponse.Events.Count;
            return cityEventsResponse;
        }
        catch (HttpRequestException ex)
        {
            throw new InvalidOperationException($"Failed to retrieve events from SerpAPI: {ex.Message}", ex);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException($"Failed to parse SerpAPI response: {ex.Message}", ex);
        }
    }

    private static string FormatDate(SerpApiDate? date)
    {
        if (date == null)
            return "Date not available";

        var parts = new List<string>();
        
        if (!string.IsNullOrEmpty(date.StartDate))
            parts.Add($"From: {date.StartDate}");
        
        if (!string.IsNullOrEmpty(date.When))
            parts.Add(date.When);

        return parts.Count > 0 ? string.Join(" - ", parts) : "Date not available";
    }

    // SerpAPI response models
    private class SerpApiResponse
    {
        public List<EventResult>? EventsResults { get; set; }
    }

    private class EventResult
    {
        public string? Title { get; set; }
        public SerpApiDate? Date { get; set; }
        public List<string>? Address { get; set; }
        public string? Link { get; set; }
        public string? Description { get; set; }
        public string? Thumbnail { get; set; }
    }

    private class SerpApiDate
    {
        public string? StartDate { get; set; }
        public string? When { get; set; }
    }
}
