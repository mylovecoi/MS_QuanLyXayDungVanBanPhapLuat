using QuanTriHeThongService.Domain.Entities.Systems;

namespace QuanTriHeThongService.Application.DTOs.Systems;

public static class GroupPermissionMapper
{
    public static GroupPermissionDto ToDto(this GroupPermissionEntity entity)
    {
        return new GroupPermissionDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            Status = entity.Status
        };
    }

    public static PermissionDto ToDto(this PermissionEntity entity)
    {
        return new PermissionDto
        {
            Id = entity.Id,
            GroupPermissionId = entity.GroupPermissionId,
            RoleActionId = entity.RoleActionId,
            Index = entity.Index,
            Create = entity.Create,
            Edit = entity.Edit,
            Delete = entity.Delete,
            Approve = entity.Approve,
            Public = entity.Public,
            PhanLoai = entity.PhanLoai,
            RoleActionGroupId = entity.RoleActionGroupId,
            Level = entity.Level,
            STTSapXep = entity.STTSapXep,
            Title = entity.Title,
            Role = entity.Role,
            Controller = entity.Controller,
            ActionName = entity.ActionName,
            Table = entity.Table,
            Icon = entity.Icon
        };
    }

    public static OptionItemDto ToDto(this OptionItemEntity entity)
    {
        return new OptionItemDto
        {
            Value = entity.Value,
            DisplayName = entity.DisplayName
        };
    }
}

