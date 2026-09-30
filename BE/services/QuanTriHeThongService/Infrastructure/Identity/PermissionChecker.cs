using BuildingBlocks.Abstractions;
using QuanTriHeThongService.Application.Common.Interfaces;

namespace QuanTriHeThongService.Infrastructure.Identity;

public sealed class PermissionChecker(ICurrentUserContext currentUserContext) : IPermissionChecker
{
    private readonly ICurrentUserContext _currentUserContext = currentUserContext;

    public Task<bool> HasPermissionAsync(
        string controller,
        string action,
        string permissionType,
        CancellationToken cancellationToken = default)
    {
        if (_currentUserContext.IsSSA)
        {
            return Task.FromResult(true);
        }

        if (!_currentUserContext.IsAuthenticated || _currentUserContext.UserId is null)
        {
            return Task.FromResult(false);
        }

        // Skeleton phase:
        // Tạm thời chỉ trả false cho user thường để tránh khẳng định sai quyền.
        // Bước refactor tiếp theo sẽ thay bằng truy vấn DB/cache theo UserId + GroupPermissionId.
        return Task.FromResult(false);
    }

    public Task<string> GetMenuActiveAsync(
        string controller,
        string action,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult("menu_home");
    }

    public Task<string?> GetRoleAsync(
        string controller,
        string action,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult<string?>(null);
    }
}

