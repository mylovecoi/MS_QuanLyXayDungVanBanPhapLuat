using KhaoSatThiHanhPhapLuatService.Application;
using KhaoSatThiHanhPhapLuatService.Infrastructure.Persistence;
using KhaoSatThiHanhPhapLuatService.Infrastructure.Persistence.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KhaoSatThiHanhPhapLuatService.Controllers;

[Authorize]
[ApiController]
[Route("api/khao-sat-thi-hanh-phap-luat/doc-mau-phieu")]
public sealed class KhaoSatThiHanhPhapLuatDocMauPhieuController(KhaoSatThiHanhPhapLuatDbContext db, IWordSurveyParser parser, IWordDocumentConverter converter, IWebHostEnvironment environment) : ControllerBase
{
    [HttpPost("upload")]
    public async Task<ActionResult> Upload(Guid cuocKhaoSatId, Guid nhomDoiTuongId, IFormFile file, CancellationToken ct)
    {
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant(); if (extension is not ".doc" and not ".docx") return BadRequest("Chỉ nhận file Word .doc hoặc .docx.");
        if (!await db.NhomDoiTuongKhaoSats.AnyAsync(x => x.Id == nhomDoiTuongId && x.CuocKhaoSatId == cuocKhaoSatId && !x.IsDeleted, ct)) return NotFound();
        var session = new PhienDocMauPhieuKhaoSat { CuocKhaoSatId = cuocKhaoSatId, NhomDoiTuongKhaoSatId = nhomDoiTuongId, TenFile = Path.GetFileName(file.FileName), DuongDanFile = string.Empty };
        var directory = Path.Combine(environment.ContentRootPath, "uploads", "khao-sat-thi-hanh-phap-luat", cuocKhaoSatId.ToString("N"), "doc-mau-phieu", session.Id.ToString("N")); Directory.CreateDirectory(directory);
        var stored = $"{Guid.NewGuid():N}_{session.TenFile}"; await using (var output = System.IO.File.Create(Path.Combine(directory, stored))) await file.CopyToAsync(output, ct);
        session.DuongDanFile = Path.Combine("uploads", "khao-sat-thi-hanh-phap-luat", cuocKhaoSatId.ToString("N"), "doc-mau-phieu", session.Id.ToString("N"), stored).Replace('\\', '/');
        var parsePath = Path.Combine(directory, stored);
        if (extension == ".doc") { try { parsePath = await converter.ConvertDocToDocxAsync(parsePath, directory, ct); session.DuongDanFileChuyenDoi = Path.Combine("uploads", "khao-sat-thi-hanh-phap-luat", cuocKhaoSatId.ToString("N"), "doc-mau-phieu", session.Id.ToString("N"), Path.GetFileName(parsePath)).Replace('\\', '/'); } catch (InvalidOperationException ex) { session.TrangThai = "LOI_CHUYEN_DOI"; session.LoiChuyenDoi = ex.Message; db.PhienDocMauPhieuKhaoSats.Add(session); await db.SaveChangesAsync(ct); return BadRequest(new { session.Id, Loi = ex.Message }); } }
        WordSurveyParseResult parsed; await using (var input = System.IO.File.OpenRead(parsePath)) { try { parsed = await parser.ParseAsync(input, ct); } catch (InvalidOperationException ex) { return BadRequest(ex.Message); } }
        db.PhienDocMauPhieuKhaoSats.Add(session);
        foreach (var row in parsed.Rows) db.CauHoiNhapMauPhieuKhaoSats.Add(new CauHoiNhapMauPhieuKhaoSat { PhienDocMauPhieuKhaoSatId = session.Id, MaCauHoi = row.MaCauHoi, NoiDung = row.NoiDung, LoaiCauHoi = row.LoaiCauHoi, MaLuaChon = row.MaDapAn, NoiDungLuaChon = row.DapAn, BatBuoc = row.BatBuoc, ChoPhepNhieuLuaChon = row.ChoPhepNhieuLuaChon, CoYKienTuDo = row.CoYKienTuDo, ThuTu = row.RowNumber });
        await db.SaveChangesAsync(ct); return Ok(new { session.Id, SoDong = parsed.Rows.Count });
    }
    [HttpGet("{id:guid}")]
    public async Task<ActionResult> Get(Guid id, CancellationToken ct) => Ok(await db.CauHoiNhapMauPhieuKhaoSats.AsNoTracking().Where(x => x.PhienDocMauPhieuKhaoSatId == id && !x.IsDeleted).OrderBy(x => x.ThuTu).ToListAsync(ct));

    [HttpPut("cau-hoi/{id:guid}")]
    public async Task<ActionResult> UpdateQuestion(Guid id, [FromBody] UpdateDraftQuestionRequest request, CancellationToken ct)
    {
        var item = await db.CauHoiNhapMauPhieuKhaoSats.SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
        if (item is null) return NotFound();
        item.MaCauHoi = request.MaCauHoi.Trim(); item.NoiDung = request.NoiDung.Trim(); item.LoaiCauHoi = request.LoaiCauHoi.Trim(); item.MaLuaChon = request.MaLuaChon; item.NoiDungLuaChon = request.NoiDungLuaChon; item.BatBuoc = request.BatBuoc; item.ChoPhepNhieuLuaChon = request.ChoPhepNhieuLuaChon; item.CoYKienTuDo = request.CoYKienTuDo; item.ThuTu = request.ThuTu; item.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct); return NoContent();
    }

    [HttpDelete("cau-hoi/{id:guid}")]
    public async Task<ActionResult> DeleteQuestion(Guid id, CancellationToken ct)
    {
        var item = await db.CauHoiNhapMauPhieuKhaoSats.SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
        if (item is null) return NotFound(); item.IsDeleted = true; item.UpdatedAt = DateTime.UtcNow; await db.SaveChangesAsync(ct); return NoContent();
    }

    [HttpPost("{id:guid}/xac-nhan")]
    public async Task<ActionResult> Confirm(Guid id, CancellationToken ct)
    {
        var session = await db.PhienDocMauPhieuKhaoSats.SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
        if (session is null) return NotFound();
        if (session.TrangThai == "DA_XAC_NHAN") return Conflict("Phiên đọc đã được xác nhận.");
        var rows = await db.CauHoiNhapMauPhieuKhaoSats.Where(x => x.PhienDocMauPhieuKhaoSatId == id && !x.IsDeleted).OrderBy(x => x.ThuTu).ToListAsync(ct);
        if (rows.Count == 0 || rows.Any(x => string.IsNullOrWhiteSpace(x.MaCauHoi) || string.IsNullOrWhiteSpace(x.NoiDung))) return BadRequest("Bản nháp chưa có câu hỏi hợp lệ.");
        if (rows.GroupBy(x => new { x.MaCauHoi, x.MaLuaChon }).Any(x => !string.IsNullOrWhiteSpace(x.Key.MaLuaChon) && x.Count() > 1)) return BadRequest("Có mã đáp án trùng trong cùng câu hỏi.");
        var version = (await db.MauPhieuKhaoSats.Where(x => x.NhomDoiTuongKhaoSatId == session.NhomDoiTuongKhaoSatId).Select(x => (int?)x.PhienBan).MaxAsync(ct) ?? 0) + 1;
        var template = new MauPhieuKhaoSat { CuocKhaoSatId = session.CuocKhaoSatId, NhomDoiTuongKhaoSatId = session.NhomDoiTuongKhaoSatId, PhienBan = version, TenFile = session.TenFile, DuongDanFile = session.DuongDanFile, MaHash = string.Empty, PhienDocMauPhieuKhaoSatId = session.Id };
        db.MauPhieuKhaoSats.Add(template);
        foreach (var group in rows.GroupBy(x => x.MaCauHoi, StringComparer.OrdinalIgnoreCase))
        {
            var first = group.First();
            var standard = await db.CauHoiThongKes.SingleOrDefaultAsync(x => x.CuocKhaoSatId == session.CuocKhaoSatId && x.MaCauHoiThongKe == first.MaCauHoi && !x.IsDeleted, ct)
                ?? new CauHoiThongKe { CuocKhaoSatId = session.CuocKhaoSatId, MaCauHoiThongKe = first.MaCauHoi, NoiDung = first.NoiDung, LoaiCauHoi = first.LoaiCauHoi, ChoPhepNhieuLuaChon = first.ChoPhepNhieuLuaChon, CoYKienTuDo = first.CoYKienTuDo };
            var question = new CauHoiMauPhieu { MauPhieuKhaoSatId = template.Id, CauHoiThongKeId = standard.Id, MaCauHoi = first.MaCauHoi, NoiDung = first.NoiDung, LoaiCauHoi = first.LoaiCauHoi, BatBuoc = first.BatBuoc, ChoPhepNhieuLuaChon = first.ChoPhepNhieuLuaChon, CoYKienTuDo = first.CoYKienTuDo, ThuTu = first.ThuTu };
            if (db.Entry(standard).State == EntityState.Detached) db.CauHoiThongKes.Add(standard); db.CauHoiMauPhieus.Add(question);
            foreach (var row in group.Where(x => !string.IsNullOrWhiteSpace(x.MaLuaChon))) db.LuaChonTraLois.Add(new LuaChonTraLoi { CauHoiMauPhieuId = question.Id, MaLuaChon = row.MaLuaChon!, NoiDung = row.NoiDungLuaChon ?? string.Empty, ThuTu = row.ThuTu });
        }
        session.TrangThai = "DA_XAC_NHAN"; session.UpdatedAt = DateTime.UtcNow; await db.SaveChangesAsync(ct);
        return Ok(new { template.Id, template.PhienBan });
    }
}

public sealed record UpdateDraftQuestionRequest(string MaCauHoi, string NoiDung, string LoaiCauHoi, string? MaLuaChon, string? NoiDungLuaChon, bool BatBuoc, bool ChoPhepNhieuLuaChon, bool CoYKienTuDo, int ThuTu);
