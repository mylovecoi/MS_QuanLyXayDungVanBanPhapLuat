using Microsoft.EntityFrameworkCore;
using XayDungVanBanService.Application.Abstractions;
using XayDungVanBanService.Application.DTOs;
using XayDungVanBanService.Infrastructure.Persistence;

namespace XayDungVanBanService.Application.Services;

public sealed class XayDungVanBanHoSoQueryService(
    XayDungVanBanDbContext dbContext) : IXayDungVanBanHoSoQueryService
{
    public async Task<PagedResultDto<XayDungVanBanHoSoListItemDto>> GetListAsync(
        XayDungVanBanHoSoListRequest request,
        CancellationToken cancellationToken = default)
    {
        var pageSize = Math.Clamp(request.PageSize, 1, 200);
        var pageCurrent = Math.Max(request.PageCurrent, 1);
        var query = ApplyFilters(dbContext.HoSoXayDungVanBans.AsNoTracking()
            .Where(x => !x.IsDeleted), request);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((pageCurrent - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new XayDungVanBanHoSoListItemDto(
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
                x.CreatedAt))
            .ToListAsync(cancellationToken);

        return new PagedResultDto<XayDungVanBanHoSoListItemDto>(items, totalCount, pageSize, pageCurrent);
    }

    public async Task<XayDungVanBanHoSoDetailDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var hoSo = await dbContext.HoSoXayDungVanBans
            .AsNoTracking()
            .Include(x => x.BoHoSos)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);

        if (hoSo is null)
        {
            return null;
        }

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

        return new XayDungVanBanHoSoDetailDto(
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
            hoSo.MoTa,
            hoSo.HoSoDangKyXayDungVanBanId,
            hoSo.CreatedAt,
            boHoSos);
    }

    public async Task<IReadOnlyList<XayDungVanBanTimelineItemDto>?> GetTimelineAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var exists = await dbContext.HoSoXayDungVanBans
            .AsNoTracking()
            .AnyAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);

        if (!exists)
        {
            return null;
        }

        return await dbContext.HoSoXayDungVanBanLichSuXuLys
            .AsNoTracking()
            .Where(x => x.HoSoXayDungVanBanId == id && !x.IsDeleted)
            .OrderBy(x => x.ThoiGianXuLy)
            .ThenBy(x => x.CreatedAt)
            .Select(x => new XayDungVanBanTimelineItemDto(
                x.Id,
                x.BoHoSoNghiepVuId,
                x.BuocQuyTrinhTruocId,
                x.BuocQuyTrinhSauId,
                x.TrangThaiTruocId,
                x.TrangThaiSauId,
                x.TenBuocTruoc,
                x.TenBuocSau,
                x.ThuTuBuocTruoc,
                x.ThuTuBuocSau,
                x.HanhDong,
                x.NoiDung,
                x.LyDo,
                x.NguoiXuLyId,
                x.DonViXuLyId,
                x.ThoiGianXuLy,
                x.HoSoXayDungVanBanFileId,
                x.BoHoSoNghiepVuTaiLieuId))
            .ToListAsync(cancellationToken);
    }

    private static IQueryable<Infrastructure.Persistence.Entities.HoSoXayDungVanBan> ApplyFilters(
        IQueryable<Infrastructure.Persistence.Entities.HoSoXayDungVanBan> query,
        XayDungVanBanHoSoListRequest request)
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

        return query;
    }
}
