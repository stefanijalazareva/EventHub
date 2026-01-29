using EventHub.Domain.DTO;

namespace EventHub.Service.Interface;

public interface IEventService
{
    Task<IEnumerable<EventDto>> GetAllEventsAsync();
    Task<EventDto?> GetEventByIdAsync(int id);
    Task<EventDto> CreateEventAsync(CreateEventDto createEventDto);
    Task<EventDto> UpdateEventAsync(int id, UpdateEventDto updateEventDto);
    Task DeleteEventAsync(int id);
    Task<IEnumerable<EventDto>> GetEventsByCityAsync(string city);
    Task<IEnumerable<EventDto>> GetUpcomingEventsAsync();
}
