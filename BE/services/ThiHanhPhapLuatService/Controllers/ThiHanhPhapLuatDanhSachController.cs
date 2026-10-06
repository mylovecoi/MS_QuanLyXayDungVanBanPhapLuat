using BuildingBlocks.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThiHanhPhapLuatService.Infrastructure.Authorization;
using ThiHanhPhapLuatService.Infrastructure.Persistence;

namespace ThiHanhPhapLuatService.Controllers;

[ApiController]
[Route("api/thi-hanh-phap-luat/danh-sach")]
public sealed class ThiHanhPhapLuatDanhSachController(
    ICurrentUserContext user,
    IQuanTriHeThongPermissionClient permissionClient,
    ThiHanhPhapLuatDbContext dbContext) : ThiHanhPhapLuatControllerBase(user, permissionClient)
{
    [HttpGet]
    public async Task<ActionResult> GetList([FromQuery] int? nam, [FromQuery] Guid? trangThaiId, [FromQuery] string? search, CancellationToken cancellationToken)
    {
        var denied = await EnsurePermissionAsync("ThiHanhPhapLuatDanhSach", "Index", "Index", cancellationToken);
        if (denied is not null) return denied;

        var query = dbContext.KeHoachThiHanhPhapLuats.AsNoTracking().Where(x => !x.IsDeleted);
        if (!CurrentUser.IsSSA && CurrentUser.DonViId is { } donViId) query = query.Where(x => x.DonViChuTriId == donViId);
        if (nam.HasValue) query = query.Where(x => x.Nam == nam);
        if (trangThaiId.HasValue) query = query.Where(x => x.TrangThaiId == trangThaiId);
        if (!string.IsNullOrWhiteSpace(search)) query = query.Where(x => x.MaKeHoach.Contains(search) || x.TenKeHoach.Contains(search));

        var items = await query.OrderByDescending(x => x.Nam).ThenByDescending(x => x.CreatedAt).Take(100)
            .Select(x => new { x.Id, x.MaKeHoach, x.TenKeHoach, x.Nam, x.TuNgay, x.DenNgay, x.DonViChuTriId, x.TrangThaiId })
            .ToListAsync(cancellationToken);
        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var denied = await EnsurePermissionAsync("ThiHanhPhapLuatDanhSach", "Index", "Index", cancellationToken);
        if (denied is not null) return denied;

        var query = dbContext.KeHoachThiHanhPhapLuats.AsNoTracking().Where(x => x.Id == id && !x.IsDeleted);
        if (!CurrentUser.IsSSA && CurrentUser.DonViId is { } donViId) query = query.Where(x => x.DonViChuTriId == donViId);
        var item = await query.Select(x => new { x.Id, x.MaKeHoach, x.TenKeHoach, x.Nam, x.TuNgay, x.DenNgay, x.DonViChuTriId, x.NguoiPhuTrachId, x.TrangThaiId, x.PhamVi, x.MucTieu }).SingleOrDefaultAsync(cancellationToken);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpGet("{id:guid}/timeline")]
    public async Task<ActionResult> GetTimeline(Guid id, CancellationToken cancellationToken)
    {
        var denied = await EnsurePermissionAsync("ThiHanhPhapLuatDanhSach", "Index", "Index", cancellationToken);
        if (denied is not null) return denied;
        var keHoachQuery = dbContext.KeHoachThiHanhPhapLuats.AsNoTracking().Where(x => x.Id == id && !x.IsDeleted);
        if (!CurrentUser.IsSSA && CurrentUser.DonViId is { } donViId) keHoachQuery = keHoachQuery.Where(x => x.DonViChuTriId == donViId);
        if (!await keHoachQuery.AnyAsync(cancellationToken)) return NotFound();
        var items = await dbContext.LichSuXuLyThiHanhs.AsNoTracking().Where(x => x.KeHoachId == id).OrderBy(x => x.ThoiGianXuLy).ToListAsync(cancellationToken);
        return Ok(items);
    }
}
