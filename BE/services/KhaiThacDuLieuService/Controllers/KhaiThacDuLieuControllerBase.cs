using BuildingBlocks.Abstractions;
using KhaiThacDuLieuService.Infrastructure.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KhaiThacDuLieuService.Controllers;

public abstract class KhaiThacDuLieuControllerBase(
    ICurrentUserContext currentUser,
    IQuanTriHeThongPermissionClient permissionClient) : ControllerBase
{
    protected ICurrentUserContext CurrentUser => currentUser;

    protected async Task<ActionResult?> EnsurePermissionAsync(
        string controller,
        string permissionType,
        CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.UserId is null)
        {
            return Unauthorized();
        }

        return await permissionClient.HasPermissionAsync(controller, "Index", permissionType, cancellationToken)
            ? null
            : Forbid();
    }
}
