namespace EventHub.Domain.DomainModels;

public class Ticket
{
    public int Id { get; set; }
    public string TicketNumber { get; set; } = string.Empty;
    public int EventId { get; set; }
    public int AttendeeId { get; set; }
    public decimal Price { get; set; }
    public DateTime PurchaseDate { get; set; }
    public TicketStatus Status { get; set; }
    public string? SeatNumber { get; set; }
    public string? QRCode { get; set; }

    // Navigation properties
    public Event Event { get; set; } = null!;
    public Attendee Attendee { get; set; } = null!;
}

public enum TicketStatus
{
    Active,
    Used,
    Cancelled,
    Refunded
}
