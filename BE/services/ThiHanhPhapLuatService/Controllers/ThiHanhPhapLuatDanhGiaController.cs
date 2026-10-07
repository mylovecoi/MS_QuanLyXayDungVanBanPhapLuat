using BuildingBlocks.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThiHanhPhapLuatService.Infrastructure.Authorization;
using ThiHanhPhapLuatService.Infrastructure.DanhMuc;
using ThiHanhPhapLuatService.Infrastructure.Persistence;
using ThiHanhPhapLuatService.Infrastructure.Persistence.Entities;

namespace ThiHanhPhapLuatService.Controllers;

[ApiController]
[Route("api/thi-hanh-phap-luat/danh-gia")]
public sealed class ThiHanhPhapLuatDanhGiaController(
    ICurrentUserContext user,
    IQuanTriHeThongPermissionClient permissionClient,
    IDanhMucTrangThaiClient trangThaiClient,
    ThiHanhPhapLuatDbContext dbContext) : ThiHanhPhapLuatControllerBase(user, permissionClient)
{
    [HttpGet]
    public async Task<ActionResult> GetList([FromQuery] Guid noiDungKeHoachId, CancellationToken cancellationToken)
    {
        var denied = await EnsurePermissionAsync("ThiHanhPhapLuatDanhGia", "Index", "Index", cancellationToken);
        if (denied is not null) return denied;
        return Ok(await dbContext.DanhGiaThiHanhs.AsNoTracking().Where(x => x.NoiDungKeHoachId == noiDungKeHoachId && !x.IsDeleted).OrderByDescending(x => x.NgayDanhGia).ToListAsync(cancellationToken));
    }

    [HttpPost("{baoCaoTienDoId:guid}/dat")]
    public Task<ActionResult> Dat(Guid baoCaoTienDoId, DanhGiaKetQuaRequest request, CancellationToken cancellationToken) => EvaluateAsync(baoCaoTienDoId, request, "DAT", cancellationToken);

    [HttpPost("{baoCaoTienDoId:guid}/khong-dat")]
    public Task<ActionResult> KhongDat(Guid baoCaoTienDoId, DanhGiaKetQuaRequest request, CancellationToken cancellationToken) => EvaluateAsync(baoCaoTienDoId, request, "KHONG_DAT", cancellationToken);

    [HttpPost("{baoCaoTienDoId:guid}/yeu-cau-bo-sung")]
    public async Task<ActionResult> RequestSupplement(Guid baoCaoTienDoId, YeuCauBoSungRequest request, CancellationToken cancellationToken)
    {
        var denied = await EnsurePermissionAsync("ThiHanhPhapLuatDanhGia", "Approve", "Approve", cancellationToken);
        if (denied is not null) return denied;
        var report = await GetSubmittedReportAsync(baoCaoTienDoId, cancellationToken);
        if (report is null) return BadRequest("Báo cáo không tồn tại hoặc chưa ở trạng thái DA_GUI.");
        if (request.HanBoSung < DateOnly.FromDateTime(DateTime.UtcNow)) return BadRequest("Hạn bổ sung không hợp lệ.");
        if (!await IsReportStatusAsync(request.TrangThaiBaoCaoId, "CAN_BO_SUNG", cancellationToken) || !await IsContentStatusAsync(request.TrangThaiNoiDungId, "YEU_CAU_BO_SUNG", cancellationToken)) return BadRequest("Trạng thái yêu cầu bổ sung không hợp lệ.");
        var content = await dbContext.NoiDungKeHoachs.SingleAsync(x => x.Id == report.NoiDungKeHoachId && !x.IsDeleted, cancellationToken);
        var oldContentStatus = content.TrangThaiId;
        report.TrangThaiId = request.TrangThaiBaoCaoId; report.UpdatedAt = DateTime.UtcNow; report.UpdatedBy = CurrentUser.UserId!.Value.ToString();
        content.TrangThaiId = request.TrangThaiNoiDungId; content.UpdatedAt = DateTime.UtcNow; content.UpdatedBy = CurrentUser.UserId!.Value.ToString();
        dbContext.YeuCauBoSungThiHanhs.Add(new YeuCauBoSungThiHanh { NoiDungKeHoachId = content.Id, BaoCaoTienDoThiHanhId = report.Id, NoiDungYeuCau = request.NoiDungYeuCau.Trim(), HanBoSung = request.HanBoSung, TrangThai = "DANG_MO", CreatedBy = CurrentUser.UserId!.Value.ToString() });
        AddEvaluation(content, report, "YEU_CAU_BO_SUNG", request.NhanXet, request.TrangThaiNoiDungId);
        AddHistory(content, oldContentStatus, content.TrangThaiId, "YEU_CAU_BO_SUNG", request.NhanXet);
        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private async Task<ActionResult> EvaluateAsync(Guid reportId, DanhGiaKetQuaRequest request, string result, CancellationToken cancellationToken)
    {
        var denied = await EnsurePermissionAsync("ThiHanhPhapLuatDanhGia", "Approve", "Approve", cancellationToken);
        if (denied is not null) return denied;
        var report = await GetSubmittedReportAsync(reportId, cancellationToken);
        if (report is null) return BadRequest("Báo cáo không tồn tại hoặc chưa ở trạng thái DA_GUI.");
        if (!await IsReportStatusAsync(request.TrangThaiBaoCaoId, "DA_XAC_NHAN", cancellationToken) || !await IsContentStatusAsync(request.TrangThaiNoiDungId, result, cancellationToken)) return BadRequest("Trạng thái đánh giá không hợp lệ.");
        var content = await dbContext.NoiDungKeHoachs.SingleAsync(x => x.Id == report.NoiDungKeHoachId && !x.IsDeleted, cancellationToken);
        var oldContentStatus = content.TrangThaiId;
        report.TrangThaiId = request.TrangThaiBaoCaoId; report.UpdatedAt = DateTime.UtcNow; report.UpdatedBy = CurrentUser.UserId!.Value.ToString();
        content.TrangThaiId = request.TrangThaiNoiDungId; content.UpdatedAt = DateTime.UtcNow; content.UpdatedBy = CurrentUser.UserId!.Value.ToString();
        AddEvaluation(content, report, result, request.NhanXet, request.TrangThaiNoiDungId);
        AddHistory(content, oldContentStatus, content.TrangThaiId, $"DANH_GIA_{result}", request.NhanXet);
        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private async Task<BaoCaoTienDoThiHanh?> GetSubmittedReportAsync(Guid id, CancellationToken cancellationToken)
    {
        var report = await dbContext.BaoCaoTienDoThiHanhs.SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        return report is not null && await IsReportStatusAsync(report.TrangThaiId, "DA_GUI", cancellationToken) ? report : null;
    }
    private void AddEvaluation(NoiDungKeHoach content, BaoCaoTienDoThiHanh report, string result, string? comment, Guid statusId) => dbContext.DanhGiaThiHanhs.Add(new DanhGiaThiHanh { NoiDungKeHoachId = content.Id, BaoCaoTienDoThiHanhId = report.Id, KetQuaDanhGia = result, NhanXet = comment, TrangThaiSauId = statusId, NguoiDanhGiaId = CurrentUser.UserId!.Value, NgayDanhGia = DateTime.UtcNow, CreatedBy = CurrentUser.UserId!.Value.ToString() });
    private void AddHistory(NoiDungKeHoach content, Guid oldStatus, Guid newStatus, string action, string? comment) => dbContext.LichSuXuLyThiHanhs.Add(new LichSuXuLyThiHanh { KeHoachId = content.KeHoachId, NoiDungKeHoachId = content.Id, HanhDong = action, TrangThaiTruocId = oldStatus, TrangThaiSauId = newStatus, NoiDung = comment, NguoiXuLyId = CurrentUser.UserId!.Value, DonViXuLyId = CurrentUser.DonViId });
    private async Task<bool> IsReportStatusAsync(Guid id, string code, CancellationToken cancellationToken) { var status = await trangThaiClient.GetAsync(id, cancellationToken); return status is { TrangThai: true, NhomTrangThai: "BAO_CAO_TIEN_DO_THI_HANH" } && status.MaTrangThai == code; }
    private async Task<bool> IsContentStatusAsync(Guid id, string code, CancellationToken cancellationToken) { var status = await trangThaiClient.GetAsync(id, cancellationToken); return status is { TrangThai: true, NhomTrangThai: "NOI_DUNG_THI_HANH_PHAP_LUAT" } && status.MaTrangThai == code; }
}

public sealed record DanhGiaKetQuaRequest(Guid TrangThaiBaoCaoId, Guid TrangThaiNoiDungId, string? NhanXet);
public sealed record YeuCauBoSungRequest(Guid TrangThaiBaoCaoId, Guid TrangThaiNoiDungId, string NoiDungYeuCau, DateOnly HanBoSung, string? NhanXet);
