using BuildingBlocks.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThiHanhPhapLuatService.Infrastructure.Authorization;
using ThiHanhPhapLuatService.Infrastructure.Persistence;

namespace ThiHanhPhapLuatService.Controllers;

/// <summary>
/// Danh sách đầu việc mà đơn vị/cán bộ đang đăng nhập được phân công thực hiện.
/// Tài khoản SSA được xem toàn bộ để phục vụ quản trị và kiểm thử.
/// </summary>
[ApiController]
[Route("api/thi-hanh-phap-luat/viec-duoc-giao")]
public sealed class ThiHanhPhapLuatViecDuocGiaoController(
    ICurrentUserContext user,
    IQuanTriHeThongPermissionClient permissionClient,
    ThiHanhPhapLuatDbContext dbContext) : ThiHanhPhapLuatControllerBase(user, permissionClient)
{
    [HttpGet]
    public async Task<ActionResult> GetList(
        [FromQuery] int? nam,
        [FromQuery] Guid? keHoachId,
        [FromQuery] bool? chiQuaHan,
        CancellationToken cancellationToken)
    {
        var denied = await EnsurePermissionAsync("ThiHanhPhapLuatViecDuocGiao", "Index", "Index", cancellationToken);
        if (denied is not null) return denied;

        if (!CurrentUser.IsSSA && (CurrentUser.DonViId is null || CurrentUser.UserId is null))
            return Forbid();

        var query =
            from assignment in dbContext.PhanCongThiHanhs.AsNoTracking()
            join content in dbContext.NoiDungKeHoachs.AsNoTracking() on assignment.NoiDungKeHoachId equals content.Id
            join plan in dbContext.KeHoachThiHanhPhapLuats.AsNoTracking() on content.KeHoachId equals plan.Id
            where !assignment.IsDeleted && !content.IsDeleted && !plan.IsDeleted
            select new { assignment, content, plan };

        if (!CurrentUser.IsSSA)
        {
            var donViId = CurrentUser.DonViId!.Value;
            var userId = CurrentUser.UserId!.Value;
            query = query.Where(x => x.assignment.DonViDuocGiaoId == donViId
                && (x.assignment.CanBoDuocGiaoId == null || x.assignment.CanBoDuocGiaoId == userId));
        }

        if (nam.HasValue) query = query.Where(x => x.plan.Nam == nam.Value);
        if (keHoachId.HasValue) query = query.Where(x => x.plan.Id == keHoachId.Value);
        if (chiQuaHan == true)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            query = query.Where(x => x.assignment.HanThucHien < today && x.content.TyLeHoanThanh < 100);
        }

        var items = await query
            .OrderBy(x => x.assignment.HanThucHien)
            .ThenByDescending(x => x.assignment.MucDoUuTien)
            .Select(x => new
            {
                PhanCongId = x.assignment.Id,
                x.assignment.DonViDuocGiaoId,
                x.assignment.CanBoDuocGiaoId,
                x.assignment.VaiTro,
                x.assignment.HanThucHien,
                x.assignment.MucDoUuTien,
                NoiDungKeHoachId = x.content.Id,
                x.content.MaNoiDung,
                TenNoiDung = x.content.TenNoiDung,
                x.content.NoiDung,
                x.content.ChiTieu,
                x.content.DonViTinh,
                HanHoanThanhNoiDung = x.content.HanHoanThanh,
                TrangThaiNoiDungId = x.content.TrangThaiId,
                x.content.TyLeHoanThanh,
                KeHoachId = x.plan.Id,
                x.plan.MaKeHoach,
                TenKeHoach = x.plan.TenKeHoach,
                x.plan.Nam,
                DonViChuTriId = x.plan.DonViChuTriId,
                BaoCaoGanNhat = dbContext.BaoCaoTienDoThiHanhs
                    .Where(report => !report.IsDeleted && report.PhanCongThiHanhId == x.assignment.Id)
                    .OrderByDescending(report => report.NgayBaoCao)
                    .Select(report => new
                    {
                        report.Id,
                        report.KyBaoCao,
                        report.TyLeHoanThanh,
                        report.TrangThaiId,
                        report.NgayBaoCao
                    })
                    .FirstOrDefault()
            })
            .ToListAsync(cancellationToken);

        return Ok(items);
    }
}
