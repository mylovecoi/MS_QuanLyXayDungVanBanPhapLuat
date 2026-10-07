namespace KhaiThacDuLieuService.Infrastructure.Authorization;

public interface IQuanTriHeThongPermissionClient
{
    Task<bool> HasPermissionAsync(
        string controller,
        string action,
        string permissionType,
        CancellationToken cancellationToken = default);
}
