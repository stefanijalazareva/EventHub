using EventHub.Domain.DomainModels;

namespace EventHub.Domain.Interfaces;

public interface ITicketRepository : IRepository<Ticket>
{
    Task<IEnumerable<Ticket>> GetTicketsByEventAsync(int eventId);
    Task<IEnumerable<Ticket>> GetTicketsByAttendeeAsync(int attendeeId);
    Task<Ticket?> GetByTicketNumberAsync(string ticketNumber);
}
