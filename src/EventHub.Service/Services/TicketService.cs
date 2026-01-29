using EventHub.Domain.DTO;
using EventHub.Service.Interface;
using EventHub.Domain.DomainModels;
using EventHub.Domain.Interfaces;

namespace EventHub.Service.Implementation;

public class TicketService : ITicketService
{
    private readonly IUnitOfWork _unitOfWork;

    public TicketService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<TicketDto>> GetAllTicketsAsync()
    {
        var tickets = await _unitOfWork.Tickets.GetAllAsync();
        return tickets.Select(MapToDto);
    }

    public async Task<TicketDto?> GetTicketByIdAsync(int id)
    {
        var ticket = await _unitOfWork.Tickets.GetByIdAsync(id);
        return ticket != null ? MapToDto(ticket) : null;
    }

    public async Task<IEnumerable<TicketDto>> GetTicketsByEventAsync(int eventId)
    {
        var tickets = await _unitOfWork.Tickets.GetTicketsByEventAsync(eventId);
        return tickets.Select(MapToDto);
    }

    public async Task<IEnumerable<TicketDto>> GetTicketsByAttendeeAsync(int attendeeId)
    {
        var tickets = await _unitOfWork.Tickets.GetTicketsByAttendeeAsync(attendeeId);
        return tickets.Select(MapToDto);
    }

    public async Task<IEnumerable<TicketDto>> BookTicketsAsync(BookTicketDto bookTicketDto)
    {
        // Validate event exists
        var eventEntity = await _unitOfWork.Events.GetByIdAsync(bookTicketDto.EventId);
        if (eventEntity == null)
        {
            throw new KeyNotFoundException($"Event with ID {bookTicketDto.EventId} not found.");
        }

        // Validate attendee exists
        var attendee = await _unitOfWork.Attendees.GetByIdAsync(bookTicketDto.AttendeeId);
        if (attendee == null)
        {
            throw new KeyNotFoundException($"Attendee with ID {bookTicketDto.AttendeeId} not found.");
        }

        // Validate ticket availability
        if (eventEntity.AvailableTickets < bookTicketDto.Quantity)
        {
            throw new InvalidOperationException(
                $"Not enough tickets available. Requested: {bookTicketDto.Quantity}, Available: {eventEntity.AvailableTickets}");
        }

        var bookedTickets = new List<Ticket>();

        // Create tickets
        for (int i = 0; i < bookTicketDto.Quantity; i++)
        {
            var ticket = new Ticket
            {
                TicketNumber = GenerateTicketNumber(),
                EventId = bookTicketDto.EventId,
                AttendeeId = bookTicketDto.AttendeeId,
                Price = eventEntity.TicketPrice,
                PurchaseDate = DateTime.UtcNow,
                Status = TicketStatus.Active,
                SeatNumber = bookTicketDto.SeatNumber,
                QRCode = Guid.NewGuid().ToString()
            };

            var createdTicket = await _unitOfWork.Tickets.AddAsync(ticket);
            bookedTickets.Add(createdTicket);
        }

        // Update available tickets
        eventEntity.AvailableTickets -= bookTicketDto.Quantity;
        eventEntity.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.Events.UpdateAsync(eventEntity);

        await _unitOfWork.SaveChangesAsync();

        return bookedTickets.Select(MapToDto);
    }

    public async Task CancelTicketAsync(int id)
    {
        var ticket = await _unitOfWork.Tickets.GetByIdAsync(id);
        if (ticket == null)
        {
            throw new KeyNotFoundException($"Ticket with ID {id} not found.");
        }

        if (ticket.Status == TicketStatus.Cancelled)
        {
            throw new InvalidOperationException("Ticket is already cancelled.");
        }

        ticket.Status = TicketStatus.Cancelled;
        await _unitOfWork.Tickets.UpdateAsync(ticket);

        // Increase available tickets
        var eventEntity = await _unitOfWork.Events.GetByIdAsync(ticket.EventId);
        if (eventEntity != null)
        {
            eventEntity.AvailableTickets++;
            eventEntity.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Events.UpdateAsync(eventEntity);
        }

        await _unitOfWork.SaveChangesAsync();
    }

    private static string GenerateTicketNumber()
    {
        return $"TKT-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
    }

    private static TicketDto MapToDto(Ticket ticket)
    {
        return new TicketDto
        {
            Id = ticket.Id,
            TicketNumber = ticket.TicketNumber,
            EventId = ticket.EventId,
            EventName = ticket.Event?.Name ?? string.Empty,
            AttendeeId = ticket.AttendeeId,
            AttendeeName = ticket.Attendee != null
                ? $"{ticket.Attendee.FirstName} {ticket.Attendee.LastName}"
                : string.Empty,
            AttendeeEmail = ticket.Attendee?.Email ?? string.Empty,
            Price = ticket.Price,
            PurchaseDate = ticket.PurchaseDate,
            Status = ticket.Status,
            SeatNumber = ticket.SeatNumber
        };
    }
}
