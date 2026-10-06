using BuildingBlocks.Abstractions;
using Microsoft.EntityFrameworkCore;
using XayDungVanBanService.Application.Abstractions;
using XayDungVanBanService.Application.DTOs;
using XayDungVanBanService.Infrastructure.DanhMuc;
using XayDungVanBanService.Infrastructure.Persistence;
using XayDungVanBanService.Infrastructure.Persistence.Entities;

namespace XayDungVanBanService.Application.Services;

public sealed class XayDungVanBanChamDiemService(
    XayDungVanBanDbContext db,
    ICurrentUserContext currentUser,
    IDanhMucTrangThaiClient trangThaiClient,
    IDanhMucTieuChiDiemClient tieuChiClient) : IXayDungVanBanChamDiemService
{
    private const string NhomTrangThai = "CHAM_DIEM_HO_SO";

    public async Task<PagedResultDto<ChamDiemListItemDto>> GetListAsync(ChamDiemListRequest request, CancellationToken ct = default)
    {
        var pageSize = Math.Clamp(request.PageSize, 1, 200);
        var pageCurrent = Math.Max(request.PageCurrent, 1);
        var query = from score in db.HoSoXayDungVanBanChamDiems.AsNoTracking()
                    join dossier in db.HoSoXayDungVanBans.AsNoTracking() on score.HoSoXayDungVanBanId equals dossier.Id
                    where !score.IsDeleted && !dossier.IsDeleted
                    select new { score, dossier };
        if (request.HoSoId.HasValue) query = query.Where(x => x.score.HoSoXayDungVanBanId == request.HoSoId.Value);
        if (request.TrangThaiId.HasValue) query = query.Where(x => x.score.TrangThaiId == request.TrangThaiId.Value);
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            query = query.Where(x => x.dossier.MaHoSo.Contains(search) || x.dossier.TenHoSo.Contains(search) || x.dossier.TenDuThaoVanBan.Contains(search));
        }
        var total = await query.CountAsync(ct);
        var items = await query.OrderByDescending(x => x.score.NgayCham).Skip((pageCurrent - 1) * pageSize).Take(pageSize)
            .Select(x => new ChamDiemListItemDto(x.score.Id, x.dossier.Id, x.dossier.MaHoSo, x.dossier.TenHoSo, x.dossier.TenDuThaoVanBan, x.score.LanCham, x.score.TrangThaiId, x.score.TongDiemChinhThuc, x.score.NgayCham, x.score.NgayChot)).ToListAsync(ct);
        return new PagedResultDto<ChamDiemListItemDto>(items, total, pageSize, pageCurrent);
    }

    public async Task<IReadOnlyList<ChamDiemDto>> GetByHoSoAsync(Guid hoSoId, CancellationToken ct = default) =>
        await db.HoSoXayDungVanBanChamDiems.AsNoTracking().Where(x => x.HoSoXayDungVanBanId == hoSoId && !x.IsDeleted)
            .OrderByDescending(x => x.LanCham).Select(MapQuery()).ToListAsync(ct);

    public async Task<ChamDiemDto?> GetAsync(Guid id, CancellationToken ct = default) =>
        await db.HoSoXayDungVanBanChamDiems.AsNoTracking().Where(x => x.Id == id && !x.IsDeleted).Select(MapQuery()).FirstOrDefaultAsync(ct);

    public async Task<ChamDiemDto> CreateAsync(Guid hoSoId, TaoChamDiemRequest request, CancellationToken ct = default)
    {
        var actor = Actor();
        await EnsureStatusAsync(request.TrangThaiNhapId, "NHAP", ct);
        var hoSo = await db.HoSoXayDungVanBans.AsNoTracking().FirstOrDefaultAsync(x => x.Id == hoSoId && !x.IsDeleted, ct)
            ?? throw new InvalidOperationException("Không tìm thấy hồ sơ xây dựng văn bản.");
        var lanCham = (await db.HoSoXayDungVanBanChamDiems.Where(x => x.HoSoXayDungVanBanId == hoSoId).Select(x => (int?)x.LanCham).MaxAsync(ct) ?? 0) + 1;
        var item = new HoSoXayDungVanBanChamDiem { HoSoXayDungVanBanId = hoSoId, LanCham = lanCham, TrangThaiId = request.TrangThaiNhapId, NguoiChamId = actor, GhiChu = request.GhiChu?.Trim(), CreatedBy = actor.ToString() };
        db.HoSoXayDungVanBanChamDiems.Add(item);
        await BuildAutoScoreAsync(item, hoSo, actor, ct);
        AddHistory(item.Id, null, "TAO", "Tạo bảng điểm và tính điểm tự động.", actor);
        await db.SaveChangesAsync(ct);
        return (await GetAsync(item.Id, ct))!;
    }

    public async Task<ChamDiemDto?> TinhLaiAsync(Guid id, CancellationToken ct = default)
    {
        var actor = Actor();
        var item = await db.HoSoXayDungVanBanChamDiems.Include(x => x.ChiTiets).FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
        if (item is null) return null;
        await EnsureCurrentStatusAsync(item.TrangThaiId, "NHAP", ct);
        var hoSo = await db.HoSoXayDungVanBans.AsNoTracking().FirstAsync(x => x.Id == item.HoSoXayDungVanBanId, ct);
        db.HoSoXayDungVanBanChamDiemChiTiets.RemoveRange(item.ChiTiets);
        await BuildAutoScoreAsync(item, hoSo, actor, ct);
        AddHistory(id, null, "TINH_LAI", "Tính lại điểm tự động theo danh mục hiện hành.", actor);
        await db.SaveChangesAsync(ct);
        return await GetAsync(id, ct);
    }

    public async Task<ChamDiemDto?> DieuChinhAsync(Guid id, Guid chiTietId, DieuChinhChamDiemRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.LyDoDieuChinh)) throw new InvalidOperationException("Phải nhập lý do điều chỉnh điểm.");
        var actor = Actor();
        var item = await db.HoSoXayDungVanBanChamDiems.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
        if (item is null) return null;
        await EnsureCurrentStatusAsync(item.TrangThaiId, "NHAP", ct);
        var detail = await db.HoSoXayDungVanBanChamDiemChiTiets.FirstOrDefaultAsync(x => x.Id == chiTietId && x.HoSoXayDungVanBanChamDiemId == id && !x.IsDeleted, ct);
        if (detail is null) return null;
        var old = detail.DiemChinhThuc;
        detail.DiemDieuChinh = request.DiemDieuChinh;
        detail.DiemChinhThuc = Math.Clamp(detail.DiemTuDong + request.DiemDieuChinh, 0m, detail.DiemToiDa);
        detail.LyDoDieuChinh = request.LyDoDieuChinh.Trim(); detail.NguoiDieuChinhId = actor; detail.NgayDieuChinh = DateTime.UtcNow; detail.UpdatedBy = actor.ToString();
        await RefreshTotalsAsync(item, ct);
        AddHistory(id, detail.Id, "DIEU_CHINH", "Điều chỉnh điểm tiêu chí.", actor, old.ToString(), detail.DiemChinhThuc.ToString());
        await db.SaveChangesAsync(ct);
        return await GetAsync(id, ct);
    }

    public async Task<ChamDiemDto?> ChuyenTrangThaiAsync(Guid id, ChuyenTrangThaiChamDiemRequest request, string expectedCode, CancellationToken ct = default)
    {
        var actor = Actor(); var item = await db.HoSoXayDungVanBanChamDiems.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
        if (item is null) return null;
        await EnsureStatusAsync(request.TrangThaiId, expectedCode, ct);
        if (expectedCode == "DA_CHOT") await EnsureCurrentStatusAsync(item.TrangThaiId, "NHAP", ct);
        if (expectedCode == "DA_HUY") await EnsureCurrentStatusAsync(item.TrangThaiId, "DA_CHOT", ct);
        item.TrangThaiId = request.TrangThaiId; item.GhiChu = request.GhiChu?.Trim() ?? item.GhiChu; item.UpdatedBy = actor.ToString();
        if (expectedCode == "DA_CHOT") { item.NgayChot = DateTime.UtcNow; item.NguoiChotId = actor; }
        AddHistory(id, null, expectedCode == "DA_CHOT" ? "CHOT" : "HUY", expectedCode == "DA_CHOT" ? "Chốt kết quả chấm điểm." : "Hủy bảng điểm đã chốt.", actor);
        await db.SaveChangesAsync(ct); return await GetAsync(id, ct);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var item = await db.HoSoXayDungVanBanChamDiems.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct); if (item is null) return false;
        await EnsureCurrentStatusAsync(item.TrangThaiId, "NHAP", ct); item.IsDeleted = true; item.UpdatedBy = Actor().ToString(); await db.SaveChangesAsync(ct); return true;
    }

    public async Task<IReadOnlyList<ChamDiemLichSuDto>?> GetLichSuAsync(Guid id, CancellationToken ct = default)
    {
        if (!await db.HoSoXayDungVanBanChamDiems.AnyAsync(x => x.Id == id && !x.IsDeleted, ct)) return null;
        return await db.HoSoXayDungVanBanChamDiemLichSus.AsNoTracking().Where(x => x.HoSoXayDungVanBanChamDiemId == id && !x.IsDeleted).OrderBy(x => x.ThoiGianThucHien)
            .Select(x => new ChamDiemLichSuDto(x.Id, x.HoSoXayDungVanBanChamDiemChiTietId, x.LoaiThaoTac, x.NoiDung, x.ThoiGianThucHien, x.NguoiThucHienId)).ToListAsync(ct);
    }

    private async Task BuildAutoScoreAsync(HoSoXayDungVanBanChamDiem item, HoSoXayDungVanBan hoSo, Guid actor, CancellationToken ct)
    {
        var criteria = await tieuChiClient.GetActiveAsync(ct); if (criteria.Count == 0) throw new InvalidOperationException("Không lấy được danh mục tiêu chí chấm điểm.");
        var firstDraft = await db.BoHoSoNghiepVus.Where(x => x.HoSoXayDungVanBanId == hoSo.Id && x.LoaiBoHoSo == LoaiBoHoSo.SoanThao && !x.IsDeleted).Select(x => (DateTime?)x.CreatedAt).MinAsync(ct) ?? hoSo.CreatedAt;
        var end = DateTime.UtcNow; var planned = hoSo.ThoiGianDuKienBatDau.HasValue && hoSo.ThoiGianDuKienHoanThanh.HasValue ? Math.Max(1, (int)Math.Ceiling((hoSo.ThoiGianDuKienHoanThanh.Value - hoSo.ThoiGianDuKienBatDau.Value).TotalDays)) : (int?)null;
        var actual = Math.Max(0, (int)Math.Ceiling((end - firstDraft).TotalDays)); var ratio = planned.HasValue ? Math.Round(actual * 100m / planned.Value, 2) : (decimal?)null;
        item.NgayBatDauThucTe = firstDraft; item.NgayHoanThanhThucTe = end; item.SoNgayKeHoach = planned; item.SoNgayThucTe = actual; item.TyLeThoiGianThucTe = ratio;
        foreach (var criterion in criteria.Where(x => x.MaTieuChi is "CHAT_LUONG_SOAN_THAO" or "TIEN_DO_SOAN_THAO"))
        {
            var value = criterion.MaTieuChi == "CHAT_LUONG_SOAN_THAO"
                ? await db.BoHoSoNghiepVus.CountAsync(x => x.HoSoXayDungVanBanId == hoSo.Id && x.LoaiBoHoSo == LoaiBoHoSo.TrinhThamDinh && x.TrangThai == TrangThaiBoHoSo.DaGui && !x.IsDeleted, ct)
                : ratio ?? throw new InvalidOperationException("Hồ sơ chưa đủ thời gian kế hoạch để chấm tiêu chí tiến độ.");
            var band = criterion.Mucs.FirstOrDefault(x => x.TrangThai && InRange(value, x));
            if (band is null) throw new InvalidOperationException($"Không tìm thấy mức điểm cho tiêu chí {criterion.TenTieuChi}.");
            item.ChiTiets.Add(new HoSoXayDungVanBanChamDiemChiTiet { DanhMucTieuChiDiemId = criterion.Id, DanhMucTieuChiDiemMucId = band.Id, MaTieuChi = criterion.MaTieuChi, TenTieuChi = criterion.TenTieuChi, NhanMucDiem = band.NhanHienThi, GiaTriDauVao = value, DiemToiDa = criterion.DiemToiDa, DiemTuDong = band.Diem, DiemChinhThuc = band.Diem, CreatedBy = actor.ToString() });
        }
        await RefreshTotalsAsync(item, ct);
    }

    private static bool InRange(decimal value, DanhMucTieuChiDiemMucItem band) => (!band.TuGiaTri.HasValue || (band.BaoGomTuGiaTri ? value >= band.TuGiaTri : value > band.TuGiaTri)) && (!band.DenGiaTri.HasValue || (band.BaoGomDenGiaTri ? value <= band.DenGiaTri : value < band.DenGiaTri));
    private async Task RefreshTotalsAsync(HoSoXayDungVanBanChamDiem item, CancellationToken ct) { var details = item.ChiTiets.Count > 0 ? item.ChiTiets : await db.HoSoXayDungVanBanChamDiemChiTiets.Where(x => x.HoSoXayDungVanBanChamDiemId == item.Id && !x.IsDeleted).ToListAsync(ct); item.TongDiemTuDong = details.Sum(x => x.DiemTuDong); item.TongDiemChinhThuc = details.Sum(x => x.DiemChinhThuc); item.TongDiemDieuChinh = item.TongDiemChinhThuc - item.TongDiemTuDong; }
    private void AddHistory(Guid id, Guid? detailId, string action, string content, Guid actor, string? oldValue = null, string? newValue = null) => db.HoSoXayDungVanBanChamDiemLichSus.Add(new() { HoSoXayDungVanBanChamDiemId = id, HoSoXayDungVanBanChamDiemChiTietId = detailId, LoaiThaoTac = action, NoiDung = content, DuLieuCu = oldValue, DuLieuMoi = newValue, NguoiThucHienId = actor, CreatedBy = actor.ToString() });
    private async Task EnsureStatusAsync(Guid id, string code, CancellationToken ct) { var status = await trangThaiClient.GetAsync(id, ct); if (status is null || !status.TrangThai || status.NhomTrangThai != NhomTrangThai || status.MaTrangThai != code) throw new InvalidOperationException("Trạng thái chấm điểm không hợp lệ."); }
    private async Task EnsureCurrentStatusAsync(Guid id, string code, CancellationToken ct) => await EnsureStatusAsync(id, code, ct);
    private Guid Actor() => currentUser.IsAuthenticated && currentUser.UserId is Guid id && id != Guid.Empty ? id : throw new UnauthorizedAccessException();
    private static System.Linq.Expressions.Expression<Func<HoSoXayDungVanBanChamDiem, ChamDiemDto>> MapQuery() => x => new ChamDiemDto(x.Id, x.HoSoXayDungVanBanId, x.LanCham, x.TrangThaiId, x.TongDiemTuDong, x.TongDiemDieuChinh, x.TongDiemChinhThuc, x.NgayCham, x.NgayChot, x.ChiTiets.Where(d => !d.IsDeleted).OrderBy(d => d.MaTieuChi).Select(d => new ChamDiemChiTietDto(d.Id, d.DanhMucTieuChiDiemId, d.MaTieuChi, d.TenTieuChi, d.NhanMucDiem, d.GiaTriDauVao, d.DiemTuDong, d.DiemDieuChinh, d.DiemChinhThuc, d.DiemToiDa, d.LyDoDieuChinh)).ToList());
}
