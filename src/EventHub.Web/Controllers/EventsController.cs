using EventHub.Domain.DTO;
using EventHub.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace EventHub.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly IEventService _eventService;

    public EventsController(IEventService eventService)
    {
        _eventService = eventService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EventDto>>> GetAll()
    {
        var events = await _eventService.GetAllEventsAsync();
        return Ok(events);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EventDto>> GetById(int id)
    {
        var eventDto = await _eventService.GetEventByIdAsync(id);
        if (eventDto == null)
        {
            return NotFound($"Event with ID {id} not found.");
        }
        return Ok(eventDto);
    }

    [HttpGet("city/{city}")]
    public async Task<ActionResult<IEnumerable<EventDto>>> GetByCity(string city)
    {
        var events = await _eventService.GetEventsByCityAsync(city);
        return Ok(events);
    }

    [HttpGet("upcoming")]
    public async Task<ActionResult<IEnumerable<EventDto>>> GetUpcoming()
    {
        var events = await _eventService.GetUpcomingEventsAsync();
        return Ok(events);
    }

    [HttpPost]
    public async Task<ActionResult<EventDto>> Create([FromBody] CreateEventDto createEventDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var createdEvent = await _eventService.CreateEventAsync(createEventDto);
        return CreatedAtAction(nameof(GetById), new { id = createdEvent.Id }, createdEvent);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<EventDto>> Update(int id, [FromBody] UpdateEventDto updateEventDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var updatedEvent = await _eventService.UpdateEventAsync(id, updateEventDto);
            return Ok(updatedEvent);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            await _eventService.DeleteEventAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
