using EventHub.Domain.DTO;
using EventHub.Service.Interface;
using EventHub.Domain.DomainModels;
using EventHub.Domain.Interfaces;

namespace EventHub.Service.Implementation;

public class ScheduleService : IScheduleService
{
    private readonly IUnitOfWork _unitOfWork;

    public ScheduleService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<ScheduleDto>> GetAllSchedulesAsync()
    {
        var schedules = await _unitOfWork.Schedules.GetAllAsync();
        return schedules.Select(MapToDto);
    }

    public async Task<ScheduleDto?> GetScheduleByIdAsync(int id)
    {
        var schedule = await _unitOfWork.Schedules.GetByIdAsync(id);
        return schedule != null ? MapToDto(schedule) : null;
    }

    public async Task<IEnumerable<ScheduleDto>> GetSchedulesByEventAsync(int eventId)
    {
        var schedules = await _unitOfWork.Schedules.GetSchedulesByEventAsync(eventId);
        return schedules.Select(MapToDto);
    }

    public async Task<ScheduleDto> CreateScheduleAsync(CreateScheduleDto createScheduleDto)
    {
        // Validate event exists
        var eventExists = await _unitOfWork.Events.ExistsAsync(createScheduleDto.EventId);
        if (!eventExists)
        {
            throw new KeyNotFoundException($"Event with ID {createScheduleDto.EventId} not found.");
        }

        var schedule = new Schedule
        {
            EventId = createScheduleDto.EventId,
            Title = createScheduleDto.Title,
            Description = createScheduleDto.Description,
            StartTime = createScheduleDto.StartTime,
            EndTime = createScheduleDto.EndTime,
            Location = createScheduleDto.Location,
            Speaker = createScheduleDto.Speaker
        };

        var createdSchedule = await _unitOfWork.Schedules.AddAsync(schedule);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(createdSchedule);
    }

    public async Task<ScheduleDto> UpdateScheduleAsync(int id, UpdateScheduleDto updateScheduleDto)
    {
        var schedule = await _unitOfWork.Schedules.GetByIdAsync(id);
        if (schedule == null)
        {
            throw new KeyNotFoundException($"Schedule with ID {id} not found.");
        }

        schedule.Title = updateScheduleDto.Title;
        schedule.Description = updateScheduleDto.Description;
        schedule.StartTime = updateScheduleDto.StartTime;
        schedule.EndTime = updateScheduleDto.EndTime;
        schedule.Location = updateScheduleDto.Location;
        schedule.Speaker = updateScheduleDto.Speaker;

        await _unitOfWork.Schedules.UpdateAsync(schedule);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(schedule);
    }

    public async Task DeleteScheduleAsync(int id)
    {
        await _unitOfWork.Schedules.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }

    private static ScheduleDto MapToDto(Schedule schedule)
    {
        return new ScheduleDto
        {
            Id = schedule.Id,
            EventId = schedule.EventId,
            EventName = schedule.Event?.Name ?? string.Empty,
            Title = schedule.Title,
            Description = schedule.Description,
            StartTime = schedule.StartTime,
            EndTime = schedule.EndTime,
            Location = schedule.Location,
            Speaker = schedule.Speaker
        };
    }
}
