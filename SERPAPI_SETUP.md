# SerpAPI Setup Guide

## Getting Your SerpAPI Key

1. Go to [https://serpapi.com](https://serpapi.com)
2. Click "Sign Up" or "Get Started Free"
3. Create an account (free tier available)
4. Once logged in, go to your Dashboard
5. Find your API key in the "API Key" section
6. Copy the API key

## Configuring the Application

### Option 1: appsettings.json (Development)

1. Open `src/EventHub.Web/appsettings.Development.json`
2. Replace `YOUR_SERPAPI_KEY_HERE` with your actual API key:

```json
{
  "SerpApi": {
    "ApiKey": "your_actual_api_key_here"
  }
}
```

### Option 2: User Secrets (Recommended for Development)

```powershell
cd "src/EventHub.Web"
dotnet user-secrets init
dotnet user-secrets set "SerpApi:ApiKey" "your_actual_api_key_here"
```

### Option 3: Environment Variables (Production)

Set the environment variable:
```powershell
$env:SerpApi__ApiKey = "your_actual_api_key_here"
```

## Testing the External API Integration

### Using Swagger UI

1. Run the application:
   ```powershell
   cd "src/EventHub.Web"
   dotnet run
   ```

2. Open browser to: `https://localhost:7XXX/swagger`

3. Find the **ExternalEvents** controller

4. Test the endpoint:
   - Endpoint: `GET /api/externalevents/city/{city}`
   - Try with: `Austin`, `New York`, `Los Angeles`, etc.

### Using curl or Postman

```bash
curl -X GET "https://localhost:7XXX/api/externalevents/city/Austin"
```

## Example Response

```json
{
  "city": "Austin",
  "totalEvents": 5,
  "events": [
    {
      "title": "Austin Food & Wine Festival",
      "description": "Annual food and wine festival featuring local chefs",
      "date": "From: 2025-04-15 - April 15-17, 2025",
      "location": "Austin Convention Center",
      "link": "https://example.com/event",
      "thumbnailUrl": "https://example.com/image.jpg"
    }
  ]
}
```

## SerpAPI Free Tier Limits

- **100 searches per month** (free tier)
- Upgrade available for more searches
- Check your usage at: https://serpapi.com/users/welcome

## Troubleshooting

### Error: "SerpApi:ApiKey not configured"
- Make sure you've set the API key in one of the configuration methods above
- Restart the application after making changes

### Error: "Failed to retrieve events from SerpAPI"
- Check your internet connection
- Verify your API key is correct
- Check if you've exceeded your monthly limit
- Verify the city name is spelled correctly

### No Events Returned
- Some cities might not have events in the Google Events index
- Try popular cities like: Austin, New York, Los Angeles, Chicago, Miami
- The API returns what Google Events has indexed for that location

## API Endpoint Details

### Request
```
GET /api/externalevents/city/{city}
```

**Parameters:**
- `city` (string, required): Name of the city to search for events

**Example:**
```
GET /api/externalevents/city/Austin
GET /api/externalevents/city/New%20York
```

### Response Structure

```typescript
{
  city: string;           // City searched
  totalEvents: number;    // Number of events found
  events: [
    {
      title: string;           // Event title
      description: string;     // Event description
      date: string;           // Formatted date information
      location: string;       // Event location/address
      link: string;          // Link to event details
      thumbnailUrl?: string; // Event image (if available)
    }
  ]
}
```

## Integration with Local Events

You can combine external events with your local database events:

1. Fetch external events for a city: `GET /api/externalevents/city/Austin`
2. Fetch local events for the same city: `GET /api/events/city/Austin`
3. Combine and display both in your frontend

This gives users a comprehensive view of:
- ✅ Events you're hosting/managing (local database)
- ✅ Other events happening in the area (external API)

## Rate Limiting Recommendations

To avoid hitting rate limits:

1. **Cache responses** - Store external event data temporarily
2. **Limit requests** - Don't call the API on every page load
3. **User-triggered only** - Make calls only when users explicitly search
4. **Background jobs** - Pre-fetch popular cities periodically

## Additional Resources

- SerpAPI Documentation: https://serpapi.com/google-events-api
- API Playground: https://serpapi.com/playground
- Support: https://serpapi.com/support
