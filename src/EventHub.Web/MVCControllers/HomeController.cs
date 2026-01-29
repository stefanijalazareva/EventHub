using EventHub.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace EventHub.Web.MVCControllers;

public class HomeController : Controller
{
    private readonly IEventService _eventService;
    private readonly IAttendeeService _attendeeService;
    private readonly ITicketService _ticketService;

    public HomeController(
        IEventService eventService,
        IAttendeeService attendeeService,
        ITicketService ticketService)
    {
        _eventService = eventService;
        _attendeeService = attendeeService;
        _ticketService = ticketService;
    }

    public async Task<IActionResult> Index()
    {
        var events = await _eventService.GetAllEventsAsync();
        var attendees = await _attendeeService.GetAllAttendeesAsync();
        var tickets = await _ticketService.GetAllTicketsAsync();

        ViewBag.TotalEvents = events.Count();
        ViewBag.TotalAttendees = attendees.Count();
        ViewBag.TotalTickets = tickets.Count();
        ViewBag.UpcomingEvents = events.Where(e => e.StartDate > DateTime.Now).Count();

        return View(events.OrderBy(e => e.StartDate).Take(6));
    }
}
