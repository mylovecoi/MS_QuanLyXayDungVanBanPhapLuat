using BuildingBlocks.Abstractions;
using KhaiThacDuLieuService.Application.Abstractions;
using KhaiThacDuLieuService.Application.DTOs;
using KhaiThacDuLieuService.Infrastructure.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KhaiThacDuLieuService.Controllers;

[ApiController]
[Route("api/khai-thac-du-lieu/canh-bao")]
public sealed class CanhBaoThongMinhController(
    ICurrentUserContext user,
    IQuanTriHeThongPermissionClient permissionClient,
    ICanhBaoKhaiThacDuLieuService canhBaoService) : KhaiThacDuLieuControllerBase(user, permissionClient)
{
    [HttpGet]
    public async Task<ActionResult<PagedResultDto<CanhBaoDto>>> GetList([FromQuery] CanhBaoListRequest request, CancellationToken cancellationToken)
    {
        var denied = await EnsurePermissionAsync("CanhBaoThongMinh", "Index", cancellationToken);
        return denied ?? Ok(await canhBaoService.GetListAsync(request, cancellationToken));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CanhBaoDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var denied = await EnsurePermissionAsync("CanhBaoThongMinh", "Index", cancellationToken);
        if (denied is not null) return denied;
        var result = await canhBaoService.GetByIdAsync(id, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<CanhBaoDto>> Create(TaoCanhBaoRequest request, CancellationToken cancellationToken)
    {
        var denied = await EnsurePermissionAsync("CanhBaoThongMinh", "Create", cancellationToken);
        if (denied is not null) return denied;

        try
        {
            var result = await canhBaoService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(exception.Message);
        }
    }

    [HttpPost("{id:guid}/danh-dau-da-xem")]
    public async Task<ActionResult<CanhBaoDto>> DanhDauDaXem(Guid id, CancellationToken cancellationToken)
    {
        var denied = await EnsurePermissionAsync("CanhBaoThongMinh", "Edit", cancellationToken);
        if (denied is not null) return denied;
        var result = await canhBaoService.DanhDauDaXemAsync(id, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("{id:guid}/xac-nhan-xu-ly")]
    public async Task<ActionResult<CanhBaoDto>> XacNhanXuLy(Guid id, XacNhanXuLyCanhBaoRequest request, CancellationToken cancellationToken)
    {
        var denied = await EnsurePermissionAsync("CanhBaoThongMinh", "Approve", cancellationToken);
        if (denied is not null) return denied;
        var result = await canhBaoService.XacNhanXuLyAsync(id, request, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("cau-hinh")]
    public async Task<ActionResult<IReadOnlyList<CauHinhCanhBaoDto>>> GetCauHinh(CancellationToken cancellationToken)
    {
        var denied = await EnsurePermissionAsync("CanhBaoThongMinh", "Index", cancellationToken);
        return denied ?? Ok(await canhBaoService.GetCauHinhAsync(cancellationToken));
    }

    [HttpPut("cau-hinh/{id:guid}")]
    public async Task<ActionResult<CauHinhCanhBaoDto>> UpdateCauHinh(Guid id, CapNhatCauHinhCanhBaoRequest request, CancellationToken cancellationToken)
    {
        var denied = await EnsurePermissionAsync("CanhBaoThongMinh", "Edit", cancellationToken);
        if (denied is not null) return denied;
        var result = await canhBaoService.UpdateCauHinhAsync(id, request, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }
}
