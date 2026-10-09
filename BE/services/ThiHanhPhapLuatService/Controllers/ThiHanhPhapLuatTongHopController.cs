using System.Text.Json;
using BuildingBlocks.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThiHanhPhapLuatService.Infrastructure.Authorization;
using ThiHanhPhapLuatService.Infrastructure.DanhMuc;
using ThiHanhPhapLuatService.Infrastructure.Persistence;
using ThiHanhPhapLuatService.Infrastructure.Persistence.Entities;

namespace ThiHanhPhapLuatService.Controllers;

[ApiController]
[Route("api/thi-hanh-phap-luat/tong-hop")]
public sealed class ThiHanhPhapLuatTongHopController(
    ICurrentUserContext user,
    IQuanTriHeThongPermissionClient permissionClient,
    IDanhMucTrangThaiClient trangThaiClient,
    ThiHanhPhapLuatDbContext dbContext) : ThiHanhPhapLuatControllerBase(user, permissionClient)
{
    [HttpGet]
    public async Task<ActionResult> GetList([FromQuery] Guid keHoachId, CancellationToken cancellationToken)
    {
        var denied = await EnsurePermissionAsync("ThiHanhPhapLuatTongHop", "Index", "Index", cancellationToken);
        if (denied is not null) return denied;
        if (await GetScopedPlanAsync(keHoachId, cancellationToken) is null) return NotFound();
        return Ok(await dbContext.BaoCaoTongHopThiHanhs.AsNoTracking().Where(x => x.KeHoachId == keHoachId && !x.IsDeleted).OrderByDescending(x => x.CreatedAt).ToListAsync(cancellationToken));
    }

    [HttpPost]
    public async Task<ActionResult> Create(CreateBaoCaoTongHopRequest request, CancellationToken cancellationToken)
    {
        var denied = await EnsurePermissionAsync("ThiHanhPhapLuatTongHop", "Create", "Create", cancellationToken);
        if (denied is not null) return denied;
        if (request.TuNgay > request.DenNgay) return BadRequest("Khoảng thời gian báo cáo không hợp lệ.");
        if (await GetScopedPlanAsync(request.KeHoachId, cancellationToken) is null) return NotFound("Không tìm thấy kế hoạch.");
        if (!await IsReportStatusAsync(request.TrangThaiId, "NHAP", cancellationToken)) return BadRequest("Trạng thái báo cáo tổng hợp mới phải là NHAP.");
        if (await dbContext.BaoCaoTongHopThiHanhs.AnyAsync(x => x.KeHoachId == request.KeHoachId && x.KyBaoCao == request.KyBaoCao && !x.IsDeleted, cancellationToken)) return Conflict("Đã có báo cáo tổng hợp cho kỳ này.");
        var entity = new BaoCaoTongHopThiHanh { MaBaoCao = request.MaBaoCao.Trim(), KeHoachId = request.KeHoachId, KyBaoCao = request.KyBaoCao.Trim(), TuNgay = request.TuNgay, DenNgay = request.DenNgay, TrangThaiId = request.TrangThaiId, CreatedBy = CurrentUser.UserId!.Value.ToString() };
        if (await dbContext.BaoCaoTongHopThiHanhs.AnyAsync(x => x.MaBaoCao == entity.MaBaoCao && !x.IsDeleted, cancellationToken)) return Conflict("Mã báo cáo đã tồn tại.");
        dbContext.BaoCaoTongHopThiHanhs.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, new { entity.Id });
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var denied = await EnsurePermissionAsync("ThiHanhPhapLuatTongHop", "Index", "Index", cancellationToken);
        if (denied is not null) return denied;
        var entity = await GetScopedReportAsync(id, tracking: false, cancellationToken);
        return entity is null ? NotFound() : Ok(entity);
    }

    [HttpGet("{id:guid}/chi-tiet")]
    public async Task<ActionResult> GetDetails(Guid id, [FromQuery] int? lanChot, CancellationToken cancellationToken)
    {
        var denied = await EnsurePermissionAsync("ThiHanhPhapLuatTongHop", "Index", "Index", cancellationToken);
        if (denied is not null) return denied;
        if (await GetScopedReportAsync(id, tracking: false, cancellationToken) is null) return NotFound();
        var query = dbContext.BaoCaoTongHopChiTiets.AsNoTracking().Where(x => x.BaoCaoTongHopThiHanhId == id && !x.IsDeleted);
        if (lanChot.HasValue) query = query.Where(x => x.LanChot == lanChot);
        return Ok(await query.OrderBy(x => x.LanChot).ThenBy(x => x.CreatedAt).ToListAsync(cancellationToken));
    }

    [HttpPost("{id:guid}/chot")]
    public async Task<ActionResult> Close(Guid id, ChotBaoCaoTongHopRequest request, CancellationToken cancellationToken)
    {
        var denied = await EnsurePermissionAsync("ThiHanhPhapLuatTongHop", "Approve", "Approve", cancellationToken);
        if (denied is not null) return denied;
        var report = await GetScopedReportAsync(id, tracking: true, cancellationToken);
        if (report is null) return NotFound();
        if (!await IsReportStatusAsync(report.TrangThaiId, "NHAP", cancellationToken) && !await IsReportStatusAsync(report.TrangThaiId, "MO_LAI", cancellationToken)) return BadRequest("Chỉ được chốt báo cáo NHAP hoặc MO_LAI.");
        if (!await IsReportStatusAsync(request.TrangThaiBaoCaoId, "DA_CHOT", cancellationToken) || !await IsPlanStatusAsync(request.TrangThaiKeHoachId, "DA_TONG_HOP", cancellationToken)) return BadRequest("Trạng thái chốt không hợp lệ.");
        var plan = await GetScopedPlanAsync(report.KeHoachId, cancellationToken);
        if (plan is null) return NotFound();
        var contents = await dbContext.NoiDungKeHoachs.Where(x => x.KeHoachId == plan.Id && !x.IsDeleted).OrderBy(x => x.ThuTu).ToListAsync(cancellationToken);
        if (contents.Count == 0) return BadRequest("Kế hoạch chưa có đầu việc để tổng hợp.");
        foreach (var content in contents)
        {
            if (!await IsContentFinishedAsync(content.TrangThaiId, cancellationToken)) return BadRequest("Chỉ được chốt khi tất cả đầu việc đã được đánh giá DAT hoặc KHONG_DAT.");
        }
        var lanChot = await dbContext.BaoCaoTongHopChiTiets.Where(x => x.BaoCaoTongHopThiHanhId == id).Select(x => (int?)x.LanChot).MaxAsync(cancellationToken) ?? 0;
        lanChot++;
        var details = new List<BaoCaoTongHopChiTiet>();
        foreach (var content in contents)
        {
            var latestReport = await dbContext.BaoCaoTienDoThiHanhs.AsNoTracking().Where(x => x.NoiDungKeHoachId == content.Id && !x.IsDeleted).OrderByDescending(x => x.NgayBaoCao).FirstOrDefaultAsync(cancellationToken);
            details.Add(new BaoCaoTongHopChiTiet { BaoCaoTongHopThiHanhId = report.Id, NoiDungKeHoachId = content.Id, LanChot = lanChot, TrangThaiId = content.TrangThaiId, TyLeHoanThanh = content.TyLeHoanThanh, KetQua = latestReport?.KetQua, KhoKhan = latestReport?.KhoKhan, KienNghi = latestReport?.KienNghi, CreatedBy = CurrentUser.UserId!.Value.ToString() });
        }
        var data = new { LanChot = lanChot, TongSoNoiDung = details.Count, Dat = await CountContentStatusAsync(contents, "DAT", cancellationToken), KhongDat = await CountContentStatusAsync(contents, "KHONG_DAT", cancellationToken), TyLeHoanThanhBinhQuan = details.Average(x => x.TyLeHoanThanh) };
        report.TrangThaiId = request.TrangThaiBaoCaoId; report.NgayChot = DateTime.UtcNow; report.NguoiChotId = CurrentUser.UserId; report.SoLieuTongHopJson = JsonSerializer.Serialize(data); report.UpdatedAt = DateTime.UtcNow; report.UpdatedBy = CurrentUser.UserId!.Value.ToString();
        plan.TrangThaiId = request.TrangThaiKeHoachId; plan.UpdatedAt = DateTime.UtcNow; plan.UpdatedBy = CurrentUser.UserId!.Value.ToString();
        dbContext.BaoCaoTongHopChiTiets.AddRange(details);
        AddHistory(plan.Id, "CHOT_BAO_CAO_TONG_HOP", request.GhiChu, plan.TrangThaiId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Ok(new { report.Id, LanChot = lanChot, report.SoLieuTongHopJson });
    }

    [HttpPost("{id:guid}/mo-lai")]
    public async Task<ActionResult> Reopen(Guid id, MoLaiBaoCaoTongHopRequest request, CancellationToken cancellationToken)
    {
        var denied = await EnsurePermissionAsync("ThiHanhPhapLuatTongHop", "Approve", "Approve", cancellationToken);
        if (denied is not null) return denied;
        var report = await GetScopedReportAsync(id, tracking: true, cancellationToken);
        if (report is null) return NotFound();
        if (!await IsReportStatusAsync(report.TrangThaiId, "DA_CHOT", cancellationToken) || !await IsReportStatusAsync(request.TrangThaiBaoCaoId, "MO_LAI", cancellationToken) || !await IsPlanStatusAsync(request.TrangThaiKeHoachId, "DANG_THUC_HIEN", cancellationToken)) return BadRequest("Chuyển trạng thái mở lại không hợp lệ.");
        var plan = await GetScopedPlanAsync(report.KeHoachId, cancellationToken);
        if (plan is null) return NotFound();
        report.TrangThaiId = request.TrangThaiBaoCaoId; report.UpdatedAt = DateTime.UtcNow; report.UpdatedBy = CurrentUser.UserId!.Value.ToString();
        plan.TrangThaiId = request.TrangThaiKeHoachId; plan.UpdatedAt = DateTime.UtcNow; plan.UpdatedBy = CurrentUser.UserId!.Value.ToString();
        AddHistory(plan.Id, "MO_LAI_BAO_CAO_TONG_HOP", request.LyDo, plan.TrangThaiId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:guid}/hoan-thanh")]
    public async Task<ActionResult> CompletePlan(Guid id, HoanThanhKeHoachRequest request, CancellationToken cancellationToken)
    {
        var denied = await EnsurePermissionAsync("ThiHanhPhapLuatTongHop", "Approve", "Approve", cancellationToken);
        if (denied is not null) return denied;
        var plan = await GetScopedPlanAsync(id, cancellationToken);
        if (plan is null) return NotFound();
        if (!await IsPlanStatusAsync(plan.TrangThaiId, "DA_TONG_HOP", cancellationToken) || !await IsPlanStatusAsync(request.TrangThaiKeHoachId, "HOAN_THANH", cancellationToken)) return BadRequest("Chuyển trạng thái hoàn thành không hợp lệ.");
        if (!await dbContext.BaoCaoTongHopThiHanhs.AnyAsync(x => x.KeHoachId == id && !x.IsDeleted && x.NgayChot != null, cancellationToken)) return BadRequest("Kế hoạch chưa có báo cáo tổng hợp đã chốt.");
        plan.TrangThaiId = request.TrangThaiKeHoachId; plan.UpdatedAt = DateTime.UtcNow; plan.UpdatedBy = CurrentUser.UserId!.Value.ToString(); AddHistory(plan.Id, "HOAN_THANH_KE_HOACH", request.GhiChu, plan.TrangThaiId);
        await dbContext.SaveChangesAsync(cancellationToken); return NoContent();
    }

    private async Task<KeHoachThiHanhPhapLuat?> GetScopedPlanAsync(Guid id, CancellationToken cancellationToken)
    {
        var query = dbContext.KeHoachThiHanhPhapLuats.Where(x => x.Id == id && !x.IsDeleted);
        if (!CurrentUser.IsSSA)
        {
            if (CurrentUser.DonViId is not { } donViId) return null;
            query = query.Where(x => x.DonViChuTriId == donViId);
        }
        return await query.SingleOrDefaultAsync(cancellationToken);
    }

    private async Task<BaoCaoTongHopThiHanh?> GetScopedReportAsync(Guid id, bool tracking, CancellationToken cancellationToken)
    {
        var reports = tracking ? dbContext.BaoCaoTongHopThiHanhs : dbContext.BaoCaoTongHopThiHanhs.AsNoTracking();
        var report = await reports.SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        return report is not null && await GetScopedPlanAsync(report.KeHoachId, cancellationToken) is not null ? report : null;
    }
    private async Task<int> CountContentStatusAsync(IEnumerable<NoiDungKeHoach> contents, string code, CancellationToken cancellationToken) { var count = 0; foreach (var content in contents) if (await IsContentStatusAsync(content.TrangThaiId, code, cancellationToken)) count++; return count; }
    private async Task<bool> IsContentFinishedAsync(Guid id, CancellationToken cancellationToken) => await IsContentStatusAsync(id, "DAT", cancellationToken) || await IsContentStatusAsync(id, "KHONG_DAT", cancellationToken);
    private async Task<bool> IsContentStatusAsync(Guid id, string code, CancellationToken cancellationToken) { var status = await trangThaiClient.GetAsync(id, cancellationToken); return status is { TrangThai: true, NhomTrangThai: "NOI_DUNG_THI_HANH_PHAP_LUAT" } && status.MaTrangThai == code; }
    private async Task<bool> IsPlanStatusAsync(Guid id, string code, CancellationToken cancellationToken) { var status = await trangThaiClient.GetAsync(id, cancellationToken); return status is { TrangThai: true, NhomTrangThai: "KE_HOACH_THI_HANH_PHAP_LUAT" } && status.MaTrangThai == code; }
    private async Task<bool> IsReportStatusAsync(Guid id, string code, CancellationToken cancellationToken) { var status = await trangThaiClient.GetAsync(id, cancellationToken); return status is { TrangThai: true, NhomTrangThai: "BAO_CAO_TONG_HOP_THI_HANH" } && status.MaTrangThai == code; }
    private void AddHistory(Guid planId, string action, string? content, Guid statusId) => dbContext.LichSuXuLyThiHanhs.Add(new LichSuXuLyThiHanh { KeHoachId = planId, HanhDong = action, TrangThaiSauId = statusId, NoiDung = content, NguoiXuLyId = CurrentUser.UserId!.Value, DonViXuLyId = CurrentUser.DonViId });
}

public sealed record CreateBaoCaoTongHopRequest(string MaBaoCao, Guid KeHoachId, string KyBaoCao, DateOnly TuNgay, DateOnly DenNgay, Guid TrangThaiId);
public sealed record ChotBaoCaoTongHopRequest(Guid TrangThaiBaoCaoId, Guid TrangThaiKeHoachId, string? GhiChu);
public sealed record MoLaiBaoCaoTongHopRequest(Guid TrangThaiBaoCaoId, Guid TrangThaiKeHoachId, string LyDo);
public sealed record HoanThanhKeHoachRequest(Guid TrangThaiKeHoachId, string? GhiChu);
