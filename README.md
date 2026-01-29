# EventHub - Event Management Application

EventHub is a comprehensive web application for event management built with ASP.NET Core using Onion Architecture. It provides full CRUD operations for events, attendees, tickets, and schedules, along with external API integration for discovering events in various cities.

## Architecture

This project follows the **Onion Architecture** pattern with the following layers:

### 1. Domain Layer (`EventHub.Domain`)
- **Entities**: Core business entities (Event, Attendee, Ticket, Schedule)
- **Interfaces**: Repository interfaces and contracts
- **No external dependencies** - Pure business logic

### 2. Application Layer (`EventHub.Application`)
- **DTOs**: Data Transfer Objects for API communication
- **Services**: Business logic implementation
- **Interfaces**: Service interfaces
- **Dependencies**: Domain layer only

### 3. Infrastructure Layer (`EventHub.Infrastructure`)
- **Data**: DbContext and database configuration
- **Repositories**: Implementation of repository interfaces
- **External Services**: SerpAPI integration for external event data
- **Dependencies**: Domain and Application layers

### 4. Web Layer (`EventHub.Web`)
- **Controllers**: RESTful API endpoints
- **Configuration**: Dependency injection setup
- **Dependencies**: Application and Infrastructure layers

## Features

### Core Functionality
- **Events Management**: Create, read, update, and delete events
- **Attendees Management**: Manage event attendees
- **Tickets Management**: Book and cancel tickets with inventory tracking
- **Schedules Management**: Manage event schedules and sessions

### Special Features
- **Ticket Booking**: Book multiple tickets with automatic availability validation
- **Inventory Management**: Automatic ticket availability updates
- **External API Integration**: Fetch events from Google Events via SerpAPI
- **Data Transformation**: Transform external event data for display

## Domain Models

### Event
```csharp
- Id, Name, Description, Location, City
- StartDate, EndDate, Organizer, Category
- TotalTickets, AvailableTickets, TicketPrice
- ImageUrl, CreatedAt, UpdatedAt
- Navigation: Tickets[], Schedules[]
```

### Attendee
```csharp
- Id, FirstName, LastName, Email, PhoneNumber
- DateOfBirth, Address, RegisteredAt
- Navigation: Tickets[]
```

### Ticket
```csharp
- Id, TicketNumber, EventId, AttendeeId
- Price, PurchaseDate, Status (Active/Used/Cancelled/Refunded)
- SeatNumber, QRCode
- Navigation: Event, Attendee
```

### Schedule
```csharp
- Id, EventId, Title, Description
- StartTime, EndTime, Location, Speaker
- Navigation: Event
```

## API Endpoints

### Events
- `GET /api/events` - Get all events
- `GET /api/events/{id}` - Get event by ID
- `GET /api/events/city/{city}` - Get events by city
- `GET /api/events/upcoming` - Get upcoming events
- `POST /api/events` - Create new event
- `PUT /api/events/{id}` - Update event
- `DELETE /api/events/{id}` - Delete event

### Attendees
- `GET /api/attendees` - Get all attendees
- `GET /api/attendees/{id}` - Get attendee by ID
- `POST /api/attendees` - Create new attendee
- `PUT /api/attendees/{id}` - Update attendee
- `DELETE /api/attendees/{id}` - Delete attendee

### Tickets
- `GET /api/tickets` - Get all tickets
- `GET /api/tickets/{id}` - Get ticket by ID
- `GET /api/tickets/event/{eventId}` - Get tickets by event
- `GET /api/tickets/attendee/{attendeeId}` - Get tickets by attendee
- `POST /api/tickets/book` - Book tickets (with validation)
- `POST /api/tickets/{id}/cancel` - Cancel ticket

### Schedules
- `GET /api/schedules` - Get all schedules
- `GET /api/schedules/{id}` - Get schedule by ID
- `GET /api/schedules/event/{eventId}` - Get schedules by event
- `POST /api/schedules` - Create new schedule
- `PUT /api/schedules/{id}` - Update schedule
- `DELETE /api/schedules/{id}` - Delete schedule

### External Events
- `GET /api/externalevents/city/{city}` - Get events from external API (SerpAPI)

## Setup Instructions

### Prerequisites
- .NET 9.0 SDK or later
- SQL Server (LocalDB or full instance)
- SerpAPI Key (get from https://serpapi.com)

### Configuration

1. **Update Connection String** in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=EventHubDb;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true"
  }
}
```

2. **Add SerpAPI Key** in `appsettings.json`:
```json
{
  "SerpApi": {
    "ApiKey": "YOUR_SERPAPI_KEY_HERE"
  }
}
```

### Database Setup

1. **Create Initial Migration**:
```powershell
cd "src/EventHub.Web"
dotnet ef migrations add InitialCreate --project ../EventHub.Infrastructure --startup-project .
```

2. **Update Database**:
```powershell
dotnet ef database update --project ../EventHub.Infrastructure --startup-project .
```

### Running the Application

```powershell
cd "src/EventHub.Web"
dotnet run
```

The API will be available at:
- HTTPS: https://localhost:7XXX
- HTTP: http://localhost:5XXX
- Swagger UI: https://localhost:7XXX/swagger

## Technology Stack

- **Framework**: ASP.NET Core 9.0
- **ORM**: Entity Framework Core 9.0
- **Database**: SQL Server
- **API Documentation**: Swagger/OpenAPI
- **External API**: SerpAPI (Google Events)
- **Architecture**: Onion Architecture
- **Dependency Injection**: Built-in .NET DI Container

## Project Structure

```
EventHub/
├── src/
│   ├── EventHub.Domain/
│   │   ├── Entities/
│   │   │   ├── Event.cs
│   │   │   ├── Attendee.cs
│   │   │   ├── Ticket.cs
│   │   │   └── Schedule.cs
│   │   └── Interfaces/
│   │       ├── IRepository.cs
│   │       ├── IEventRepository.cs
│   │       ├── IAttendeeRepository.cs
│   │       ├── ITicketRepository.cs
│   │       ├── IScheduleRepository.cs
│   │       └── IUnitOfWork.cs
│   ├── EventHub.Application/
│   │   ├── DTOs/
│   │   │   ├── EventDto.cs
│   │   │   ├── AttendeeDto.cs
│   │   │   ├── TicketDto.cs
│   │   │   ├── ScheduleDto.cs
│   │   │   └── ExternalEventDto.cs
│   │   ├── Interfaces/
│   │   │   ├── IEventService.cs
│   │   │   ├── IAttendeeService.cs
│   │   │   ├── ITicketService.cs
│   │   │   ├── IScheduleService.cs
│   │   │   └── IExternalEventService.cs
│   │   └── Services/
│   │       ├── EventService.cs
│   │       ├── AttendeeService.cs
│   │       ├── TicketService.cs
│   │       └── ScheduleService.cs
│   ├── EventHub.Infrastructure/
│   │   ├── Data/
│   │   │   └── ApplicationDbContext.cs
│   │   ├── Repositories/
│   │   │   ├── Repository.cs
│   │   │   ├── EventRepository.cs
│   │   │   ├── AttendeeRepository.cs
│   │   │   ├── TicketRepository.cs
│   │   │   ├── ScheduleRepository.cs
│   │   │   └── UnitOfWork.cs
│   │   └── ExternalServices/
│   │       └── SerpApiEventService.cs
│   └── EventHub.Web/
│       ├── Controllers/
│       │   ├── EventsController.cs
│       │   ├── AttendeesController.cs
│       │   ├── TicketsController.cs
│       │   ├── SchedulesController.cs
│       │   └── ExternalEventsController.cs
│       ├── Program.cs
│       ├── appsettings.json
│       └── appsettings.Development.json
└── EventHub.sln
```

## Example Usage

### Creating an Event
```json
POST /api/events
{
  "name": "Tech Conference 2025",
  "description": "Annual technology conference",
  "location": "Convention Center",
  "city": "Austin",
  "startDate": "2025-06-15T09:00:00",
  "endDate": "2025-06-17T18:00:00",
  "organizer": "Tech Events Inc",
  "category": "Technology",
  "totalTickets": 500,
  "ticketPrice": 299.99
}
```

### Booking Tickets
```json
POST /api/tickets/book
{
  "eventId": 1,
  "attendeeId": 1,
  "quantity": 2,
  "seatNumber": "A12-13"
}
```

### Getting External Events
```
GET /api/externalevents/city/Austin
```

## License

This project is for educational purposes.

## Authors

Created for Integrated Systems course at FINKI.
