using QuanTriHeThongService.Domain.Entities.Systems;

namespace QuanTriHeThongService.Application.DTOs.Systems;

public static class RoleActionMapper
{
    public static RoleActionDto ToDto(this RoleActionEntity entity)
    {
        return new RoleActionDto
        {
            Id = entity.Id,
            STTSapXep = entity.STTSapXep,
            PhanLoai = entity.PhanLoai,
            Level = entity.Level,
            Role = entity.Role,
            ParentId = entity.ParentId,
            ParentTitle = entity.ParentTitle,
            Title = entity.Title,
            Controller = entity.Controller,
            Action = entity.Action,
            Parameter = entity.Parameter,
            Table = entity.Table,
            Status = entity.Status,
            UseGroup = entity.UseGroup,
            FrontendPath = entity.FrontendPath,
            IsVisibleInMenu = entity.IsVisibleInMenu,
            ClientApp = entity.ClientApp,
            MenuTitle = entity.MenuTitle,
            MenuIcon = entity.MenuIcon,
            Icon = entity.Icon
        };
    }

    public static RoleActionEntity ToEntity(this UpsertRoleActionRequest request)
    {
        return new RoleActionEntity
        {
            Id = request.Id ?? Guid.Empty,
            STTSapXep = request.STTSapXep,
            PhanLoai = request.PhanLoai,
            Role = request.Role,
            ParentId = request.ParentId,
            Title = request.Title,
            Controller = request.Controller,
            Action = request.Action,
            Parameter = request.Parameter,
            Table = request.Table,
            Status = request.Status,
            UseGroup = request.UseGroup,
            FrontendPath = request.FrontendPath,
            IsVisibleInMenu = request.IsVisibleInMenu,
            ClientApp = request.ClientApp,
            MenuTitle = request.MenuTitle,
            MenuIcon = request.MenuIcon,
            Icon = request.Icon
        };
    }
}

