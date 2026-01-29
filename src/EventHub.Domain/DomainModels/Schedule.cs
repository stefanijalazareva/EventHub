namespace EventHub.Domain.DomainModels;

public class Schedule
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Location { get; set; } = string.Empty;
    public string? Speaker { get; set; }

    // Navigation properties
    public Event Event { get; set; } = null!;
}
