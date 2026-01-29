using EventHub.Domain.DTO;

namespace EventHub.Service.Interface;

public interface IWeatherService
{
    Task<WeatherDto?> GetWeatherForEventAsync(int eventId);
}
