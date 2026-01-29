using EventHub.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace EventHub.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherController : ControllerBase
{
    private readonly IWeatherService _weatherService;

    public WeatherController(IWeatherService weatherService)
    {
        _weatherService = weatherService;
    }

    /// <summary>
    /// Get weather forecast for a specific event location and date
    /// </summary>
    /// <param name="eventId">The ID of the event</param>
    /// <returns>Weather information transformed and tailored for the event</returns>
    [HttpGet("event/{eventId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetWeatherForEvent(int eventId)
    {
        var weather = await _weatherService.GetWeatherForEventAsync(eventId);
        
        if (weather == null)
        {
            return NotFound(new { message = $"Could not retrieve weather data for event {eventId}" });
        }

        return Ok(weather);
    }
}
