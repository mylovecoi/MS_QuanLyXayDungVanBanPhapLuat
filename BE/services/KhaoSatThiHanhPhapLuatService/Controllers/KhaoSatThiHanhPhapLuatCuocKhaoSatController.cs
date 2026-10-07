using KhaoSatThiHanhPhapLuatService.Infrastructure.Persistence;
using KhaoSatThiHanhPhapLuatService.Infrastructure.Persistence.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KhaoSatThiHanhPhapLuatService.Controllers;

[Authorize]
[ApiController]
[Route("api/khao-sat-thi-hanh-phap-luat/cuoc-khao-sat")]
public sealed class KhaoSatThiHanhPhapLuatCuocKhaoSatController(KhaoSatThiHanhPhapLuatDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult> List([FromQuery] SurveyListQuery query, CancellationToken ct)
    {
        var source = db.CuocKhaoSats.AsNoTracking().Where(x => !x.IsDeleted);
        if (!string.IsNullOrWhiteSpace(query.Keyword)) source = source.Where(x => x.MaCuocKhaoSat.Contains(query.Keyword) || x.TenCuocKhaoSat.Contains(query.Keyword));
        if (query.TrangThaiId.HasValue) source = source.Where(x => x.TrangThaiId == query.TrangThaiId);
        if (query.DonViChuTriId.HasValue) source = source.Where(x => x.DonViChuTriId == query.DonViChuTriId);
        var total = await source.CountAsync(ct);
        var data = await source.OrderByDescending(x => x.CreatedAt).Skip((Math.Max(query.Page, 1) - 1) * Math.Clamp(query.PageSize, 1, 100)).Take(Math.Clamp(query.PageSize, 1, 100)).ToListAsync(ct);
        var ids = data.Select(x => x.Id).ToList();
        var subjects = await db.DoiTuongKhaoSats.AsNoTracking().Where(x => ids.Contains(x.CuocKhaoSatId) && !x.IsDeleted).ToListAsync(ct);
        var submissions = await db.PhieuNopKhaoSats.AsNoTracking().Where(x => subjects.Select(y => y.Id).Contains(x.DoiTuongKhaoSatId) && !x.IsDeleted).ToListAsync(ct);
        return Ok(new { Total = total, Items = data.Select(x => new { x.Id, x.MaCuocKhaoSat, x.TenCuocKhaoSat, x.TuNgay, x.DenNgay, x.TrangThaiId, SoDoiTuong = subjects.Count(y => y.CuocKhaoSatId == x.Id), SoDaNop = submissions.Where(y => subjects.Any(z => z.Id == y.DoiTuongKhaoSatId && z.CuocKhaoSatId == x.Id)).Select(y => y.DoiTuongKhaoSatId).Distinct().Count() }) });
    }
    [HttpPost]
    public async Task<ActionResult> Create(CreateCuocKhaoSatRequest request, CancellationToken ct)
    {
        if (request.TuNgay > request.DenNgay) return BadRequest("Thời gian khảo sát không hợp lệ.");
        if (await db.CuocKhaoSats.AnyAsync(x => x.MaCuocKhaoSat == request.MaCuocKhaoSat && !x.IsDeleted, ct)) return Conflict("Mã cuộc khảo sát đã tồn tại.");
        var entity = new CuocKhaoSat { MaCuocKhaoSat = request.MaCuocKhaoSat.Trim(), TenCuocKhaoSat = request.TenCuocKhaoSat.Trim(), MucDich = request.MucDich, PhamVi = request.PhamVi, DonViChuTriId = request.DonViChuTriId, TuNgay = request.TuNgay, DenNgay = request.DenNgay, TrangThaiId = request.TrangThaiId, LinhVucId = request.LinhVucId, VanBanId = request.VanBanId };
        db.CuocKhaoSats.Add(entity); await db.SaveChangesAsync(ct); return Ok(new { entity.Id });
    }
    [HttpPost("{id:guid}/nhom-doi-tuong")]
    public async Task<ActionResult> AddGroup(Guid id, CreateNhomDoiTuongRequest request, CancellationToken ct)
    {
        if (!await db.CuocKhaoSats.AnyAsync(x => x.Id == id && !x.IsDeleted, ct)) return NotFound();
        if (await db.NhomDoiTuongKhaoSats.AnyAsync(x => x.CuocKhaoSatId == id && x.MaNhom == request.MaNhom && !x.IsDeleted, ct)) return Conflict("Mã nhóm đã tồn tại.");
        var entity = new NhomDoiTuongKhaoSat { CuocKhaoSatId = id, MaNhom = request.MaNhom.Trim(), TenNhom = request.TenNhom.Trim(), ThuTu = request.ThuTu };
        db.NhomDoiTuongKhaoSats.Add(entity); await db.SaveChangesAsync(ct); return Ok(new { entity.Id });
    }

    [HttpPost("{id:guid}/doi-tuong")]
    public async Task<ActionResult> AddSubject(Guid id, ThemDoiTuongKhaoSatRequest request, CancellationToken ct)
    {
        if (!await db.CuocKhaoSats.AnyAsync(x => x.Id == id && !x.IsDeleted, ct)) return NotFound();

        var groupExists = await db.NhomDoiTuongKhaoSats.AnyAsync(x => x.Id == request.NhomDoiTuongKhaoSatId && x.CuocKhaoSatId == id && !x.IsDeleted, ct);
        if (!groupExists) return BadRequest("Nhóm đối tượng không thuộc cuộc khảo sát.");

        MauPhieuKhaoSat? template;
        if (request.MauPhieuKhaoSatId.HasValue)
        {
            template = await db.MauPhieuKhaoSats.SingleOrDefaultAsync(x => x.Id == request.MauPhieuKhaoSatId.Value && x.CuocKhaoSatId == id && x.NhomDoiTuongKhaoSatId == request.NhomDoiTuongKhaoSatId && !x.IsDeleted, ct);
            if (template is null) return BadRequest("Mẫu phiếu không thuộc cuộc khảo sát hoặc nhóm đối tượng.");
        }
        else
        {
            template = await db.MauPhieuKhaoSats
                .Where(x => x.CuocKhaoSatId == id && x.NhomDoiTuongKhaoSatId == request.NhomDoiTuongKhaoSatId && !x.IsDeleted)
                .OrderByDescending(x => x.PhienBan)
                .FirstOrDefaultAsync(ct);
            if (template is null) return BadRequest("Chưa có mẫu phiếu gắn với nhóm đối tượng của cuộc khảo sát.");
        }

        if (await db.DoiTuongKhaoSats.AnyAsync(x => x.CuocKhaoSatId == id && x.DonViId == request.DonViId && !x.IsDeleted, ct)) return Conflict("Đơn vị đã có đối tượng khảo sát trong cuộc khảo sát này.");

        var entity = new DoiTuongKhaoSat
        {
            CuocKhaoSatId = id,
            NhomDoiTuongKhaoSatId = request.NhomDoiTuongKhaoSatId,
            MauPhieuKhaoSatId = template.Id,
            DonViId = request.DonViId,
            CanBoId = request.CanBoId,
            HanNop = request.HanNop,
            TrangThaiId = request.TrangThaiId
        };
        db.DoiTuongKhaoSats.Add(entity); await db.SaveChangesAsync(ct); return Ok(new { entity.Id });
    }
}
public sealed record SurveyListQuery(string? Keyword, Guid? TrangThaiId, Guid? DonViChuTriId, int Page = 1, int PageSize = 20);
public sealed record CreateCuocKhaoSatRequest(string MaCuocKhaoSat, string TenCuocKhaoSat, string? MucDich, string? PhamVi, Guid DonViChuTriId, DateOnly TuNgay, DateOnly DenNgay, Guid TrangThaiId, Guid? LinhVucId, Guid? VanBanId);
public sealed record CreateNhomDoiTuongRequest(string MaNhom, string TenNhom, int ThuTu);
public sealed record ThemDoiTuongKhaoSatRequest(Guid NhomDoiTuongKhaoSatId, Guid? MauPhieuKhaoSatId, Guid DonViId, Guid? CanBoId, DateOnly HanNop, Guid TrangThaiId);
