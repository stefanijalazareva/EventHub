namespace EventHub.Domain.DTO;

public class ExternalEventDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Link { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
}

public class CityEventsResponseDto
{
    public string City { get; set; } = string.Empty;
    public List<ExternalEventDto> Events { get; set; } = new();
    public int TotalEvents { get; set; }
}
