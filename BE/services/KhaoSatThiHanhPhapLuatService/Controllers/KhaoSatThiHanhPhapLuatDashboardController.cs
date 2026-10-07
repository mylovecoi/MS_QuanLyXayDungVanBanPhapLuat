using KhaoSatThiHanhPhapLuatService.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KhaoSatThiHanhPhapLuatService.Controllers;

[Authorize]
[ApiController]
[Route("api/khao-sat-thi-hanh-phap-luat/dashboard")]
public sealed class KhaoSatThiHanhPhapLuatDashboardController(KhaoSatThiHanhPhapLuatDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult> Get(Guid cuocKhaoSatId, CancellationToken ct)
    {
        var survey = await db.CuocKhaoSats.AsNoTracking().SingleOrDefaultAsync(x => x.Id == cuocKhaoSatId && !x.IsDeleted, ct);
        if (survey is null) return NotFound();
        var groups = await db.NhomDoiTuongKhaoSats.AsNoTracking().Where(x => x.CuocKhaoSatId == cuocKhaoSatId && !x.IsDeleted).OrderBy(x => x.ThuTu).ToListAsync(ct);
        var submissions = await db.PhieuNopKhaoSats.AsNoTracking().Where(x => x.CuocKhaoSatId == cuocKhaoSatId && !x.IsDeleted).ToListAsync(ct);
        var latest = submissions.GroupBy(x => new { x.NhomDoiTuongKhaoSatId, x.MauPhieuKhaoSatId }).Select(x => x.OrderByDescending(y => y.CreatedAt).First()).ToList();
        var reports = await db.BaoCaoKhaoSats.AsNoTracking().Where(x => x.CuocKhaoSatId == cuocKhaoSatId && !x.IsDeleted).ToListAsync(ct);
        return Ok(new { survey.Id, survey.MaCuocKhaoSat, survey.TenCuocKhaoSat, TongNhom = groups.Count, SoNhomDaNhapKetQua = latest.Select(x => x.NhomDoiTuongKhaoSatId).Distinct().Count(), SoFileKetQua = submissions.Count, SoFileMoiNhat = latest.Count, NhomDoiTuong = groups.Select(g => new { g.Id, g.MaNhom, g.TenNhom, DaNhapKetQua = latest.Any(x => x.NhomDoiTuongKhaoSatId == g.Id), SoLanUpload = submissions.Count(x => x.NhomDoiTuongKhaoSatId == g.Id) }), BaoCao = reports.Select(x => new { x.Id, x.TenBaoCao, x.TrangThai, x.NgayBaoCao, x.NgayXuat }) });
    }
}
