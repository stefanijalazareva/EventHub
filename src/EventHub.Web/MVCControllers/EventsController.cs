using EventHub.Domain.DTO;
using EventHub.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventHub.Web.MVCControllers;

public class EventsController : Controller
{
    private readonly IEventService _eventService;
    private readonly IWeatherService _weatherService;

    public EventsController(IEventService eventService, IWeatherService weatherService)
    {
        _eventService = eventService;
        _weatherService = weatherService;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index(string category = null, string city = null)
    {
        var events = await _eventService.GetAllEventsAsync();

        if (!string.IsNullOrEmpty(category))
        {
            events = events.Where(e => e.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(city))
        {
            events = events.Where(e => e.City.Equals(city, StringComparison.OrdinalIgnoreCase));
        }

        ViewBag.Categories = (await _eventService.GetAllEventsAsync())
            .Select(e => e.Category)
            .Distinct()
            .OrderBy(c => c);

        ViewBag.Cities = (await _eventService.GetAllEventsAsync())
            .Select(e => e.City)
            .Distinct()
            .OrderBy(c => c);

        ViewBag.SelectedCategory = category;
        ViewBag.SelectedCity = city;

        return View(events.OrderBy(e => e.StartDate));
    }

    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        var eventDto = await _eventService.GetEventByIdAsync(id);
        if (eventDto == null)
        {
            return NotFound();
        }

        var weather = await _weatherService.GetWeatherForEventAsync(id);
        ViewBag.Weather = weather;

        return View(eventDto);
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        return View();
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateEventDto eventDto)
    {
        if (ModelState.IsValid)
        {
            await _eventService.CreateEventAsync(eventDto);
            TempData["SuccessMessage"] = "Event created successfully!";
            return RedirectToAction(nameof(Index));
        }
        return View(eventDto);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var eventDto = await _eventService.GetEventByIdAsync(id);
        if (eventDto == null)
        {
            return NotFound();
        }

        var updateDto = new UpdateEventDto
        {
            Name = eventDto.Name,
            Description = eventDto.Description,
            Location = eventDto.Location,
            City = eventDto.City,
            StartDate = eventDto.StartDate,
            EndDate = eventDto.EndDate,
            Organizer = eventDto.Organizer,
            Category = eventDto.Category,
            TotalTickets = eventDto.TotalTickets,
            TicketPrice = eventDto.TicketPrice,
            ImageUrl = eventDto.ImageUrl
        };

        return View(updateDto);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UpdateEventDto eventDto)
    {
        if (ModelState.IsValid)
        {
            await _eventService.UpdateEventAsync(id, eventDto);
            TempData["SuccessMessage"] = "Event updated successfully!";
            return RedirectToAction(nameof(Details), new { id });
        }
        return View(eventDto);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _eventService.DeleteEventAsync(id);
        TempData["SuccessMessage"] = "Event deleted successfully!";
        return RedirectToAction(nameof(Index));
    }
}
