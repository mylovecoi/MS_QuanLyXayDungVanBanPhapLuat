using BuildingBlocks.Abstractions;
using Microsoft.AspNetCore.Mvc;
using XayDungVanBanService.Application.Abstractions;
using XayDungVanBanService.Application.DTOs;
using XayDungVanBanService.Infrastructure.Authorization;

namespace XayDungVanBanService.Controllers;

[ApiController]
[Route("api/xay-dung-van-ban/trinh-tham-dinh")]
public sealed class XayDungVanBanTrinhThamDinhController(ICurrentUserContext user, IQuanTriHeThongPermissionClient permissionClient, IXayDungVanBanTrinhThamDinhService service) : XayDungVanBanControllerBase(user, permissionClient)
{
    [HttpPost]
    public async Task<ActionResult<XayDungVanBanTrinhThamDinhDto>> Create([FromBody] TaoHoSoTrinhThamDinhRequest request, CancellationToken cancellationToken)
    {
        var denied = await EnsurePermissionAsync("XayDungVanBanTrinhThamDinh", "Create", "Create", cancellationToken);
        if (denied is not null) return denied;
        try { var result = await service.CreateAsync(request, cancellationToken); return CreatedAtAction(nameof(GetByHoSoId), new { hoSoId = result.HoSoId }, result); }
        catch (InvalidOperationException exception) { return BadRequest(exception.Message); }
    }

    [HttpGet("{hoSoId:guid}")]
    public async Task<ActionResult<XayDungVanBanTrinhThamDinhDto>> GetByHoSoId(Guid hoSoId, CancellationToken cancellationToken)
    {
        var denied = await EnsurePermissionAsync("XayDungVanBanTrinhThamDinh", "Index", "Index", cancellationToken);
        if (denied is not null) return denied;
        var result = await service.GetByHoSoIdAsync(hoSoId, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPut("{hoSoId:guid}")]
    public async Task<ActionResult<XayDungVanBanTrinhThamDinhDto>> Update(Guid hoSoId, [FromBody] CapNhatHoSoTrinhThamDinhRequest request, CancellationToken cancellationToken)
    {
        var denied = await EnsurePermissionAsync("XayDungVanBanTrinhThamDinh", "Edit", "Edit", cancellationToken);
        if (denied is not null) return denied;
        try { var result = await service.UpdateAsync(hoSoId, request, cancellationToken); return result is null ? NotFound() : Ok(result); }
        catch (InvalidOperationException exception) { return BadRequest(exception.Message); }
    }

    [HttpGet("{hoSoId:guid}/tai-lieu")]
    public async Task<ActionResult<IReadOnlyList<XayDungVanBanTaiLieuDto>>> GetTaiLieu(Guid hoSoId, CancellationToken cancellationToken)
    {
        var denied = await EnsurePermissionAsync("XayDungVanBanTrinhThamDinh", "Index", "Index", cancellationToken);
        if (denied is not null) return denied;
        var result = await service.GetTaiLieuAsync(hoSoId, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("{hoSoId:guid}/tai-lieu")]
    [RequestSizeLimit(100_000_000)]
    public async Task<ActionResult<XayDungVanBanTaiLieuDto>> UploadTaiLieu(Guid hoSoId, [FromForm] IFormFile file, [FromForm] Guid loaiTaiLieuId, [FromForm] string tenTaiLieu, CancellationToken cancellationToken)
    {
        if (file.Length == 0) return BadRequest("File tải lên không có nội dung.");
        var denied = await EnsurePermissionAsync("XayDungVanBanTrinhThamDinh", "Create", "Create", cancellationToken);
        if (denied is not null) return denied;
        try { await using var stream = file.OpenReadStream(); var result = await service.UploadTaiLieuAsync(hoSoId, new TaiTaiLieuTrinhThamDinhRequest(loaiTaiLieuId, tenTaiLieu, file.FileName, file.ContentType, stream), cancellationToken); return result is null ? NotFound() : Ok(result); }
        catch (InvalidOperationException exception) { return BadRequest(exception.Message); }
    }

    [HttpDelete("{hoSoId:guid}/tai-lieu/{boHoSoTaiLieuId:guid}")]
    public async Task<IActionResult> DeleteTaiLieu(Guid hoSoId, Guid boHoSoTaiLieuId, CancellationToken cancellationToken)
    {
        var denied = await EnsurePermissionAsync("XayDungVanBanTrinhThamDinh", "Delete", "Delete", cancellationToken);
        if (denied is not null) return denied;
        try { return await service.DeleteTaiLieuAsync(hoSoId, boHoSoTaiLieuId, cancellationToken) ? NoContent() : NotFound(); }
        catch (InvalidOperationException exception) { return BadRequest(exception.Message); }
    }

    [HttpGet("{hoSoId:guid}/kiem-tra-truoc-gui-tham-dinh")]
    public async Task<ActionResult<DieuKienGuiThamDinhDto>> KiemTraTruocGui(Guid hoSoId, CancellationToken cancellationToken)
    {
        var denied = await EnsurePermissionAsync("XayDungVanBanTrinhThamDinh", "Index", "Index", cancellationToken);
        if (denied is not null) return denied;
        var result = await service.KiemTraTruocGuiAsync(hoSoId, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("{hoSoId:guid}/gui-tham-dinh")]
    public async Task<ActionResult<XayDungVanBanTrinhThamDinhDto>> Gui(Guid hoSoId, [FromBody] GuiThamDinhRequest request, CancellationToken cancellationToken)
    {
        var denied = await EnsurePermissionAsync("XayDungVanBanTrinhThamDinh", "Approve", "Approve", cancellationToken);
        if (denied is not null) return denied;
        try { var result = await service.GuiAsync(hoSoId, request, cancellationToken); return result is null ? NotFound() : Ok(result); }
        catch (InvalidOperationException exception) { return BadRequest(exception.Message); }
    }

    [HttpPost("{hoSoId:guid}/huy-trinh-tham-dinh")]
    public async Task<IActionResult> Huy(Guid hoSoId, CancellationToken cancellationToken)
    {
        var denied = await EnsurePermissionAsync("XayDungVanBanTrinhThamDinh", "Delete", "Delete", cancellationToken);
        if (denied is not null) return denied;
        try { return await service.HuyAsync(hoSoId, cancellationToken) ? NoContent() : NotFound(); }
        catch (InvalidOperationException exception) { return BadRequest(exception.Message); }
    }
}
