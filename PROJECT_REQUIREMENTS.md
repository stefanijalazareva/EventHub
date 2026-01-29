# EventHub - Project Requirements Documentation

## Overview
EventHub is a comprehensive event management web application built with ASP.NET Core that demonstrates all required project components for the Integrated Systems course.

---

## ✅ Requirement 1: Onion Architecture Implementation

**Status:** ✅ **COMPLETED**

The application follows the Onion Architecture pattern with clear separation of concerns across four layers:

### Layer Structure:

1. **EventHub.Domain** (Core Layer)
   - Contains: Domain models, DTOs, Repository interfaces
   - Dependencies: None (pure domain logic)
   - Files:
     - `DomainModels/`: Event, Attendee, Ticket, Schedule
     - `DTO/`: EventDto, AttendeeDto, TicketDto, ScheduleDto, WeatherDto
     - `Interfaces/`: IEventRepository, IAttendeeRepository, ITicketRepository, IScheduleRepository

2. **EventHub.Service** (Application Layer)
   - Contains: Business logic, Service interfaces and implementations
   - Dependencies: EventHub.Domain
   - Files:
     - `Interface/`: IEventService, IAttendeeService, ITicketService, IScheduleService, IWeatherService
     - `Implementation/`: EventService, AttendeeService, TicketService, ScheduleService, WeatherService, DatabaseSeeder

3. **EventHub.Repository** (Infrastructure Layer)
   - Contains: Data access, External API integrations, Database context
   - Dependencies: EventHub.Domain, EventHub.Service
   - Files:
     - `Implementation/`: EventRepository, AttendeeRepository, TicketRepository, ScheduleRepository, UnitOfWork
     - `Data/`: ApplicationDbContext
     - `ExternalServices/`: SerpApiEventService
     - `Migrations/`: EF Core migrations

4. **EventHub.Web** (Presentation Layer)
   - Contains: API Controllers, Configuration, Startup logic
   - Dependencies: All other layers
   - Files:
     - `Controllers/`: EventsController, AttendeesController, TicketsController, SchedulesController, WeatherController
     - `Program.cs`: Dependency injection configuration

---

## ✅ Requirement 2: At Least 4 Domain Models

**Status:** ✅ **COMPLETED**

The application includes **4 complete domain models** with full entity relationships:

### 1. **Event** (`EventHub.Domain/DomainModels/Event.cs`)
- **Properties (15):**
  - Id, Name, Description, Location, City
  - StartDate, EndDate, Organizer, Category
  - TotalTickets, AvailableTickets, TicketPrice
  - ImageUrl, CreatedAt, UpdatedAt
- **Relationships:**
  - One-to-Many with Ticket
  - One-to-Many with Schedule

### 2. **Attendee** (`EventHub.Domain/DomainModels/Attendee.cs`)
- **Properties (8):**
  - Id, FirstName, LastName, Email (Unique)
  - PhoneNumber, DateOfBirth, Address, RegisteredAt
- **Relationships:**
  - One-to-Many with Ticket

### 3. **Ticket** (`EventHub.Domain/DomainModels/Ticket.cs`)
- **Properties (9):**
  - Id, TicketNumber (Unique), EventId, AttendeeId
  - Price, PurchaseDate, Status (Enum: Active/Cancelled)
  - SeatNumber, QRCode
- **Relationships:**
  - Many-to-One with Event
  - Many-to-One with Attendee

### 4. **Schedule** (`EventHub.Domain/DomainModels/Schedule.cs`)
- **Properties (8):**
  - Id, EventId, Title, Description
  - StartTime, EndTime, Location, Speaker
- **Relationships:**
  - Many-to-One with Event

### Database Schema:
```
Events (1) ──────< (∞) Tickets (∞) >────── (1) Attendees
  │
  └──────< (∞) Schedules
```

---

## ✅ Requirement 3: All CRUD Operations + Additional Action

**Status:** ✅ **COMPLETED**

### CRUD Operations Implemented for All Models:

#### **Events API** (`/api/events`)
- ✅ **Create:** `POST /api/events` - Create new event
- ✅ **Read All:** `GET /api/events` - Get all events
- ✅ **Read One:** `GET /api/events/{id}` - Get event by ID
- ✅ **Update:** `PUT /api/events/{id}` - Update event
- ✅ **Delete:** `DELETE /api/events/{id}` - Delete event

#### **Attendees API** (`/api/attendees`)
- ✅ **Create:** `POST /api/attendees` - Register new attendee
- ✅ **Read All:** `GET /api/attendees` - Get all attendees
- ✅ **Read One:** `GET /api/attendees/{id}` - Get attendee by ID
- ✅ **Update:** `PUT /api/attendees/{id}` - Update attendee
- ✅ **Delete:** `DELETE /api/attendees/{id}` - Delete attendee

#### **Tickets API** (`/api/tickets`)
- ✅ **Create:** `POST /api/tickets` - Create ticket
- ✅ **Read All:** `GET /api/tickets` - Get all tickets
- ✅ **Read One:** `GET /api/tickets/{id}` - Get ticket by ID
- ✅ **Update:** `PUT /api/tickets/{id}` - Update ticket
- ✅ **Delete:** `DELETE /api/tickets/{id}` - Delete ticket

#### **Schedules API** (`/api/schedules`)
- ✅ **Create:** `POST /api/schedules` - Create schedule
- ✅ **Read All:** `GET /api/schedules` - Get all schedules
- ✅ **Read One:** `GET /api/schedules/{id}` - Get schedule by ID
- ✅ **Update:** `PUT /api/schedules/{id}` - Update schedule
- ✅ **Delete:** `DELETE /api/schedules/{id}` - Delete schedule

### Additional Actions (Beyond CRUD):

#### 1. **Ticket Booking System** 🎫
**Endpoint:** `POST /api/tickets/book`

**Implementation:** `TicketService.BookTicketsAsync()`

**Features:**
- Books multiple tickets for an event in a single transaction
- Validates event existence and availability
- Checks if enough tickets are available
- Updates event's available ticket count atomically
- Generates unique ticket numbers (format: `TICK-{eventId}-{timestamp}`)
- Generates QR codes for each ticket
- Assigns seat numbers sequentially
- Uses Unit of Work pattern for data consistency

**Request Body:**
```json
{
  "eventId": 1,
  "attendeeId": 1,
  "quantity": 2
}
```

**Business Logic:**
1. Verify event exists
2. Check if `AvailableTickets >= quantity`
3. Deduct tickets from `AvailableTickets`
4. Create ticket records with unique numbers
5. Commit transaction or rollback on failure

#### 2. **Ticket Cancellation** ❌
**Endpoint:** `POST /api/tickets/{id}/cancel`

**Implementation:** `TicketService.CancelTicketAsync()`

**Features:**
- Changes ticket status to "Cancelled"
- Returns ticket back to event inventory
- Increments event's `AvailableTickets`

---

## ✅ Requirement 4: External API Integration with Data Transformation

**Status:** ✅ **COMPLETED**

### Weather API Integration for Event Locations

**External API:** [Open-Meteo Weather API](https://open-meteo.com/)
- Free, no API key required
- Real-time weather forecasts
- 16-day forecast data

**Endpoint:** `GET /api/weather/event/{eventId}`

**Implementation:** `WeatherService.GetWeatherForEventAsync()`

**Location:** `EventHub.Service/Implementation/WeatherService.cs`

### How It Works:

1. **Fetches Event Data:**
   - Retrieves event from database
   - Extracts city and start date

2. **Calls External API:**
   - Maps Macedonian cities to GPS coordinates (Skopje, Bitola, Ohrid, etc.)
   - Calls Open-Meteo API with latitude/longitude
   - Requests hourly forecast for 16 days
   - Gets temperature, humidity, wind, precipitation, weather codes

3. **Transforms Data:**
   - Finds weather data matching event date/time
   - Converts weather codes to human-readable descriptions:
     - `0` → "Clear sky"
     - `61-65` → "Rain"
     - `95` → "Thunderstorm"
   - Rounds numerical values (temperature, wind speed)
   - Generates personalized recommendations based on conditions

### Transformed Response Example:

**API Response:**
```json
{
  "eventName": "Tech Conference 2026",
  "city": "Skopje",
  "eventDate": "2026-06-15T09:00:00",
  "temperature": 24.5,
  "feelsLike": 23.8,
  "weatherCondition": "Partly cloudy",
  "humidity": 65.0,
  "windSpeed": 12.3,
  "precipitationChance": 20.0,
  "recommendation": "Warm weather - light clothing recommended. Perfect weather conditions for the event!"
}
```

### Data Transformation Features:

✅ **Weather Code Translation:**
- Raw API returns numeric codes (0-99)
- Transformed to readable text ("Clear sky", "Rain", "Snow")

✅ **Smart Recommendations:**
- Temperature-based clothing advice
- Rain probability warnings
- Wind condition alerts
- Event-specific suggestions

✅ **City Coordinates Mapping:**
- API requires latitude/longitude
- Service maps Macedonian cities to coordinates
- Fallback to Skopje if city not found

✅ **Date/Time Matching:**
- Finds closest hour in forecast to event start time
- Handles timezone (Europe/Skopje)

---

## ✅ Requirement 5: Cloud Hosting (Optional - 20% Bonus)

**Status:** ⏳ **PENDING**

### Options for Cloud Deployment:

1. **Microsoft Azure** (Recommended)
   - Azure App Service for ASP.NET Core
   - Azure SQL Database for data
   - Free tier available for students

2. **AWS**
   - Elastic Beanstalk
   - RDS for database

3. **Heroku**
   - Container deployment
   - PostgreSQL add-on

### Current Setup (Local):
- Database: SQLite (`EventHub.db`)
- Server: Kestrel (localhost:5283)
- Documentation: Swagger UI

### Migration Steps for Cloud:
1. Change database from SQLite to SQL Server/PostgreSQL
2. Update connection string in `appsettings.json`
3. Deploy via Azure DevOps or GitHub Actions
4. Configure environment variables for API keys

---

## 🎯 Summary of Requirements Compliance

| Requirement | Status | Implementation |
|------------|--------|----------------|
| **Onion Architecture** | ✅ Complete | 4 layers: Domain, Service, Repository, Web |
| **4+ Domain Models** | ✅ Complete | Event, Attendee, Ticket, Schedule |
| **CRUD Operations** | ✅ Complete | All CRUD for all 4 models |
| **Additional Action** | ✅ Complete | Ticket Booking + Cancellation |
| **External API Integration** | ✅ Complete | Open-Meteo Weather API with transformation |
| **Data Transformation** | ✅ Complete | Weather codes → text, recommendations, rounding |
| **Cloud Hosting** | ⏳ Optional | 20% bonus (not mandatory) |

---

## 📋 Testing Guide

### 1. Start Application:
```bash
cd "c:\Stefanija\FINKI\Semestar 6\Integrirani sistemi\EventHub\src\EventHub.Web"
dotnet run
```

### 2. Access Swagger UI:
Navigate to: http://localhost:5283/swagger

### 3. Test Weather API Integration:
```bash
GET /api/weather/event/1
```

This will return transformed weather data for the first event.

### 4. Test Ticket Booking:
```bash
POST /api/tickets/book
{
  "eventId": 1,
  "attendeeId": 1,
  "quantity": 3
}
```

### 5. Verify CRUD Operations:
- Create event: `POST /api/events`
- Get all events: `GET /api/events`
- Update event: `PUT /api/events/1`
- Delete event: `DELETE /api/events/1`

---

## 📊 Sample Data

The application includes a **DatabaseSeeder** that populates the database with:

- **5 Attendees** (Macedonian names: Marko Petrovski, Ana Nikolovska, etc.)
- **5 Events** (Tech Conference, Music Festival, Art Exhibition, Food Festival, Marathon)
- **15 Schedules** (3 per event: Opening, Main Session, Closing)
- **15 Tickets** (3 per event, pre-booked)

All events are set in **Skopje** with realistic dates in 2026.

---

## 🛠️ Technologies Used

- **Framework:** ASP.NET Core 9.0
- **ORM:** Entity Framework Core 9.0
- **Database:** SQLite (local), SQL Server (production-ready)
- **API Documentation:** Swagger/OpenAPI
- **External APIs:** Open-Meteo Weather API
- **Architecture:** Onion Architecture
- **Patterns:** Repository Pattern, Unit of Work, Dependency Injection

---

## 📝 Conclusion

EventHub fully satisfies all **mandatory requirements** for the Integrated Systems project:

1. ✅ Implements Onion Architecture with 4 distinct layers
2. ✅ Contains 4 domain models with relationships
3. ✅ Provides complete CRUD operations for all models
4. ✅ Includes additional business logic (ticket booking/cancellation)
5. ✅ Integrates external REST API (Open-Meteo) with comprehensive data transformation

The application is production-ready and demonstrates professional software engineering practices including separation of concerns, clean architecture, and robust error handling.

**Optional cloud hosting** can be added for a 20% bonus but is not required for passing grade.
