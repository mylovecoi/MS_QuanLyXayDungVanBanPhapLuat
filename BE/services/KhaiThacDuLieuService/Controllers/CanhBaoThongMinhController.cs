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
    ICanhBaoKhaiThacDuLieuService canhBaoService,
    ICanhBaoThongMinhGeneratorService generatorService) : KhaiThacDuLieuControllerBase(user, permissionClient)
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

    [HttpPost("quet-tu-dong")]
    public async Task<ActionResult<SinhCanhBaoResultDto>> SinhCanhBaoTuDong(CancellationToken cancellationToken)
    {
        var denied = await EnsurePermissionAsync("CanhBaoThongMinh", "Approve", cancellationToken);
        return denied ?? Ok(await generatorService.SinhCanhBaoTuDongAsync(cancellationToken));
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

    [HttpGet("{id:guid}/lich-su")]
    public async Task<ActionResult<IReadOnlyList<CanhBaoLichSuXuLyDto>>> GetLichSu(Guid id, CancellationToken cancellationToken)
    {
        var denied = await EnsurePermissionAsync("CanhBaoThongMinh", "Index", cancellationToken);
        if (denied is not null) return denied;
        var result = await canhBaoService.GetLichSuAsync(id, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("nhac-viec")]
    public async Task<ActionResult<IReadOnlyList<CanhBaoNhacViecDto>>> GetNhacViecCuaToi(CancellationToken cancellationToken)
    {
        var denied = await EnsurePermissionAsync("CanhBaoThongMinh", "Index", cancellationToken);
        return denied ?? Ok(await canhBaoService.GetNhacViecCuaToiAsync(cancellationToken));
    }

    [HttpGet("{id:guid}/nhac-viec")]
    public async Task<ActionResult<IReadOnlyList<CanhBaoNhacViecDto>>> GetNhacViec(Guid id, CancellationToken cancellationToken)
    {
        var denied = await EnsurePermissionAsync("CanhBaoThongMinh", "Index", cancellationToken);
        if (denied is not null) return denied;
        var result = await canhBaoService.GetNhacViecAsync(id, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("{id:guid}/nhac-viec")]
    public async Task<ActionResult<CanhBaoNhacViecDto>> TaoNhacViec(Guid id, TaoCanhBaoNhacViecRequest request, CancellationToken cancellationToken)
    {
        var denied = await EnsurePermissionAsync("CanhBaoThongMinh", "Create", cancellationToken);
        if (denied is not null) return denied;

        try
        {
            var result = await canhBaoService.TaoNhacViecAsync(id, request, cancellationToken);
            return result is null ? NotFound() : Ok(result);
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(exception.Message);
        }
    }

    [HttpPost("{id:guid}/nhac-viec/{nhacViecId:guid}/danh-dau-da-xem")]
    public async Task<ActionResult<CanhBaoNhacViecDto>> DanhDauDaXemNhacViec(Guid id, Guid nhacViecId, CancellationToken cancellationToken)
    {
        var denied = await EnsurePermissionAsync("CanhBaoThongMinh", "Edit", cancellationToken);
        if (denied is not null) return denied;
        var result = await canhBaoService.DanhDauDaXemNhacViecAsync(id, nhacViecId, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("{id:guid}/nhac-viec/{nhacViecId:guid}/hoan-thanh")]
    public async Task<ActionResult<CanhBaoNhacViecDto>> HoanThanhNhacViec(Guid id, Guid nhacViecId, HoanThanhCanhBaoNhacViecRequest request, CancellationToken cancellationToken)
    {
        var denied = await EnsurePermissionAsync("CanhBaoThongMinh", "Edit", cancellationToken);
        if (denied is not null) return denied;
        var result = await canhBaoService.HoanThanhNhacViecAsync(id, nhacViecId, request, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("{id:guid}/nhac-viec/{nhacViecId:guid}/huy")]
    public async Task<ActionResult<CanhBaoNhacViecDto>> HuyNhacViec(Guid id, Guid nhacViecId, CancellationToken cancellationToken)
    {
        var denied = await EnsurePermissionAsync("CanhBaoThongMinh", "Edit", cancellationToken);
        if (denied is not null) return denied;
        var result = await canhBaoService.HuyNhacViecAsync(id, nhacViecId, cancellationToken);
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
