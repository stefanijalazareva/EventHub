using EventHub.Domain.DTO;
using EventHub.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace EventHub.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SchedulesController : ControllerBase
{
    private readonly IScheduleService _scheduleService;

    public SchedulesController(IScheduleService scheduleService)
    {
        _scheduleService = scheduleService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ScheduleDto>>> GetAll()
    {
        var schedules = await _scheduleService.GetAllSchedulesAsync();
        return Ok(schedules);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ScheduleDto>> GetById(int id)
    {
        var schedule = await _scheduleService.GetScheduleByIdAsync(id);
        if (schedule == null)
        {
            return NotFound($"Schedule with ID {id} not found.");
        }
        return Ok(schedule);
    }

    [HttpGet("event/{eventId}")]
    public async Task<ActionResult<IEnumerable<ScheduleDto>>> GetByEvent(int eventId)
    {
        var schedules = await _scheduleService.GetSchedulesByEventAsync(eventId);
        return Ok(schedules);
    }

    [HttpPost]
    public async Task<ActionResult<ScheduleDto>> Create([FromBody] CreateScheduleDto createScheduleDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var createdSchedule = await _scheduleService.CreateScheduleAsync(createScheduleDto);
            return CreatedAtAction(nameof(GetById), new { id = createdSchedule.Id }, createdSchedule);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ScheduleDto>> Update(int id, [FromBody] UpdateScheduleDto updateScheduleDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var updatedSchedule = await _scheduleService.UpdateScheduleAsync(id, updateScheduleDto);
            return Ok(updatedSchedule);
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
            await _scheduleService.DeleteScheduleAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
