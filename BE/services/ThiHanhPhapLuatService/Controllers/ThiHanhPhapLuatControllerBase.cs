using BuildingBlocks.Abstractions;
using Microsoft.AspNetCore.Mvc;
using ThiHanhPhapLuatService.Infrastructure.Authorization;

namespace ThiHanhPhapLuatService.Controllers;

public abstract class ThiHanhPhapLuatControllerBase(
    ICurrentUserContext currentUser,
    IQuanTriHeThongPermissionClient permissionClient) : ControllerBase
{
    protected ICurrentUserContext CurrentUser => currentUser;

    protected async Task<ActionResult?> EnsurePermissionAsync(string controller, string action, string permissionType, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.UserId is null) return Unauthorized();
        return await permissionClient.HasPermissionAsync(controller, action, permissionType, cancellationToken) ? null : Forbid();
    }
}
