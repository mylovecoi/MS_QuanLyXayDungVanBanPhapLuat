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
    [HttpGet("ket-qua-tong-hop")]
    public async Task<ActionResult> ListAggregate(Guid cuocKhaoSatId, Guid? nhomDoiTuongId, CancellationToken ct)
    {
        var query = db.PhieuNopKhaoSats.AsNoTracking().Where(x => x.CuocKhaoSatId == cuocKhaoSatId && !x.IsDeleted);
        if (nhomDoiTuongId.HasValue) query = query.Where(x => x.NhomDoiTuongKhaoSatId == nhomDoiTuongId);
        var items = await query.OrderByDescending(x => x.CreatedAt).ToListAsync(ct);
        var ids = items.Select(x => x.Id).ToList();
        var errorCounts = await db.LoiImportKhaoSats.AsNoTracking().Where(x => ids.Contains(x.PhieuNopKhaoSatId) && !x.IsDeleted).GroupBy(x => x.PhieuNopKhaoSatId).Select(x => new { Id = x.Key, Count = x.Count() }).ToListAsync(ct);
        return Ok(items.Select(x => new { x.Id, x.NhomDoiTuongKhaoSatId, x.MauPhieuKhaoSatId, x.TenFile, x.NgayImport, x.TrangThaiId, SoLoi = errorCounts.SingleOrDefault(y => y.Id == x.Id)?.Count ?? 0 }));
    }

    [HttpGet("ket-qua-tong-hop/{id:guid}/loi")]
    public async Task<ActionResult> GetErrors(Guid id, CancellationToken ct) => Ok(await db.LoiImportKhaoSats.AsNoTracking().Where(x => x.PhieuNopKhaoSatId == id && !x.IsDeleted).OrderBy(x => x.CreatedAt).ToListAsync(ct));

    [HttpDelete("ket-qua-tong-hop/{id:guid}")]
    public async Task<ActionResult> DeleteAggregate(Guid id, CancellationToken ct)
    {
        var item = await db.PhieuNopKhaoSats.SingleOrDefaultAsync(x => x.Id == id && x.CuocKhaoSatId.HasValue && !x.IsDeleted, ct);
        if (item is null) return NotFound();
        item.IsDeleted = true; item.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct); return NoContent();
    }
    [HttpPost("ket-qua-tong-hop/upload")]
    [RequestSizeLimit(20_000_000)]
    public async Task<ActionResult> UploadAggregate(Guid cuocKhaoSatId, Guid nhomDoiTuongId, Guid mauPhieuId, Guid trangThaiThanhCongId, Guid trangThaiLoiId, IFormFile file, CancellationToken ct)
    {
        if (file.Length == 0 || !string.Equals(Path.GetExtension(file.FileName), ".docx", StringComparison.OrdinalIgnoreCase)) return BadRequest("Chỉ nhận file kết quả Word .docx.");
        var template = await db.MauPhieuKhaoSats.SingleOrDefaultAsync(x => x.Id == mauPhieuId && x.CuocKhaoSatId == cuocKhaoSatId && x.NhomDoiTuongKhaoSatId == nhomDoiTuongId && !x.IsDeleted, ct);
        if (template is null) return BadRequest("Mẫu phiếu không thuộc cuộc khảo sát hoặc nhóm đối tượng.");
        WordSurveyParseResult parsed; await using (var source = file.OpenReadStream()) { try { parsed = await parser.ParseAsync(source, ct); } catch (InvalidOperationException ex) { return BadRequest(ex.Message); } }
        var questions = await db.CauHoiMauPhieus.Where(x => x.MauPhieuKhaoSatId == template.Id && !x.IsDeleted).ToListAsync(ct);
        var options = await db.LuaChonTraLois.Where(x => questions.Select(q => q.Id).Contains(x.CauHoiMauPhieuId) && !x.IsDeleted).ToListAsync(ct);
        await using var hashStream = file.OpenReadStream();
        var submission = new PhieuNopKhaoSat { CuocKhaoSatId = cuocKhaoSatId, NhomDoiTuongKhaoSatId = nhomDoiTuongId, DoiTuongKhaoSatId = Guid.Empty, MauPhieuKhaoSatId = template.Id, TenFile = Path.GetFileName(file.FileName), MaHash = Convert.ToHexString(await SHA256.HashDataAsync(hashStream, ct)), DuongDanFile = string.Empty, NgayImport = DateTime.UtcNow, TrangThaiId = trangThaiThanhCongId };
        var errors = new List<LoiImportKhaoSat>();
        foreach (var row in parsed.Rows)
        {
            var question = questions.SingleOrDefault(x => string.Equals(x.MaCauHoi, row.MaCauHoi, StringComparison.OrdinalIgnoreCase));
            if (question is null) { errors.Add(new LoiImportKhaoSat { PhieuNopKhaoSatId = submission.Id, ViTri = $"Dòng {row.RowNumber}", MaCauHoi = row.MaCauHoi, NoiDungLoi = "Mã câu hỏi không thuộc mẫu phiếu." }); continue; }
            var option = options.SingleOrDefault(x => x.CauHoiMauPhieuId == question.Id && string.Equals(x.MaLuaChon, row.MaDapAn, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(row.MaDapAn) && option is null) { errors.Add(new LoiImportKhaoSat { PhieuNopKhaoSatId = submission.Id, ViTri = $"Dòng {row.RowNumber}", MaCauHoi = row.MaCauHoi, NoiDungLoi = "Mã đáp án không thuộc câu hỏi." }); continue; }
            if (option is not null && (!row.SoLuong.HasValue || !row.TongSoTraLoi.HasValue || row.SoLuong < 0 || row.TongSoTraLoi <= 0 || row.SoLuong > row.TongSoTraLoi)) { errors.Add(new LoiImportKhaoSat { PhieuNopKhaoSatId = submission.Id, ViTri = $"Dòng {row.RowNumber}", MaCauHoi = row.MaCauHoi, NoiDungLoi = "Số lượng hoặc tổng số trả lời không hợp lệ." }); continue; }
            if (option is not null) db.CauTraLoiKhaoSats.Add(new CauTraLoiKhaoSat { PhieuNopKhaoSatId = submission.Id, CauHoiThongKeId = question.CauHoiThongKeId, CauHoiMauPhieuId = question.Id, LuaChonTraLoiId = option.Id, SoLuong = row.SoLuong, TongSoTraLoi = row.TongSoTraLoi });
            else if (!string.IsNullOrWhiteSpace(row.GiaTriTraLoi) && question.CoYKienTuDo) db.CauTraLoiKhaoSats.Add(new CauTraLoiKhaoSat { PhieuNopKhaoSatId = submission.Id, CauHoiThongKeId = question.CauHoiThongKeId, CauHoiMauPhieuId = question.Id, GiaTriText = row.GiaTriTraLoi });
        }
        if (errors.Count > 0) submission.TrangThaiId = trangThaiLoiId;
        var directory = Path.Combine(environment.ContentRootPath, "uploads", "khao-sat-thi-hanh-phap-luat", cuocKhaoSatId.ToString("N"), "ket-qua-tong-hop", submission.Id.ToString("N")); Directory.CreateDirectory(directory);
        var stored = $"{Guid.NewGuid():N}_{submission.TenFile}"; await using (var output = System.IO.File.Create(Path.Combine(directory, stored))) await file.CopyToAsync(output, ct);
        submission.DuongDanFile = Path.Combine("uploads", "khao-sat-thi-hanh-phap-luat", cuocKhaoSatId.ToString("N"), "ket-qua-tong-hop", submission.Id.ToString("N"), stored).Replace('\\', '/');
        db.PhieuNopKhaoSats.Add(submission); db.LoiImportKhaoSats.AddRange(errors); await db.SaveChangesAsync(ct);
        return Ok(new { submission.Id, ThanhCong = errors.Count == 0, SoLoi = errors.Count });
    }
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
        var submission = new PhieuNopKhaoSat { CuocKhaoSatId = subject.CuocKhaoSatId, NhomDoiTuongKhaoSatId = subject.NhomDoiTuongKhaoSatId, DoiTuongKhaoSatId = subject.Id, MauPhieuKhaoSatId = template.Id, TenFile = Path.GetFileName(file.FileName), MaHash = hash, DuongDanFile = string.Empty, NgayImport = DateTime.UtcNow, TrangThaiId = trangThaiThanhCongId };
        var errors = new List<LoiImportKhaoSat>();
        foreach (var requiredQuestion in questions.Where(x => x.BatBuoc && !parsed.Rows.Any(row => string.Equals(row.MaCauHoi, x.MaCauHoi, StringComparison.OrdinalIgnoreCase))))
            errors.Add(new LoiImportKhaoSat { PhieuNopKhaoSatId = submission.Id, ViTri = "Toàn bộ tệp", MaCauHoi = requiredQuestion.MaCauHoi, NoiDungLoi = "Thiếu kết quả của câu hỏi bắt buộc." });
        foreach (var row in parsed.Rows)
        {
            var question = questions.SingleOrDefault(x => x.MaCauHoi == row.MaCauHoi);
            if (question is null) { errors.Add(new LoiImportKhaoSat { PhieuNopKhaoSatId = submission.Id, ViTri = $"Dòng {row.RowNumber}", MaCauHoi = row.MaCauHoi, NoiDungLoi = "Mã câu hỏi không thuộc mẫu phiếu." }); continue; }
            if (!string.IsNullOrWhiteSpace(row.MaDapAn))
            {
                var option = options.SingleOrDefault(x => x.CauHoiMauPhieuId == question.Id && x.MaLuaChon == row.MaDapAn);
                if (option is null) { errors.Add(new LoiImportKhaoSat { PhieuNopKhaoSatId = submission.Id, ViTri = $"Dòng {row.RowNumber}", MaCauHoi = row.MaCauHoi, NoiDungLoi = "Mã đáp án không thuộc câu hỏi." }); continue; }
                if (!row.SoLuong.HasValue || !row.TongSoTraLoi.HasValue || row.SoLuong < 0 || row.TongSoTraLoi <= 0 || row.SoLuong > row.TongSoTraLoi) { errors.Add(new LoiImportKhaoSat { PhieuNopKhaoSatId = submission.Id, ViTri = $"Dòng {row.RowNumber}", MaCauHoi = row.MaCauHoi, NoiDungLoi = "Số lượng hoặc tổng số trả lời không hợp lệ." }); continue; }
                db.CauTraLoiKhaoSats.Add(new CauTraLoiKhaoSat { PhieuNopKhaoSatId = submission.Id, CauHoiThongKeId = question.CauHoiThongKeId, CauHoiMauPhieuId = question.Id, LuaChonTraLoiId = option.Id, SoLuong = row.SoLuong, TongSoTraLoi = row.TongSoTraLoi });
            }
            else if (!string.IsNullOrWhiteSpace(row.GiaTriTraLoi))
            {
                if (!question.CoYKienTuDo) errors.Add(new LoiImportKhaoSat { PhieuNopKhaoSatId = submission.Id, ViTri = $"Dòng {row.RowNumber}", MaCauHoi = row.MaCauHoi, NoiDungLoi = "Câu hỏi chưa được cấu hình nhận ý kiến tự do." });
                else db.CauTraLoiKhaoSats.Add(new CauTraLoiKhaoSat { PhieuNopKhaoSatId = submission.Id, CauHoiThongKeId = question.CauHoiThongKeId, CauHoiMauPhieuId = question.Id, GiaTriText = row.GiaTriTraLoi });
            }
        }
        foreach (var questionRows in parsed.Rows.Where(x => !string.IsNullOrWhiteSpace(x.MaDapAn)).GroupBy(x => x.MaCauHoi, StringComparer.OrdinalIgnoreCase))
        {
            var question = questions.SingleOrDefault(x => string.Equals(x.MaCauHoi, questionRows.Key, StringComparison.OrdinalIgnoreCase));
            if (question is null) continue;
            var validRows = questionRows.Where(x => x.SoLuong.HasValue && x.TongSoTraLoi.HasValue).ToList();
            if (validRows.Count == 0) continue;
            if (validRows.Select(x => x.TongSoTraLoi!.Value).Distinct().Count() > 1) errors.Add(new LoiImportKhaoSat { PhieuNopKhaoSatId = submission.Id, ViTri = $"Câu hỏi {question.MaCauHoi}", MaCauHoi = question.MaCauHoi, NoiDungLoi = "Tổng số trả lời phải thống nhất giữa các đáp án của cùng câu hỏi." });
            if (!question.ChoPhepNhieuLuaChon && validRows.Sum(x => x.SoLuong!.Value) > validRows.First().TongSoTraLoi!.Value) errors.Add(new LoiImportKhaoSat { PhieuNopKhaoSatId = submission.Id, ViTri = $"Câu hỏi {question.MaCauHoi}", MaCauHoi = question.MaCauHoi, NoiDungLoi = "Câu hỏi một lựa chọn có tổng số lượt chọn vượt tổng số trả lời." });
        }
        if (errors.Count > 0) submission.TrangThaiId = trangThaiLoiId;
        var directory = Path.Combine(environment.ContentRootPath, "uploads", "khao-sat-thi-hanh-phap-luat", subject.CuocKhaoSatId.ToString("N"), "phieu-nop", submission.Id.ToString("N")); Directory.CreateDirectory(directory);
        var stored = $"{Guid.NewGuid():N}_{submission.TenFile}"; await using (var output = System.IO.File.Create(Path.Combine(directory, stored))) await file.CopyToAsync(output, ct);
        submission.DuongDanFile = Path.Combine("uploads", "khao-sat-thi-hanh-phap-luat", subject.CuocKhaoSatId.ToString("N"), "phieu-nop", submission.Id.ToString("N"), stored).Replace('\\', '/');
        db.PhieuNopKhaoSats.Add(submission); db.LoiImportKhaoSats.AddRange(errors); await db.SaveChangesAsync(ct);
        return Ok(new { submission.Id, ThanhCong = errors.Count == 0, SoLoi = errors.Count });
    }
}
