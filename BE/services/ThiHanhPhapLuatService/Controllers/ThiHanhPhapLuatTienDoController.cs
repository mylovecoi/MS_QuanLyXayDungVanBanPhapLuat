using BuildingBlocks.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThiHanhPhapLuatService.Infrastructure.Authorization;
using ThiHanhPhapLuatService.Infrastructure.DanhMuc;
using ThiHanhPhapLuatService.Infrastructure.Persistence;
using ThiHanhPhapLuatService.Infrastructure.Persistence.Entities;

namespace ThiHanhPhapLuatService.Controllers;

[ApiController]
[Route("api/thi-hanh-phap-luat/tien-do")]
public sealed class ThiHanhPhapLuatTienDoController(
    ICurrentUserContext user,
    IQuanTriHeThongPermissionClient permissionClient,
    IDanhMucTrangThaiClient trangThaiClient,
    ThiHanhPhapLuatDbContext dbContext) : ThiHanhPhapLuatControllerBase(user, permissionClient)
{
    [HttpGet]
    public async Task<ActionResult> GetList([FromQuery] Guid noiDungKeHoachId, [FromQuery] string? kyBaoCao, CancellationToken cancellationToken)
    {
        var denied = await EnsurePermissionAsync("ThiHanhPhapLuatTienDo", "Index", "Index", cancellationToken);
        if (denied is not null) return denied;
        if (!await CanAccessContentAsync(noiDungKeHoachId, cancellationToken)) return NotFound();
        var query = dbContext.BaoCaoTienDoThiHanhs.AsNoTracking().Where(x => x.NoiDungKeHoachId == noiDungKeHoachId && !x.IsDeleted);
        if (!string.IsNullOrWhiteSpace(kyBaoCao)) query = query.Where(x => x.KyBaoCao == kyBaoCao);
        return Ok(await query.OrderByDescending(x => x.NgayBaoCao).ToListAsync(cancellationToken));
    }

    [HttpPost]
    public async Task<ActionResult> Create(CreateTienDoRequest request, CancellationToken cancellationToken)
    {
        var denied = await EnsurePermissionAsync("ThiHanhPhapLuatTienDo", "Create", "Create", cancellationToken);
        if (denied is not null) return denied;
        if (!IsPercentValid(request.TyLeHoanThanh)) return BadRequest("Tỷ lệ hoàn thành phải từ 0 đến 100.");
        var assignment = await GetCurrentUserAssignmentAsync(request.PhanCongThiHanhId, cancellationToken);
        if (assignment is null || assignment.NoiDungKeHoachId != request.NoiDungKeHoachId) return Forbid();
        if (!await IsContentInProgressAsync(request.NoiDungKeHoachId, cancellationToken)) return BadRequest("Chỉ được cập nhật tiến độ khi đầu việc đang thực hiện.");
        if (!await IsReportStatusAsync(request.TrangThaiId, "NHAP", cancellationToken)) return BadRequest("Trạng thái báo cáo mới phải là NHAP.");
        if (await dbContext.BaoCaoTienDoThiHanhs.AnyAsync(x => x.PhanCongThiHanhId == assignment.Id && x.KyBaoCao == request.KyBaoCao && !x.IsDeleted, cancellationToken)) return Conflict("Đã có báo cáo cho phân công này trong kỳ báo cáo.");

        var entity = new BaoCaoTienDoThiHanh
        {
            NoiDungKeHoachId = request.NoiDungKeHoachId, PhanCongThiHanhId = assignment.Id, KyBaoCao = request.KyBaoCao.Trim(),
            TyLeHoanThanh = request.TyLeHoanThanh, KetQua = request.KetQua, KhoKhan = request.KhoKhan, KienNghi = request.KienNghi,
            TrangThaiId = request.TrangThaiId, NgayBaoCao = DateTime.UtcNow, CreatedBy = CurrentUser.UserId!.Value.ToString()
        };
        dbContext.BaoCaoTienDoThiHanhs.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, new { entity.Id });
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var denied = await EnsurePermissionAsync("ThiHanhPhapLuatTienDo", "Index", "Index", cancellationToken);
        if (denied is not null) return denied;
        var entity = await GetAccessibleReportAsync(id, cancellationToken);
        return entity is null ? NotFound() : Ok(entity);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> Update(Guid id, UpdateTienDoRequest request, CancellationToken cancellationToken)
    {
        var denied = await EnsurePermissionAsync("ThiHanhPhapLuatTienDo", "Edit", "Edit", cancellationToken);
        if (denied is not null) return denied;
        if (!IsPercentValid(request.TyLeHoanThanh)) return BadRequest("Tỷ lệ hoàn thành phải từ 0 đến 100.");
        var entity = await GetCurrentUserReportAsync(id, cancellationToken);
        if (entity is null) return NotFound();
        if (!await IsEditableReportStatusAsync(entity.TrangThaiId, cancellationToken)) return BadRequest("Chỉ được sửa báo cáo ở trạng thái NHAP hoặc CAN_BO_SUNG.");
        entity.TyLeHoanThanh = request.TyLeHoanThanh; entity.KetQua = request.KetQua; entity.KhoKhan = request.KhoKhan; entity.KienNghi = request.KienNghi;
        entity.UpdatedAt = DateTime.UtcNow; entity.UpdatedBy = CurrentUser.UserId!.Value.ToString();
        await dbContext.SaveChangesAsync(cancellationToken);
        return Ok(entity);
    }

    [HttpPost("{id:guid}/gui")]
    public async Task<ActionResult> Submit(Guid id, SubmitTienDoRequest request, CancellationToken cancellationToken)
    {
        var denied = await EnsurePermissionAsync("ThiHanhPhapLuatTienDo", "Edit", "Edit", cancellationToken);
        if (denied is not null) return denied;
        var report = await GetCurrentUserReportAsync(id, cancellationToken);
        if (report is null) return NotFound();
        if (!await IsEditableReportStatusAsync(report.TrangThaiId, cancellationToken) || !await IsReportStatusAsync(request.TrangThaiBaoCaoId, "DA_GUI", cancellationToken) || !await IsContentStatusAsync(request.TrangThaiNoiDungId, "CHO_DANH_GIA", cancellationToken)) return BadRequest("Chuyển trạng thái gửi báo cáo không hợp lệ.");
        var content = await dbContext.NoiDungKeHoachs.SingleAsync(x => x.Id == report.NoiDungKeHoachId && !x.IsDeleted, cancellationToken);
        var oldReportStatus = report.TrangThaiId; var oldContentStatus = content.TrangThaiId;
        report.TrangThaiId = request.TrangThaiBaoCaoId; report.UpdatedAt = DateTime.UtcNow; report.UpdatedBy = CurrentUser.UserId!.Value.ToString();
        content.TrangThaiId = request.TrangThaiNoiDungId; content.TyLeHoanThanh = report.TyLeHoanThanh; content.UpdatedAt = DateTime.UtcNow; content.UpdatedBy = CurrentUser.UserId!.Value.ToString();
        dbContext.LichSuXuLyThiHanhs.Add(new LichSuXuLyThiHanh { KeHoachId = content.KeHoachId, NoiDungKeHoachId = content.Id, HanhDong = "GUI_BAO_CAO_TIEN_DO", TrangThaiTruocId = oldContentStatus, TrangThaiSauId = content.TrangThaiId, NoiDung = request.GhiChu, NguoiXuLyId = CurrentUser.UserId!.Value, DonViXuLyId = CurrentUser.DonViId });
        if (await IsReportStatusAsync(oldReportStatus, "CAN_BO_SUNG", cancellationToken))
        {
            var requirements = await dbContext.YeuCauBoSungThiHanhs.Where(x => x.BaoCaoTienDoThiHanhId == report.Id && x.TrangThai == "DANG_MO" && !x.IsDeleted).ToListAsync(cancellationToken);
            foreach (var requirement in requirements) { requirement.TrangThai = "DA_HOAN_THANH"; requirement.NgayHoanThanh = DateTime.UtcNow; requirement.UpdatedAt = DateTime.UtcNow; requirement.UpdatedBy = CurrentUser.UserId!.Value.ToString(); }
        }
        await dbContext.SaveChangesAsync(cancellationToken);
        return Ok(new { report.Id, TrangThaiBaoCaoTruocId = oldReportStatus, TrangThaiBaoCaoId = report.TrangThaiId, TrangThaiNoiDungTruocId = oldContentStatus, TrangThaiNoiDungId = content.TrangThaiId });
    }

    [HttpPost("{id:guid}/tep-dinh-kem")]
    public async Task<ActionResult> AddAttachment(Guid id, CreateTepDinhKemRequest request, CancellationToken cancellationToken)
    {
        var denied = await EnsurePermissionAsync("ThiHanhPhapLuatTienDo", "Create", "Create", cancellationToken);
        if (denied is not null) return denied;
        var report = await GetCurrentUserReportAsync(id, cancellationToken);
        if (report is null) return NotFound();
        if (!await IsEditableReportStatusAsync(report.TrangThaiId, cancellationToken)) return BadRequest("Chỉ được thêm minh chứng khi báo cáo ở trạng thái NHAP hoặc CAN_BO_SUNG.");
        var currentFiles = await dbContext.TepDinhKemThiHanhs.Where(x => x.BaoCaoTienDoThiHanhId == id && x.LoaiTaiLieu == request.LoaiTaiLieu && x.IsCurrent && !x.IsDeleted).ToListAsync(cancellationToken);
        foreach (var file in currentFiles) { file.IsCurrent = false; file.UpdatedAt = DateTime.UtcNow; file.UpdatedBy = CurrentUser.UserId!.Value.ToString(); }
        var version = await dbContext.TepDinhKemThiHanhs.Where(x => x.BaoCaoTienDoThiHanhId == id && x.LoaiTaiLieu == request.LoaiTaiLieu).Select(x => (int?)x.PhienBan).MaxAsync(cancellationToken) ?? 0;
        var entity = new TepDinhKemThiHanh { BaoCaoTienDoThiHanhId = id, NoiDungKeHoachId = report.NoiDungKeHoachId, LoaiTaiLieu = request.LoaiTaiLieu.Trim(), TenTep = request.TenTep.Trim(), DuongDan = request.DuongDan.Trim(), PhienBan = version + 1, IsCurrent = true, CreatedBy = CurrentUser.UserId!.Value.ToString() };
        dbContext.TepDinhKemThiHanhs.Add(entity); await dbContext.SaveChangesAsync(cancellationToken);
        return Ok(new { entity.Id, entity.PhienBan });
    }

    private async Task<bool> CanAccessContentAsync(Guid contentId, CancellationToken cancellationToken)
    {
        if (CurrentUser.IsSSA) return await dbContext.NoiDungKeHoachs.AnyAsync(x => x.Id == contentId && !x.IsDeleted, cancellationToken);
        if (CurrentUser.DonViId is not { } donViId) return false;
        return await (from content in dbContext.NoiDungKeHoachs join plan in dbContext.KeHoachThiHanhPhapLuats on content.KeHoachId equals plan.Id join assignment in dbContext.PhanCongThiHanhs on content.Id equals assignment.NoiDungKeHoachId where content.Id == contentId && !content.IsDeleted && !plan.IsDeleted && !assignment.IsDeleted && (plan.DonViChuTriId == donViId || assignment.DonViDuocGiaoId == donViId) select content.Id).AnyAsync(cancellationToken);
    }
    private async Task<PhanCongThiHanh?> GetCurrentUserAssignmentAsync(Guid id, CancellationToken cancellationToken)
    {
        var query = dbContext.PhanCongThiHanhs.Where(x => x.Id == id && !x.IsDeleted);
        if (!CurrentUser.IsSSA)
        {
            if (CurrentUser.DonViId is not { } donViId || CurrentUser.UserId is not { } userId) return null;
            query = query.Where(x => x.DonViDuocGiaoId == donViId && (x.CanBoDuocGiaoId == null || x.CanBoDuocGiaoId == userId));
        }
        return await query.SingleOrDefaultAsync(cancellationToken);
    }
    private async Task<BaoCaoTienDoThiHanh?> GetAccessibleReportAsync(Guid id, CancellationToken cancellationToken)
    {
        var report = await dbContext.BaoCaoTienDoThiHanhs.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        return report is not null && await CanAccessContentAsync(report.NoiDungKeHoachId, cancellationToken) ? report : null;
    }
    private async Task<BaoCaoTienDoThiHanh?> GetCurrentUserReportAsync(Guid id, CancellationToken cancellationToken)
    {
        var report = await dbContext.BaoCaoTienDoThiHanhs.SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        if (report is null || report.PhanCongThiHanhId is null) return null;
        var assignment = await GetCurrentUserAssignmentAsync(report.PhanCongThiHanhId.Value, cancellationToken);
        return assignment is not null && assignment.NoiDungKeHoachId == report.NoiDungKeHoachId ? report : null;
    }
    private async Task<bool> IsContentInProgressAsync(Guid id, CancellationToken cancellationToken) { var content = await dbContext.NoiDungKeHoachs.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken); return content is not null && await IsContentStatusAsync(content.TrangThaiId, "DANG_THUC_HIEN", cancellationToken); }
    private async Task<bool> IsReportStatusAsync(Guid id, string code, CancellationToken cancellationToken) { var status = await trangThaiClient.GetAsync(id, cancellationToken); return status is { TrangThai: true, NhomTrangThai: "BAO_CAO_TIEN_DO_THI_HANH" } && status.MaTrangThai == code; }
    private async Task<bool> IsEditableReportStatusAsync(Guid id, CancellationToken cancellationToken) => await IsReportStatusAsync(id, "NHAP", cancellationToken) || await IsReportStatusAsync(id, "CAN_BO_SUNG", cancellationToken);
    private async Task<bool> IsContentStatusAsync(Guid id, string code, CancellationToken cancellationToken) { var status = await trangThaiClient.GetAsync(id, cancellationToken); return status is { TrangThai: true, NhomTrangThai: "NOI_DUNG_THI_HANH_PHAP_LUAT" } && status.MaTrangThai == code; }
    private static bool IsPercentValid(decimal value) => value is >= 0 and <= 100;
}

public sealed record CreateTienDoRequest(Guid NoiDungKeHoachId, Guid PhanCongThiHanhId, string KyBaoCao, decimal TyLeHoanThanh, string? KetQua, string? KhoKhan, string? KienNghi, Guid TrangThaiId);
public sealed record UpdateTienDoRequest(decimal TyLeHoanThanh, string? KetQua, string? KhoKhan, string? KienNghi);
public sealed record SubmitTienDoRequest(Guid TrangThaiBaoCaoId, Guid TrangThaiNoiDungId, string? GhiChu);
public sealed record CreateTepDinhKemRequest(string LoaiTaiLieu, string TenTep, string DuongDan);
