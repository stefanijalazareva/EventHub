using EventHub.Domain.DomainModels;
using EventHub.Domain.Interfaces;
using EventHub.Repository.Data;
using Microsoft.EntityFrameworkCore;

namespace EventHub.Repository.Implementation;

public class ScheduleRepository : Repository<Schedule>, IScheduleRepository
{
    public ScheduleRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Schedule>> GetSchedulesByEventAsync(int eventId)
    {
        return await _dbSet
            .Include(s => s.Event)
            .Where(s => s.EventId == eventId)
            .OrderBy(s => s.StartTime)
            .ToListAsync();
    }

    public override async Task<IEnumerable<Schedule>> GetAllAsync()
    {
        return await _dbSet
            .Include(s => s.Event)
            .ToListAsync();
    }

    public override async Task<Schedule?> GetByIdAsync(int id)
    {
        return await _dbSet
            .Include(s => s.Event)
            .FirstOrDefaultAsync(s => s.Id == id);
    }
}
