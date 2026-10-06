using BuildingBlocks.Abstractions;
using Microsoft.AspNetCore.Mvc;
using XayDungVanBanService.Infrastructure.Authorization;

namespace XayDungVanBanService.Controllers;

[ApiController]
[Route("api/xay-dung-van-ban/tham-tra-hdnd")]
public sealed class XayDungVanBanThamTraHdndController(
    ICurrentUserContext user,
    IQuanTriHeThongPermissionClient permissionClient) : XayDungVanBanControllerBase(user, permissionClient)
{
    [HttpGet]
    public Task<ActionResult> Get(CancellationToken cancellationToken) =>
        IndexAsync("XayDungVanBanThamTraHdnd", "VanBanQPPL.XayDungVanBan.ThamTraHdnd", cancellationToken);
}
