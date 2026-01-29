using EventHub.Domain.DomainModels;

namespace EventHub.Domain.Interfaces;

public interface IScheduleRepository : IRepository<Schedule>
{
    Task<IEnumerable<Schedule>> GetSchedulesByEventAsync(int eventId);
}
