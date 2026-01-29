using EventHub.Domain.DTO;

namespace EventHub.Service.Interface;

public interface IScheduleService
{
    Task<IEnumerable<ScheduleDto>> GetAllSchedulesAsync();
    Task<ScheduleDto?> GetScheduleByIdAsync(int id);
    Task<IEnumerable<ScheduleDto>> GetSchedulesByEventAsync(int eventId);
    Task<ScheduleDto> CreateScheduleAsync(CreateScheduleDto createScheduleDto);
    Task<ScheduleDto> UpdateScheduleAsync(int id, UpdateScheduleDto updateScheduleDto);
    Task DeleteScheduleAsync(int id);
}
