using BuildingBlocks.Abstractions;
using KhaiThacDuLieuService.Application.Abstractions;
using KhaiThacDuLieuService.Application.DTOs;
using KhaiThacDuLieuService.Infrastructure.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KhaiThacDuLieuService.Controllers;

[ApiController]
[Route("api/khai-thac-du-lieu/dashboard")]
public sealed class DashboardTongQuanController(
    ICurrentUserContext user,
    IQuanTriHeThongPermissionClient permissionClient,
    IDashboardKhaiThacDuLieuService dashboardService) : KhaiThacDuLieuControllerBase(user, permissionClient)
{
    [HttpGet("tong-quan")]
    public async Task<ActionResult<DashboardTongQuanDto>> GetTongQuan(CancellationToken cancellationToken)
    {
        var denied = await EnsurePermissionAsync("DashboardTongQuan", "Index", cancellationToken);
        return denied ?? Ok(await dashboardService.GetTongQuanAsync(cancellationToken));
    }
}
