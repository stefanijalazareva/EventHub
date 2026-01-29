namespace EventHub.Domain.DomainModels;

public class Event
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Organizer { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int TotalTickets { get; set; }
    public int AvailableTickets { get; set; }
    public decimal TicketPrice { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    public ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}
