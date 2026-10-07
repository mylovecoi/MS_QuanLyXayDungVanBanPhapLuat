using BuildingBlocks.Abstractions;
using QuanTriHeThongService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using QuanTriHeThongService.Application.Common.Interfaces;

namespace QuanTriHeThongService.Infrastructure.Identity;

public sealed class DatabasePermissionChecker(
    QuanTriHeThongDbContext dbContext,
    ICurrentUserContext currentUserContext) : IPermissionChecker
{
    private readonly QuanTriHeThongDbContext _dbContext = dbContext;
    private readonly ICurrentUserContext _currentUserContext = currentUserContext;

    public async Task<bool> HasPermissionAsync(
        string controller,
        string action,
        string permissionType,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(controller) || string.IsNullOrWhiteSpace(action))
        {
            return false;
        }

        var contextInfo = await ResolveCurrentContextAsync(cancellationToken);
        if (!contextInfo.IsAuthenticated || contextInfo.UserId is null)
        {
            return false;
        }

        if (contextInfo.IsSSA)
        {
            return true;
        }

        if (contextInfo.GroupPermissionId is null || contextInfo.GroupPermissionId == Guid.Empty)
        {
            return false;
        }

        var mappedAction = MapActionName(action);
        var mappedPermission = MapPermissionName(permissionType);

        var permission = await BuildPermissionQuery(contextInfo.GroupPermissionId.Value)
            .Where(x => x.Controller == controller && (x.Action == mappedAction || x.Action == "Index"))
            .OrderByDescending(x => x.Action == mappedAction)
            .FirstOrDefaultAsync(cancellationToken);

        if (permission is null)
        {
            return false;
        }

        return mappedPermission switch
        {
            "Index" => permission.Index,
            "Create" => permission.Create,
            "Edit" => permission.Edit,
            "Delete" => permission.Delete,
            "Approve" => permission.Approve,
            "Public" => permission.Public,
            _ => false
        };
    }

    public async Task<string> GetMenuActiveAsync(
        string controller,
        string action,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(controller) || string.IsNullOrWhiteSpace(action))
        {
            return "menu_home";
        }

        var contextInfo = await ResolveCurrentContextAsync(cancellationToken);
        if (!contextInfo.IsAuthenticated || contextInfo.UserId is null)
        {
            return "menu_home";
        }

        if (contextInfo.IsSSA)
        {
            return controller + "_" + MapActionName(action);
        }

        if (contextInfo.GroupPermissionId is null || contextInfo.GroupPermissionId == Guid.Empty)
        {
            return "menu_home";
        }

        var mappedAction = action is "Create" or "Edit" or "Store" or "Update"
            ? "Index"
            : MapActionName(action);

        var permission = await BuildPermissionQuery(contextInfo.GroupPermissionId.Value)
            .FirstOrDefaultAsync(
                x => x.Controller == controller && x.Action == mappedAction,
                cancellationToken);

        return permission?.MenuActive ?? "menu_home";
    }

    public async Task<string?> GetRoleAsync(
        string controller,
        string action,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(controller) || string.IsNullOrWhiteSpace(action))
        {
            return null;
        }

        var contextInfo = await ResolveCurrentContextAsync(cancellationToken);
        if (!contextInfo.IsAuthenticated || contextInfo.UserId is null)
        {
            return null;
        }

        if (contextInfo.IsSSA)
        {
            return controller + "." + MapActionName(action);
        }

        if (contextInfo.GroupPermissionId is null || contextInfo.GroupPermissionId == Guid.Empty)
        {
            return null;
        }

        var mappedAction = action is "Create" or "Edit" or "Delete" or "Approve" or "Public"
            ? "Index"
            : MapActionName(action);

        var permission = await BuildPermissionQuery(contextInfo.GroupPermissionId.Value)
            .FirstOrDefaultAsync(
                x => x.Controller == controller && x.Action == mappedAction,
                cancellationToken);

        return permission?.Role;
    }

    private IQueryable<PermissionProjection> BuildPermissionQuery(Guid groupPermissionId)
    {
        return from permission in _dbContext.Permission.AsNoTracking()
               join roleAction in _dbContext.RoleActions.AsNoTracking()
                   on permission.RoleActionId equals roleAction.Id
               where permission.GroupPermissionId == groupPermissionId
                     && roleAction.Status == "Kích hoạt"
               select new PermissionProjection
               {
                   Controller = roleAction.Controller,
                   Action = roleAction.Action,
                   Role = roleAction.Role,
                   MenuActive = roleAction.Role.Replace(".", "_"),
                   Index = permission.Index,
                   Create = permission.Create,
                   Edit = permission.Edit,
                   Delete = permission.Delete,
                   Approve = permission.Approve,
                   Public = permission.Public
               };
    }

    private async Task<ResolvedCurrentUserContext> ResolveCurrentContextAsync(CancellationToken cancellationToken)
    {
        if (_currentUserContext.UserId is null || _currentUserContext.UserId == Guid.Empty)
        {
            return ResolvedCurrentUserContext.Empty;
        }

        if (_currentUserContext.IsSSA && _currentUserContext.GroupPermissionId is not null)
        {
            return new ResolvedCurrentUserContext(
                _currentUserContext.UserId,
                _currentUserContext.GroupPermissionId,
                true,
                _currentUserContext.IsAuthenticated);
        }

        var user = await _dbContext.Users.AsNoTracking()
            .Where(x => x.Id == _currentUserContext.UserId.Value)
            .Select(x => new
            {
                x.Id,
                x.GroupPermissionId,
                x.SSA,
                x.Status
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null || user.Status == "Khóa")
        {
            return ResolvedCurrentUserContext.Empty;
        }

        return new ResolvedCurrentUserContext(
            user.Id,
            user.GroupPermissionId,
            user.SSA,
            true);
    }

    private static string MapActionName(string actionName)
    {
        return actionName switch
        {
            "Store" => "Create",
            "Update" => "Edit",
            "Show" => "Index",
            "Chuyen" => "Approve",
            "Duyet" => "Approve",
            "HuyDuyet" => "Approve",
            "TraLai" => "Approve",
            "TiepNhan" => "Approve",
            "XacNhan" => "Approve",
            "HoanThanh" => "Approve",
            "CongBo" => "Public",
            "HuyCongBo" => "Public",
            _ => actionName
        };
    }

    private static string MapPermissionName(string permission)
    {
        return permission switch
        {
            "Store" => "Create",
            "Update" => "Edit",
            "Show" => "Index",
            _ => permission
        };
    }

    private sealed class PermissionProjection
    {
        public string? Controller { get; init; }
        public string? Action { get; init; }
        public string? Role { get; init; }
        public string MenuActive { get; init; } = "menu_home";
        public bool Index { get; init; }
        public bool Create { get; init; }
        public bool Edit { get; init; }
        public bool Delete { get; init; }
        public bool Approve { get; init; }
        public bool Public { get; init; }
    }

    private sealed record ResolvedCurrentUserContext(
        Guid? UserId,
        Guid? GroupPermissionId,
        bool IsSSA,
        bool IsAuthenticated)
    {
        public static ResolvedCurrentUserContext Empty => new(null, null, false, false);
    }
}

