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
[Route("api/khao-sat-thi-hanh-phap-luat/mau-phieu")]
public sealed class KhaoSatThiHanhPhapLuatMauPhieuController(
    KhaoSatThiHanhPhapLuatDbContext dbContext,
    IWordSurveyParser parser,
    IWebHostEnvironment environment) : ControllerBase
{
    [HttpPost("upload")]
    [RequestSizeLimit(20_000_000)]
    public async Task<ActionResult> Upload(Guid cuocKhaoSatId, Guid nhomDoiTuongId, int phienBan, IFormFile file, CancellationToken cancellationToken)
    {
        if (file.Length == 0 || !string.Equals(Path.GetExtension(file.FileName), ".docx", StringComparison.OrdinalIgnoreCase)) return BadRequest("Chỉ nhận file Word định dạng .docx.");
        if (!await dbContext.CuocKhaoSats.AnyAsync(x => x.Id == cuocKhaoSatId && !x.IsDeleted, cancellationToken) || !await dbContext.NhomDoiTuongKhaoSats.AnyAsync(x => x.Id == nhomDoiTuongId && x.CuocKhaoSatId == cuocKhaoSatId && !x.IsDeleted, cancellationToken)) return NotFound("Không tìm thấy cuộc khảo sát hoặc nhóm đối tượng.");
        if (await dbContext.MauPhieuKhaoSats.AnyAsync(x => x.NhomDoiTuongKhaoSatId == nhomDoiTuongId && x.PhienBan == phienBan && !x.IsDeleted, cancellationToken)) return Conflict("Phiên bản mẫu phiếu đã tồn tại.");

        await using var input = file.OpenReadStream();
        WordSurveyParseResult parsed;
        try { parsed = await parser.ParseAsync(input, cancellationToken); }
        catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
        var invalid = parsed.Rows.Where(x => string.IsNullOrWhiteSpace(x.NoiDung) || string.IsNullOrWhiteSpace(x.LoaiCauHoi)).ToList();
        if (invalid.Count > 0) return BadRequest($"Mẫu Word có {invalid.Count} dòng thiếu nội dung hoặc loại câu hỏi.");
        var duplicate = parsed.Rows.GroupBy(x => new { x.MaCauHoi, x.MaDapAn }).FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.Key.MaDapAn) && x.Count() > 1);
        if (duplicate is not null) return BadRequest($"Mã đáp án bị trùng: {duplicate.Key.MaCauHoi}/{duplicate.Key.MaDapAn}.");

        await using var hashStream = file.OpenReadStream();
        var hash = Convert.ToHexString(await SHA256.HashDataAsync(hashStream, cancellationToken));
        var template = new MauPhieuKhaoSat { CuocKhaoSatId = cuocKhaoSatId, NhomDoiTuongKhaoSatId = nhomDoiTuongId, PhienBan = phienBan, TenFile = Path.GetFileName(file.FileName), MaHash = hash, DuongDanFile = string.Empty };
        var directory = Path.Combine(environment.ContentRootPath, "uploads", "khao-sat-thi-hanh-phap-luat", cuocKhaoSatId.ToString("N"), template.Id.ToString("N"));
        Directory.CreateDirectory(directory);
        var storedName = $"{Guid.NewGuid():N}_{template.TenFile}";
        var path = Path.Combine(directory, storedName);
        try { await using var output = System.IO.File.Create(path); await file.CopyToAsync(output, cancellationToken); }
        catch { if (System.IO.File.Exists(path)) System.IO.File.Delete(path); throw; }
        template.DuongDanFile = Path.Combine("uploads", "khao-sat-thi-hanh-phap-luat", cuocKhaoSatId.ToString("N"), template.Id.ToString("N"), storedName).Replace('\\', '/');
        dbContext.MauPhieuKhaoSats.Add(template);

        foreach (var group in parsed.Rows.GroupBy(x => x.MaCauHoi, StringComparer.OrdinalIgnoreCase))
        {
            var first = group.First();
            var standard = await dbContext.CauHoiThongKes.SingleOrDefaultAsync(x => x.CuocKhaoSatId == cuocKhaoSatId && x.MaCauHoiThongKe == first.MaCauHoi && !x.IsDeleted, cancellationToken);
            if (standard is null)
            {
                standard = new CauHoiThongKe { CuocKhaoSatId = cuocKhaoSatId, MaCauHoiThongKe = first.MaCauHoi, NoiDung = first.NoiDung, LoaiCauHoi = first.LoaiCauHoi };
                dbContext.CauHoiThongKes.Add(standard);
            }
            else if (!string.Equals(standard.LoaiCauHoi, first.LoaiCauHoi, StringComparison.OrdinalIgnoreCase)) return BadRequest($"Câu hỏi {first.MaCauHoi} có loại khác với mẫu đã tồn tại.");
            var question = new CauHoiMauPhieu { MauPhieuKhaoSatId = template.Id, CauHoiThongKeId = standard.Id, MaCauHoi = first.MaCauHoi, NoiDung = first.NoiDung, LoaiCauHoi = first.LoaiCauHoi, ThuTu = first.RowNumber };
            dbContext.CauHoiMauPhieus.Add(question);
            foreach (var option in group.Where(x => !string.IsNullOrWhiteSpace(x.MaDapAn))) dbContext.LuaChonTraLois.Add(new LuaChonTraLoi { CauHoiMauPhieuId = question.Id, MaLuaChon = option.MaDapAn, NoiDung = option.DapAn, ThuTu = option.RowNumber });
        }
        await dbContext.SaveChangesAsync(cancellationToken);
        return Ok(new { template.Id, template.PhienBan, SoCauHoi = parsed.Rows.Select(x => x.MaCauHoi).Distinct(StringComparer.OrdinalIgnoreCase).Count(), SoDongDocDuoc = parsed.Rows.Count });
    }

    [HttpGet("{id:guid}/cau-hoi")]
    public async Task<ActionResult> GetQuestions(Guid id, CancellationToken cancellationToken)
    {
        var template = await dbContext.MauPhieuKhaoSats.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        if (template is null) return NotFound();
        var questions = await dbContext.CauHoiMauPhieus.AsNoTracking().Where(x => x.MauPhieuKhaoSatId == id && !x.IsDeleted).OrderBy(x => x.ThuTu).ToListAsync(cancellationToken);
        var options = await dbContext.LuaChonTraLois.AsNoTracking().Where(x => questions.Select(q => q.Id).Contains(x.CauHoiMauPhieuId) && !x.IsDeleted).OrderBy(x => x.ThuTu).ToListAsync(cancellationToken);
        return Ok(questions.Select(question => new { question.Id, question.MaCauHoi, question.NoiDung, question.LoaiCauHoi, LuaChon = options.Where(x => x.CauHoiMauPhieuId == question.Id).Select(x => new { x.MaLuaChon, x.NoiDung }) }));
    }
}
