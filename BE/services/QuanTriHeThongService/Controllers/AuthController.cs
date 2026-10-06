using BuildingBlocks.Abstractions;
using QuanTriHeThongService.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanTriHeThongService.Application.Common.Interfaces;
using QuanTriHeThongService.Contracts.Requests.Auth;
using QuanTriHeThongService.Contracts.Responses;
using QuanTriHeThongService.Contracts.Responses.Auth;

namespace QuanTriHeThongService.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(
    QuanTriHeThongDbContext dbContext,
    ICurrentUserContext currentUserContext,
    IJwtTokenService jwtTokenService) : ControllerBase
{
    private readonly QuanTriHeThongDbContext _dbContext = dbContext;
    private readonly ICurrentUserContext _currentUserContext = currentUserContext;
    private readonly IJwtTokenService _jwtTokenService = jwtTokenService;

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<LoginApiResponse>>> Login(
        [FromBody] LoginApiRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new ApiResponse<LoginApiResponse>
                {
                    IsSuccess = false,
                    Message = "Tên đăng nhập và mật khẩu không được để trống."
                });
            }

            var systemInfo = await _dbContext.SystemInfo
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .FirstOrDefaultAsync(cancellationToken);
            var loginLimit = systemInfo?.LoginLock ?? 5;

            var user = await _dbContext.Users
                .FirstOrDefaultAsync(x => x.Username == request.Username || x.Email == request.Username, cancellationToken);

            if (user == null)
            {
                return BadRequest(new ApiResponse<LoginApiResponse>
                {
                    IsSuccess = false,
                    Message = "Tài khoản hoặc mật khẩu không đúng."
                });
            }

            if (string.Equals(user.Status, "Khóa", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new ApiResponse<LoginApiResponse>
                {
                    IsSuccess = false,
                    Message = "Tài khoản của bạn đã bị khóa. Vui lòng liên hệ quản trị viên."
                });
            }

            if (string.Equals(user.Status, "Chờ kích hoạt", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new ApiResponse<LoginApiResponse>
                {
                    IsSuccess = false,
                    Message = "Tài khoản của bạn chưa được kích hoạt. Vui lòng liên hệ quản trị viên."
                });
            }

            if (!global::BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                user.LoginCount++;
                if (user.LoginCount >= loginLimit)
                {
                    user.Status = "Khóa";
                }

                await _dbContext.SaveChangesAsync(cancellationToken);

                return BadRequest(new ApiResponse<LoginApiResponse>
                {
                    IsSuccess = false,
                    Message = user.Status == "Khóa"
                        ? "Tài khoản của bạn đã bị khóa do đăng nhập sai quá số lần cho phép."
                        : "Tài khoản hoặc mật khẩu không đúng."
                });
            }

            var wasFirstLogin = user.FirstLogin;
            user.LoginCount = 0;
            user.FirstLogin = false;
            await _dbContext.SaveChangesAsync(cancellationToken);

            var (accessToken, expiresAt) = _jwtTokenService.CreateAccessToken(user);

            return Ok(new ApiResponse<LoginApiResponse>
            {
                IsSuccess = true,
                Message = "Đăng nhập thành công.",
                Data = new LoginApiResponse
                {
                    UserId = user.Id,
                    Username = user.Username,
                    DisplayName = user.Name,
                    DonViId = user.DanhMucDonViId,
                    GroupPermissionId = user.GroupPermissionId,
                    IsSSA = user.SSA,
                    FirstLogin = wasFirstLogin,
                    MustChangePassword = string.Equals(request.Password, "Cs@2012!", StringComparison.Ordinal),
                    AccessToken = accessToken,
                    ExpiresAt = expiresAt
                }
            });
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            return StatusCode(StatusCodes.Status408RequestTimeout, new ApiResponse<LoginApiResponse>
            {
                IsSuccess = false,
                Message = "Yêu cầu đăng nhập đã bị hủy trong quá trình xử lý."
            });
        }
    }

    [HttpGet("frontend-menu")]
    public async Task<ActionResult<ApiResponse<FrontendMenuApiResponse>>> GetFrontendMenu(CancellationToken cancellationToken = default)
    {
        var currentUser = await ResolveCurrentUserAsync(cancellationToken);
        if (currentUser is null)
        {
            return Ok(new ApiResponse<FrontendMenuApiResponse>
            {
                IsSuccess = true,
                Message = "Người dùng chưa đăng nhập.",
                Data = new FrontendMenuApiResponse()
            });
        }

        var menuItems = await BuildFrontendMenuItemsAsync(currentUser.Value, cancellationToken);
        var normalizedMenuItems = NormalizeFrontendMenuItems(menuItems);

        return Ok(new ApiResponse<FrontendMenuApiResponse>
        {
            IsSuccess = true,
            Message = "Lấy danh sách chức năng thành công.",
            Data = new FrontendMenuApiResponse
            {
                Items = BuildMenuTree(normalizedMenuItems)
            }
        });
    }

    [HttpGet("frontend-modules")]
    public async Task<ActionResult<ApiResponse<FrontendModuleApiResponse>>> GetFrontendModules(CancellationToken cancellationToken = default)
    {
        var currentUser = await ResolveCurrentUserAsync(cancellationToken);
        if (currentUser is null)
        {
            return Ok(new ApiResponse<FrontendModuleApiResponse>
            {
                IsSuccess = true,
                Message = "Người dùng chưa đăng nhập.",
                Data = new FrontendModuleApiResponse()
            });
        }

        var modules = await BuildFrontendModulesAsync(currentUser.Value, cancellationToken);
        return Ok(new ApiResponse<FrontendModuleApiResponse>
        {
            IsSuccess = true,
            Message = "Lấy danh sách module quyền thành công.",
            Data = new FrontendModuleApiResponse
            {
                Items = modules
            }
        });
    }

    private async Task<(Guid UserId, Guid GroupPermissionId, bool SSA)?> ResolveCurrentUserAsync(CancellationToken cancellationToken)
    {
        if (_currentUserContext.UserId is null || _currentUserContext.UserId == Guid.Empty)
        {
            return null;
        }

        var user = await _dbContext.Users.AsNoTracking()
            .Where(x => x.Id == _currentUserContext.UserId.Value && x.Status != "Khóa")
            .Select(x => new
            {
                x.Id,
                x.GroupPermissionId,
                x.SSA
            })
            .FirstOrDefaultAsync(cancellationToken);

        return user is null ? null : (user.Id, user.GroupPermissionId, user.SSA);
    }

    private async Task<List<FrontendMenuItemApiResponse>> BuildFrontendMenuItemsAsync(
        (Guid UserId, Guid GroupPermissionId, bool SSA) currentUser,
        CancellationToken cancellationToken)
    {
        List<FrontendMenuItemApiResponse> menuItems;

        if (currentUser.SSA)
        {
            var rawRoles = await _dbContext.RoleActions.AsNoTracking()
                .Where(role => role.IsVisibleInMenu)
                .OrderBy(role => role.Level)
                .ThenBy(role => role.STTSapXep)
                .Select(role => new
                {
                    role.Id,
                    role.RoleGroupId,
                    role.Role,
                    role.MenuTitle,
                    role.Title,
                    role.FrontendPath,
                    role.PhanLoai,
                    role.Level,
                    role.STTSapXep,
                    role.Controller,
                    role.Action,
                    role.Parameter,
                    role.MenuIcon,
                    role.Icon,
                    role.ClientApp,
                    role.Status
                })
                .ToListAsync(cancellationToken);

            menuItems = rawRoles
                .Where(role => IsFrontendClientApp(role.ClientApp))
                .Where(role => IsActiveStatus(role.Status))
                .Select(role => new FrontendMenuItemApiResponse
                {
                    RoleActionId = role.Id,
                    ParentRoleActionId = role.Level == 0 ? null : role.RoleGroupId,
                    Role = role.Role,
                    Title = ResolveMenuTitle(role.MenuTitle, role.Title, role.Role),
                    Url = ResolveMenuUrl(role.ClientApp, role.FrontendPath),
                    PhanLoai = role.PhanLoai,
                    Level = role.Level,
                    STTSapXep = role.STTSapXep,
                    Controller = role.Controller,
                    Action = role.Action,
                    Parameter = role.Parameter,
                    Icon = ResolveMenuIcon(role.MenuIcon, role.Icon),
                    ClientApp = role.ClientApp
                })
                .ToList();
        }
        else
        {
            var rawRoles = await (
                from permission in _dbContext.Permission.AsNoTracking()
                join role in _dbContext.RoleActions.AsNoTracking() on permission.RoleActionId equals role.Id
                where permission.GroupPermissionId == currentUser.GroupPermissionId
                      && permission.Index
                      && role.IsVisibleInMenu
                orderby role.Level, role.STTSapXep
                select new
                {
                    role.Id,
                    role.RoleGroupId,
                    role.Role,
                    role.MenuTitle,
                    role.Title,
                    role.FrontendPath,
                    role.PhanLoai,
                    role.Level,
                    role.STTSapXep,
                    role.Controller,
                    role.Action,
                    role.Parameter,
                    role.MenuIcon,
                    role.Icon,
                    role.ClientApp,
                    role.Status
                })
                .ToListAsync(cancellationToken);

            menuItems = rawRoles
                .Where(role => IsFrontendClientApp(role.ClientApp))
                .Where(role => IsActiveStatus(role.Status))
                .Select(role => new FrontendMenuItemApiResponse
                {
                    RoleActionId = role.Id,
                    ParentRoleActionId = role.Level == 0 ? null : role.RoleGroupId,
                    Role = role.Role,
                    Title = ResolveMenuTitle(role.MenuTitle, role.Title, role.Role),
                    Url = ResolveMenuUrl(role.ClientApp, role.FrontendPath),
                    PhanLoai = role.PhanLoai,
                    Level = role.Level,
                    STTSapXep = role.STTSapXep,
                    Controller = role.Controller,
                    Action = role.Action,
                    Parameter = role.Parameter,
                    Icon = ResolveMenuIcon(role.MenuIcon, role.Icon),
                    ClientApp = role.ClientApp
                })
                .ToList();
        }

        return menuItems
            .GroupBy(item => item.RoleActionId)
            .Select(group => group.First())
            .OrderBy(item => item.Level)
            .ThenBy(item => item.STTSapXep)
            .ToList();
    }

    private static List<FrontendMenuItemApiResponse> NormalizeFrontendMenuItems(List<FrontendMenuItemApiResponse> menuItems)
    {
        return RemoveEmptyGroupNodes(menuItems)
            .GroupBy(item => item.RoleActionId)
            .Select(group => group.First())
            .OrderBy(item => item.Level)
            .ThenBy(item => item.STTSapXep)
            .ToList();
    }

    private static string ResolveMenuTitle(string? menuTitle, string? title, string role)
    {
        if (!string.IsNullOrWhiteSpace(menuTitle))
        {
            return menuTitle.Trim();
        }

        if (!string.IsNullOrWhiteSpace(title))
        {
            return title.Trim();
        }

        return role;
    }

    private static string? ResolveMenuUrl(string? clientApp, string? frontendPath)
    {
        if (!string.IsNullOrWhiteSpace(frontendPath)
            && (string.IsNullOrWhiteSpace(clientApp)
                || string.Equals(clientApp, "frontend", StringComparison.OrdinalIgnoreCase)
                || string.Equals(clientApp, "both", StringComparison.OrdinalIgnoreCase)))
        {
            return frontendPath.Trim();
        }

        return null;
    }

    private static string? ResolveMenuIcon(string? menuIcon, string? icon)
    {
        if (!string.IsNullOrWhiteSpace(menuIcon))
        {
            return menuIcon.Trim();
        }

        return string.IsNullOrWhiteSpace(icon) ? null : icon.Trim();
    }

    private static bool IsActiveStatus(string? status)
    {
        if (string.IsNullOrWhiteSpace(status))
        {
            return false;
        }

        var normalizedStatus = NormalizeVietnameseText(status);
        return string.Equals(normalizedStatus, "kich hoat", StringComparison.OrdinalIgnoreCase);
    }

    private static string NormalizeVietnameseText(string value)
    {
        var normalized = value.Trim().Normalize(System.Text.NormalizationForm.FormD);
        var builder = new System.Text.StringBuilder(normalized.Length);

        foreach (var ch in normalized)
        {
            var unicodeCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(ch);
            if (unicodeCategory == System.Globalization.UnicodeCategory.NonSpacingMark)
            {
                continue;
            }

            builder.Append(ch);
        }

        return builder
            .ToString()
            .Normalize(System.Text.NormalizationForm.FormC)
            .Replace('đ', 'd')
            .Replace('Đ', 'D');
    }

    private static List<FrontendMenuItemApiResponse> BuildMenuTree(List<FrontendMenuItemApiResponse> items)
    {
        var rootItems = items
            .Where(item => item.ParentRoleActionId is null)
            .OrderBy(item => item.STTSapXep)
            .ThenBy(item => item.Title, StringComparer.OrdinalIgnoreCase)
            .ToList();

        var itemsByParent = items
            .Where(item => item.ParentRoleActionId.HasValue)
            .GroupBy(item => item.ParentRoleActionId!.Value)
            .ToDictionary(
                group => group.Key,
                group => group.OrderBy(item => item.STTSapXep)
                    .ThenBy(item => item.Title, StringComparer.OrdinalIgnoreCase)
                    .ToList());

        List<FrontendMenuItemApiResponse> Build(Guid parentId)
        {
            if (!itemsByParent.TryGetValue(parentId, out var children))
            {
                return [];
            }

            foreach (var child in children)
            {
                child.Children = Build(child.RoleActionId);
            }

            return children;
        }

        foreach (var rootItem in rootItems)
        {
            rootItem.Children = Build(rootItem.RoleActionId);
        }

        return rootItems;
    }

    private static List<FrontendMenuItemApiResponse> RemoveEmptyGroupNodes(List<FrontendMenuItemApiResponse> menuItems)
    {
        var normalized = menuItems.ToList();
        var removedAny = true;

        while (removedAny)
        {
            removedAny = false;
            var parentIds = normalized
                .Where(item => item.ParentRoleActionId.HasValue)
                .Select(item => item.ParentRoleActionId!.Value)
                .ToHashSet();

            var removableIds = normalized
                .Where(item =>
                    string.Equals(item.PhanLoai, "Group", StringComparison.OrdinalIgnoreCase) &&
                    string.IsNullOrWhiteSpace(item.Url) &&
                    !parentIds.Contains(item.RoleActionId))
                .Select(item => item.RoleActionId)
                .ToHashSet();

            if (removableIds.Count == 0)
            {
                continue;
            }

            normalized.RemoveAll(item => removableIds.Contains(item.RoleActionId));
            removedAny = true;
        }

        return normalized;
    }

    private async Task<List<FrontendModuleItemApiResponse>> BuildFrontendModulesAsync(
        (Guid UserId, Guid GroupPermissionId, bool SSA) currentUser,
        CancellationToken cancellationToken)
    {
        var rawRoles = await _dbContext.RoleActions.AsNoTracking()
            .Where(role => !string.IsNullOrWhiteSpace(role.FrontendPath) || role.IsVisibleInMenu)
            .Select(role => new
            {
                role.Id,
                role.Role,
                role.Title,
                role.MenuTitle,
                role.FrontendPath,
                role.IsVisibleInMenu,
                role.ClientApp,
                role.PhanLoai,
                role.Level,
                role.Status
            })
            .ToListAsync(cancellationToken);

        var activeRoles = rawRoles
            .Where(role => IsFrontendClientApp(role.ClientApp))
            .Where(role => IsActiveStatus(role.Status))
            .ToList();

        var permissionMap = new Dictionary<Guid, FrontendModuleItemApiResponse>();
        if (currentUser.SSA)
        {
            foreach (var role in activeRoles)
            {
                permissionMap[role.Id] = new FrontendModuleItemApiResponse
                {
                    Key = role.Role,
                    Role = role.Role,
                    Title = ResolveMenuTitle(role.MenuTitle, role.Title, role.Role),
                    Url = ResolveMenuUrl(role.ClientApp, role.FrontendPath),
                    IsVisibleInMenu = role.IsVisibleInMenu,
                    ClientApp = role.ClientApp,
                    CanAccess = true,
                    Index = true,
                    Create = true,
                    Edit = true,
                    Delete = true,
                    Approve = true,
                    Public = true
                };
            }
        }
        else
        {
            var roleIds = activeRoles.Select(role => role.Id).ToList();
            var permissions = await _dbContext.Permission.AsNoTracking()
                .Where(permission => permission.GroupPermissionId == currentUser.GroupPermissionId
                                     && roleIds.Contains(permission.RoleActionId))
                .Select(permission => new
                {
                    permission.RoleActionId,
                    permission.Index,
                    permission.Create,
                    permission.Edit,
                    permission.Delete,
                    permission.Approve,
                    permission.Public
                })
                .ToListAsync(cancellationToken);

            var permissionByRoleId = permissions.ToDictionary(permission => permission.RoleActionId);
            foreach (var role in activeRoles)
            {
                permissionByRoleId.TryGetValue(role.Id, out var permission);
                permissionMap[role.Id] = new FrontendModuleItemApiResponse
                {
                    Key = role.Role,
                    Role = role.Role,
                    Title = ResolveMenuTitle(role.MenuTitle, role.Title, role.Role),
                    Url = ResolveMenuUrl(role.ClientApp, role.FrontendPath),
                    IsVisibleInMenu = role.IsVisibleInMenu,
                    ClientApp = role.ClientApp,
                    CanAccess = permission?.Index ?? false,
                    Index = permission?.Index ?? false,
                    Create = permission?.Create ?? false,
                    Edit = permission?.Edit ?? false,
                    Delete = permission?.Delete ?? false,
                    Approve = permission?.Approve ?? false,
                    Public = permission?.Public ?? false
                };
            }
        }

        return activeRoles
            .Select(role => new
            {
                role.Role,
                role.PhanLoai,
                role.Level,
                Module = permissionMap[role.Id]
            })
            .GroupBy(item => item.Role, StringComparer.OrdinalIgnoreCase)
            .Select(group => group
                .OrderByDescending(item => !string.IsNullOrWhiteSpace(item.Module.Url))
                .ThenBy(item => string.Equals(item.PhanLoai, "Group", StringComparison.OrdinalIgnoreCase) ? 1 : 0)
                .ThenByDescending(item => PermissionWeight(item.Module))
                .ThenByDescending(item => item.Level)
                .Select(item => item.Module)
                .First())
            .OrderBy(item => item.Title, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static int PermissionWeight(FrontendModuleItemApiResponse module)
    {
        var score = 0;
        if (module.Index) score += 16;
        if (module.Create) score += 8;
        if (module.Edit) score += 4;
        if (module.Delete) score += 2;
        if (module.Approve) score += 1;
        return score;
    }

    private static bool IsFrontendClientApp(string? clientApp)
    {
        if (string.IsNullOrWhiteSpace(clientApp))
        {
            return true;
        }

        return string.Equals(clientApp, "frontend", StringComparison.OrdinalIgnoreCase)
               || string.Equals(clientApp, "both", StringComparison.OrdinalIgnoreCase);
    }
}

