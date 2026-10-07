using KhaoSatThiHanhPhapLuatService.Application;
using KhaoSatThiHanhPhapLuatService.Infrastructure.Persistence;
using KhaoSatThiHanhPhapLuatService.Infrastructure.Persistence.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KhaoSatThiHanhPhapLuatService.Controllers;

[Authorize]
[ApiController]
[Route("api/khao-sat-thi-hanh-phap-luat/bao-cao")]
public sealed class KhaoSatThiHanhPhapLuatBaoCaoSnapshotController(KhaoSatThiHanhPhapLuatDbContext db, IWebHostEnvironment environment) : ControllerBase
{
    [HttpPost("snapshot")]
    public async Task<ActionResult> CreateSnapshot([FromBody] CreateSurveyReportSnapshotRequest request, CancellationToken ct)
    {
        if (!await db.CuocKhaoSats.AnyAsync(x => x.Id == request.CuocKhaoSatId && !x.IsDeleted, ct)) return NotFound("Không tìm thấy cuộc khảo sát.");
        var submissions = await db.PhieuNopKhaoSats.Where(x => x.CuocKhaoSatId == request.CuocKhaoSatId && x.TrangThaiId == request.TrangThaiPhieuHopLeId && !x.IsDeleted).ToListAsync(ct);
        var latest = submissions.GroupBy(x => new { x.NhomDoiTuongKhaoSatId, x.MauPhieuKhaoSatId }).Select(x => x.OrderByDescending(y => y.CreatedAt).First()).ToList();
        var submissionIds = latest.Select(x => x.Id).ToList();
        var questions = await db.CauHoiThongKes.Where(x => x.CuocKhaoSatId == request.CuocKhaoSatId && !x.IsDeleted).OrderBy(x => x.MaCauHoiThongKe).ToListAsync(ct);
        var answers = await db.CauTraLoiKhaoSats.Where(x => submissionIds.Contains(x.PhieuNopKhaoSatId) && !x.IsDeleted).ToListAsync(ct);
        var options = await (from option in db.LuaChonTraLois
                             join question in db.CauHoiMauPhieus on option.CauHoiMauPhieuId equals question.Id
                             where !option.IsDeleted && !question.IsDeleted && questions.Select(x => x.Id).Contains(question.CauHoiThongKeId)
                             select new { option, question.CauHoiThongKeId }).ToListAsync(ct);

        var report = new BaoCaoKhaoSat { CuocKhaoSatId = request.CuocKhaoSatId, TenBaoCao = request.TenBaoCao, SoKyHieu = request.SoKyHieu, NgayBaoCao = request.NgayBaoCao, UuDiem = request.UuDiem, HanChe = request.HanChe, KienNghi = request.KienNghi };
        db.BaoCaoKhaoSats.Add(report);
        foreach (var question in questions)
        {
            var questionAnswers = answers.Where(x => x.CauHoiThongKeId == question.Id).ToList();
            var perSubmission = questionAnswers.GroupBy(x => x.PhieuNopKhaoSatId).Select(x => new { Valid = x.Where(y => y.TongSoTraLoi.HasValue).Select(y => y.TongSoTraLoi!.Value).DefaultIfEmpty().Max(), Choices = x.Where(y => y.LuaChonTraLoiId.HasValue).Sum(y => y.SoLuong ?? 0) }).ToList();
            var denominator = string.Equals(question.MauSoTyLe, "TONG_LUOT_CHON", StringComparison.OrdinalIgnoreCase) ? perSubmission.Sum(x => x.Choices) : perSubmission.Sum(x => x.Valid);
            foreach (var group in options.Where(x => x.CauHoiThongKeId == question.Id).GroupBy(x => x.option.MaLuaChon))
            {
                var ids = group.Select(x => x.option.Id).ToList();
                var count = questionAnswers.Where(x => x.LuaChonTraLoiId.HasValue && ids.Contains(x.LuaChonTraLoiId.Value)).Sum(x => x.SoLuong ?? 0);
                db.ChiTietBaoCaoKhaoSats.Add(new ChiTietBaoCaoKhaoSat { BaoCaoKhaoSatId = report.Id, CauHoiThongKeId = question.Id, MaCauHoi = question.MaCauHoiThongKe, NoiDungCauHoi = question.NoiDung, MaLuaChon = group.Key, NoiDungLuaChon = group.First().option.NoiDung, SoLuong = count, MauSoTyLe = denominator, TyLe = denominator == 0 ? 0 : Math.Round(count * 100m / denominator, 2) });
            }
            foreach (var text in questionAnswers.Where(x => !string.IsNullOrWhiteSpace(x.GiaTriText)).Select(x => x.GiaTriText!)) db.ChiTietBaoCaoKhaoSats.Add(new ChiTietBaoCaoKhaoSat { BaoCaoKhaoSatId = report.Id, CauHoiThongKeId = question.Id, MaCauHoi = question.MaCauHoiThongKe, NoiDungCauHoi = question.NoiDung, MauSoTyLe = denominator, YKienTuDo = text });
        }
        await db.SaveChangesAsync(ct);
        return CreatedAtAction(nameof(Get), new { id = report.Id }, new { report.Id, report.TrangThai });
    }

    [HttpPost("{id:guid}/chot")]
    public async Task<ActionResult> FinalizeReport(Guid id, CancellationToken ct)
    {
        var report = await db.BaoCaoKhaoSats.SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
        if (report is null) return NotFound();
        if (report.TrangThai == "DA_CHOT") return Conflict("Báo cáo đã chốt.");
        report.TrangThai = "DA_CHOT";
        report.NgayChot = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        return NoContent();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult> Get(Guid id, CancellationToken ct)
    {
        var report = await db.BaoCaoKhaoSats.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
        if (report is null) return NotFound();
        var details = await db.ChiTietBaoCaoKhaoSats.AsNoTracking().Where(x => x.BaoCaoKhaoSatId == id && !x.IsDeleted).OrderBy(x => x.MaCauHoi).ThenBy(x => x.MaLuaChon).ToListAsync(ct);
        return Ok(new { report, ChiTiet = details });
    }

    [HttpPost("{id:guid}/xuat-word")]
    public async Task<ActionResult> ExportWord(Guid id, CancellationToken ct)
    {
        var report = await db.BaoCaoKhaoSats.SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
        if (report is null) return NotFound();
        if (report.TrangThai != "DA_CHOT") return Conflict("Chỉ được xuất báo cáo đã chốt.");
        var details = await db.ChiTietBaoCaoKhaoSats.AsNoTracking().Where(x => x.BaoCaoKhaoSatId == id && !x.IsDeleted).OrderBy(x => x.MaCauHoi).ThenBy(x => x.MaLuaChon).ToListAsync(ct);
        var fileName = $"BaoCaoKhaoSat_{report.Id:N}.docx";
        var relativePath = Path.Combine("uploads", "khao-sat-thi-hanh-phap-luat", "bao-cao", report.Id.ToString("N"), fileName).Replace('\\', '/');
        var physicalPath = Path.Combine(environment.ContentRootPath, relativePath.Replace('/', Path.DirectorySeparatorChar));
        Directory.CreateDirectory(Path.GetDirectoryName(physicalPath)!);
        SurveyReportWordWriter.Write(physicalPath, report, details);
        report.TenFileXuat = fileName;
        report.DuongDanFileXuat = relativePath;
        report.NgayXuat = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        return File(await System.IO.File.ReadAllBytesAsync(physicalPath, ct), "application/vnd.openxmlformats-officedocument.wordprocessingml.document", fileName);
    }

    [HttpGet("{id:guid}/tep-xuat")]
    public async Task<ActionResult> DownloadExport(Guid id, CancellationToken ct)
    {
        var report = await db.BaoCaoKhaoSats.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
        if (report is null) return NotFound();
        if (string.IsNullOrWhiteSpace(report.DuongDanFileXuat)) return NotFound("Báo cáo chưa được xuất Word.");
        var path = Path.Combine(environment.ContentRootPath, report.DuongDanFileXuat.Replace('/', Path.DirectorySeparatorChar));
        if (!System.IO.File.Exists(path)) return NotFound("Không tìm thấy tệp báo cáo đã xuất.");
        return PhysicalFile(path, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", report.TenFileXuat ?? Path.GetFileName(path));
    }
}

public sealed record CreateSurveyReportSnapshotRequest(Guid CuocKhaoSatId, Guid TrangThaiPhieuHopLeId, string TenBaoCao, string? SoKyHieu, DateOnly NgayBaoCao, string? UuDiem, string? HanChe, string? KienNghi);
