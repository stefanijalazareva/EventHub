namespace EventHub.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IEventRepository Events { get; }
    IAttendeeRepository Attendees { get; }
    ITicketRepository Tickets { get; }
    IScheduleRepository Schedules { get; }
    Task<int> SaveChangesAsync();
}
