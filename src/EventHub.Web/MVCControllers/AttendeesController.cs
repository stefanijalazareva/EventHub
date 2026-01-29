using EventHub.Domain.DTO;
using EventHub.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventHub.Web.MVCControllers;

[Authorize(Roles = "Admin")]
public class AttendeesController : Controller
{
    private readonly IAttendeeService _attendeeService;
    private readonly ITicketService _ticketService;

    public AttendeesController(IAttendeeService attendeeService, ITicketService ticketService)
    {
        _attendeeService = attendeeService;
        _ticketService = ticketService;
    }

    public async Task<IActionResult> Index()
    {
        var attendees = await _attendeeService.GetAllAttendeesAsync();
        return View(attendees.OrderBy(a => a.LastName));
    }

    public async Task<IActionResult> Details(int id)
    {
        var attendee = await _attendeeService.GetAttendeeByIdAsync(id);
        if (attendee == null)
        {
            return NotFound();
        }

        var tickets = await _ticketService.GetAllTicketsAsync();
        ViewBag.Tickets = tickets.Where(t => t.AttendeeId == id);

        return View(attendee);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateAttendeeDto attendeeDto)
    {
        if (ModelState.IsValid)
        {
            await _attendeeService.CreateAttendeeAsync(attendeeDto);
            TempData["SuccessMessage"] = "Attendee registered successfully!";
            return RedirectToAction(nameof(Index));
        }
        return View(attendeeDto);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var attendee = await _attendeeService.GetAttendeeByIdAsync(id);
        if (attendee == null)
        {
            return NotFound();
        }

        var updateDto = new UpdateAttendeeDto
        {
            FirstName = attendee.FirstName,
            LastName = attendee.LastName,
            Email = attendee.Email,
            PhoneNumber = attendee.PhoneNumber,
            DateOfBirth = attendee.DateOfBirth,
            Address = attendee.Address
        };

        return View(updateDto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UpdateAttendeeDto attendeeDto)
    {
        if (ModelState.IsValid)
        {
            await _attendeeService.UpdateAttendeeAsync(id, attendeeDto);
            TempData["SuccessMessage"] = "Attendee updated successfully!";
            return RedirectToAction(nameof(Details), new { id });
        }
        return View(attendeeDto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _attendeeService.DeleteAttendeeAsync(id);
        TempData["SuccessMessage"] = "Attendee deleted successfully!";
        return RedirectToAction(nameof(Index));
    }
}
