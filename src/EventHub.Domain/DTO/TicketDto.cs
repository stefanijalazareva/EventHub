using EventHub.Domain.DomainModels;

namespace EventHub.Domain.DTO;

public class TicketDto
{
    public int Id { get; set; }
    public string TicketNumber { get; set; } = string.Empty;
    public int EventId { get; set; }
    public string EventName { get; set; } = string.Empty;
    public int AttendeeId { get; set; }
    public string AttendeeName { get; set; } = string.Empty;
    public string AttendeeEmail { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime PurchaseDate { get; set; }
    public TicketStatus Status { get; set; }
    public string? SeatNumber { get; set; }
}

public class BookTicketDto
{
    public int EventId { get; set; }
    public int AttendeeId { get; set; }
    public int Quantity { get; set; }
    public string? SeatNumber { get; set; }
}
