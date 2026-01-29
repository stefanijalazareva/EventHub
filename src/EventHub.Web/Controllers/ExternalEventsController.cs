using EventHub.Domain.DTO;
using EventHub.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace EventHub.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExternalEventsController : ControllerBase
{
    private readonly IExternalEventService _externalEventService;

    public ExternalEventsController(IExternalEventService externalEventService)
    {
        _externalEventService = externalEventService;
    }

    [HttpGet("city/{city}")]
    public async Task<ActionResult<CityEventsResponseDto>> GetEventsByCity(string city)
    {
        try
        {
            var events = await _externalEventService.GetEventsByCityAsync(city);
            return Ok(events);
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}
