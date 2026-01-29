using EventHub.Domain.DTO;
using EventHub.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace EventHub.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketsController : ControllerBase
{
    private readonly ITicketService _ticketService;

    public TicketsController(ITicketService ticketService)
    {
        _ticketService = ticketService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TicketDto>>> GetAll()
    {
        var tickets = await _ticketService.GetAllTicketsAsync();
        return Ok(tickets);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TicketDto>> GetById(int id)
    {
        var ticket = await _ticketService.GetTicketByIdAsync(id);
        if (ticket == null)
        {
            return NotFound($"Ticket with ID {id} not found.");
        }
        return Ok(ticket);
    }

    [HttpGet("event/{eventId}")]
    public async Task<ActionResult<IEnumerable<TicketDto>>> GetByEvent(int eventId)
    {
        var tickets = await _ticketService.GetTicketsByEventAsync(eventId);
        return Ok(tickets);
    }

    [HttpGet("attendee/{attendeeId}")]
    public async Task<ActionResult<IEnumerable<TicketDto>>> GetByAttendee(int attendeeId)
    {
        var tickets = await _ticketService.GetTicketsByAttendeeAsync(attendeeId);
        return Ok(tickets);
    }

    [HttpPost("book")]
    public async Task<ActionResult<IEnumerable<TicketDto>>> BookTickets([FromBody] BookTicketDto bookTicketDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var tickets = await _ticketService.BookTicketsAsync(bookTicketDto);
            return Ok(tickets);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id}/cancel")]
    public async Task<ActionResult> CancelTicket(int id)
    {
        try
        {
            await _ticketService.CancelTicketAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
