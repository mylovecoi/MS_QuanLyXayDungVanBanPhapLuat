using QuanTriHeThongService.Domain.Entities.Systems;

namespace QuanTriHeThongService.Application.DTOs.Systems;

public static class UserAccountMapper
{
    public static UserAccountDto ToDto(this UserAccountEntity entity)
    {
        return new UserAccountDto
        {
            Id = entity.Id,
            Username = entity.Username,
            Email = entity.Email,
            Name = entity.Name,
            Status = entity.Status,
            Content = entity.Content,
            GroupPermissionId = entity.GroupPermissionId,
            GroupPermissionName = entity.GroupPermissionName,
            FirstLogin = entity.FirstLogin,
            Level = entity.Level
        };
    }
}

