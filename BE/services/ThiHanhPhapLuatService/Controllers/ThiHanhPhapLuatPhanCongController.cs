using BuildingBlocks.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThiHanhPhapLuatService.Infrastructure.Authorization;
using ThiHanhPhapLuatService.Infrastructure.DanhMuc;
using ThiHanhPhapLuatService.Infrastructure.Persistence;
using ThiHanhPhapLuatService.Infrastructure.Persistence.Entities;

namespace ThiHanhPhapLuatService.Controllers;

[ApiController]
[Route("api/thi-hanh-phap-luat/phan-cong")]
public sealed class ThiHanhPhapLuatPhanCongController(
    ICurrentUserContext user,
    IQuanTriHeThongPermissionClient permissionClient,
    IDanhMucTrangThaiClient trangThaiClient,
    ThiHanhPhapLuatDbContext dbContext) : ThiHanhPhapLuatControllerBase(user, permissionClient)
{
    [HttpGet]
    public async Task<ActionResult> GetList([FromQuery] Guid noiDungKeHoachId, CancellationToken cancellationToken)
    {
        var denied = await EnsurePermissionAsync("ThiHanhPhapLuatPhanCong", "Index", "Index", cancellationToken);
        if (denied is not null) return denied;
        if (await GetScopedContentAsync(noiDungKeHoachId, cancellationToken) is null) return NotFound();
        return Ok(await dbContext.PhanCongThiHanhs.AsNoTracking().Where(x => x.NoiDungKeHoachId == noiDungKeHoachId && !x.IsDeleted).OrderBy(x => x.CreatedAt).ToListAsync(cancellationToken));
    }

    [HttpPost]
    public async Task<ActionResult> Create(CreatePhanCongRequest request, CancellationToken cancellationToken)
    {
        var denied = await EnsurePermissionAsync("ThiHanhPhapLuatPhanCong", "Create", "Create", cancellationToken);
        if (denied is not null) return denied;
        var content = await GetDraftContentAsync(request.NoiDungKeHoachId, cancellationToken);
        if (content is null) return BadRequest("Chỉ được phân công cho đầu việc thuộc kế hoạch NHAP.");
        if (request.HanThucHien > content.HanHoanThanh) return BadRequest("Hạn phân công không được sau hạn đầu việc.");
        var entity = new PhanCongThiHanh { NoiDungKeHoachId = content.Id, DonViDuocGiaoId = request.DonViDuocGiaoId, CanBoDuocGiaoId = request.CanBoDuocGiaoId, VaiTro = request.VaiTro.Trim(), HanThucHien = request.HanThucHien, MucDoUuTien = request.MucDoUuTien, CreatedBy = CurrentUser.UserId!.Value.ToString() };
        dbContext.PhanCongThiHanhs.Add(entity); AddHistory(content.KeHoachId, content.Id, "PHAN_CONG", "Phân công thực hiện đầu việc.");
        await dbContext.SaveChangesAsync(cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, new { entity.Id });
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var denied = await EnsurePermissionAsync("ThiHanhPhapLuatPhanCong", "Index", "Index", cancellationToken);
        if (denied is not null) return denied;
        var entity = await GetScopedAssignmentAsync(id, cancellationToken); return entity is null ? NotFound() : Ok(entity);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> Update(Guid id, UpdatePhanCongRequest request, CancellationToken cancellationToken)
    {
        var denied = await EnsurePermissionAsync("ThiHanhPhapLuatPhanCong", "Edit", "Edit", cancellationToken);
        if (denied is not null) return denied;
        var entity = await GetScopedAssignmentAsync(id, cancellationToken); if (entity is null) return NotFound();
        var content = await GetDraftContentAsync(entity.NoiDungKeHoachId, cancellationToken); if (content is null) return BadRequest("Chỉ được sửa phân công thuộc kế hoạch NHAP.");
        if (request.HanThucHien > content.HanHoanThanh) return BadRequest("Hạn phân công không được sau hạn đầu việc.");
        entity.DonViDuocGiaoId = request.DonViDuocGiaoId; entity.CanBoDuocGiaoId = request.CanBoDuocGiaoId; entity.VaiTro = request.VaiTro.Trim(); entity.HanThucHien = request.HanThucHien; entity.MucDoUuTien = request.MucDoUuTien; entity.UpdatedAt = DateTime.UtcNow; entity.UpdatedBy = CurrentUser.UserId!.Value.ToString();
        AddHistory(content.KeHoachId, content.Id, "CAP_NHAT_PHAN_CONG", "Cập nhật phân công thực hiện."); await dbContext.SaveChangesAsync(cancellationToken); return Ok(entity);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var denied = await EnsurePermissionAsync("ThiHanhPhapLuatPhanCong", "Delete", "Delete", cancellationToken);
        if (denied is not null) return denied;
        var entity = await GetScopedAssignmentAsync(id, cancellationToken); if (entity is null) return NotFound();
        var content = await GetDraftContentAsync(entity.NoiDungKeHoachId, cancellationToken); if (content is null) return BadRequest("Chỉ được xóa phân công thuộc kế hoạch NHAP.");
        entity.IsDeleted = true; entity.UpdatedAt = DateTime.UtcNow; entity.UpdatedBy = CurrentUser.UserId!.Value.ToString(); AddHistory(content.KeHoachId, content.Id, "XOA_PHAN_CONG", "Xóa mềm phân công thực hiện."); await dbContext.SaveChangesAsync(cancellationToken); return NoContent();
    }

    private async Task<PhanCongThiHanh?> GetScopedAssignmentAsync(Guid id, CancellationToken cancellationToken)
    {
        var query = from assignment in dbContext.PhanCongThiHanhs where assignment.Id == id && !assignment.IsDeleted join content in dbContext.NoiDungKeHoachs on assignment.NoiDungKeHoachId equals content.Id join plan in dbContext.KeHoachThiHanhPhapLuats on content.KeHoachId equals plan.Id where !content.IsDeleted && !plan.IsDeleted select new { assignment, plan.DonViChuTriId };
        if (!CurrentUser.IsSSA && CurrentUser.DonViId is { } donViId) query = query.Where(x => x.DonViChuTriId == donViId); return (await query.SingleOrDefaultAsync(cancellationToken))?.assignment;
    }
    private async Task<NoiDungKeHoach?> GetScopedContentAsync(Guid id, CancellationToken cancellationToken)
    {
        var query = from content in dbContext.NoiDungKeHoachs where content.Id == id && !content.IsDeleted join plan in dbContext.KeHoachThiHanhPhapLuats on content.KeHoachId equals plan.Id where !plan.IsDeleted select new { content, plan.DonViChuTriId };
        if (!CurrentUser.IsSSA && CurrentUser.DonViId is { } donViId) query = query.Where(x => x.DonViChuTriId == donViId); return (await query.SingleOrDefaultAsync(cancellationToken))?.content;
    }
    private async Task<NoiDungKeHoach?> GetDraftContentAsync(Guid id, CancellationToken cancellationToken)
    {
        var content = await GetScopedContentAsync(id, cancellationToken); if (content is null) return null;
        var planStatus = await dbContext.KeHoachThiHanhPhapLuats.Where(x => x.Id == content.KeHoachId).Select(x => x.TrangThaiId).SingleAsync(cancellationToken);
        var status = await trangThaiClient.GetAsync(planStatus, cancellationToken); return status is { TrangThai: true, NhomTrangThai: "KE_HOACH_THI_HANH_PHAP_LUAT", MaTrangThai: "NHAP" } ? content : null;
    }
    private void AddHistory(Guid planId, Guid contentId, string action, string message) => dbContext.LichSuXuLyThiHanhs.Add(new LichSuXuLyThiHanh { KeHoachId = planId, NoiDungKeHoachId = contentId, HanhDong = action, NoiDung = message, NguoiXuLyId = CurrentUser.UserId!.Value, DonViXuLyId = CurrentUser.DonViId });
}

public sealed record CreatePhanCongRequest(Guid NoiDungKeHoachId, Guid DonViDuocGiaoId, Guid? CanBoDuocGiaoId, string VaiTro, DateOnly HanThucHien, int MucDoUuTien);
public sealed record UpdatePhanCongRequest(Guid DonViDuocGiaoId, Guid? CanBoDuocGiaoId, string VaiTro, DateOnly HanThucHien, int MucDoUuTien);
