using BuildingBlocks.Abstractions;
using Microsoft.AspNetCore.Mvc;
using XayDungVanBanService.Application.Abstractions;
using XayDungVanBanService.Application.DTOs;
using XayDungVanBanService.Infrastructure.Authorization;

namespace XayDungVanBanService.Controllers;

[ApiController]
[Route("api/xay-dung-van-ban/ban-hanh")]
public sealed class XayDungVanBanBanHanhController(
    ICurrentUserContext user,
    IQuanTriHeThongPermissionClient permissionClient,
    IXayDungVanBanBanHanhService service) : XayDungVanBanControllerBase(user, permissionClient)
{
    private async Task<ActionResult?> CheckAsync(string action, CancellationToken cancellationToken) =>
        await EnsurePermissionAsync("XayDungVanBanBanHanh", action, action, cancellationToken);

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<HoSoBanHanhListItemDto>>> Get(CancellationToken cancellationToken)
    {
        var denied = await CheckAsync("Index", cancellationToken);
        return denied ?? Ok(await service.GetListAsync(cancellationToken));
    }

    [HttpGet("{hoSoId:guid}")]
    public async Task<ActionResult<XayDungVanBanBanHanhDto>> GetByHoSo(Guid hoSoId, CancellationToken cancellationToken)
    {
        var denied = await CheckAsync("Index", cancellationToken);
        if (denied is not null) return denied;
        return await service.GetAsync(hoSoId, cancellationToken) is { } result ? Ok(result) : NotFound();
    }

    [HttpPut("{hoSoId:guid}")]
    public async Task<ActionResult<XayDungVanBanBanHanhDto>> Update(Guid hoSoId, CapNhatBanHanhRequest request, CancellationToken cancellationToken)
    {
        var denied = await CheckAsync("Edit", cancellationToken);
        if (denied is not null) return denied;
        try { return await service.UpdateAsync(hoSoId, request, cancellationToken) is { } result ? Ok(result) : NotFound(); }
        catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
    }

    [HttpGet("{hoSoId:guid}/tai-lieu")]
    public async Task<ActionResult<IReadOnlyList<XayDungVanBanTaiLieuDto>>> GetTaiLieu(Guid hoSoId, CancellationToken cancellationToken)
    {
        var denied = await CheckAsync("Index", cancellationToken);
        if (denied is not null) return denied;
        return await service.GetTaiLieuAsync(hoSoId, cancellationToken) is { } result ? Ok(result) : NotFound();
    }

    [HttpPost("{hoSoId:guid}/tai-lieu")]
    public async Task<ActionResult<XayDungVanBanTaiLieuDto>> Upload(Guid hoSoId, IFormFile file, [FromForm] Guid loaiTaiLieuId, [FromForm] string tenTaiLieu, CancellationToken cancellationToken)
    {
        var denied = await CheckAsync("Create", cancellationToken);
        if (denied is not null) return denied;
        if (file.Length == 0) return BadRequest("Tệp tải lên không hợp lệ.");
        try
        {
            await using var stream = file.OpenReadStream();
            var result = await service.UploadTaiLieuAsync(hoSoId, new TaiTaiLieuBanHanhRequest(loaiTaiLieuId, tenTaiLieu, file.FileName, file.ContentType, stream), cancellationToken);
            return result is null ? NotFound() : Ok(result);
        }
        catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
    }

    [HttpGet("{hoSoId:guid}/kiem-tra-truoc-hoan-thanh")]
    public async Task<ActionResult<DieuKienHoanThanhBanHanhDto>> KiemTra(Guid hoSoId, CancellationToken cancellationToken)
    {
        var denied = await CheckAsync("Index", cancellationToken);
        if (denied is not null) return denied;
        return await service.KiemTraAsync(hoSoId, cancellationToken) is { } result ? Ok(result) : NotFound();
    }

    [HttpPost("{hoSoId:guid}/hoan-thanh")]
    public async Task<ActionResult<XayDungVanBanBanHanhDto>> HoanThanh(Guid hoSoId, CancellationToken cancellationToken)
    {
        var denied = await CheckAsync("Approve", cancellationToken);
        if (denied is not null) return denied;
        try { return await service.HoanThanhAsync(hoSoId, cancellationToken) is { } result ? Ok(result) : NotFound(); }
        catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
    }
}
