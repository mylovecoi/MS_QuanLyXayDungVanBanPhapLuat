using BuildingBlocks.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThiHanhPhapLuatService.Infrastructure.Authorization;
using ThiHanhPhapLuatService.Infrastructure.DanhMuc;
using ThiHanhPhapLuatService.Infrastructure.Persistence;
using ThiHanhPhapLuatService.Infrastructure.Persistence.Entities;

namespace ThiHanhPhapLuatService.Controllers;

[ApiController]
[Route("api/thi-hanh-phap-luat/ke-hoach")]
public sealed class ThiHanhPhapLuatKeHoachController(
    ICurrentUserContext user,
    IQuanTriHeThongPermissionClient permissionClient,
    IDanhMucTrangThaiClient trangThaiClient,
    ThiHanhPhapLuatDbContext dbContext) : ThiHanhPhapLuatControllerBase(user, permissionClient)
{
    [HttpPost]
    public async Task<ActionResult> Create(CreateKeHoachRequest request, CancellationToken cancellationToken)
    {
        var denied = await EnsurePermissionAsync("ThiHanhPhapLuatKeHoach", "Create", "Create", cancellationToken);
        if (denied is not null) return denied;
        if (!CurrentUser.IsSSA && CurrentUser.DonViId != request.DonViChuTriId) return Forbid();
        if (request.TuNgay > request.DenNgay) return BadRequest("Thời gian thực hiện không hợp lệ.");
        if (!await IsStatusAsync(request.TrangThaiId, "NHAP", cancellationToken)) return BadRequest("Trạng thái phải là NHAP thuộc nhóm kế hoạch thi hành pháp luật.");

        var entity = new KeHoachThiHanhPhapLuat
        {
            MaKeHoach = request.MaKeHoach.Trim(), TenKeHoach = request.TenKeHoach.Trim(), Nam = request.Nam,
            TuNgay = request.TuNgay, DenNgay = request.DenNgay, DonViChuTriId = request.DonViChuTriId,
            NguoiPhuTrachId = request.NguoiPhuTrachId, TrangThaiId = request.TrangThaiId, PhamVi = request.PhamVi,
            MucTieu = request.MucTieu, CreatedBy = CurrentUser.UserId!.Value.ToString()
        };
        if (await dbContext.KeHoachThiHanhPhapLuats.AnyAsync(x => x.MaKeHoach == entity.MaKeHoach && !x.IsDeleted, cancellationToken)) return Conflict("Mã kế hoạch đã tồn tại.");
        dbContext.KeHoachThiHanhPhapLuats.Add(entity);
        AddHistory(entity.Id, null, "TAO_KE_HOACH", null, entity.TrangThaiId, "Tạo kế hoạch.");
        await dbContext.SaveChangesAsync(cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, new { entity.Id });
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var denied = await EnsurePermissionAsync("ThiHanhPhapLuatKeHoach", "Index", "Index", cancellationToken);
        if (denied is not null) return denied;
        var entity = await GetScopedAsync(id, cancellationToken);
        return entity is null ? NotFound() : Ok(entity);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> Update(Guid id, UpdateKeHoachRequest request, CancellationToken cancellationToken)
    {
        var denied = await EnsurePermissionAsync("ThiHanhPhapLuatKeHoach", "Edit", "Edit", cancellationToken);
        if (denied is not null) return denied;
        var entity = await GetScopedAsync(id, cancellationToken);
        if (entity is null) return NotFound();
        if (request.TuNgay > request.DenNgay) return BadRequest("Thời gian thực hiện không hợp lệ.");
        if (!await IsStatusAsync(entity.TrangThaiId, "NHAP", cancellationToken)) return BadRequest("Chỉ được sửa kế hoạch ở trạng thái NHAP.");
        entity.TenKeHoach = request.TenKeHoach.Trim(); entity.Nam = request.Nam; entity.TuNgay = request.TuNgay; entity.DenNgay = request.DenNgay;
        entity.NguoiPhuTrachId = request.NguoiPhuTrachId; entity.PhamVi = request.PhamVi; entity.MucTieu = request.MucTieu;
        entity.UpdatedAt = DateTime.UtcNow; entity.UpdatedBy = CurrentUser.UserId!.Value.ToString();
        AddHistory(entity.Id, null, "CAP_NHAT_KE_HOACH", entity.TrangThaiId, entity.TrangThaiId, "Cập nhật kế hoạch.");
        await dbContext.SaveChangesAsync(cancellationToken);
        return Ok(entity);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var denied = await EnsurePermissionAsync("ThiHanhPhapLuatKeHoach", "Delete", "Delete", cancellationToken);
        if (denied is not null) return denied;
        var entity = await GetScopedAsync(id, cancellationToken);
        if (entity is null) return NotFound();
        if (!await IsStatusAsync(entity.TrangThaiId, "NHAP", cancellationToken)) return BadRequest("Chỉ được xóa kế hoạch ở trạng thái NHAP.");
        entity.IsDeleted = true; entity.UpdatedAt = DateTime.UtcNow; entity.UpdatedBy = CurrentUser.UserId!.Value.ToString();
        AddHistory(entity.Id, null, "XOA_KE_HOACH", entity.TrangThaiId, null, "Xóa mềm kế hoạch.");
        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:guid}/bat-dau-thuc-hien")]
    public async Task<ActionResult> Start(Guid id, ChangeKeHoachStatusRequest request, CancellationToken cancellationToken)
    {
        var denied = await EnsurePermissionAsync("ThiHanhPhapLuatKeHoach", "Edit", "Edit", cancellationToken);
        if (denied is not null) return denied;
        var entity = await GetScopedAsync(id, cancellationToken);
        if (entity is null) return NotFound();
        if (!await IsStatusAsync(entity.TrangThaiId, "NHAP", cancellationToken) || !await IsStatusAsync(request.TrangThaiId, "DANG_THUC_HIEN", cancellationToken) || !await IsNoiDungStatusAsync(request.TrangThaiNoiDungId, "DANG_THUC_HIEN", cancellationToken)) return BadRequest("Chuyển trạng thái không hợp lệ.");
        if (!await dbContext.NoiDungKeHoachs.AnyAsync(x => x.KeHoachId == id && !x.IsDeleted, cancellationToken)) return BadRequest("Kế hoạch phải có ít nhất một nội dung thực hiện trước khi bắt đầu.");
        var hasUnassignedContent = await dbContext.NoiDungKeHoachs.AnyAsync(x => x.KeHoachId == id && !x.IsDeleted && !dbContext.PhanCongThiHanhs.Any(p => p.NoiDungKeHoachId == x.Id && !p.IsDeleted), cancellationToken);
        if (hasUnassignedContent) return BadRequest("Mỗi nội dung kế hoạch phải có ít nhất một phân công trước khi bắt đầu.");
        var contents = await dbContext.NoiDungKeHoachs.Where(x => x.KeHoachId == id && !x.IsDeleted).ToListAsync(cancellationToken);
        foreach (var content in contents)
        {
            if (!await IsNoiDungStatusAsync(content.TrangThaiId, "CHUA_THUC_HIEN", cancellationToken)) return BadRequest("Chỉ được bắt đầu kế hoạch khi các đầu việc ở trạng thái CHUA_THUC_HIEN.");
        }
        var before = entity.TrangThaiId; entity.TrangThaiId = request.TrangThaiId; entity.UpdatedAt = DateTime.UtcNow; entity.UpdatedBy = CurrentUser.UserId!.Value.ToString();
        foreach (var content in contents)
        {
            var contentBefore = content.TrangThaiId;
            content.TrangThaiId = request.TrangThaiNoiDungId; content.UpdatedAt = DateTime.UtcNow; content.UpdatedBy = CurrentUser.UserId!.Value.ToString();
            AddHistory(entity.Id, content.Id, "BAT_DAU_NOI_DUNG_KE_HOACH", contentBefore, content.TrangThaiId, "Bắt đầu thực hiện theo kế hoạch.");
        }
        AddHistory(entity.Id, null, "BAT_DAU_THUC_HIEN", before, entity.TrangThaiId, request.GhiChu);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Ok(entity);
    }

    [HttpPut("{id:guid}/can-cu-phap-ly")]
    public async Task<ActionResult> ReplaceLegalBases(Guid id, IReadOnlyList<CanCuPhapLyRequest> requests, CancellationToken cancellationToken)
    {
        var denied = await EnsurePermissionAsync("ThiHanhPhapLuatKeHoach", "Edit", "Edit", cancellationToken);
        if (denied is not null) return denied;
        var entity = await GetScopedAsync(id, cancellationToken);
        if (entity is null) return NotFound();
        if (!await IsStatusAsync(entity.TrangThaiId, "NHAP", cancellationToken)) return BadRequest("Chỉ được cập nhật căn cứ pháp lý ở trạng thái NHAP.");
        dbContext.KeHoachCanCuPhapLys.RemoveRange(dbContext.KeHoachCanCuPhapLys.Where(x => x.KeHoachId == id));
        dbContext.KeHoachCanCuPhapLys.AddRange(requests.Select((x, index) => new KeHoachCanCuPhapLy { KeHoachId = id, VanBanId = x.VanBanId, SoKyHieu = x.SoKyHieu, TrichYeu = x.TrichYeu.Trim(), ThuTu = index + 1, CreatedBy = CurrentUser.UserId!.Value.ToString() }));
        AddHistory(entity.Id, null, "CAP_NHAT_CAN_CU_PHAP_LY", entity.TrangThaiId, entity.TrangThaiId, "Cập nhật căn cứ pháp lý.");
        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private async Task<KeHoachThiHanhPhapLuat?> GetScopedAsync(Guid id, CancellationToken cancellationToken)
    {
        var query = dbContext.KeHoachThiHanhPhapLuats.Where(x => x.Id == id && !x.IsDeleted);
        if (!CurrentUser.IsSSA && CurrentUser.DonViId is { } donViId) query = query.Where(x => x.DonViChuTriId == donViId);
        return await query.SingleOrDefaultAsync(cancellationToken);
    }

    private async Task<bool> IsStatusAsync(Guid id, string expectedCode, CancellationToken cancellationToken)
    {
        var status = await trangThaiClient.GetAsync(id, cancellationToken);
        return status is { TrangThai: true, NhomTrangThai: "KE_HOACH_THI_HANH_PHAP_LUAT" } && status.MaTrangThai == expectedCode;
    }
    private async Task<bool> IsNoiDungStatusAsync(Guid id, string expectedCode, CancellationToken cancellationToken)
    {
        var status = await trangThaiClient.GetAsync(id, cancellationToken);
        return status is { TrangThai: true, NhomTrangThai: "NOI_DUNG_THI_HANH_PHAP_LUAT" } && status.MaTrangThai == expectedCode;
    }

    private void AddHistory(Guid keHoachId, Guid? noiDungKeHoachId, string action, Guid? before, Guid? after, string? content) =>
        dbContext.LichSuXuLyThiHanhs.Add(new LichSuXuLyThiHanh { KeHoachId = keHoachId, NoiDungKeHoachId = noiDungKeHoachId, HanhDong = action, TrangThaiTruocId = before, TrangThaiSauId = after, NoiDung = content, NguoiXuLyId = CurrentUser.UserId!.Value, DonViXuLyId = CurrentUser.DonViId });
}

public sealed record CreateKeHoachRequest(string MaKeHoach, string TenKeHoach, int Nam, DateOnly TuNgay, DateOnly DenNgay, Guid DonViChuTriId, Guid? NguoiPhuTrachId, Guid TrangThaiId, string? PhamVi, string? MucTieu);
public sealed record UpdateKeHoachRequest(string TenKeHoach, int Nam, DateOnly TuNgay, DateOnly DenNgay, Guid? NguoiPhuTrachId, string? PhamVi, string? MucTieu);
public sealed record ChangeKeHoachStatusRequest(Guid TrangThaiId, Guid TrangThaiNoiDungId, string? GhiChu);
public sealed record CanCuPhapLyRequest(Guid? VanBanId, string? SoKyHieu, string TrichYeu);
