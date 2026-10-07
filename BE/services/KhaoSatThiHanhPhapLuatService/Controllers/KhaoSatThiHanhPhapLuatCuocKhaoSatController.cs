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
}
public sealed record CreateCuocKhaoSatRequest(string MaCuocKhaoSat, string TenCuocKhaoSat, string? MucDich, string? PhamVi, Guid DonViChuTriId, DateOnly TuNgay, DateOnly DenNgay, Guid TrangThaiId, Guid? LinhVucId, Guid? VanBanId);
public sealed record CreateNhomDoiTuongRequest(string MaNhom, string TenNhom, int ThuTu);
