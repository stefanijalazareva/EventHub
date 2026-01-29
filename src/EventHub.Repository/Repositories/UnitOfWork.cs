using EventHub.Domain.Interfaces;
using EventHub.Repository.Data;

namespace EventHub.Repository.Implementation;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public UnitOfWork(
        ApplicationDbContext context,
        IEventRepository events,
        IAttendeeRepository attendees,
        ITicketRepository tickets,
        IScheduleRepository schedules)
    {
        _context = context;
        Events = events;
        Attendees = attendees;
        Tickets = tickets;
        Schedules = schedules;
    }

    public IEventRepository Events { get; }
    public IAttendeeRepository Attendees { get; }
    public ITicketRepository Tickets { get; }
    public IScheduleRepository Schedules { get; }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
