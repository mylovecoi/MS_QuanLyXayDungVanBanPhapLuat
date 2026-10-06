using BuildingBlocks.Abstractions;
using Microsoft.AspNetCore.Mvc;
using XayDungVanBanService.Application.Abstractions;
using XayDungVanBanService.Application.DTOs;
using XayDungVanBanService.Infrastructure.Authorization;

namespace XayDungVanBanService.Controllers;

[ApiController]
[Route("api/xay-dung-van-ban/cham-diem")]
public sealed class XayDungVanBanChamDiemController(
    ICurrentUserContext user,
    IQuanTriHeThongPermissionClient permissionClient,
    IXayDungVanBanChamDiemService service) : XayDungVanBanControllerBase(user, permissionClient)
{
    private Task<ActionResult?> Check(string action, CancellationToken ct) => EnsurePermissionAsync("XayDungVanBanChamDiem", action, action, ct);

    [HttpGet]
    public async Task<ActionResult> GetList([FromQuery] ChamDiemListRequest request, CancellationToken ct)
    {
        var denied = await Check("Index", ct);
        return denied ?? Ok(await service.GetListAsync(request, ct));
    }

    [HttpGet("ho-so/{hoSoId:guid}")]
    public async Task<ActionResult> GetByHoSo(Guid hoSoId, CancellationToken ct) { var denied = await Check("Index", ct); return denied ?? Ok(await service.GetByHoSoAsync(hoSoId, ct)); }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult> Get(Guid id, CancellationToken ct) { var denied = await Check("Index", ct); if (denied is not null) return denied; return (await service.GetAsync(id, ct)) is { } data ? Ok(data) : NotFound(); }

    [HttpGet("{id:guid}/lich-su")]
    public async Task<ActionResult> LichSu(Guid id, CancellationToken ct) { var denied = await Check("Index", ct); if (denied is not null) return denied; return (await service.GetLichSuAsync(id, ct)) is { } data ? Ok(data) : NotFound(); }

    [HttpPost("ho-so/{hoSoId:guid}")]
    public async Task<ActionResult> Create(Guid hoSoId, TaoChamDiemRequest request, CancellationToken ct) { var denied = await Check("Create", ct); if (denied is not null) return denied; try { return Ok(await service.CreateAsync(hoSoId, request, ct)); } catch (InvalidOperationException ex) { return BadRequest(ex.Message); } }

    [HttpPost("{id:guid}/tinh-lai")]
    public async Task<ActionResult> TinhLai(Guid id, CancellationToken ct) { var denied = await Check("Create", ct); if (denied is not null) return denied; try { return (await service.TinhLaiAsync(id, ct)) is { } data ? Ok(data) : NotFound(); } catch (InvalidOperationException ex) { return BadRequest(ex.Message); } }

    [HttpPut("{id:guid}/chi-tiet/{chiTietId:guid}/dieu-chinh")]
    public async Task<ActionResult> DieuChinh(Guid id, Guid chiTietId, DieuChinhChamDiemRequest request, CancellationToken ct) { var denied = await Check("Edit", ct); if (denied is not null) return denied; try { return (await service.DieuChinhAsync(id, chiTietId, request, ct)) is { } data ? Ok(data) : NotFound(); } catch (InvalidOperationException ex) { return BadRequest(ex.Message); } }

    [HttpPost("{id:guid}/chot")]
    public async Task<ActionResult> Chot(Guid id, ChuyenTrangThaiChamDiemRequest request, CancellationToken ct) => await ChuyenTrangThai(id, request, "DA_CHOT", ct);

    [HttpPost("{id:guid}/huy-chot")]
    public async Task<ActionResult> HuyChot(Guid id, ChuyenTrangThaiChamDiemRequest request, CancellationToken ct) => await ChuyenTrangThai(id, request, "DA_HUY", ct);

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken ct) { var denied = await Check("Delete", ct); if (denied is not null) return denied; try { return await service.DeleteAsync(id, ct) ? NoContent() : NotFound(); } catch (InvalidOperationException ex) { return BadRequest(ex.Message); } }

    private async Task<ActionResult> ChuyenTrangThai(Guid id, ChuyenTrangThaiChamDiemRequest request, string expectedCode, CancellationToken ct)
    {
        var denied = await Check("Approve", ct); if (denied is not null) return denied;
        try { return (await service.ChuyenTrangThaiAsync(id, request, expectedCode, ct)) is { } data ? Ok(data) : NotFound(); } catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
    }
}
