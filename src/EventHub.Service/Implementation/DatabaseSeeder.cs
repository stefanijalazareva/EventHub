using EventHub.Domain.DomainModels;
using EventHub.Domain.Interfaces;
using EventHub.Service.Interface;

namespace EventHub.Service.Implementation;

public class DatabaseSeeder
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IExternalEventService _externalEventService;

    public DatabaseSeeder(IUnitOfWork unitOfWork, IExternalEventService externalEventService)
    {
        _unitOfWork = unitOfWork;
        _externalEventService = externalEventService;
    }

    public async Task SeedAsync()
    {
        // Check if database already has data
        var existingEvents = await _unitOfWork.Events.GetAllAsync();
        if (existingEvents.Any())
        {
            return; // Database already seeded
        }

        // Seed Attendees first
        var attendees = await SeedAttendeesAsync();

        // Fetch and seed real events from SerpAPI
        await SeedRealEventsAsync();

        // Get the created events
        var events = (await _unitOfWork.Events.GetAllAsync()).ToList();

        // Create schedules for events
        await SeedSchedulesAsync(events);

        // Create some tickets
        await SeedTicketsAsync(events, attendees);
    }

    private async Task<List<Attendee>> SeedAttendeesAsync()
    {
        var attendees = new List<Attendee>
        {
            new Attendee
            {
                FirstName = "Marko",
                LastName = "Petrovski",
                Email = "marko.petrovski@example.com",
                PhoneNumber = "+38970123456",
                DateOfBirth = new DateTime(1995, 5, 15),
                Address = "Partizanska 12, Skopje",
                RegisteredAt = DateTime.UtcNow.AddDays(-30)
            },
            new Attendee
            {
                FirstName = "Ana",
                LastName = "Nikolovska",
                Email = "ana.nikolovska@example.com",
                PhoneNumber = "+38970234567",
                DateOfBirth = new DateTime(1992, 8, 20),
                Address = "Makedonija 45, Skopje",
                RegisteredAt = DateTime.UtcNow.AddDays(-25)
            },
            new Attendee
            {
                FirstName = "Stefan",
                LastName = "Dimitrievski",
                Email = "stefan.dimitrievski@example.com",
                PhoneNumber = "+38970345678",
                DateOfBirth = new DateTime(1998, 3, 10),
                Address = "Ilindenska 8, Skopje",
                RegisteredAt = DateTime.UtcNow.AddDays(-20)
            },
            new Attendee
            {
                FirstName = "Elena",
                LastName = "Stojanovska",
                Email = "elena.stojanovska@example.com",
                PhoneNumber = "+38970456789",
                DateOfBirth = new DateTime(1990, 11, 25),
                Address = "Bulevar Turistička 67, Skopje",
                RegisteredAt = DateTime.UtcNow.AddDays(-15)
            },
            new Attendee
            {
                FirstName = "Nikola",
                LastName = "Jovanov",
                Email = "nikola.jovanov@example.com",
                PhoneNumber = "+38970567890",
                DateOfBirth = new DateTime(1997, 7, 5),
                Address = "Kej 13 Noemvri 22, Skopje",
                RegisteredAt = DateTime.UtcNow.AddDays(-10)
            }
        };

        foreach (var attendee in attendees)
        {
            await _unitOfWork.Attendees.AddAsync(attendee);
        }

        await _unitOfWork.SaveChangesAsync();
        return attendees;
    }

    private async Task SeedRealEventsAsync()
    {
        // Try to fetch real events from external API, but use fallback if it fails
        await SeedFallbackEventsAsync();
    }

    private async Task SeedFallbackEventsAsync()
    {
        var fallbackEvents = new List<Event>
        {
            new Event
            {
                Name = "Tech Conference 2026",
                Description = "Annual technology conference featuring latest innovations in AI and cloud computing",
                Location = "MKC - Skopje",
                City = "Skopje",
                StartDate = DateTime.UtcNow.AddDays(15),
                EndDate = DateTime.UtcNow.AddDays(17),
                Organizer = "Tech Community Skopje",
                Category = "Technology",
                TotalTickets = 200,
                AvailableTickets = 200,
                TicketPrice = 50,
                ImageUrl = "https://picsum.photos/400/300?random=1",
                CreatedAt = DateTime.UtcNow
            },
            new Event
            {
                Name = "Summer Music Festival",
                Description = "Three-day outdoor music festival with international and local artists",
                Location = "City Park",
                City = "Skopje",
                StartDate = DateTime.UtcNow.AddDays(30),
                EndDate = DateTime.UtcNow.AddDays(32),
                Organizer = "Music Events MK",
                Category = "Music",
                TotalTickets = 500,
                AvailableTickets = 500,
                TicketPrice = 35,
                ImageUrl = "https://picsum.photos/400/300?random=2",
                CreatedAt = DateTime.UtcNow
            },
            new Event
            {
                Name = "Art Exhibition: Modern Masters",
                Description = "Contemporary art exhibition featuring works from local and international artists",
                Location = "National Gallery",
                City = "Skopje",
                StartDate = DateTime.UtcNow.AddDays(7),
                EndDate = DateTime.UtcNow.AddDays(37),
                Organizer = "National Gallery of Macedonia",
                Category = "Art",
                TotalTickets = 100,
                AvailableTickets = 100,
                TicketPrice = 15,
                ImageUrl = "https://picsum.photos/400/300?random=3",
                CreatedAt = DateTime.UtcNow
            },
            new Event
            {
                Name = "Food & Wine Festival",
                Description = "Taste the best of local and international cuisine paired with fine wines",
                Location = "Old Bazaar",
                City = "Skopje",
                StartDate = DateTime.UtcNow.AddDays(20),
                EndDate = DateTime.UtcNow.AddDays(22),
                Organizer = "Culinary Association",
                Category = "Food & Drink",
                TotalTickets = 300,
                AvailableTickets = 300,
                TicketPrice = 40,
                ImageUrl = "https://picsum.photos/400/300?random=4",
                CreatedAt = DateTime.UtcNow
            },
            new Event
            {
                Name = "Marathon 2026",
                Description = "Annual city marathon - 5K, 10K and full marathon routes available",
                Location = "City Center - Starting Point",
                City = "Skopje",
                StartDate = DateTime.UtcNow.AddDays(45),
                EndDate = DateTime.UtcNow.AddDays(45),
                Organizer = "Running Club Skopje",
                Category = "Sports",
                TotalTickets = 1000,
                AvailableTickets = 1000,
                TicketPrice = 25,
                ImageUrl = "https://picsum.photos/400/300?random=5",
                CreatedAt = DateTime.UtcNow
            }
        };

        foreach (var evt in fallbackEvents)
        {
            await _unitOfWork.Events.AddAsync(evt);
        }

        await _unitOfWork.SaveChangesAsync();
    }

    private async Task SeedSchedulesAsync(List<Event> events)
    {
        foreach (var evt in events.Take(5))
        {
            var schedules = new List<Schedule>
            {
                new Schedule
                {
                    EventId = evt.Id,
                    Title = "Opening Ceremony",
                    Description = "Welcome and introduction to the event",
                    StartTime = evt.StartDate.AddHours(9),
                    EndTime = evt.StartDate.AddHours(10),
                    Location = evt.Location,
                    Speaker = "Event Organizer"
                },
                new Schedule
                {
                    EventId = evt.Id,
                    Title = "Main Session",
                    Description = "Core activities and presentations",
                    StartTime = evt.StartDate.AddHours(10),
                    EndTime = evt.StartDate.AddHours(13),
                    Location = evt.Location,
                    Speaker = "Featured Speakers"
                },
                new Schedule
                {
                    EventId = evt.Id,
                    Title = "Closing Remarks",
                    Description = "Summary and closing notes",
                    StartTime = evt.StartDate.AddHours(16),
                    EndTime = evt.StartDate.AddHours(17),
                    Location = evt.Location,
                    Speaker = "Event Director"
                }
            };

            foreach (var schedule in schedules)
            {
                await _unitOfWork.Schedules.AddAsync(schedule);
            }
        }

        await _unitOfWork.SaveChangesAsync();
    }

    private async Task SeedTicketsAsync(List<Event> events, List<Attendee> attendees)
    {
        var random = new Random();
        
        // Create some tickets for the first few events
        foreach (var evt in events.Take(5))
        {
            var ticketCount = random.Next(2, 5);
            
            for (int i = 0; i < ticketCount && i < attendees.Count; i++)
            {
                var ticket = new Ticket
                {
                    TicketNumber = $"TKT-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString()[..8].ToUpper()}",
                    EventId = evt.Id,
                    AttendeeId = attendees[i].Id,
                    Price = evt.TicketPrice,
                    PurchaseDate = DateTime.UtcNow.AddDays(-random.Next(1, 10)),
                    Status = TicketStatus.Active,
                    SeatNumber = $"{(char)('A' + random.Next(0, 5))}{random.Next(1, 20)}",
                    QRCode = Guid.NewGuid().ToString()
                };

                await _unitOfWork.Tickets.AddAsync(ticket);
                
                // Update available tickets
                evt.AvailableTickets--;
                await _unitOfWork.Events.UpdateAsync(evt);
            }
        }

        await _unitOfWork.SaveChangesAsync();
    }

    private string DetermineCategory(string title)
    {
        var titleLower = title.ToLower();
        
        if (titleLower.Contains("music") || titleLower.Contains("concert") || titleLower.Contains("festival"))
            return "Music";
        if (titleLower.Contains("tech") || titleLower.Contains("conference") || titleLower.Contains("workshop"))
            return "Technology";
        if (titleLower.Contains("art") || titleLower.Contains("exhibition") || titleLower.Contains("gallery"))
            return "Art";
        if (titleLower.Contains("food") || titleLower.Contains("wine") || titleLower.Contains("culinary"))
            return "Food & Drink";
        if (titleLower.Contains("sport") || titleLower.Contains("game") || titleLower.Contains("match"))
            return "Sports";
        
        return "General";
    }
}
