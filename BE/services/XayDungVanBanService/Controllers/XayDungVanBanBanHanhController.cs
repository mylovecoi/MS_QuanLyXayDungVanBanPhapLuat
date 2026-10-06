using BuildingBlocks.Abstractions;
using Microsoft.AspNetCore.Mvc;
using XayDungVanBanService.Infrastructure.Authorization;

namespace XayDungVanBanService.Controllers;

[ApiController]
[Route("api/xay-dung-van-ban/ban-hanh")]
public sealed class XayDungVanBanBanHanhController(
    ICurrentUserContext user,
    IQuanTriHeThongPermissionClient permissionClient) : XayDungVanBanControllerBase(user, permissionClient)
{
    [HttpGet]
    public Task<ActionResult> Get(CancellationToken cancellationToken) =>
        IndexAsync("XayDungVanBanBanHanh", "VanBanQPPL.XayDungVanBan.BanHanh", cancellationToken);
}
