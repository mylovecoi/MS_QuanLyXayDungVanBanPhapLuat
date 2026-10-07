using KhaoSatThiHanhPhapLuatService.Infrastructure.Persistence;
using KhaoSatThiHanhPhapLuatService.Infrastructure.Persistence.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KhaoSatThiHanhPhapLuatService.Controllers;

[Authorize]
[ApiController]
[Route("api/khao-sat-thi-hanh-phap-luat/ra-soat")]
public sealed class KhaoSatThiHanhPhapLuatRaSoatController(KhaoSatThiHanhPhapLuatDbContext db) : ControllerBase
{
    [HttpPost("{phieuNopId:guid}/xac-nhan")]
    public async Task<ActionResult> Confirm(Guid phieuNopId, Guid trangThaiXacNhanId, CancellationToken ct)
    {
        var item = await db.PhieuNopKhaoSats.SingleOrDefaultAsync(x => x.Id == phieuNopId && !x.IsDeleted, ct);
        if (item is null) return NotFound();
        if (await db.LoiImportKhaoSats.AnyAsync(x => x.PhieuNopKhaoSatId == item.Id && !x.IsDeleted, ct)) return BadRequest("Phiếu còn lỗi import, không thể xác nhận.");
        item.TrangThaiId = trangThaiXacNhanId; item.UpdatedAt = DateTime.UtcNow;
        var cuocKhaoSatId = item.CuocKhaoSatId ?? (await db.DoiTuongKhaoSats.SingleAsync(x => x.Id == item.DoiTuongKhaoSatId, ct)).CuocKhaoSatId;
        db.LichSuXuLyKhaoSats.Add(new LichSuXuLyKhaoSat { CuocKhaoSatId = cuocKhaoSatId, PhieuNopKhaoSatId = item.Id, HanhDong = "XAC_NHAN_KET_QUA_TONG_HOP", NoiDung = "Xác nhận file kết quả tổng hợp hợp lệ.", NguoiXuLyId = Guid.Empty });
        await db.SaveChangesAsync(ct); return NoContent();
    }
    [HttpPost("{phieuNopId:guid}/yeu-cau-bo-sung")]
    public async Task<ActionResult> RequestSupplement(Guid phieuNopId, Guid trangThaiCanBoSungId, string noiDung, CancellationToken ct)
    {
        var item = await db.PhieuNopKhaoSats.SingleOrDefaultAsync(x => x.Id == phieuNopId && !x.IsDeleted, ct);
        if (item is null) return NotFound();
        item.TrangThaiId = trangThaiCanBoSungId; item.UpdatedAt = DateTime.UtcNow;
        var cuocKhaoSatId = item.CuocKhaoSatId ?? (await db.DoiTuongKhaoSats.SingleAsync(x => x.Id == item.DoiTuongKhaoSatId, ct)).CuocKhaoSatId;
        db.LichSuXuLyKhaoSats.Add(new LichSuXuLyKhaoSat { CuocKhaoSatId = cuocKhaoSatId, PhieuNopKhaoSatId = item.Id, HanhDong = "YEU_CAU_BO_SUNG_KET_QUA_TONG_HOP", NoiDung = noiDung, NguoiXuLyId = Guid.Empty });
        await db.SaveChangesAsync(ct); return NoContent();
    }
}
