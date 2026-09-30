namespace QuanTriHeThongService.Application.Common.Interfaces;

public interface IPermissionChecker
{
    Task<bool> HasPermissionAsync(
        string controller,
        string action,
        string permissionType,
        CancellationToken cancellationToken = default);

    Task<string> GetMenuActiveAsync(
        string controller,
        string action,
        CancellationToken cancellationToken = default);

    Task<string?> GetRoleAsync(
        string controller,
        string action,
        CancellationToken cancellationToken = default);
}

