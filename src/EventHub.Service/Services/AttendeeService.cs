using EventHub.Domain.DTO;
using EventHub.Service.Interface;
using EventHub.Domain.DomainModels;
using EventHub.Domain.Interfaces;

namespace EventHub.Service.Implementation;

public class AttendeeService : IAttendeeService
{
    private readonly IUnitOfWork _unitOfWork;

    public AttendeeService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<AttendeeDto>> GetAllAttendeesAsync()
    {
        var attendees = await _unitOfWork.Attendees.GetAllAsync();
        return attendees.Select(MapToDto);
    }

    public async Task<AttendeeDto?> GetAttendeeByIdAsync(int id)
    {
        var attendee = await _unitOfWork.Attendees.GetByIdAsync(id);
        return attendee != null ? MapToDto(attendee) : null;
    }

    public async Task<AttendeeDto> CreateAttendeeAsync(CreateAttendeeDto createAttendeeDto)
    {
        var attendee = new Attendee
        {
            FirstName = createAttendeeDto.FirstName,
            LastName = createAttendeeDto.LastName,
            Email = createAttendeeDto.Email,
            PhoneNumber = createAttendeeDto.PhoneNumber,
            DateOfBirth = createAttendeeDto.DateOfBirth,
            Address = createAttendeeDto.Address,
            RegisteredAt = DateTime.UtcNow
        };

        var createdAttendee = await _unitOfWork.Attendees.AddAsync(attendee);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(createdAttendee);
    }

    public async Task<AttendeeDto> UpdateAttendeeAsync(int id, UpdateAttendeeDto updateAttendeeDto)
    {
        var attendee = await _unitOfWork.Attendees.GetByIdAsync(id);
        if (attendee == null)
        {
            throw new KeyNotFoundException($"Attendee with ID {id} not found.");
        }

        attendee.FirstName = updateAttendeeDto.FirstName;
        attendee.LastName = updateAttendeeDto.LastName;
        attendee.Email = updateAttendeeDto.Email;
        attendee.PhoneNumber = updateAttendeeDto.PhoneNumber;
        attendee.DateOfBirth = updateAttendeeDto.DateOfBirth;
        attendee.Address = updateAttendeeDto.Address;

        await _unitOfWork.Attendees.UpdateAsync(attendee);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(attendee);
    }

    public async Task DeleteAttendeeAsync(int id)
    {
        await _unitOfWork.Attendees.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }

    private static AttendeeDto MapToDto(Attendee attendee)
    {
        return new AttendeeDto
        {
            Id = attendee.Id,
            FirstName = attendee.FirstName,
            LastName = attendee.LastName,
            Email = attendee.Email,
            PhoneNumber = attendee.PhoneNumber,
            DateOfBirth = attendee.DateOfBirth,
            Address = attendee.Address,
            RegisteredAt = attendee.RegisteredAt
        };
    }
}
