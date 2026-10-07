using BuildingBlocks.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThiHanhPhapLuatService.Infrastructure.Authorization;
using ThiHanhPhapLuatService.Infrastructure.DanhMuc;
using ThiHanhPhapLuatService.Infrastructure.Persistence;
using ThiHanhPhapLuatService.Infrastructure.Persistence.Entities;

namespace ThiHanhPhapLuatService.Controllers;

[ApiController]
[Route("api/thi-hanh-phap-luat/noi-dung-ke-hoach")]
public sealed class ThiHanhPhapLuatNoiDungKeHoachController(
    ICurrentUserContext user,
    IQuanTriHeThongPermissionClient permissionClient,
    IDanhMucTrangThaiClient trangThaiClient,
    ThiHanhPhapLuatDbContext dbContext) : ThiHanhPhapLuatControllerBase(user, permissionClient)
{
    [HttpGet]
    public async Task<ActionResult> GetList([FromQuery] Guid keHoachId, CancellationToken cancellationToken)
    {
        var denied = await EnsurePermissionAsync("ThiHanhPhapLuatNoiDungKeHoach", "Index", "Index", cancellationToken);
        if (denied is not null) return denied;
        if (!await CanAccessPlanAsync(keHoachId, cancellationToken)) return NotFound();
        return Ok(await dbContext.NoiDungKeHoachs.AsNoTracking().Where(x => x.KeHoachId == keHoachId && !x.IsDeleted).OrderBy(x => x.ThuTu).ToListAsync(cancellationToken));
    }

    [HttpPost]
    public async Task<ActionResult> Create(CreateNoiDungKeHoachRequest request, CancellationToken cancellationToken)
    {
        var denied = await EnsurePermissionAsync("ThiHanhPhapLuatNoiDungKeHoach", "Create", "Create", cancellationToken);
        if (denied is not null) return denied;
        var plan = await GetDraftPlanAsync(request.KeHoachId, cancellationToken);
        if (plan is null) return BadRequest("Kế hoạch không tồn tại, ngoài phạm vi dữ liệu hoặc không ở trạng thái NHAP.");
        if (request.HanHoanThanh < plan.TuNgay || request.HanHoanThanh > plan.DenNgay) return BadRequest("Hạn hoàn thành phải nằm trong thời gian kế hoạch.");
        if (!await IsStatusAsync(request.TrangThaiId, "CHUA_THUC_HIEN", cancellationToken)) return BadRequest("Trạng thái đầu việc phải là CHUA_THUC_HIEN.");
        if (await dbContext.NoiDungKeHoachs.AnyAsync(x => x.KeHoachId == plan.Id && x.MaNoiDung == request.MaNoiDung && !x.IsDeleted, cancellationToken)) return Conflict("Mã nội dung đã tồn tại trong kế hoạch.");

        var entity = new NoiDungKeHoach
        {
            KeHoachId = plan.Id, MaNoiDung = request.MaNoiDung.Trim(), TenNoiDung = request.TenNoiDung.Trim(), NoiDung = request.NoiDung,
            ChiTieu = request.ChiTieu, DonViTinh = request.DonViTinh, HanHoanThanh = request.HanHoanThanh, ThuTu = request.ThuTu,
            TrangThaiId = request.TrangThaiId, TyLeHoanThanh = 0, CreatedBy = CurrentUser.UserId!.Value.ToString()
        };
        dbContext.NoiDungKeHoachs.Add(entity);
        AddHistory(plan.Id, entity.Id, "TAO_NOI_DUNG_KE_HOACH", null, entity.TrangThaiId, "Tạo nội dung kế hoạch.");
        await dbContext.SaveChangesAsync(cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, new { entity.Id });
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var denied = await EnsurePermissionAsync("ThiHanhPhapLuatNoiDungKeHoach", "Index", "Index", cancellationToken);
        if (denied is not null) return denied;
        var entity = await GetScopedAsync(id, cancellationToken);
        return entity is null ? NotFound() : Ok(entity);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> Update(Guid id, UpdateNoiDungKeHoachRequest request, CancellationToken cancellationToken)
    {
        var denied = await EnsurePermissionAsync("ThiHanhPhapLuatNoiDungKeHoach", "Edit", "Edit", cancellationToken);
        if (denied is not null) return denied;
        var entity = await GetScopedAsync(id, cancellationToken);
        if (entity is null) return NotFound();
        var plan = await GetDraftPlanAsync(entity.KeHoachId, cancellationToken);
        if (plan is null) return BadRequest("Chỉ được sửa đầu việc khi kế hoạch ở trạng thái NHAP.");
        if (request.HanHoanThanh < plan.TuNgay || request.HanHoanThanh > plan.DenNgay) return BadRequest("Hạn hoàn thành phải nằm trong thời gian kế hoạch.");
        entity.TenNoiDung = request.TenNoiDung.Trim(); entity.NoiDung = request.NoiDung; entity.ChiTieu = request.ChiTieu; entity.DonViTinh = request.DonViTinh; entity.HanHoanThanh = request.HanHoanThanh; entity.ThuTu = request.ThuTu;
        entity.UpdatedAt = DateTime.UtcNow; entity.UpdatedBy = CurrentUser.UserId!.Value.ToString();
        AddHistory(plan.Id, entity.Id, "CAP_NHAT_NOI_DUNG_KE_HOACH", entity.TrangThaiId, entity.TrangThaiId, "Cập nhật nội dung kế hoạch.");
        await dbContext.SaveChangesAsync(cancellationToken);
        return Ok(entity);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var denied = await EnsurePermissionAsync("ThiHanhPhapLuatNoiDungKeHoach", "Delete", "Delete", cancellationToken);
        if (denied is not null) return denied;
        var entity = await GetScopedAsync(id, cancellationToken);
        if (entity is null) return NotFound();
        var plan = await GetDraftPlanAsync(entity.KeHoachId, cancellationToken);
        if (plan is null) return BadRequest("Chỉ được xóa đầu việc khi kế hoạch ở trạng thái NHAP.");
        entity.IsDeleted = true; entity.UpdatedAt = DateTime.UtcNow; entity.UpdatedBy = CurrentUser.UserId!.Value.ToString();
        AddHistory(plan.Id, entity.Id, "XOA_NOI_DUNG_KE_HOACH", entity.TrangThaiId, null, "Xóa mềm nội dung kế hoạch.");
        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private async Task<NoiDungKeHoach?> GetScopedAsync(Guid id, CancellationToken cancellationToken)
    {
        var query = from content in dbContext.NoiDungKeHoachs where content.Id == id && !content.IsDeleted join plan in dbContext.KeHoachThiHanhPhapLuats on content.KeHoachId equals plan.Id where !plan.IsDeleted select new { content, plan.DonViChuTriId };
        if (!CurrentUser.IsSSA && CurrentUser.DonViId is { } donViId) query = query.Where(x => x.DonViChuTriId == donViId);
        return (await query.SingleOrDefaultAsync(cancellationToken))?.content;
    }

    private async Task<KeHoachThiHanhPhapLuat?> GetDraftPlanAsync(Guid id, CancellationToken cancellationToken)
    {
        var plan = await GetPlanAsync(id, cancellationToken);
        return plan is not null && await IsPlanStatusAsync(plan.TrangThaiId, "NHAP", cancellationToken) ? plan : null;
    }

    private async Task<bool> CanAccessPlanAsync(Guid id, CancellationToken cancellationToken) => await GetPlanAsync(id, cancellationToken) is not null;
    private async Task<KeHoachThiHanhPhapLuat?> GetPlanAsync(Guid id, CancellationToken cancellationToken)
    {
        var query = dbContext.KeHoachThiHanhPhapLuats.Where(x => x.Id == id && !x.IsDeleted);
        if (!CurrentUser.IsSSA && CurrentUser.DonViId is { } donViId) query = query.Where(x => x.DonViChuTriId == donViId);
        return await query.SingleOrDefaultAsync(cancellationToken);
    }
    private async Task<bool> IsPlanStatusAsync(Guid id, string code, CancellationToken cancellationToken) { var status = await trangThaiClient.GetAsync(id, cancellationToken); return status is { TrangThai: true, NhomTrangThai: "KE_HOACH_THI_HANH_PHAP_LUAT" } && status.MaTrangThai == code; }
    private async Task<bool> IsStatusAsync(Guid id, string code, CancellationToken cancellationToken) { var status = await trangThaiClient.GetAsync(id, cancellationToken); return status is { TrangThai: true, NhomTrangThai: "NOI_DUNG_THI_HANH_PHAP_LUAT" } && status.MaTrangThai == code; }
    private void AddHistory(Guid planId, Guid contentId, string action, Guid? before, Guid? after, string content) => dbContext.LichSuXuLyThiHanhs.Add(new LichSuXuLyThiHanh { KeHoachId = planId, NoiDungKeHoachId = contentId, HanhDong = action, TrangThaiTruocId = before, TrangThaiSauId = after, NoiDung = content, NguoiXuLyId = CurrentUser.UserId!.Value, DonViXuLyId = CurrentUser.DonViId });
}

public sealed record CreateNoiDungKeHoachRequest(Guid KeHoachId, string MaNoiDung, string TenNoiDung, string? NoiDung, decimal? ChiTieu, string? DonViTinh, DateOnly HanHoanThanh, int ThuTu, Guid TrangThaiId);
public sealed record UpdateNoiDungKeHoachRequest(string TenNoiDung, string? NoiDung, decimal? ChiTieu, string? DonViTinh, DateOnly HanHoanThanh, int ThuTu);
