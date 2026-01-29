# Testing Guide & Sample Data

## Quick Start Testing

### Step 1: Create the Database

```powershell
cd "src/EventHub.Web"
dotnet ef database update --project ../EventHub.Infrastructure
```

### Step 2: Run the Application

```powershell
dotnet run
```

### Step 3: Open Swagger UI

Navigate to: `https://localhost:7XXX/swagger`

## Sample Test Data

### 1. Create Attendees First

**POST /api/attendees**

```json
{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "phoneNumber": "+1234567890",
  "dateOfBirth": "1990-05-15",
  "address": "123 Main St, Austin, TX"
}
```

```json
{
  "firstName": "Jane",
  "lastName": "Smith",
  "email": "jane.smith@example.com",
  "phoneNumber": "+1234567891",
  "dateOfBirth": "1992-08-20",
  "address": "456 Oak Ave, Austin, TX"
}
```

### 2. Create Events

**POST /api/events**

```json
{
  "name": "Austin Tech Summit 2025",
  "description": "The largest technology conference in Austin featuring keynotes from industry leaders, technical workshops, and networking opportunities.",
  "location": "Austin Convention Center",
  "city": "Austin",
  "startDate": "2025-09-15T09:00:00",
  "endDate": "2025-09-17T18:00:00",
  "organizer": "Tech Events Inc",
  "category": "Technology",
  "totalTickets": 500,
  "ticketPrice": 299.99,
  "imageUrl": "https://example.com/tech-summit.jpg"
}
```

```json
{
  "name": "Austin Food & Wine Festival",
  "description": "Annual celebration of culinary excellence featuring renowned chefs, wine tastings, and cooking demonstrations.",
  "location": "Zilker Park",
  "city": "Austin",
  "startDate": "2025-10-20T11:00:00",
  "endDate": "2025-10-22T22:00:00",
  "organizer": "Austin Culinary Association",
  "category": "Food & Beverage",
  "totalTickets": 1000,
  "ticketPrice": 85.00
}
```

```json
{
  "name": "Live Music Showcase",
  "description": "Experience the best of Austin's live music scene with performances from local and touring artists.",
  "location": "Moody Theater",
  "city": "Austin",
  "startDate": "2025-08-05T19:00:00",
  "endDate": "2025-08-05T23:00:00",
  "organizer": "Austin Music Foundation",
  "category": "Music",
  "totalTickets": 300,
  "ticketPrice": 45.00
}
```

### 3. Create Schedules for Events

**POST /api/schedules** (for Tech Summit - Event ID 1)

```json
{
  "eventId": 1,
  "title": "Keynote: Future of AI",
  "description": "Opening keynote discussing the future of artificial intelligence and its impact on society.",
  "startTime": "2025-09-15T09:00:00",
  "endTime": "2025-09-15T10:30:00",
  "location": "Main Hall",
  "speaker": "Dr. Sarah Johnson"
}
```

```json
{
  "eventId": 1,
  "title": "Workshop: Cloud Architecture",
  "description": "Hands-on workshop covering modern cloud architecture patterns and best practices.",
  "startTime": "2025-09-15T11:00:00",
  "endTime": "2025-09-15T13:00:00",
  "location": "Room 201",
  "speaker": "Michael Chen"
}
```

```json
{
  "eventId": 1,
  "title": "Panel: Cybersecurity Trends",
  "description": "Expert panel discussing current cybersecurity challenges and emerging solutions.",
  "startTime": "2025-09-15T14:00:00",
  "endTime": "2025-09-15T15:30:00",
  "location": "Main Hall",
  "speaker": "Security Experts Panel"
}
```

### 4. Book Tickets

**POST /api/tickets/book**

```json
{
  "eventId": 1,
  "attendeeId": 1,
  "quantity": 2,
  "seatNumber": "A12-13"
}
```

```json
{
  "eventId": 2,
  "attendeeId": 2,
  "quantity": 1,
  "seatNumber": "B05"
}
```

## Test Scenarios

### Scenario 1: Complete Event Lifecycle

1. **Create an Event**
   ```
   POST /api/events
   ```

2. **Add Schedules**
   ```
   POST /api/schedules (multiple times)
   ```

3. **View Event with Schedules**
   ```
   GET /api/events/{id}
   GET /api/schedules/event/{eventId}
   ```

4. **Book Tickets**
   ```
   POST /api/tickets/book
   ```

5. **Verify Ticket Count Decreased**
   ```
   GET /api/events/{id}
   // Check availableTickets field
   ```

6. **Cancel Ticket**
   ```
   POST /api/tickets/{id}/cancel
   ```

7. **Verify Ticket Count Increased**
   ```
   GET /api/events/{id}
   ```

### Scenario 2: Ticket Booking Validation

1. **Attempt to Book More Tickets Than Available**
   ```json
   POST /api/tickets/book
   {
     "eventId": 3,
     "attendeeId": 1,
     "quantity": 1000
   }
   ```
   Expected: 400 Bad Request with error message

2. **Attempt to Book for Non-Existent Event**
   ```json
   POST /api/tickets/book
   {
     "eventId": 9999,
     "attendeeId": 1,
     "quantity": 1
   }
   ```
   Expected: 404 Not Found

3. **Attempt to Cancel Already Cancelled Ticket**
   ```
   POST /api/tickets/{id}/cancel (same ID twice)
   ```
   Expected: 400 Bad Request on second attempt

### Scenario 3: External API Integration

1. **Get External Events for Austin**
   ```
   GET /api/externalevents/city/Austin
   ```

2. **Get External Events for New York**
   ```
   GET /api/externalevents/city/New York
   ```

3. **Compare with Local Events**
   ```
   GET /api/events/city/Austin
   ```

### Scenario 4: Search and Filter

1. **Get All Events**
   ```
   GET /api/events
   ```

2. **Get Events by City**
   ```
   GET /api/events/city/Austin
   ```

3. **Get Upcoming Events**
   ```
   GET /api/events/upcoming
   ```

4. **Get Tickets for Specific Event**
   ```
   GET /api/tickets/event/{eventId}
   ```

5. **Get Tickets for Specific Attendee**
   ```
   GET /api/tickets/attendee/{attendeeId}
   ```

### Scenario 5: Update Operations

1. **Update Event Details**
   ```json
   PUT /api/events/{id}
   {
     "name": "Austin Tech Summit 2025 - UPDATED",
     "description": "Updated description",
     // ... other fields
   }
   ```

2. **Update Attendee Information**
   ```json
   PUT /api/attendees/{id}
   {
     "firstName": "John",
     "lastName": "Doe-Updated",
     // ... other fields
   }
   ```

3. **Update Schedule**
   ```json
   PUT /api/schedules/{id}
   {
     "title": "Updated Workshop Title",
     // ... other fields
   }
   ```

## Database Queries to Verify Data

You can use SQL Server Management Studio or Azure Data Studio to verify:

```sql
-- Check all events
SELECT * FROM Events;

-- Check ticket availability
SELECT Id, Name, TotalTickets, AvailableTickets 
FROM Events;

-- Check all tickets with details
SELECT t.*, e.Name as EventName, 
       a.FirstName + ' ' + a.LastName as AttendeeName
FROM Tickets t
JOIN Events e ON t.EventId = e.Id
JOIN Attendees a ON t.AttendeeId = a.Id;

-- Check schedules for an event
SELECT * FROM Schedules WHERE EventId = 1;

-- Count tickets by event
SELECT e.Name, COUNT(t.Id) as TicketCount
FROM Events e
LEFT JOIN Tickets t ON e.Id = t.EventId
GROUP BY e.Name;
```

## Expected Behavior

### Ticket Booking
- ✅ Decreases `AvailableTickets` count
- ✅ Generates unique `TicketNumber`
- ✅ Creates `QRCode`
- ✅ Sets status to `Active`
- ✅ Records `PurchaseDate`

### Ticket Cancellation
- ✅ Changes status to `Cancelled`
- ✅ Increases `AvailableTickets` count
- ✅ Cannot cancel already cancelled ticket

### Event Deletion
- ✅ Cascades to delete associated tickets
- ✅ Cascades to delete associated schedules

### Attendee Deletion
- ✅ Restricted if attendee has tickets
- ⚠️ Must cancel/delete tickets first

## Troubleshooting Common Issues

### Issue: "Database does not exist"
**Solution:**
```powershell
dotnet ef database update --project ../EventHub.Infrastructure
```

### Issue: "Connection string error"
**Solution:** 
- Verify SQL Server LocalDB is installed
- Or update connection string to point to your SQL Server instance

### Issue: "Duplicate email for attendee"
**Solution:** 
- Email field has unique constraint
- Use different email addresses

### Issue: "Not enough tickets available"
**Solution:** 
- Check `AvailableTickets` before booking
- This is expected validation behavior

## Performance Testing

### Load Test Scenario

1. Create 100 attendees
2. Create 10 events
3. Book 500 tickets across events
4. Verify all operations complete successfully
5. Check database performance

### Stress Test

1. Attempt simultaneous ticket bookings for same event
2. Verify ticket count remains accurate
3. Test transaction isolation

## Next Steps

After testing the basic functionality:

1. ✅ Test all CRUD operations for each entity
2. ✅ Test ticket booking edge cases
3. ✅ Test external API integration
4. ✅ Verify data relationships
5. ✅ Test error handling
6. 📱 Build a frontend application
7. 🔐 Add authentication and authorization
8. 📧 Add email notifications for ticket purchases
9. 💳 Integrate payment processing
10. 📊 Add analytics and reporting
