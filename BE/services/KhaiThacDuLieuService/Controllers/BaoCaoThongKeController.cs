using BuildingBlocks.Abstractions;
using KhaiThacDuLieuService.Application.Abstractions;
using KhaiThacDuLieuService.Application.DTOs;
using KhaiThacDuLieuService.Infrastructure.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KhaiThacDuLieuService.Controllers;

[ApiController]
[Route("api/khai-thac-du-lieu/bao-cao")]
public sealed class BaoCaoThongKeController(
    ICurrentUserContext user,
    IQuanTriHeThongPermissionClient permissionClient,
    IBaoCaoKhaiThacDuLieuService baoCaoService) : KhaiThacDuLieuControllerBase(user, permissionClient)
{
    [HttpGet("dang-ky-ban-hanh")]
    public Task<ActionResult<BaoCaoTongHopDto>> DangKyBanHanh([FromQuery] BaoCaoRequest request, CancellationToken cancellationToken) =>
        GetBaoCaoAsync("DANG_KY_BAN_HANH", request, cancellationToken);

    [HttpGet("van-ban-ban-hanh")]
    public Task<ActionResult<BaoCaoTongHopDto>> VanBanBanHanh([FromQuery] BaoCaoRequest request, CancellationToken cancellationToken) =>
        GetBaoCaoAsync("VAN_BAN_BAN_HANH", request, cancellationToken);

    [HttpGet("thi-hanh-phap-luat")]
    public Task<ActionResult<BaoCaoTongHopDto>> ThiHanhPhapLuat([FromQuery] BaoCaoRequest request, CancellationToken cancellationToken) =>
        GetBaoCaoAsync("THI_HANH_PHAP_LUAT", request, cancellationToken);

    [HttpGet("muc-do-hoan-thanh-don-vi")]
    public Task<ActionResult<BaoCaoTongHopDto>> MucDoHoanThanhDonVi([FromQuery] BaoCaoRequest request, CancellationToken cancellationToken) =>
        GetBaoCaoAsync("MUC_DO_HOAN_THANH_DON_VI", request, cancellationToken);

    [HttpPost("export")]
    public async Task<ActionResult> Export(BaoCaoRequest request, CancellationToken cancellationToken)
    {
        var denied = await EnsurePermissionAsync("BaoCaoThongKe", "Public", cancellationToken);
        if (denied is not null) return denied;

        return Ok(new { message = "Chức năng xuất báo cáo sẽ được nối ở bước triển khai export.", request });
    }

    private async Task<ActionResult<BaoCaoTongHopDto>> GetBaoCaoAsync(string loaiBaoCao, BaoCaoRequest request, CancellationToken cancellationToken)
    {
        var denied = await EnsurePermissionAsync("BaoCaoThongKe", "Index", cancellationToken);
        return denied ?? Ok(await baoCaoService.GetBaoCaoAsync(loaiBaoCao, request, cancellationToken));
    }
}
