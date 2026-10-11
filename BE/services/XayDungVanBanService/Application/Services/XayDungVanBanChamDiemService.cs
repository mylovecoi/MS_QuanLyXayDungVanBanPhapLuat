using BuildingBlocks.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;
using System.Text;
using System.Xml;
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

    public async Task<PagedResultDto<HoSoCanChamDiemListItemDto>> GetHoSoCanChamAsync(ChamDiemListRequest request, CancellationToken ct = default)
    {
        var pageSize = Math.Clamp(request.PageSize, 1, 200);
        var pageCurrent = Math.Max(request.PageCurrent, 1);
        var query = db.HoSoXayDungVanBans.AsNoTracking().Where(x => !x.IsDeleted);
        if (request.HoSoId.HasValue) query = query.Where(x => x.Id == request.HoSoId.Value);
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            query = query.Where(x => x.MaHoSo.Contains(search) || x.TenHoSo.Contains(search) || x.TenDuThaoVanBan.Contains(search));
        }

        var total = await query.CountAsync(ct);
        var dossiers = await query.OrderByDescending(x => x.CreatedAt).Skip((pageCurrent - 1) * pageSize).Take(pageSize)
            .Select(x => new { x.Id, x.MaHoSo, x.TenHoSo, x.TenDuThaoVanBan, x.NamXayDung }).ToListAsync(ct);
        var ids = dossiers.Select(x => x.Id).ToList();
        var scores = await db.HoSoXayDungVanBanChamDiems.AsNoTracking()
            .Where(x => ids.Contains(x.HoSoXayDungVanBanId) && !x.IsDeleted)
            .OrderByDescending(x => x.LanCham)
            .Select(x => new { x.Id, x.HoSoXayDungVanBanId, x.LanCham, x.TrangThaiId, x.TongDiemChinhThuc, x.NgayCham, x.NgayChot })
            .ToListAsync(ct);
        var latest = scores.GroupBy(x => x.HoSoXayDungVanBanId).ToDictionary(x => x.Key, x => x.First());
        if (request.TrangThaiId.HasValue)
        {
            dossiers = dossiers.Where(x => latest.TryGetValue(x.Id, out var score) && score.TrangThaiId == request.TrangThaiId.Value).ToList();
        }
        var completed = await db.BoHoSoNghiepVus.AsNoTracking()
            .Where(x => ids.Contains(x.HoSoXayDungVanBanId) && x.LoaiBoHoSo == LoaiBoHoSo.BanHanh && x.TrangThai == TrangThaiBoHoSo.DaHoanThanh && !x.IsDeleted)
            .Select(x => x.HoSoXayDungVanBanId)
            .Distinct()
            .ToListAsync(ct);
        var completedSet = completed.ToHashSet();

        var items = dossiers.Select(x =>
        {
            latest.TryGetValue(x.Id, out var score);
            return new HoSoCanChamDiemListItemDto(
                x.Id,
                x.MaHoSo,
                x.TenHoSo,
                x.TenDuThaoVanBan,
                x.NamXayDung,
                completedSet.Contains(x.Id),
                score?.Id,
                score?.LanCham,
                score?.TrangThaiId,
                score?.TongDiemChinhThuc,
                score?.NgayCham,
                score?.NgayChot);
        }).ToList();

        return new PagedResultDto<HoSoCanChamDiemListItemDto>(items, total, pageSize, pageCurrent);
    }

    public async Task<IReadOnlyList<ChamDiemDto>> GetByHoSoAsync(Guid hoSoId, CancellationToken ct = default)
    {
        var items = await db.HoSoXayDungVanBanChamDiems.AsNoTracking().Include(x => x.ChiTiets)
            .Where(x => x.HoSoXayDungVanBanId == hoSoId && !x.IsDeleted)
            .OrderByDescending(x => x.LanCham).ToListAsync(ct);
        return items.Select(ToDto).ToList();
    }

    public async Task<ChamDiemDto?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var item = await db.HoSoXayDungVanBanChamDiems.AsNoTracking().Include(x => x.ChiTiets).FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
        return item is null ? null : ToDto(item);
    }

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
        if (expectedCode == "NHAP") await EnsureCurrentStatusAsync(item.TrangThaiId, "DA_CHOT", ct);
        item.TrangThaiId = request.TrangThaiId; item.GhiChu = request.GhiChu?.Trim() ?? item.GhiChu; item.UpdatedBy = actor.ToString();
        if (expectedCode == "DA_CHOT") { item.NgayChot = DateTime.UtcNow; item.NguoiChotId = actor; }
        else { item.NgayChot = null; item.NguoiChotId = null; }
        AddHistory(id, null, expectedCode == "DA_CHOT" ? "CHOT" : "BO_CHOT", expectedCode == "DA_CHOT" ? "Chốt kết quả chấm điểm." : "Bỏ chốt kết quả chấm điểm để tiếp tục điều chỉnh.", actor);
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

    public async Task<ChamDiemExportFileDto> ExportKetQuaAsync(ChamDiemListRequest request, CancellationToken ct = default)
    {
        var query = from score in db.HoSoXayDungVanBanChamDiems.AsNoTracking().Include(x => x.ChiTiets)
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

        var items = await query.OrderBy(x => x.dossier.MaHoSo).ThenBy(x => x.score.LanCham).ToListAsync(ct);
        var rows = new List<IReadOnlyList<string?>>
        {
            new[]
            {
                "Mã hồ sơ",
                "Tên hồ sơ",
                "Dự thảo văn bản",
                "Lần chấm",
                "Tổng điểm",
                "Xếp loại",
                "Ngày chấm",
                "Ngày chốt",
                "Mã tiêu chí",
                "Tên tiêu chí",
                "Giá trị đầu vào",
                "Mức điểm",
                "Điểm tự động",
                "Điểm điều chỉnh",
                "Điểm chính thức",
                "Điểm tối đa",
                "Giải thích điểm",
                "Lý do điều chỉnh"
            }
        };

        foreach (var item in items)
        {
            var details = item.score.ChiTiets.Where(x => !x.IsDeleted).OrderBy(x => x.MaTieuChi).ToList();
            if (details.Count == 0)
            {
                rows.Add(BuildExportRow(item.dossier, item.score, null));
                continue;
            }

            foreach (var detail in details) rows.Add(BuildExportRow(item.dossier, item.score, detail));
        }

        var content = BuildXlsx(rows);
        return new ChamDiemExportFileDto(
            $"ket-qua-cham-diem-{DateTime.Now:yyyyMMdd}.xlsx",
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            content);
    }

    private async Task BuildAutoScoreAsync(HoSoXayDungVanBanChamDiem item, HoSoXayDungVanBan hoSo, Guid actor, CancellationToken ct)
    {
        var criteria = await tieuChiClient.GetActiveAsync(ct); if (criteria.Count == 0) throw new InvalidOperationException("Không lấy được danh mục tiêu chí chấm điểm.");
        var firstDraft = await db.BoHoSoNghiepVus.Where(x => x.HoSoXayDungVanBanId == hoSo.Id && x.LoaiBoHoSo == LoaiBoHoSo.SoanThao && !x.IsDeleted).Select(x => (DateTime?)x.CreatedAt).MinAsync(ct) ?? hoSo.CreatedAt;
        var end = await NgayHoanThanhThucTeAsync(hoSo.Id, ct) ?? DateTime.UtcNow; var planned = hoSo.ThoiGianDuKienBatDau.HasValue && hoSo.ThoiGianDuKienHoanThanh.HasValue ? Math.Max(1, (int)Math.Ceiling((hoSo.ThoiGianDuKienHoanThanh.Value - hoSo.ThoiGianDuKienBatDau.Value).TotalDays)) : (int?)null;
        var actual = Math.Max(0, (int)Math.Ceiling((end - firstDraft).TotalDays)); var ratio = planned.HasValue ? Math.Round(actual * 100m / planned.Value, 2) : (decimal?)null;
        item.NgayBatDauThucTe = firstDraft; item.NgayHoanThanhThucTe = end; item.SoNgayKeHoach = planned; item.SoNgayThucTe = actual; item.TyLeThoiGianThucTe = ratio;
        foreach (var criterion in criteria.OrderBy(x => x.MaTieuChi))
        {
            var input = await GetInputValueAsync(criterion.MaTieuChi, hoSo, ratio, actual, planned, ct);
            var value = input.Value;
            var band = criterion.Mucs.FirstOrDefault(x => x.TrangThai && InRange(value, x));
            if (band is null) throw new InvalidOperationException($"Không tìm thấy mức điểm cho tiêu chí {criterion.TenTieuChi}.");
            item.ChiTiets.Add(new HoSoXayDungVanBanChamDiemChiTiet { DanhMucTieuChiDiemId = criterion.Id, DanhMucTieuChiDiemMucId = band.Id, MaTieuChi = criterion.MaTieuChi, TenTieuChi = criterion.TenTieuChi, NhanMucDiem = band.NhanHienThi, GiaTriDauVao = value, DiemToiDa = criterion.DiemToiDa, DiemTuDong = band.Diem, DiemChinhThuc = band.Diem, CreatedBy = actor.ToString() });
        }
        await RefreshTotalsAsync(item, ct);
    }

    private async Task<ScoringInput> GetInputValueAsync(string maTieuChi, HoSoXayDungVanBan hoSo, decimal? tyLeThoiGian, int soNgayThucTe, int? soNgayKeHoach, CancellationToken ct)
    {
        var code = maTieuChi.Trim().ToUpperInvariant();
        return code switch
        {
            "CHAT_LUONG_SOAN_THAO" => new(Math.Max(1, await db.BoHoSoNghiepVus.CountAsync(x => x.HoSoXayDungVanBanId == hoSo.Id && x.LoaiBoHoSo == LoaiBoHoSo.TrinhThamDinh && x.TrangThai == TrangThaiBoHoSo.DaGui && !x.IsDeleted, ct)), "Số lần gửi hồ sơ thẩm định."),
            "TIEN_DO_SOAN_THAO" or "TIEN_DO_THUC_HIEN" => new(tyLeThoiGian ?? throw new InvalidOperationException("Hồ sơ chưa đủ thời gian kế hoạch để chấm tiêu chí tiến độ."), $"Tỷ lệ thời gian thực tế/kế hoạch = {soNgayThucTe}/{soNgayKeHoach} ngày."),
            "DAY_DU_TAI_LIEU" => new(await TinhSoTaiLieuConThieuAsync(hoSo.Id, ct), "Số tài liệu bắt buộc còn thiếu trong các bộ hồ sơ nghiệp vụ."),
            "TUAN_THU_QUY_TRINH" => new(await TinhSoLoiQuyTrinhAsync(hoSo.Id, ct), "Số lần trả lại hoặc yêu cầu bổ sung trong timeline/quy trình."),
            "KET_QUA_XU_LY" => new(await TinhGiaTriKetQuaXuLyAsync(hoSo.Id, ct), "Giá trị quy đổi từ kết quả xử lý cuối cùng của hồ sơ."),
            _ => throw new InvalidOperationException($"Tiêu chí {maTieuChi} chưa có công thức chấm điểm tự động.")
        };
    }

    private async Task<decimal> TinhSoTaiLieuConThieuAsync(Guid hoSoId, CancellationToken ct)
    {
        var required = await (from bo in db.BoHoSoNghiepVus.AsNoTracking()
                              join link in db.BoHoSoNghiepVuTaiLieus.AsNoTracking() on bo.Id equals link.BoHoSoNghiepVuId
                              where bo.HoSoXayDungVanBanId == hoSoId && !bo.IsDeleted && !link.IsDeleted && link.BatBuoc
                              select new { link.Id, link.HoSoXayDungVanBanFileId }).ToListAsync(ct);
        if (required.Count == 0)
        {
            return await db.HoSoXayDungVanBanFiles.AnyAsync(x => x.HoSoXayDungVanBanId == hoSoId && !x.IsDeleted && x.IsCurrent, ct) ? 0m : 1m;
        }
        var fileIds = required.Select(x => x.HoSoXayDungVanBanFileId).ToList();
        var existing = await db.HoSoXayDungVanBanFiles.AsNoTracking().Where(x => fileIds.Contains(x.Id) && !x.IsDeleted && x.IsCurrent).Select(x => x.Id).ToListAsync(ct);
        return required.Count(x => !existing.Contains(x.HoSoXayDungVanBanFileId));
    }

    private async Task<decimal> TinhSoLoiQuyTrinhAsync(Guid hoSoId, CancellationToken ct)
    {
        var returned = await db.HoSoXayDungVanBanLichSuXuLys.CountAsync(x => x.HoSoXayDungVanBanId == hoSoId
            && !x.IsDeleted
            && (x.HanhDong.Contains("TRA_LAI") || x.HanhDong.Contains("YEU_CAU_BO_SUNG")), ct);
        var businessReturns = await db.BoHoSoNghiepVus.CountAsync(x => x.HoSoXayDungVanBanId == hoSoId
            && !x.IsDeleted
            && x.TrangThai == TrangThaiBoHoSo.YeuCauBoSung, ct);
        return returned + businessReturns;
    }

    private async Task<decimal> TinhGiaTriKetQuaXuLyAsync(Guid hoSoId, CancellationToken ct)
    {
        var banHanh = await db.BoHoSoNghiepVus.AnyAsync(x => x.HoSoXayDungVanBanId == hoSoId && x.LoaiBoHoSo == LoaiBoHoSo.BanHanh && x.TrangThai == TrangThaiBoHoSo.DaHoanThanh && !x.IsDeleted, ct);
        if (banHanh) return 100m;
        var completedSteps = await db.BoHoSoNghiepVus.CountAsync(x => x.HoSoXayDungVanBanId == hoSoId && x.TrangThai == TrangThaiBoHoSo.DaHoanThanh && !x.IsDeleted, ct);
        return completedSteps >= 3 ? 80m : completedSteps > 0 ? 50m : 0m;
    }

    private async Task<DateTime?> NgayHoanThanhThucTeAsync(Guid hoSoId, CancellationToken ct)
    {
        var banHanh = await db.BoHoSoNghiepVus.AsNoTracking()
            .Where(x => x.HoSoXayDungVanBanId == hoSoId && x.LoaiBoHoSo == LoaiBoHoSo.BanHanh && x.TrangThai == TrangThaiBoHoSo.DaHoanThanh && !x.IsDeleted)
            .OrderByDescending(x => x.NgayHoanThanh ?? x.UpdatedAt)
            .Select(x => x.NgayHoanThanh ?? x.UpdatedAt)
            .FirstOrDefaultAsync(ct);
        if (banHanh != default) return banHanh;
        return await db.BoHoSoNghiepVus.AsNoTracking()
            .Where(x => x.HoSoXayDungVanBanId == hoSoId && x.TrangThai == TrangThaiBoHoSo.DaHoanThanh && !x.IsDeleted)
            .OrderByDescending(x => x.NgayHoanThanh ?? x.UpdatedAt)
            .Select(x => (DateTime?)(x.NgayHoanThanh ?? x.UpdatedAt))
            .FirstOrDefaultAsync(ct);
    }

    private static ChamDiemDto ToDto(HoSoXayDungVanBanChamDiem x) => new(x.Id, x.HoSoXayDungVanBanId, x.LanCham, x.TrangThaiId, x.TongDiemTuDong, x.TongDiemDieuChinh, x.TongDiemChinhThuc, x.NgayCham, x.NgayChot, x.ChiTiets.Where(d => !d.IsDeleted).OrderBy(d => d.MaTieuChi).Select(ToDetailDto).ToList());

    private static ChamDiemChiTietDto ToDetailDto(HoSoXayDungVanBanChamDiemChiTiet d) =>
        new(d.Id, d.DanhMucTieuChiDiemId, d.MaTieuChi, d.TenTieuChi, d.NhanMucDiem, d.GiaTriDauVao, d.DiemTuDong, d.DiemDieuChinh, d.DiemChinhThuc, d.DiemToiDa, d.LyDoDieuChinh, Explain(d));

    private static string Explain(HoSoXayDungVanBanChamDiemChiTiet d)
    {
        var unit = d.MaTieuChi.ToUpperInvariant() switch
        {
            "TIEN_DO_SOAN_THAO" or "TIEN_DO_THUC_HIEN" => "%",
            "DAY_DU_TAI_LIEU" => " tài liệu thiếu",
            "TUAN_THU_QUY_TRINH" => " lần",
            "CHAT_LUONG_SOAN_THAO" => " lần gửi",
            "KET_QUA_XU_LY" => " điểm quy đổi",
            _ => ""
        };
        var level = string.IsNullOrWhiteSpace(d.NhanMucDiem) ? "mức điểm phù hợp" : d.NhanMucDiem;
        return $"Giá trị đầu vào {d.GiaTriDauVao}{unit}; áp dụng {level}; điểm tự động {d.DiemTuDong}/{d.DiemToiDa}.";
    }

    private static bool InRange(decimal value, DanhMucTieuChiDiemMucItem band) => (!band.TuGiaTri.HasValue || (band.BaoGomTuGiaTri ? value >= band.TuGiaTri : value > band.TuGiaTri)) && (!band.DenGiaTri.HasValue || (band.BaoGomDenGiaTri ? value <= band.DenGiaTri : value < band.DenGiaTri));
    private static IReadOnlyList<string?> BuildExportRow(HoSoXayDungVanBan dossier, HoSoXayDungVanBanChamDiem score, HoSoXayDungVanBanChamDiemChiTiet? detail) => new[]
    {
        dossier.MaHoSo,
        dossier.TenHoSo,
        dossier.TenDuThaoVanBan,
        score.LanCham.ToString(),
        score.TongDiemChinhThuc.ToString("0.##"),
        ClassifyScore(score.TongDiemChinhThuc),
        FormatDate(score.NgayCham),
        FormatDate(score.NgayChot),
        detail?.MaTieuChi,
        detail?.TenTieuChi,
        detail?.GiaTriDauVao.ToString("0.##"),
        detail?.NhanMucDiem,
        detail?.DiemTuDong.ToString("0.##"),
        detail?.DiemDieuChinh.ToString("0.##"),
        detail?.DiemChinhThuc.ToString("0.##"),
        detail?.DiemToiDa.ToString("0.##"),
        detail is null ? null : Explain(detail),
        detail?.LyDoDieuChinh
    };

    private static string ClassifyScore(decimal value) => value >= 90m ? "Tốt" : value >= 70m ? "Khá" : value >= 50m ? "Trung bình" : "Chưa đạt";
    private static string FormatDate(DateTime? value) => value.HasValue ? value.Value.ToLocalTime().ToString("dd/MM/yyyy") : "";

    private static byte[] BuildXlsx(IReadOnlyList<IReadOnlyList<string?>> rows)
    {
        using var output = new MemoryStream();
        using (var archive = new ZipArchive(output, ZipArchiveMode.Create, leaveOpen: true))
        {
            WriteEntry(archive, "[Content_Types].xml", """
                <?xml version="1.0" encoding="UTF-8" standalone="yes"?>
                <Types xmlns="http://schemas.openxmlformats.org/package/2006/content-types">
                  <Default Extension="rels" ContentType="application/vnd.openxmlformats-package.relationships+xml"/>
                  <Default Extension="xml" ContentType="application/xml"/>
                  <Override PartName="/xl/workbook.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml"/>
                  <Override PartName="/xl/worksheets/sheet1.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml"/>
                  <Override PartName="/xl/styles.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.styles+xml"/>
                </Types>
                """);
            WriteEntry(archive, "_rels/.rels", """
                <?xml version="1.0" encoding="UTF-8" standalone="yes"?>
                <Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships">
                  <Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument" Target="xl/workbook.xml"/>
                </Relationships>
                """);
            WriteEntry(archive, "xl/workbook.xml", """
                <?xml version="1.0" encoding="UTF-8" standalone="yes"?>
                <workbook xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main" xmlns:r="http://schemas.openxmlformats.org/officeDocument/2006/relationships">
                  <sheets>
                    <sheet name="KetQuaChamDiem" sheetId="1" r:id="rId1"/>
                  </sheets>
                </workbook>
                """);
            WriteEntry(archive, "xl/_rels/workbook.xml.rels", """
                <?xml version="1.0" encoding="UTF-8" standalone="yes"?>
                <Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships">
                  <Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet" Target="worksheets/sheet1.xml"/>
                  <Relationship Id="rId2" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles" Target="styles.xml"/>
                </Relationships>
                """);
            WriteEntry(archive, "xl/styles.xml", """
                <?xml version="1.0" encoding="UTF-8" standalone="yes"?>
                <styleSheet xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main">
                  <fonts count="1"><font><sz val="11"/><name val="Calibri"/></font></fonts>
                  <fills count="1"><fill><patternFill patternType="none"/></fill></fills>
                  <borders count="1"><border/></borders>
                  <cellStyleXfs count="1"><xf numFmtId="0" fontId="0" fillId="0" borderId="0"/></cellStyleXfs>
                  <cellXfs count="1"><xf numFmtId="0" fontId="0" fillId="0" borderId="0" xfId="0"/></cellXfs>
                </styleSheet>
                """);
            WriteWorksheet(archive, rows);
        }
        return output.ToArray();
    }

    private static void WriteEntry(ZipArchive archive, string name, string content)
    {
        var entry = archive.CreateEntry(name, CompressionLevel.Fastest);
        using var writer = new StreamWriter(entry.Open(), new UTF8Encoding(false));
        writer.Write(content.Trim());
    }

    private static void WriteWorksheet(ZipArchive archive, IReadOnlyList<IReadOnlyList<string?>> rows)
    {
        var entry = archive.CreateEntry("xl/worksheets/sheet1.xml", CompressionLevel.Fastest);
        var settings = new XmlWriterSettings { Encoding = new UTF8Encoding(false), CloseOutput = false };
        using var stream = entry.Open();
        using var writer = XmlWriter.Create(stream, settings);
        writer.WriteStartDocument();
        writer.WriteStartElement("worksheet", "http://schemas.openxmlformats.org/spreadsheetml/2006/main");
        writer.WriteStartElement("sheetData");
        for (var rowIndex = 0; rowIndex < rows.Count; rowIndex++)
        {
            var rowNumber = rowIndex + 1;
            writer.WriteStartElement("row");
            writer.WriteAttributeString("r", rowNumber.ToString());
            var row = rows[rowIndex];
            for (var columnIndex = 0; columnIndex < row.Count; columnIndex++)
            {
                writer.WriteStartElement("c");
                writer.WriteAttributeString("r", $"{ColumnName(columnIndex + 1)}{rowNumber}");
                writer.WriteAttributeString("t", "inlineStr");
                writer.WriteStartElement("is");
                writer.WriteElementString("t", row[columnIndex] ?? "");
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }
        writer.WriteEndElement();
        writer.WriteEndElement();
        writer.WriteEndDocument();
    }

    private static string ColumnName(int index)
    {
        var name = "";
        while (index > 0)
        {
            index--;
            name = (char)('A' + index % 26) + name;
            index /= 26;
        }
        return name;
    }

    private async Task RefreshTotalsAsync(HoSoXayDungVanBanChamDiem item, CancellationToken ct) { var details = item.ChiTiets.Count > 0 ? item.ChiTiets : await db.HoSoXayDungVanBanChamDiemChiTiets.Where(x => x.HoSoXayDungVanBanChamDiemId == item.Id && !x.IsDeleted).ToListAsync(ct); item.TongDiemTuDong = details.Sum(x => x.DiemTuDong); item.TongDiemChinhThuc = details.Sum(x => x.DiemChinhThuc); item.TongDiemDieuChinh = item.TongDiemChinhThuc - item.TongDiemTuDong; }
    private void AddHistory(Guid id, Guid? detailId, string action, string content, Guid actor, string? oldValue = null, string? newValue = null) => db.HoSoXayDungVanBanChamDiemLichSus.Add(new() { HoSoXayDungVanBanChamDiemId = id, HoSoXayDungVanBanChamDiemChiTietId = detailId, LoaiThaoTac = action, NoiDung = content, DuLieuCu = oldValue, DuLieuMoi = newValue, NguoiThucHienId = actor, CreatedBy = actor.ToString() });
    private async Task EnsureStatusAsync(Guid id, string code, CancellationToken ct) { var status = await trangThaiClient.GetAsync(id, ct); if (status is null || !status.TrangThai || status.NhomTrangThai != NhomTrangThai || status.MaTrangThai != code) throw new InvalidOperationException("Trạng thái chấm điểm không hợp lệ."); }
    private async Task EnsureCurrentStatusAsync(Guid id, string code, CancellationToken ct) => await EnsureStatusAsync(id, code, ct);
    private Guid Actor() => currentUser.IsAuthenticated && currentUser.UserId is Guid id && id != Guid.Empty ? id : throw new UnauthorizedAccessException();
    private sealed record ScoringInput(decimal Value, string Explanation);
}
