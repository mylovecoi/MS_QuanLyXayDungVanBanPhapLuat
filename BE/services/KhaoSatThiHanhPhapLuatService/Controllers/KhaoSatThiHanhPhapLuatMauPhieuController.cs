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
    private static readonly HashSet<string> MauSoTyLeHopLe = ["PHIEU_HOP_LE", "TONG_LUOT_CHON"];

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
            var mauSoTyLe = ChuanHoaMauSoTyLe(first.MauSoTyLe);
            if (!MauSoTyLeHopLe.Contains(mauSoTyLe)) return BadRequest($"Câu hỏi {first.MaCauHoi} có mẫu số tỷ lệ không hợp lệ.");
            if (group.Any(x => x.ChoPhepNhieuLuaChon != first.ChoPhepNhieuLuaChon || ChuanHoaMauSoTyLe(x.MauSoTyLe) != mauSoTyLe || x.CoYKienTuDo != first.CoYKienTuDo)) return BadRequest($"Câu hỏi {first.MaCauHoi} có cấu hình không thống nhất giữa các dòng đáp án.");
            var standard = await dbContext.CauHoiThongKes.SingleOrDefaultAsync(x => x.CuocKhaoSatId == cuocKhaoSatId && x.MaCauHoiThongKe == first.MaCauHoi && !x.IsDeleted, cancellationToken);
            if (standard is null)
            {
                standard = new CauHoiThongKe { CuocKhaoSatId = cuocKhaoSatId, MaCauHoiThongKe = first.MaCauHoi, NoiDung = first.NoiDung, LoaiCauHoi = first.LoaiCauHoi, ChoPhepNhieuLuaChon = first.ChoPhepNhieuLuaChon, MauSoTyLe = mauSoTyLe, CoYKienTuDo = first.CoYKienTuDo };
                dbContext.CauHoiThongKes.Add(standard);
            }
            else if (!string.Equals(standard.LoaiCauHoi, first.LoaiCauHoi, StringComparison.OrdinalIgnoreCase) || standard.ChoPhepNhieuLuaChon != first.ChoPhepNhieuLuaChon || standard.CoYKienTuDo != first.CoYKienTuDo || !string.Equals(standard.MauSoTyLe, mauSoTyLe, StringComparison.OrdinalIgnoreCase)) return BadRequest($"Câu hỏi {first.MaCauHoi} có cấu hình khác với mẫu đã tồn tại.");
            var question = new CauHoiMauPhieu { MauPhieuKhaoSatId = template.Id, CauHoiThongKeId = standard.Id, MaCauHoi = first.MaCauHoi, NoiDung = first.NoiDung, LoaiCauHoi = first.LoaiCauHoi, BatBuoc = first.BatBuoc, ChoPhepNhieuLuaChon = first.ChoPhepNhieuLuaChon, MauSoTyLe = mauSoTyLe, CoYKienTuDo = first.CoYKienTuDo, ThuTu = first.RowNumber };
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
        return Ok(questions.Select(question => new { question.Id, question.MaCauHoi, question.NoiDung, question.LoaiCauHoi, question.BatBuoc, question.ChoPhepNhieuLuaChon, question.MauSoTyLe, question.CoYKienTuDo, LuaChon = options.Where(x => x.CauHoiMauPhieuId == question.Id).Select(x => new { x.MaLuaChon, x.NoiDung }) }));
    }

    [HttpPut("{id:guid}/cau-hoi/cau-hinh")]
    public async Task<ActionResult> ConfigureQuestions(Guid id, [FromBody] ConfigureSurveyQuestionsRequest request, CancellationToken cancellationToken)
    {
        var template = await dbContext.MauPhieuKhaoSats.SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        if (template is null) return NotFound();
        var questions = await dbContext.CauHoiMauPhieus.Where(x => x.MauPhieuKhaoSatId == id && !x.IsDeleted).ToListAsync(cancellationToken);
        if (request.CauHoi.Count != questions.Count || request.CauHoi.Select(x => x.MaCauHoi).Distinct(StringComparer.OrdinalIgnoreCase).Count() != request.CauHoi.Count) return BadRequest("Danh sách cấu hình phải chứa đúng một dòng cho mỗi câu hỏi của mẫu phiếu.");
        foreach (var configured in request.CauHoi)
        {
            var question = questions.SingleOrDefault(x => string.Equals(x.MaCauHoi, configured.MaCauHoi, StringComparison.OrdinalIgnoreCase));
            if (question is null) return BadRequest($"Không tìm thấy câu hỏi {configured.MaCauHoi} trong mẫu phiếu.");
            var mauSoTyLe = ChuanHoaMauSoTyLe(configured.MauSoTyLe);
            if (!MauSoTyLeHopLe.Contains(mauSoTyLe)) return BadRequest($"Câu hỏi {configured.MaCauHoi} có mẫu số tỷ lệ không hợp lệ.");
            var otherQuestion = await dbContext.CauHoiMauPhieus.AsNoTracking().FirstOrDefaultAsync(x => x.CauHoiThongKeId == question.CauHoiThongKeId && x.Id != question.Id && !x.IsDeleted, cancellationToken);
            if (otherQuestion is not null && (otherQuestion.ChoPhepNhieuLuaChon != configured.ChoPhepNhieuLuaChon || otherQuestion.CoYKienTuDo != configured.CoYKienTuDo || !string.Equals(otherQuestion.MauSoTyLe, mauSoTyLe, StringComparison.OrdinalIgnoreCase))) return Conflict($"Câu hỏi {configured.MaCauHoi} đã được dùng trong mẫu phiếu khác với cấu hình khác.");
            question.BatBuoc = configured.BatBuoc;
            question.ChoPhepNhieuLuaChon = configured.ChoPhepNhieuLuaChon;
            question.MauSoTyLe = mauSoTyLe;
            question.CoYKienTuDo = configured.CoYKienTuDo;
            var standard = await dbContext.CauHoiThongKes.SingleAsync(x => x.Id == question.CauHoiThongKeId, cancellationToken);
            standard.ChoPhepNhieuLuaChon = configured.ChoPhepNhieuLuaChon;
            standard.MauSoTyLe = mauSoTyLe;
            standard.CoYKienTuDo = configured.CoYKienTuDo;
        }
        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private static string ChuanHoaMauSoTyLe(string? value) => string.IsNullOrWhiteSpace(value) ? "PHIEU_HOP_LE" : value.Trim().ToUpperInvariant();
}

public sealed record ConfigureSurveyQuestionsRequest(IReadOnlyList<ConfigureSurveyQuestionRequest> CauHoi);
public sealed record ConfigureSurveyQuestionRequest(string MaCauHoi, bool BatBuoc, bool ChoPhepNhieuLuaChon, string? MauSoTyLe, bool CoYKienTuDo);
