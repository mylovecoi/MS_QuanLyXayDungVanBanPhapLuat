using QuanTriHeThongService.Domain.Entities.Systems;

namespace QuanTriHeThongService.Application.DTOs.Systems;

public static class LogEntryMapper
{
    public static LogEntryDto ToDto(this LogEntryEntity entity)
    {
        return new LogEntryDto
        {
            Id = entity.Id,
            Username = entity.Username,
            IpAddress = entity.IpAddress,
            Url = entity.Url,
            Method = entity.Method,
            Request = entity.Request,
            CreatedDate = entity.CreatedDate
        };
    }
}

