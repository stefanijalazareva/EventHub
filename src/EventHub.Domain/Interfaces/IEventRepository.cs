using EventHub.Domain.DomainModels;

namespace EventHub.Domain.Interfaces;

public interface IEventRepository : IRepository<Event>
{
    Task<IEnumerable<Event>> GetEventsByCityAsync(string city);
    Task<IEnumerable<Event>> GetUpcomingEventsAsync();
    Task<Event?> GetEventWithDetailsAsync(int id);
}
