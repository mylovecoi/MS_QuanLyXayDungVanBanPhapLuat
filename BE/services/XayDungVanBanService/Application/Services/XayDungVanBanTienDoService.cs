using BuildingBlocks.Abstractions;
using Microsoft.EntityFrameworkCore;
using XayDungVanBanService.Application.Abstractions;
using XayDungVanBanService.Application.DTOs;
using XayDungVanBanService.Infrastructure.Persistence;
using XayDungVanBanService.Infrastructure.Persistence.Entities;

namespace XayDungVanBanService.Application.Services;

public sealed class XayDungVanBanTienDoService(
    XayDungVanBanDbContext dbContext,
    ICurrentUserContext currentUser) : IXayDungVanBanTienDoService
{
    private const int SapDenHanTrongSoNgay = 5;

    public async Task<PagedResultDto<XayDungVanBanTienDoListItemDto>> GetListAsync(
        XayDungVanBanTienDoListRequest request,
        CancellationToken cancellationToken = default)
    {
        var pageSize = Math.Clamp(request.PageSize, 1, 200);
        var pageCurrent = Math.Max(request.PageCurrent, 1);
        var now = DateTime.UtcNow;

        var query = ApplyFilters(ApplyDataScope(dbContext.HoSoXayDungVanBans.AsNoTracking().Where(x => !x.IsDeleted)), request, now);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(x => x.ThoiGianDuKienHoanThanh == null)
            .ThenBy(x => x.ThoiGianDuKienHoanThanh)
            .ThenByDescending(x => x.CreatedAt)
            .Skip((pageCurrent - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new TienDoListProjection(
                x.Id,
                x.MaHoSo,
                x.TenHoSo,
                x.TenDuThaoVanBan,
                x.DanhMucVanBanId,
                x.QuyTrinhSoanThaoId,
                x.BuocHienTaiId,
                x.TrangThaiHoSoId,
                x.DonViChuTriSoanThaoId,
                x.NguoiPhuTrachId,
                x.NamXayDung,
                x.ThoiGianDuKienBatDau,
                x.ThoiGianDuKienHoanThanh,
                x.BoHoSos.Any(boHoSo => !boHoSo.IsDeleted
                    && boHoSo.LoaiBoHoSo == LoaiBoHoSo.BanHanh
                    && boHoSo.TrangThai == TrangThaiBoHoSo.DaHoanThanh),
                x.NhacTienDos.Count(nhac => !nhac.IsDeleted),
                x.NhacTienDos
                    .Where(nhac => !nhac.IsDeleted)
                    .Select(nhac => (DateTime?)nhac.NgayGui)
                    .Max(),
                x.CreatedAt))
            .ToListAsync(cancellationToken);

        return new PagedResultDto<XayDungVanBanTienDoListItemDto>(
            items.Select(x => ToListDto(x, now)).ToList(),
            totalCount,
            pageSize,
            pageCurrent);
    }

    public async Task<XayDungVanBanTienDoDetailDto?> GetByIdAsync(
        Guid hoSoId,
        CancellationToken cancellationToken = default)
    {
        var hoSo = await ApplyDataScope(dbContext.HoSoXayDungVanBans.AsNoTracking())
            .Include(x => x.BoHoSos)
            .Include(x => x.NhacTienDos)
            .FirstOrDefaultAsync(x => x.Id == hoSoId && !x.IsDeleted, cancellationToken);

        if (hoSo is null)
        {
            return null;
        }

        var now = DateTime.UtcNow;
        var daHoanThanh = IsHoanThanh(hoSo.BoHoSos);
        var nhacTienDos = hoSo.NhacTienDos
            .Where(x => !x.IsDeleted)
            .OrderByDescending(x => x.NgayGui)
            .Select(ToDto)
            .ToList();
        var boHoSos = hoSo.BoHoSos
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.NgayTao)
            .Select(x => new XayDungVanBanBoHoSoDto(
                x.Id,
                x.BuocQuyTrinhId,
                (int)x.LoaiBoHoSo,
                (int)x.TrangThai,
                x.LanXuLy,
                x.NgayTao,
                x.NgayGui,
                x.NgayHoanThanh))
            .ToList();

        return new XayDungVanBanTienDoDetailDto(
            hoSo.Id,
            hoSo.MaHoSo,
            hoSo.TenHoSo,
            hoSo.TenDuThaoVanBan,
            hoSo.DanhMucVanBanId,
            hoSo.QuyTrinhSoanThaoId,
            hoSo.BuocHienTaiId,
            hoSo.TrangThaiHoSoId,
            hoSo.DonViChuTriSoanThaoId,
            hoSo.NguoiPhuTrachId,
            hoSo.NamXayDung,
            hoSo.ThoiGianDuKienBatDau,
            hoSo.ThoiGianDuKienHoanThanh,
            TinhTrang(hoSo.ThoiGianDuKienHoanThanh, daHoanThanh, now),
            SoNgayConLai(hoSo.ThoiGianDuKienHoanThanh, now),
            nhacTienDos.Count,
            nhacTienDos.FirstOrDefault()?.NgayGui,
            boHoSos,
            nhacTienDos);
    }

    public async Task<IReadOnlyList<XayDungVanBanNhacTienDoDto>?> GetNhacNhoAsync(
        Guid hoSoId,
        CancellationToken cancellationToken = default)
    {
        if (!await CanAccessHoSoAsync(hoSoId, cancellationToken))
        {
            return null;
        }

        var reminders = await dbContext.HoSoXayDungVanBanNhacTienDos
            .AsNoTracking()
            .Where(x => x.HoSoXayDungVanBanId == hoSoId && !x.IsDeleted)
            .OrderByDescending(x => x.NgayGui)
            .ToListAsync(cancellationToken);

        return reminders.Select(ToDto).ToList();
    }

    public async Task<XayDungVanBanNhacTienDoDto?> TaoNhacNhoAsync(
        Guid hoSoId,
        TaoNhacTienDoRequest request,
        CancellationToken cancellationToken = default)
    {
        var actor = Actor();
        if (string.IsNullOrWhiteSpace(request.NoiDungNhacNho))
        {
            throw new InvalidOperationException("Nội dung nhắc nhở không được để trống.");
        }

        var hoSo = await ApplyDataScope(dbContext.HoSoXayDungVanBans)
            .FirstOrDefaultAsync(x => x.Id == hoSoId && !x.IsDeleted, cancellationToken);
        if (hoSo is null)
        {
            return null;
        }

        var now = DateTime.UtcNow;
        var nhacTienDo = new HoSoXayDungVanBanNhacTienDo
        {
            HoSoXayDungVanBanId = hoSoId,
            LoaiNhacNho = NormalizeCode(request.LoaiNhacNho, "NHAC_TIEN_DO"),
            TrangThaiXuLy = "DA_GUI",
            NoiDungNhacNho = request.NoiDungNhacNho.Trim(),
            NguoiGuiId = actor.UserId,
            DonViGuiId = actor.DonViId,
            NguoiNhanId = request.NguoiNhanId,
            DonViNhanId = request.DonViNhanId,
            NgayGui = now,
            CreatedBy = actor.UserId.ToString()
        };

        dbContext.HoSoXayDungVanBanNhacTienDos.Add(nhacTienDo);
        AddTimeline(hoSo, "NHAC_NHO_TIEN_DO", nhacTienDo.NoiDungNhacNho, actor, now);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToDto(nhacTienDo);
    }

    public async Task<XayDungVanBanNhacTienDoDto?> PhanHoiAsync(
        Guid hoSoId,
        Guid nhacNhoId,
        CapNhatPhanHoiNhacTienDoRequest request,
        CancellationToken cancellationToken = default)
    {
        var actor = Actor();
        if (string.IsNullOrWhiteSpace(request.PhanHoi))
        {
            throw new InvalidOperationException("Nội dung phản hồi không được để trống.");
        }

        var data = await LoadNhacTienDoAsync(hoSoId, nhacNhoId, cancellationToken);
        if (data is null)
        {
            return null;
        }

        if (!CanInteractWithReminder(data.NhacTienDo, actor))
        {
            throw new UnauthorizedAccessException();
        }

        var now = DateTime.UtcNow;
        data.NhacTienDo.PhanHoi = request.PhanHoi.Trim();
        data.NhacTienDo.TrangThaiXuLy = "DA_PHAN_HOI";
        data.NhacTienDo.NgayPhanHoi = now;
        data.NhacTienDo.NguoiPhanHoiId = actor.UserId;
        data.NhacTienDo.UpdatedAt = now;
        data.NhacTienDo.UpdatedBy = actor.UserId.ToString();
        AddTimeline(data.HoSo, "PHAN_HOI_NHAC_TIEN_DO", data.NhacTienDo.PhanHoi, actor, now);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToDto(data.NhacTienDo);
    }

    public async Task<XayDungVanBanNhacTienDoDto?> XacNhanXuLyAsync(
        Guid hoSoId,
        Guid nhacNhoId,
        XacNhanXuLyNhacTienDoRequest request,
        CancellationToken cancellationToken = default)
    {
        var actor = Actor();
        var data = await LoadNhacTienDoAsync(hoSoId, nhacNhoId, cancellationToken);
        if (data is null)
        {
            return null;
        }

        if (!currentUser.IsSSA && data.NhacTienDo.NguoiGuiId != actor.UserId)
        {
            throw new UnauthorizedAccessException();
        }

        var now = DateTime.UtcNow;
        data.NhacTienDo.TrangThaiXuLy = "DA_XU_LY";
        data.NhacTienDo.NgayXacNhanXuLy = now;
        data.NhacTienDo.NguoiXacNhanXuLyId = actor.UserId;
        data.NhacTienDo.GhiChuXuLy = request.GhiChuXuLy;
        data.NhacTienDo.UpdatedAt = now;
        data.NhacTienDo.UpdatedBy = actor.UserId.ToString();
        AddTimeline(data.HoSo, "XAC_NHAN_XU_LY_NHAC_TIEN_DO", request.GhiChuXuLy, actor, now);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToDto(data.NhacTienDo);
    }

    private IQueryable<HoSoXayDungVanBan> ApplyDataScope(IQueryable<HoSoXayDungVanBan> query)
    {
        if (currentUser.IsSSA)
        {
            return query;
        }

        var userId = currentUser.UserId;
        var donViId = currentUser.DonViId;
        var userIdText = userId?.ToString();

        return query.Where(x =>
            (donViId.HasValue && x.DonViChuTriSoanThaoId == donViId.Value)
            || (userId.HasValue && x.NguoiPhuTrachId == userId.Value)
            || (userIdText != null && x.CreatedBy == userIdText)
            || x.NhacTienDos.Any(nhac => !nhac.IsDeleted
                && ((userId.HasValue && (nhac.NguoiGuiId == userId.Value || nhac.NguoiNhanId == userId.Value))
                    || (donViId.HasValue && (nhac.DonViGuiId == donViId.Value || nhac.DonViNhanId == donViId.Value)))));
    }

    private static IQueryable<HoSoXayDungVanBan> ApplyFilters(
        IQueryable<HoSoXayDungVanBan> query,
        XayDungVanBanTienDoListRequest request,
        DateTime now)
    {
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            query = query.Where(x => x.MaHoSo.Contains(search)
                || x.TenHoSo.Contains(search)
                || x.TenDuThaoVanBan.Contains(search));
        }

        if (request.DanhMucVanBanId is Guid danhMucVanBanId)
            query = query.Where(x => x.DanhMucVanBanId == danhMucVanBanId);
        if (request.QuyTrinhSoanThaoId is Guid quyTrinhSoanThaoId)
            query = query.Where(x => x.QuyTrinhSoanThaoId == quyTrinhSoanThaoId);
        if (request.BuocHienTaiId is Guid buocHienTaiId)
            query = query.Where(x => x.BuocHienTaiId == buocHienTaiId);
        if (request.TrangThaiHoSoId is Guid trangThaiHoSoId)
            query = query.Where(x => x.TrangThaiHoSoId == trangThaiHoSoId);
        if (request.DonViChuTriSoanThaoId is Guid donViChuTriSoanThaoId)
            query = query.Where(x => x.DonViChuTriSoanThaoId == donViChuTriSoanThaoId);
        if (request.NguoiPhuTrachId is Guid nguoiPhuTrachId)
            query = query.Where(x => x.NguoiPhuTrachId == nguoiPhuTrachId);
        if (request.NamXayDung is int namXayDung)
            query = query.Where(x => x.NamXayDung == namXayDung);
        if (request.TuNgayHan is DateTime tuNgayHan)
            query = query.Where(x => x.ThoiGianDuKienHoanThanh >= tuNgayHan);
        if (request.DenNgayHan is DateTime denNgayHan)
            query = query.Where(x => x.ThoiGianDuKienHoanThanh <= denNgayHan);

        if (!string.IsNullOrWhiteSpace(request.TinhTrangTienDo))
        {
            var code = request.TinhTrangTienDo.Trim().ToUpperInvariant();
            var today = now.Date;
            var soon = today.AddDays(SapDenHanTrongSoNgay);
            query = code switch
            {
                "CHUA_CO_HAN" => query.Where(x => !x.ThoiGianDuKienHoanThanh.HasValue),
                "QUA_HAN" => query.Where(x => x.ThoiGianDuKienHoanThanh.HasValue
                    && x.ThoiGianDuKienHoanThanh.Value < today
                    && !x.BoHoSos.Any(boHoSo => !boHoSo.IsDeleted
                        && boHoSo.LoaiBoHoSo == LoaiBoHoSo.BanHanh
                        && boHoSo.TrangThai == TrangThaiBoHoSo.DaHoanThanh)),
                "SAP_DEN_HAN" => query.Where(x => x.ThoiGianDuKienHoanThanh.HasValue
                    && x.ThoiGianDuKienHoanThanh.Value >= today
                    && x.ThoiGianDuKienHoanThanh.Value <= soon
                    && !x.BoHoSos.Any(boHoSo => !boHoSo.IsDeleted
                        && boHoSo.LoaiBoHoSo == LoaiBoHoSo.BanHanh
                        && boHoSo.TrangThai == TrangThaiBoHoSo.DaHoanThanh)),
                "DUNG_HAN" => query.Where(x =>
                    !x.BoHoSos.Any(boHoSo => !boHoSo.IsDeleted
                        && boHoSo.LoaiBoHoSo == LoaiBoHoSo.BanHanh
                        && boHoSo.TrangThai == TrangThaiBoHoSo.DaHoanThanh)
                    && x.ThoiGianDuKienHoanThanh.HasValue
                    && x.ThoiGianDuKienHoanThanh.Value > soon),
                "DA_HOAN_THANH" => query.Where(x =>
                    x.BoHoSos.Any(boHoSo => !boHoSo.IsDeleted
                        && boHoSo.LoaiBoHoSo == LoaiBoHoSo.BanHanh
                        && boHoSo.TrangThai == TrangThaiBoHoSo.DaHoanThanh)),
                _ => query
            };
        }

        return query;
    }

    private async Task<bool> CanAccessHoSoAsync(Guid hoSoId, CancellationToken cancellationToken)
    {
        return await ApplyDataScope(dbContext.HoSoXayDungVanBans.AsNoTracking())
            .AnyAsync(x => x.Id == hoSoId && !x.IsDeleted, cancellationToken);
    }

    private async Task<NhacTienDoData?> LoadNhacTienDoAsync(
        Guid hoSoId,
        Guid nhacNhoId,
        CancellationToken cancellationToken)
    {
        var hoSo = await ApplyDataScope(dbContext.HoSoXayDungVanBans)
            .FirstOrDefaultAsync(x => x.Id == hoSoId && !x.IsDeleted, cancellationToken);
        if (hoSo is null)
        {
            return null;
        }

        var nhacTienDo = await dbContext.HoSoXayDungVanBanNhacTienDos
            .FirstOrDefaultAsync(x => x.Id == nhacNhoId
                && x.HoSoXayDungVanBanId == hoSoId
                && !x.IsDeleted, cancellationToken);

        return nhacTienDo is null ? null : new NhacTienDoData(hoSo, nhacTienDo);
    }

    private static bool CanInteractWithReminder(HoSoXayDungVanBanNhacTienDo nhacTienDo, CurrentActor actor)
    {
        return nhacTienDo.NguoiNhanId == actor.UserId
            || nhacTienDo.DonViNhanId == actor.DonViId
            || nhacTienDo.NguoiGuiId == actor.UserId;
    }

    private CurrentActor Actor()
    {
        if (!currentUser.IsAuthenticated || currentUser.UserId is not Guid userId)
        {
            throw new UnauthorizedAccessException();
        }

        return new CurrentActor(userId, currentUser.DonViId);
    }

    private void AddTimeline(
        HoSoXayDungVanBan hoSo,
        string hanhDong,
        string? noiDung,
        CurrentActor actor,
        DateTime thoiGian)
    {
        dbContext.HoSoXayDungVanBanLichSuXuLys.Add(new HoSoXayDungVanBanLichSuXuLy
        {
            HoSoXayDungVanBanId = hoSo.Id,
            BuocQuyTrinhTruocId = hoSo.BuocHienTaiId,
            BuocQuyTrinhSauId = hoSo.BuocHienTaiId,
            TrangThaiTruocId = hoSo.TrangThaiHoSoId,
            TrangThaiSauId = hoSo.TrangThaiHoSoId,
            HanhDong = hanhDong,
            NoiDung = noiDung,
            NguoiXuLyId = actor.UserId,
            DonViXuLyId = actor.DonViId ?? Guid.Empty,
            ThoiGianXuLy = thoiGian,
            CreatedBy = actor.UserId.ToString()
        });
    }

    private static XayDungVanBanTienDoListItemDto ToListDto(TienDoListProjection item, DateTime now)
    {
        return new XayDungVanBanTienDoListItemDto(
            item.HoSoId,
            item.MaHoSo,
            item.TenHoSo,
            item.TenDuThaoVanBan,
            item.DanhMucVanBanId,
            item.QuyTrinhSoanThaoId,
            item.BuocHienTaiId,
            item.TrangThaiHoSoId,
            item.DonViChuTriSoanThaoId,
            item.NguoiPhuTrachId,
            item.NamXayDung,
            item.ThoiGianDuKienBatDau,
            item.ThoiGianDuKienHoanThanh,
            TinhTrang(item.ThoiGianDuKienHoanThanh, item.DaHoanThanh, now),
            SoNgayConLai(item.ThoiGianDuKienHoanThanh, now),
            item.SoLanNhacNho,
            item.LanNhacNhoGanNhat,
            item.CreatedAt);
    }

    private static XayDungVanBanNhacTienDoDto ToDto(HoSoXayDungVanBanNhacTienDo entity)
    {
        return new XayDungVanBanNhacTienDoDto(
            entity.Id,
            entity.HoSoXayDungVanBanId,
            entity.LoaiNhacNho,
            entity.TrangThaiXuLy,
            entity.NoiDungNhacNho,
            entity.NguoiGuiId,
            entity.DonViGuiId,
            entity.NguoiNhanId,
            entity.DonViNhanId,
            entity.NgayGui,
            entity.NgayXem,
            entity.PhanHoi,
            entity.NgayPhanHoi,
            entity.NguoiPhanHoiId,
            entity.NgayXacNhanXuLy,
            entity.NguoiXacNhanXuLyId,
            entity.GhiChuXuLy);
    }

    private static string TinhTrang(DateTime? hanHoanThanh, bool daHoanThanh, DateTime now)
    {
        if (!hanHoanThanh.HasValue)
        {
            return "CHUA_CO_HAN";
        }

        if (daHoanThanh)
        {
            return "DA_HOAN_THANH";
        }

        var today = now.Date;
        var deadline = hanHoanThanh.Value.Date;
        if (deadline < today)
        {
            return "QUA_HAN";
        }

        return deadline <= today.AddDays(SapDenHanTrongSoNgay) ? "SAP_DEN_HAN" : "DUNG_HAN";
    }

    private static int SoNgayConLai(DateTime? hanHoanThanh, DateTime now)
    {
        return hanHoanThanh.HasValue ? (hanHoanThanh.Value.Date - now.Date).Days : 0;
    }

    private static bool IsHoanThanh(IEnumerable<BoHoSoNghiepVu> boHoSos)
    {
        return boHoSos.Any(x => !x.IsDeleted
            && x.LoaiBoHoSo == LoaiBoHoSo.BanHanh
            && x.TrangThai == TrangThaiBoHoSo.DaHoanThanh);
    }

    private static string NormalizeCode(string? value, string fallback)
    {
        return string.IsNullOrWhiteSpace(value)
            ? fallback
            : value.Trim().ToUpperInvariant();
    }

    private sealed record CurrentActor(Guid UserId, Guid? DonViId);

    private sealed record NhacTienDoData(
        HoSoXayDungVanBan HoSo,
        HoSoXayDungVanBanNhacTienDo NhacTienDo);

    private sealed record TienDoListProjection(
        Guid HoSoId,
        string MaHoSo,
        string TenHoSo,
        string TenDuThaoVanBan,
        Guid DanhMucVanBanId,
        Guid QuyTrinhSoanThaoId,
        Guid BuocHienTaiId,
        Guid TrangThaiHoSoId,
        Guid DonViChuTriSoanThaoId,
        Guid? NguoiPhuTrachId,
        int NamXayDung,
        DateTime? ThoiGianDuKienBatDau,
        DateTime? ThoiGianDuKienHoanThanh,
        bool DaHoanThanh,
        int SoLanNhacNho,
        DateTime? LanNhacNhoGanNhat,
        DateTime CreatedAt);
}
