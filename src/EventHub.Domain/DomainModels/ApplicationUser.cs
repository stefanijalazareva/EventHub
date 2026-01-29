using Microsoft.AspNetCore.Identity;

namespace EventHub.Domain.DomainModels;

public class ApplicationUser : IdentityUser
{
    public int? AttendeeId { get; set; }
    public Attendee? Attendee { get; set; }
}
