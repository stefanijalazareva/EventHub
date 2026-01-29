using EventHub.Domain.DTO;

namespace EventHub.Service.Interface;

public interface ITicketService
{
    Task<IEnumerable<TicketDto>> GetAllTicketsAsync();
    Task<TicketDto?> GetTicketByIdAsync(int id);
    Task<IEnumerable<TicketDto>> GetTicketsByEventAsync(int eventId);
    Task<IEnumerable<TicketDto>> GetTicketsByAttendeeAsync(int attendeeId);
    Task<IEnumerable<TicketDto>> BookTicketsAsync(BookTicketDto bookTicketDto);
    Task CancelTicketAsync(int id);
}
