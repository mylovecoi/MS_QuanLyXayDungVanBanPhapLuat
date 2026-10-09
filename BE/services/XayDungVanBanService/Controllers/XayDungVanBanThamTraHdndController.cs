using BuildingBlocks.Abstractions;
using Microsoft.AspNetCore.Mvc;
using XayDungVanBanService.Application.Abstractions;
using XayDungVanBanService.Application.DTOs;
using XayDungVanBanService.Infrastructure.Authorization;

namespace XayDungVanBanService.Controllers;

[ApiController]
[Route("api/xay-dung-van-ban/tham-tra-hdnd")]
public sealed class XayDungVanBanThamTraHdndController(ICurrentUserContext user, IQuanTriHeThongPermissionClient permissionClient, IXayDungVanBanThamTraHdndService service) : XayDungVanBanControllerBase(user, permissionClient)
{
    private async Task<ActionResult?> Check(string action, CancellationToken ct) => await EnsurePermissionAsync("XayDungVanBanThamTraHdnd", action, action, ct);
    [HttpGet] public async Task<ActionResult<IReadOnlyList<HoSoThamTraHdndListItemDto>>> Get(CancellationToken ct) { var d = await Check("Index", ct); return d ?? Ok(await service.GetListAsync(ct)); }
    [HttpGet("{id:guid}")] public async Task<ActionResult<XayDungVanBanThamTraHdndDto>> GetById(Guid id, CancellationToken ct) { var d = await Check("Index", ct); if (d != null) return d; return await service.GetAsync(id, ct) is { } x ? Ok(x) : NotFound(); }
    [HttpPut("{id:guid}")] public async Task<ActionResult<XayDungVanBanThamTraHdndDto>> Update(Guid id, CapNhatThamTraHdndRequest r, CancellationToken ct) { var d = await Check("Edit", ct); if (d != null) return d; try { return await service.UpdateAsync(id, r, ct) is { } x ? Ok(x) : NotFound(); } catch (InvalidOperationException e) { return BadRequest(e.Message); } }
    [HttpGet("{id:guid}/tai-lieu")] public async Task<ActionResult<IReadOnlyList<XayDungVanBanTaiLieuDto>>> GetTaiLieu(Guid id, CancellationToken ct) { var d = await Check("Index", ct); if (d != null) return d; return await service.GetTaiLieuAsync(id, ct) is { } x ? Ok(x) : NotFound(); }
    [HttpPost("{id:guid}/tai-lieu")] public async Task<ActionResult<XayDungVanBanTaiLieuDto>> Upload(Guid id, IFormFile file, [FromForm] Guid loaiTaiLieuId, [FromForm] string tenTaiLieu, CancellationToken ct) { var d = await Check("Create", ct); if (d != null) return d; if (file.Length == 0) return BadRequest("Tệp tải lên không hợp lệ."); try { await using var stream = file.OpenReadStream(); return await service.UploadTaiLieuAsync(id, new TaiTaiLieuThamTraHdndRequest(loaiTaiLieuId, tenTaiLieu, file.FileName, file.ContentType, stream), ct) is { } x ? Ok(x) : NotFound(); } catch (InvalidOperationException e) { return BadRequest(e.Message); } }
    [HttpGet("{id:guid}/kiem-tra-truoc-gui")] public async Task<ActionResult<DieuKienGuiThamTraHdndDto>> KiemTra(Guid id, CancellationToken ct) { var d = await Check("Index", ct); if (d != null) return d; return await service.KiemTraAsync(id, ct) is { } x ? Ok(x) : NotFound(); }
    [HttpPost("{id:guid}/gui")] public async Task<ActionResult<XayDungVanBanThamTraHdndDto>> Gui(Guid id, GuiKetQuaThamTraHdndRequest r, CancellationToken ct) { var d = await Check("Approve", ct); if (d != null) return d; try { return await service.GuiAsync(id, r, ct) is { } x ? Ok(x) : NotFound(); } catch (InvalidOperationException e) { return BadRequest(e.Message); } }
}
