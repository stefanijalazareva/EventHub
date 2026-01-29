using EventHub.Domain.DTO;
using EventHub.Service.Interface;
using EventHub.Domain.DomainModels;
using EventHub.Domain.Interfaces;

namespace EventHub.Service.Implementation;

public class EventService : IEventService
{
    private readonly IUnitOfWork _unitOfWork;

    public EventService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<EventDto>> GetAllEventsAsync()
    {
        var events = await _unitOfWork.Events.GetAllAsync();
        return events.Select(MapToDto);
    }

    public async Task<EventDto?> GetEventByIdAsync(int id)
    {
        var eventEntity = await _unitOfWork.Events.GetByIdAsync(id);
        return eventEntity != null ? MapToDto(eventEntity) : null;
    }

    public async Task<EventDto> CreateEventAsync(CreateEventDto createEventDto)
    {
        var eventEntity = new Event
        {
            Name = createEventDto.Name,
            Description = createEventDto.Description,
            Location = createEventDto.Location,
            City = createEventDto.City,
            StartDate = createEventDto.StartDate,
            EndDate = createEventDto.EndDate,
            Organizer = createEventDto.Organizer,
            Category = createEventDto.Category,
            TotalTickets = createEventDto.TotalTickets,
            AvailableTickets = createEventDto.TotalTickets,
            TicketPrice = createEventDto.TicketPrice,
            ImageUrl = createEventDto.ImageUrl,
            CreatedAt = DateTime.UtcNow
        };

        var createdEvent = await _unitOfWork.Events.AddAsync(eventEntity);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(createdEvent);
    }

    public async Task<EventDto> UpdateEventAsync(int id, UpdateEventDto updateEventDto)
    {
        var eventEntity = await _unitOfWork.Events.GetByIdAsync(id);
        if (eventEntity == null)
        {
            throw new KeyNotFoundException($"Event with ID {id} not found.");
        }

        eventEntity.Name = updateEventDto.Name;
        eventEntity.Description = updateEventDto.Description;
        eventEntity.Location = updateEventDto.Location;
        eventEntity.City = updateEventDto.City;
        eventEntity.StartDate = updateEventDto.StartDate;
        eventEntity.EndDate = updateEventDto.EndDate;
        eventEntity.Organizer = updateEventDto.Organizer;
        eventEntity.Category = updateEventDto.Category;
        eventEntity.TotalTickets = updateEventDto.TotalTickets;
        eventEntity.TicketPrice = updateEventDto.TicketPrice;
        eventEntity.ImageUrl = updateEventDto.ImageUrl;
        eventEntity.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Events.UpdateAsync(eventEntity);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(eventEntity);
    }

    public async Task DeleteEventAsync(int id)
    {
        await _unitOfWork.Events.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<EventDto>> GetEventsByCityAsync(string city)
    {
        var events = await _unitOfWork.Events.GetEventsByCityAsync(city);
        return events.Select(MapToDto);
    }

    public async Task<IEnumerable<EventDto>> GetUpcomingEventsAsync()
    {
        var events = await _unitOfWork.Events.GetUpcomingEventsAsync();
        return events.Select(MapToDto);
    }

    private static EventDto MapToDto(Event eventEntity)
    {
        return new EventDto
        {
            Id = eventEntity.Id,
            Name = eventEntity.Name,
            Description = eventEntity.Description,
            Location = eventEntity.Location,
            City = eventEntity.City,
            StartDate = eventEntity.StartDate,
            EndDate = eventEntity.EndDate,
            Organizer = eventEntity.Organizer,
            Category = eventEntity.Category,
            TotalTickets = eventEntity.TotalTickets,
            AvailableTickets = eventEntity.AvailableTickets,
            TicketPrice = eventEntity.TicketPrice,
            ImageUrl = eventEntity.ImageUrl,
            CreatedAt = eventEntity.CreatedAt,
            UpdatedAt = eventEntity.UpdatedAt
        };
    }
}
