using EventHub.Domain.DTO;

namespace EventHub.Service.Interface;

public interface IAttendeeService
{
    Task<IEnumerable<AttendeeDto>> GetAllAttendeesAsync();
    Task<AttendeeDto?> GetAttendeeByIdAsync(int id);
    Task<AttendeeDto> CreateAttendeeAsync(CreateAttendeeDto createAttendeeDto);
    Task<AttendeeDto> UpdateAttendeeAsync(int id, UpdateAttendeeDto updateAttendeeDto);
    Task DeleteAttendeeAsync(int id);
}
