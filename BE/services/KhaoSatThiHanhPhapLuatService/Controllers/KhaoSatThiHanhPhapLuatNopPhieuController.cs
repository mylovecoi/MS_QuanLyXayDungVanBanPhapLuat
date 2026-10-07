using System.Security.Cryptography;
using KhaoSatThiHanhPhapLuatService.Application;
using KhaoSatThiHanhPhapLuatService.Infrastructure.Persistence;
using KhaoSatThiHanhPhapLuatService.Infrastructure.Persistence.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KhaoSatThiHanhPhapLuatService.Controllers;

[Authorize]
[ApiController]
[Route("api/khao-sat-thi-hanh-phap-luat/nop-phieu")]
public sealed class KhaoSatThiHanhPhapLuatNopPhieuController(KhaoSatThiHanhPhapLuatDbContext db, IWordSurveyParser parser, IWebHostEnvironment environment) : ControllerBase
{
    [HttpPost("{doiTuongId:guid}/upload")]
    [RequestSizeLimit(20_000_000)]
    public async Task<ActionResult> Upload(Guid doiTuongId, Guid trangThaiThanhCongId, Guid trangThaiLoiId, IFormFile file, CancellationToken ct)
    {
        var subject = await db.DoiTuongKhaoSats.SingleOrDefaultAsync(x => x.Id == doiTuongId && !x.IsDeleted, ct);
        if (subject is null) return NotFound();
        if (file.Length == 0 || !string.Equals(Path.GetExtension(file.FileName), ".docx", StringComparison.OrdinalIgnoreCase)) return BadRequest("Chỉ nhận phiếu Word .docx.");
        WordSurveyParseResult parsed; await using (var source = file.OpenReadStream()) { try { parsed = await parser.ParseAsync(source, ct); } catch (InvalidOperationException ex) { return BadRequest(ex.Message); } }
        var template = await db.MauPhieuKhaoSats.SingleAsync(x => x.Id == subject.MauPhieuKhaoSatId && !x.IsDeleted, ct);
        var questions = await db.CauHoiMauPhieus.Where(x => x.MauPhieuKhaoSatId == template.Id && !x.IsDeleted).ToListAsync(ct);
        var options = await db.LuaChonTraLois.Where(x => questions.Select(q => q.Id).Contains(x.CauHoiMauPhieuId) && !x.IsDeleted).ToListAsync(ct);
        await using var hashStream = file.OpenReadStream(); var hash = Convert.ToHexString(await SHA256.HashDataAsync(hashStream, ct));
        var submission = new PhieuNopKhaoSat { DoiTuongKhaoSatId = subject.Id, MauPhieuKhaoSatId = template.Id, TenFile = Path.GetFileName(file.FileName), MaHash = hash, DuongDanFile = string.Empty, NgayImport = DateTime.UtcNow, TrangThaiId = trangThaiThanhCongId };
        var errors = new List<LoiImportKhaoSat>();
        foreach (var row in parsed.Rows)
        {
            var question = questions.SingleOrDefault(x => x.MaCauHoi == row.MaCauHoi);
            if (question is null) { errors.Add(new LoiImportKhaoSat { PhieuNopKhaoSatId = submission.Id, ViTri = $"Dòng {row.RowNumber}", MaCauHoi = row.MaCauHoi, NoiDungLoi = "Mã câu hỏi không thuộc mẫu phiếu." }); continue; }
            if (!string.IsNullOrWhiteSpace(row.MaDapAn))
            {
                var option = options.SingleOrDefault(x => x.CauHoiMauPhieuId == question.Id && x.MaLuaChon == row.MaDapAn);
                if (option is null) { errors.Add(new LoiImportKhaoSat { PhieuNopKhaoSatId = submission.Id, ViTri = $"Dòng {row.RowNumber}", MaCauHoi = row.MaCauHoi, NoiDungLoi = "Mã đáp án không thuộc câu hỏi." }); continue; }
                if (row.SoLuong is < 0 || row.TongSoTraLoi is <= 0 || row.SoLuong > row.TongSoTraLoi) { errors.Add(new LoiImportKhaoSat { PhieuNopKhaoSatId = submission.Id, ViTri = $"Dòng {row.RowNumber}", MaCauHoi = row.MaCauHoi, NoiDungLoi = "Số lượng hoặc tổng số trả lời không hợp lệ." }); continue; }
                if (row.SoLuong.HasValue) db.CauTraLoiKhaoSats.Add(new CauTraLoiKhaoSat { PhieuNopKhaoSatId = submission.Id, CauHoiThongKeId = question.CauHoiThongKeId, CauHoiMauPhieuId = question.Id, LuaChonTraLoiId = option.Id, SoLuong = row.SoLuong, TongSoTraLoi = row.TongSoTraLoi });
            }
            else if (!string.IsNullOrWhiteSpace(row.GiaTriTraLoi)) db.CauTraLoiKhaoSats.Add(new CauTraLoiKhaoSat { PhieuNopKhaoSatId = submission.Id, CauHoiThongKeId = question.CauHoiThongKeId, CauHoiMauPhieuId = question.Id, GiaTriText = row.GiaTriTraLoi });
        }
        if (errors.Count > 0) submission.TrangThaiId = trangThaiLoiId;
        var directory = Path.Combine(environment.ContentRootPath, "uploads", "khao-sat-thi-hanh-phap-luat", subject.CuocKhaoSatId.ToString("N"), "phieu-nop", submission.Id.ToString("N")); Directory.CreateDirectory(directory);
        var stored = $"{Guid.NewGuid():N}_{submission.TenFile}"; await using (var output = System.IO.File.Create(Path.Combine(directory, stored))) await file.CopyToAsync(output, ct);
        submission.DuongDanFile = Path.Combine("uploads", "khao-sat-thi-hanh-phap-luat", subject.CuocKhaoSatId.ToString("N"), "phieu-nop", submission.Id.ToString("N"), stored).Replace('\\', '/');
        db.PhieuNopKhaoSats.Add(submission); db.LoiImportKhaoSats.AddRange(errors); await db.SaveChangesAsync(ct);
        return Ok(new { submission.Id, ThanhCong = errors.Count == 0, SoLoi = errors.Count });
    }
}
