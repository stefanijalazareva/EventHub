using EventHub.Domain.DomainModels;

namespace EventHub.Domain.Interfaces;

public interface IAttendeeRepository : IRepository<Attendee>
{
    Task<Attendee?> GetByEmailAsync(string email);
    Task<IEnumerable<Attendee>> SearchByNameAsync(string name);
}
