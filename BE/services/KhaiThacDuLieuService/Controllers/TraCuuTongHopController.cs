using BuildingBlocks.Abstractions;
using KhaiThacDuLieuService.Application.Abstractions;
using KhaiThacDuLieuService.Application.DTOs;
using KhaiThacDuLieuService.Infrastructure.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KhaiThacDuLieuService.Controllers;

[ApiController]
[Route("api/khai-thac-du-lieu/tra-cuu/tong-hop")]
public sealed class TraCuuTongHopController(
    ICurrentUserContext user,
    IQuanTriHeThongPermissionClient permissionClient,
    ITraCuuKhaiThacDuLieuService traCuuService) : KhaiThacDuLieuControllerBase(user, permissionClient)
{
    [HttpGet]
    public async Task<ActionResult<PagedResultDto<TraCuuTongHopItemDto>>> Get([FromQuery] TraCuuRequest request, CancellationToken cancellationToken)
    {
        var denied = await EnsurePermissionAsync("TraCuuTongHop", "Index", cancellationToken);
        return denied ?? Ok(await traCuuService.SearchAsync("TONG_HOP", request, cancellationToken));
    }
}

[ApiController]
[Route("api/khai-thac-du-lieu/tra-cuu/dang-ky-xay-dung-van-ban")]
public sealed class TraCuuDangKyXayDungVanBanController(
    ICurrentUserContext user,
    IQuanTriHeThongPermissionClient permissionClient,
    ITraCuuKhaiThacDuLieuService traCuuService) : KhaiThacDuLieuControllerBase(user, permissionClient)
{
    [HttpGet]
    public async Task<ActionResult<PagedResultDto<TraCuuTongHopItemDto>>> Get([FromQuery] TraCuuRequest request, CancellationToken cancellationToken)
    {
        var denied = await EnsurePermissionAsync("TraCuuDangKyXayDungVanBan", "Index", cancellationToken);
        return denied ?? Ok(await traCuuService.SearchAsync("DANG_KY_XAY_DUNG_VAN_BAN", request, cancellationToken));
    }
}

[ApiController]
[Route("api/khai-thac-du-lieu/tra-cuu/xay-dung-van-ban")]
public sealed class TraCuuXayDungVanBanController(
    ICurrentUserContext user,
    IQuanTriHeThongPermissionClient permissionClient,
    ITraCuuKhaiThacDuLieuService traCuuService) : KhaiThacDuLieuControllerBase(user, permissionClient)
{
    [HttpGet]
    public async Task<ActionResult<PagedResultDto<TraCuuTongHopItemDto>>> Get([FromQuery] TraCuuRequest request, CancellationToken cancellationToken)
    {
        var denied = await EnsurePermissionAsync("TraCuuXayDungVanBan", "Index", cancellationToken);
        return denied ?? Ok(await traCuuService.SearchAsync("XAY_DUNG_VAN_BAN", request, cancellationToken));
    }
}

[ApiController]
[Route("api/khai-thac-du-lieu/tra-cuu/thi-hanh-phap-luat")]
public sealed class TraCuuThiHanhPhapLuatController(
    ICurrentUserContext user,
    IQuanTriHeThongPermissionClient permissionClient,
    ITraCuuKhaiThacDuLieuService traCuuService) : KhaiThacDuLieuControllerBase(user, permissionClient)
{
    [HttpGet]
    public async Task<ActionResult<PagedResultDto<TraCuuTongHopItemDto>>> Get([FromQuery] TraCuuRequest request, CancellationToken cancellationToken)
    {
        var denied = await EnsurePermissionAsync("TraCuuThiHanhPhapLuat", "Index", cancellationToken);
        return denied ?? Ok(await traCuuService.SearchAsync("THI_HANH_PHAP_LUAT", request, cancellationToken));
    }
}
