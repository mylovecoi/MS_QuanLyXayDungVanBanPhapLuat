using BuildingBlocks.Abstractions;
using Microsoft.AspNetCore.Mvc;
using XayDungVanBanService.Infrastructure.Authorization;

namespace XayDungVanBanService.Controllers;

public abstract class XayDungVanBanControllerBase(
    ICurrentUserContext currentUser,
    IQuanTriHeThongPermissionClient permissionClient) : ControllerBase
{
    protected async Task<ActionResult> IndexAsync(
        string controller,
        string role,
        CancellationToken cancellationToken)
    {
        var accessResult = await EnsurePermissionAsync(controller, "Index", "Index", cancellationToken);
        return accessResult ?? Ok(new { role, userId = currentUser.UserId, donViId = currentUser.DonViId });
    }

    protected async Task<ActionResult?> EnsurePermissionAsync(
        string controller,
        string action,
        string permissionType,
        CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.UserId is null)
        {
            return Unauthorized();
        }

        return await permissionClient.HasPermissionAsync(controller, action, permissionType, cancellationToken)
            ? null
            : Forbid();
    }
}
