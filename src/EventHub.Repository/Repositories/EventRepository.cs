using EventHub.Domain.DomainModels;
using EventHub.Domain.Interfaces;
using EventHub.Repository.Data;
using Microsoft.EntityFrameworkCore;

namespace EventHub.Repository.Implementation;

public class EventRepository : Repository<Event>, IEventRepository
{
    public EventRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Event>> GetEventsByCityAsync(string city)
    {
        return await _dbSet
            .Where(e => e.City.ToLower() == city.ToLower())
            .ToListAsync();
    }

    public async Task<IEnumerable<Event>> GetUpcomingEventsAsync()
    {
        return await _dbSet
            .Where(e => e.StartDate > DateTime.UtcNow)
            .OrderBy(e => e.StartDate)
            .ToListAsync();
    }

    public async Task<Event?> GetEventWithDetailsAsync(int id)
    {
        return await _dbSet
            .Include(e => e.Tickets)
            .Include(e => e.Schedules)
            .FirstOrDefaultAsync(e => e.Id == id);
    }
}
