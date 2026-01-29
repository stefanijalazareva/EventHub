using EventHub.Domain.DomainModels;
using EventHub.Domain.Interfaces;
using EventHub.Repository.Data;
using Microsoft.EntityFrameworkCore;

namespace EventHub.Repository.Implementation;

public class TicketRepository : Repository<Ticket>, ITicketRepository
{
    public TicketRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Ticket>> GetTicketsByEventAsync(int eventId)
    {
        return await _dbSet
            .Include(t => t.Event)
            .Include(t => t.Attendee)
            .Where(t => t.EventId == eventId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Ticket>> GetTicketsByAttendeeAsync(int attendeeId)
    {
        return await _dbSet
            .Include(t => t.Event)
            .Include(t => t.Attendee)
            .Where(t => t.AttendeeId == attendeeId)
            .ToListAsync();
    }

    public async Task<Ticket?> GetByTicketNumberAsync(string ticketNumber)
    {
        return await _dbSet
            .Include(t => t.Event)
            .Include(t => t.Attendee)
            .FirstOrDefaultAsync(t => t.TicketNumber == ticketNumber);
    }

    public override async Task<IEnumerable<Ticket>> GetAllAsync()
    {
        return await _dbSet
            .Include(t => t.Event)
            .Include(t => t.Attendee)
            .ToListAsync();
    }

    public override async Task<Ticket?> GetByIdAsync(int id)
    {
        return await _dbSet
            .Include(t => t.Event)
            .Include(t => t.Attendee)
            .FirstOrDefaultAsync(t => t.Id == id);
    }
}
