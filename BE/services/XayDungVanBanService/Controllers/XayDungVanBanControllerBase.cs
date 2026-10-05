using BuildingBlocks.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace XayDungVanBanService.Controllers;

public abstract class XayDungVanBanControllerBase(ICurrentUserContext currentUser) : ControllerBase
{
    protected ActionResult Index(string role) => currentUser.IsAuthenticated
        ? Ok(new { role, userId = currentUser.UserId, donViId = currentUser.DonViId })
        : Unauthorized();
}
