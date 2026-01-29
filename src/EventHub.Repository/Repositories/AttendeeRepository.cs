using EventHub.Domain.DomainModels;
using EventHub.Domain.Interfaces;
using EventHub.Repository.Data;
using Microsoft.EntityFrameworkCore;

namespace EventHub.Repository.Implementation;

public class AttendeeRepository : Repository<Attendee>, IAttendeeRepository
{
    public AttendeeRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Attendee?> GetByEmailAsync(string email)
    {
        return await _dbSet
            .FirstOrDefaultAsync(a => a.Email.ToLower() == email.ToLower());
    }

    public async Task<IEnumerable<Attendee>> SearchByNameAsync(string name)
    {
        return await _dbSet
            .Where(a => a.FirstName.Contains(name) || a.LastName.Contains(name))
            .ToListAsync();
    }
}
