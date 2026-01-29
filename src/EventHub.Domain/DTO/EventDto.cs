namespace EventHub.Domain.DTO;

public class EventDto
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
}

public class CreateEventDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Organizer { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int TotalTickets { get; set; }
    public decimal TicketPrice { get; set; }
    public string? ImageUrl { get; set; }
}

public class UpdateEventDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Organizer { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int TotalTickets { get; set; }
    public decimal TicketPrice { get; set; }
    public string? ImageUrl { get; set; }
}
