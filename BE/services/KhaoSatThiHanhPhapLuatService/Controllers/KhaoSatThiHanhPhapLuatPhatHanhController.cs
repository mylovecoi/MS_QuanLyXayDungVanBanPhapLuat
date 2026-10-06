using KhaoSatThiHanhPhapLuatService.Infrastructure.Persistence;
using KhaoSatThiHanhPhapLuatService.Infrastructure.Persistence.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KhaoSatThiHanhPhapLuatService.Controllers;

[Authorize]
[ApiController]
[Route("api/khao-sat-thi-hanh-phap-luat/phat-hanh")]
public sealed class KhaoSatThiHanhPhapLuatPhatHanhController(KhaoSatThiHanhPhapLuatDbContext db) : ControllerBase
{
    [HttpPost("doi-tuong")]
    public async Task<ActionResult> AddSubject(CreateDoiTuongKhaoSatRequest request, CancellationToken ct)
    {
        var template = await db.MauPhieuKhaoSats.SingleOrDefaultAsync(x => x.Id == request.MauPhieuKhaoSatId && !x.IsDeleted, ct);
        if (template is null || template.CuocKhaoSatId != request.CuocKhaoSatId || template.NhomDoiTuongKhaoSatId != request.NhomDoiTuongKhaoSatId) return BadRequest("Mẫu phiếu không thuộc cuộc khảo sát hoặc nhóm đối tượng.");
        if (!template.DaPhatHanh) return BadRequest("Mẫu phiếu chưa được phát hành.");
        if (await db.DoiTuongKhaoSats.AnyAsync(x => x.CuocKhaoSatId == request.CuocKhaoSatId && x.DonViId == request.DonViId && !x.IsDeleted, ct)) return Conflict("Đơn vị đã có đối tượng khảo sát trong cuộc khảo sát này.");
        var entity = new DoiTuongKhaoSat { CuocKhaoSatId = request.CuocKhaoSatId, NhomDoiTuongKhaoSatId = request.NhomDoiTuongKhaoSatId, MauPhieuKhaoSatId = request.MauPhieuKhaoSatId, DonViId = request.DonViId, CanBoId = request.CanBoId, HanNop = request.HanNop, TrangThaiId = request.TrangThaiId };
        db.DoiTuongKhaoSats.Add(entity); await db.SaveChangesAsync(ct); return Ok(new { entity.Id });
    }
    [HttpPost("mau-phieu/{id:guid}")]
    public async Task<ActionResult> PublishTemplate(Guid id, CancellationToken ct)
    {
        var template = await db.MauPhieuKhaoSats.SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct); if (template is null) return NotFound();
        template.DaPhatHanh = true; template.UpdatedAt = DateTime.UtcNow; await db.SaveChangesAsync(ct); return NoContent();
    }
}
public sealed record CreateDoiTuongKhaoSatRequest(Guid CuocKhaoSatId, Guid NhomDoiTuongKhaoSatId, Guid MauPhieuKhaoSatId, Guid DonViId, Guid? CanBoId, DateOnly HanNop, Guid TrangThaiId);
