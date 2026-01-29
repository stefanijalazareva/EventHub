using EventHub.Domain.DTO;
using EventHub.Domain.DomainModels;
using EventHub.Service.Interface;
using EventHub.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EventHub.Web.MVCControllers;

[Authorize]
public class TicketsController : Controller
{
    private readonly ITicketService _ticketService;
    private readonly IEventService _eventService;
    private readonly IAttendeeService _attendeeService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailService _emailService;

    public TicketsController(
        ITicketService ticketService,
        IEventService eventService,
        IAttendeeService attendeeService,
        UserManager<ApplicationUser> userManager,
        IEmailService emailService)
    {
        _ticketService = ticketService;
        _eventService = eventService;
        _attendeeService = attendeeService;
        _userManager = userManager;
        _emailService = emailService;
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Index(string searchUser, int? eventId)
    {
        var tickets = await _ticketService.GetAllTicketsAsync();
        
        // Filter by user (attendee name or email)
        if (!string.IsNullOrEmpty(searchUser))
        {
            tickets = tickets.Where(t => 
                t.AttendeeName.Contains(searchUser, StringComparison.OrdinalIgnoreCase) ||
                t.AttendeeEmail.Contains(searchUser, StringComparison.OrdinalIgnoreCase)
            ).ToList();
        }
        
        // Filter by event
        if (eventId.HasValue && eventId.Value > 0)
        {
            tickets = tickets.Where(t => t.EventId == eventId.Value).ToList();
        }
        
        // Get all events for the filter dropdown
        var events = await _eventService.GetAllEventsAsync();
        ViewBag.Events = events;
        ViewBag.SearchUser = searchUser;
        ViewBag.SelectedEventId = eventId;
        
        return View(tickets.OrderByDescending(t => t.PurchaseDate));
    }

    // My Tickets - shows only current user's tickets
    public async Task<IActionResult> MyTickets()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        if (!user.AttendeeId.HasValue)
        {
            // User has no tickets yet
            return View(new List<TicketDto>());
        }

        var allTickets = await _ticketService.GetAllTicketsAsync();
        var myTickets = allTickets.Where(t => t.AttendeeId == user.AttendeeId.Value)
                                   .OrderByDescending(t => t.PurchaseDate);
        
        return View(myTickets);
    }

    public async Task<IActionResult> Book(int eventId)
    {
        var eventDto = await _eventService.GetEventByIdAsync(eventId);
        if (eventDto == null)
        {
            return NotFound();
        }

        ViewBag.Event = eventDto;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Book(int eventId, int quantity)
    {
        try
        {
            // Get current user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Get or create attendee for current user
            int attendeeId;
            if (user.AttendeeId.HasValue)
            {
                attendeeId = user.AttendeeId.Value;
            }
            else
            {
                // Create attendee for this user
                var createAttendeeDto = new CreateAttendeeDto
                {
                    FirstName = user.UserName?.Split('@')[0] ?? "User",
                    LastName = "",
                    Email = user.Email ?? "",
                    PhoneNumber = user.PhoneNumber ?? "",
                    DateOfBirth = DateTime.Now.AddYears(-25),
                    Address = ""
                };
                
                var attendee = await _attendeeService.CreateAttendeeAsync(createAttendeeDto);
                attendeeId = attendee.Id;
                
                // Update user with attendee reference
                user.AttendeeId = attendeeId;
                await _userManager.UpdateAsync(user);
            }

            var bookDto = new BookTicketDto { EventId = eventId, AttendeeId = attendeeId, Quantity = quantity };
            var tickets = await _ticketService.BookTicketsAsync(bookDto);
            
            // Get event details for email
            var eventDetails = await _eventService.GetEventByIdAsync(eventId);
            var ticketNumbers = string.Join("<br/>", tickets.Select(t => t.TicketNumber));
            var totalPrice = tickets.Sum(t => t.Price);
            
            // Send confirmation email
            await _emailService.SendTicketConfirmationAsync(
                user.Email ?? "",
                user.UserName ?? "Customer",
                eventDetails?.Name ?? "Event",
                quantity,
                totalPrice,
                ticketNumbers
            );
            
            TempData["SuccessMessage"] = $"Successfully booked {quantity} ticket(s)! A confirmation email has been sent to {user.Email}";
            
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return RedirectToAction(nameof(MyTickets));
            }
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction(nameof(Book), new { eventId });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id)
    {
        try
        {
            await _ticketService.CancelTicketAsync(id);
            TempData["SuccessMessage"] = "Ticket cancelled successfully!";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        
        // Redirect based on role
        if (User.IsInRole("Admin"))
        {
            return RedirectToAction(nameof(Index));
        }
        else
        {
            return RedirectToAction(nameof(MyTickets));
        }
    }
}
