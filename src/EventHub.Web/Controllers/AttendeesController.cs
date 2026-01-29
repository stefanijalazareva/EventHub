using EventHub.Domain.DTO;
using EventHub.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace EventHub.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AttendeesController : ControllerBase
{
    private readonly IAttendeeService _attendeeService;

    public AttendeesController(IAttendeeService attendeeService)
    {
        _attendeeService = attendeeService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AttendeeDto>>> GetAll()
    {
        var attendees = await _attendeeService.GetAllAttendeesAsync();
        return Ok(attendees);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AttendeeDto>> GetById(int id)
    {
        var attendee = await _attendeeService.GetAttendeeByIdAsync(id);
        if (attendee == null)
        {
            return NotFound($"Attendee with ID {id} not found.");
        }
        return Ok(attendee);
    }

    [HttpPost]
    public async Task<ActionResult<AttendeeDto>> Create([FromBody] CreateAttendeeDto createAttendeeDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var createdAttendee = await _attendeeService.CreateAttendeeAsync(createAttendeeDto);
        return CreatedAtAction(nameof(GetById), new { id = createdAttendee.Id }, createdAttendee);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<AttendeeDto>> Update(int id, [FromBody] UpdateAttendeeDto updateAttendeeDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var updatedAttendee = await _attendeeService.UpdateAttendeeAsync(id, updateAttendeeDto);
            return Ok(updatedAttendee);
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
            await _attendeeService.DeleteAttendeeAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
