namespace EventHub.Domain.DTO;

public class WeatherDto
{
    public string EventName { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public double Temperature { get; set; }
    public double FeelsLike { get; set; }
    public string WeatherCondition { get; set; } = string.Empty;
    public double Humidity { get; set; }
    public double WindSpeed { get; set; }
    public double PrecipitationChance { get; set; }
    public string Recommendation { get; set; } = string.Empty;
}
