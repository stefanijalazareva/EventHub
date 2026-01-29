using EventHub.Domain.DTO;

namespace EventHub.Service.Interface;

public interface IExternalEventService
{
    Task<CityEventsResponseDto> GetEventsByCityAsync(string city);
}
