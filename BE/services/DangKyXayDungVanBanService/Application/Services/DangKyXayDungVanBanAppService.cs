using BuildingBlocks.Abstractions;
using DangKyXayDungVanBanService.Application.Abstractions;
using DangKyXayDungVanBanService.Application.DTOs;
using DangKyXayDungVanBanService.Infrastructure.Persistence;
using DangKyXayDungVanBanService.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace DangKyXayDungVanBanService.Application.Services;

public class DangKyXayDungVanBanAppService : IDangKyXayDungVanBanAppService
{
    private const string TaoMoiActionCode = "TAO_MOI";
    private const string TaoMoiActionName = "Tao moi";
    private const string KhoiTaoQuyTrinhXayDungActionCode = "KHOI_TAO_QUY_TRINH_XAY_DUNG";
    private const string KhoiTaoQuyTrinhXayDungActionName = "Khoi tao quy trinh xay dung";

    private readonly DangKyXayDungVanBanDbContext _dbContext;
    private readonly IWebHostEnvironment _environment;
    private readonly ICurrentUserContext _currentUserContext;

    public DangKyXayDungVanBanAppService(
        DangKyXayDungVanBanDbContext dbContext,
        IWebHostEnvironment environment,
        ICurrentUserContext currentUserContext)
    {
        _dbContext = dbContext;
        _environment = environment;
        _currentUserContext = currentUserContext;
    }

    public async Task<PagedResultDto<DangKyXayDungVanBanDto>> GetListAsync(DangKyXayDungVanBanListRequest request, CancellationToken cancellationToken)
    {
        var pageSize = Math.Clamp(request.PageSize, 1, 200);
        var pageCurrent = Math.Max(request.PageCurrent, 1);
        var query = ApplyListFilters(
            ApplyDataScope(_dbContext.DangKyXayDungVanBans.AsNoTracking().Where(x => !x.IsDeleted)),
            request);

        var totalCount = await query.CountAsync(cancellationToken);
        var rows = await (
            from hoSo in query
            join trangThai in _dbContext.DangKyTrangThaiHoSos.AsNoTracking()
                on hoSo.TrangThaiHoSoId equals trangThai.Id into trangThaiJoin
            from trangThai in trangThaiJoin.DefaultIfEmpty()
            select new
            {
                hoSo.Id,
                hoSo.MaHoSo,
                hoSo.TenHoSo,
                hoSo.TenVanBanDuKien,
                hoSo.LoaiVanBanId,
                hoSo.QuyTrinhSoanThaoId,
                hoSo.BuocHienTaiId,
                hoSo.TrangThaiHoSoId,
                hoSo.DonViSoanThaoId,
                hoSo.DonViPheDuyetId,
                hoSo.NamDangKy,
                hoSo.DaKhoiTaoQuyTrinhXayDung,
                hoSo.HoSoXayDungVanBanId,
                hoSo.CreatedAt,
                MaTrangThaiHoSo = trangThai == null ? null : trangThai.MaTrangThai,
                TenTrangThaiHoSo = trangThai == null ? null : trangThai.TenTrangThai,
                MauTrangThaiHoSo = trangThai == null ? null : trangThai.MauHienThi
            })
            .OrderByDescending(x => x.CreatedAt)
            .Skip((pageCurrent - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = rows.Select(x => new DangKyXayDungVanBanDto(
            x.Id,
            x.MaHoSo,
            x.TenHoSo,
            x.TenVanBanDuKien,
            x.LoaiVanBanId,
            x.QuyTrinhSoanThaoId,
            x.BuocHienTaiId,
            x.TrangThaiHoSoId,
            x.DonViSoanThaoId,
            x.DonViPheDuyetId,
            x.NamDangKy,
            x.DaKhoiTaoQuyTrinhXayDung,
            x.HoSoXayDungVanBanId,
            x.CreatedAt,
            x.MaTrangThaiHoSo,
            x.TenTrangThaiHoSo,
            x.MauTrangThaiHoSo,
            GetMaBuocHienTai(x.BuocHienTaiId),
            GetTenBuocHienTai(x.BuocHienTaiId))).ToList();

        return new PagedResultDto<DangKyXayDungVanBanDto>(
            items,
            totalCount,
            pageSize,
            pageCurrent);
    }

    public async Task<PagedResultDto<DangKyXayDungVanBanKetQuaListItemDto>> GetKetQuaListAsync(DangKyXayDungVanBanListRequest request, CancellationToken cancellationToken)
    {
        var pageSize = Math.Clamp(request.PageSize, 1, 200);
        var pageCurrent = Math.Max(request.PageCurrent, 1);
        var query = ApplyListFilters(
            ApplyDataScope(_dbContext.DangKyXayDungVanBans.AsNoTracking().Where(x => !x.IsDeleted)),
            request);

        query = query.Where(hoSo => _dbContext.DangKyXayDungVanBanLichSuXuLys.Any(lichSu =>
            lichSu.DangKyXayDungVanBanId == hoSo.Id
            && !lichSu.IsDeleted
            && lichSu.ChuyenBuocId == DangKySeedIds.DanhMucChuyenBuocDangKyXayDungQppl.PheDuyetToCapNhatKetQua));

        var totalCount = await query.CountAsync(cancellationToken);
        var rows = await (
            from hoSo in query
            join trangThai in _dbContext.DangKyTrangThaiHoSos.AsNoTracking()
                on hoSo.TrangThaiHoSoId equals trangThai.Id into trangThaiJoin
            from trangThai in trangThaiJoin.DefaultIfEmpty()
            join ketQua in _dbContext.DangKyXayDungVanBanKetQuaPheDuyets.AsNoTracking().Where(x => !x.IsDeleted)
                on hoSo.KetQuaPheDuyetId equals ketQua.Id into ketQuaJoin
            from ketQua in ketQuaJoin.DefaultIfEmpty()
            select new
            {
                hoSo.Id,
                hoSo.MaHoSo,
                hoSo.TenHoSo,
                hoSo.TenVanBanDuKien,
                hoSo.NamDangKy,
                hoSo.TrangThaiHoSoId,
                hoSo.BuocHienTaiId,
                hoSo.CreatedAt,
                MaTrangThaiHoSo = trangThai == null ? null : trangThai.MaTrangThai,
                TenTrangThaiHoSo = trangThai == null ? null : trangThai.TenTrangThai,
                MauTrangThaiHoSo = trangThai == null ? null : trangThai.MauHienThi,
                KetQua = ketQua == null ? null : ketQua.KetQua,
                NgayKetQua = ketQua == null ? null : ketQua.NgayVanBan
            })
            .OrderByDescending(x => x.NgayKetQua ?? x.CreatedAt)
            .Skip((pageCurrent - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = rows.Select(x => new DangKyXayDungVanBanKetQuaListItemDto(
            x.Id,
            x.MaHoSo,
            x.TenHoSo,
            x.TenVanBanDuKien,
            x.NamDangKy,
            x.TrangThaiHoSoId,
            x.MaTrangThaiHoSo,
            x.TenTrangThaiHoSo,
            x.MauTrangThaiHoSo,
            x.BuocHienTaiId,
            GetMaBuocHienTai(x.BuocHienTaiId),
            GetTenBuocHienTai(x.BuocHienTaiId),
            x.KetQua,
            x.NgayKetQua,
            x.CreatedAt)).ToList();

        return new PagedResultDto<DangKyXayDungVanBanKetQuaListItemDto>(
            items,
            totalCount,
            pageSize,
            pageCurrent);
    }

    public async Task<DangKyXayDungVanBanDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var row = await (
            from hoSo in ApplyDataScope(_dbContext.DangKyXayDungVanBans.AsNoTracking().Where(x => !x.IsDeleted))
            join trangThai in _dbContext.DangKyTrangThaiHoSos.AsNoTracking()
                on hoSo.TrangThaiHoSoId equals trangThai.Id into trangThaiJoin
            from trangThai in trangThaiJoin.DefaultIfEmpty()
            where hoSo.Id == id
            select new
            {
                hoSo.Id,
                hoSo.MaHoSo,
                hoSo.TenHoSo,
                hoSo.TenVanBanDuKien,
                hoSo.LoaiVanBanId,
                hoSo.QuyTrinhSoanThaoId,
                hoSo.BuocHienTaiId,
                hoSo.TrangThaiHoSoId,
                hoSo.DonViSoanThaoId,
                hoSo.DonViPheDuyetId,
                hoSo.NamDangKy,
                hoSo.DaKhoiTaoQuyTrinhXayDung,
                hoSo.HoSoXayDungVanBanId,
                hoSo.CreatedAt,
                MaTrangThaiHoSo = trangThai == null ? null : trangThai.MaTrangThai,
                TenTrangThaiHoSo = trangThai == null ? null : trangThai.TenTrangThai,
                MauTrangThaiHoSo = trangThai == null ? null : trangThai.MauHienThi
            }).FirstOrDefaultAsync(cancellationToken);

        return row is null ? null : new DangKyXayDungVanBanDto(
            row.Id,
            row.MaHoSo,
            row.TenHoSo,
            row.TenVanBanDuKien,
            row.LoaiVanBanId,
            row.QuyTrinhSoanThaoId,
            row.BuocHienTaiId,
            row.TrangThaiHoSoId,
            row.DonViSoanThaoId,
            row.DonViPheDuyetId,
            row.NamDangKy,
            row.DaKhoiTaoQuyTrinhXayDung,
            row.HoSoXayDungVanBanId,
            row.CreatedAt,
            row.MaTrangThaiHoSo,
            row.TenTrangThaiHoSo,
            row.MauTrangThaiHoSo,
            GetMaBuocHienTai(row.BuocHienTaiId),
            GetTenBuocHienTai(row.BuocHienTaiId));
    }

    public async Task<DangKyXayDungVanBanDto> CreateAsync(TaoDangKyXayDungVanBanRequest request, CancellationToken cancellationToken)
    {
        var currentUser = RequireCurrentUser();
        if (!currentUser.IsSSA && currentUser.DonViId != request.DonViSoanThaoId)
        {
            throw new UnauthorizedAccessException("Nguoi dung khong thuoc don vi soan thao cua ho so.");
        }

        var entity = new DangKyXayDungVanBan
        {
            MaHoSo = await GenerateMaHoSoAsync(request.NamDangKy, cancellationToken),
            TenHoSo = request.TenHoSo,
            TenVanBanDuKien = request.TenVanBanDuKien,
            LoaiVanBanId = request.LoaiVanBanId,
            QuyTrinhSoanThaoId = DangKySeedIds.DanhMucQuyTrinh.DeXuatDangKyXayDungQppl,
            BuocHienTaiId = DangKySeedIds.DanhMucBuocDangKyXayDungQppl.LapHoSo,
            TrangThaiHoSoId = DangKySeedIds.TrangThai.DangSoanThao,
            DonViSoanThaoId = request.DonViSoanThaoId,
            DonViPheDuyetId = request.DonViPheDuyetId,
            NamDangKy = request.NamDangKy,
            CanCuDeXuat = request.CanCuDeXuat,
            SuCanThiet = request.SuCanThiet,
            NoiDungChinhSach = request.NoiDungChinhSach,
            DuKienThoiGianTrinh = request.DuKienThoiGianTrinh,
            CreatedBy = currentUser.UserId.ToString()
        };

        entity.LichSuXuLys.Add(new DangKyXayDungVanBanLichSuXuLy
        {
            DenBuocId = DangKySeedIds.DanhMucBuocDangKyXayDungQppl.LapHoSo,
            HanhDongId = DangKySeedIds.HanhDong.TaoMoi,
            MaHanhDongSnapshot = TaoMoiActionCode,
            TenHanhDongSnapshot = TaoMoiActionName,
            TrangThaiSauId = DangKySeedIds.TrangThai.DangSoanThao,
            NoiDungXuLy = "Tao moi ho so dang ky xay dung van ban",
            NguoiXuLyId = currentUser.UserId,
            TenNguoiXuLy = ResolveUserDisplayName(request.TenNguoiXuLy),
            DonViXuLyId = request.DonViSoanThaoId,
            TenDonViXuLy = request.TenDonViXuLy,
            CreatedBy = currentUser.UserId.ToString()
        });

        _dbContext.DangKyXayDungVanBans.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return ToDto(entity);
    }

    public async Task<DangKyXayDungVanBanDto?> UpdateAsync(Guid id, CapNhatDangKyXayDungVanBanRequest request, CancellationToken cancellationToken)
    {
        var entity = await ApplyDataScope(_dbContext.DangKyXayDungVanBans.Where(x => !x.IsDeleted))
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (entity is null)
        {
            return null;
        }

        if (!CanModifyDraft(entity))
        {
            throw new UnauthorizedAccessException("Nguoi dung khong duoc sua ho so o trang thai hien tai.");
        }

        entity.TenHoSo = request.TenHoSo;
        entity.TenVanBanDuKien = request.TenVanBanDuKien;
        entity.LoaiVanBanId = request.LoaiVanBanId;
        entity.DonViPheDuyetId = request.DonViPheDuyetId;
        entity.NamDangKy = request.NamDangKy;
        entity.CanCuDeXuat = request.CanCuDeXuat;
        entity.SuCanThiet = request.SuCanThiet;
        entity.NoiDungChinhSach = request.NoiDungChinhSach;
        entity.DuKienThoiGianTrinh = request.DuKienThoiGianTrinh;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = CurrentUserIdText();

        await _dbContext.SaveChangesAsync(cancellationToken);

        return ToDto(entity);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await ApplyDataScope(_dbContext.DangKyXayDungVanBans.Where(x => !x.IsDeleted))
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (entity is null)
        {
            return false;
        }

        if (!CanModifyDraft(entity))
        {
            throw new UnauthorizedAccessException("Nguoi dung khong duoc xoa ho so o trang thai hien tai.");
        }

        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = CurrentUserIdText();

        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<DangKyXayDungVanBanTimelineDto>> GetTimelineAsync(Guid id, CancellationToken cancellationToken)
    {
        if (!await CanAccessHoSoAsync(id, cancellationToken))
        {
            return Array.Empty<DangKyXayDungVanBanTimelineDto>();
        }

        return await _dbContext.DangKyXayDungVanBanLichSuXuLys
            .AsNoTracking()
            .Where(x => x.DangKyXayDungVanBanId == id && !x.IsDeleted)
            .OrderBy(x => x.NgayXuLy)
            .Select(x => new DangKyXayDungVanBanTimelineDto(
                x.Id,
                x.DangKyXayDungVanBanId,
                x.MaHanhDongSnapshot,
                x.TenHanhDongSnapshot,
                x.TenBuocTu,
                x.TenBuocDen,
                x.TenTrangThaiTruocSnapshot,
                x.TenTrangThaiSauSnapshot,
                x.NoiDungXuLy,
                x.LyDoTraLai,
                x.TenNguoiXuLy,
                x.TenDonViXuLy,
                x.NgayXuLy))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<HanhDongKhaDungDto>> GetHanhDongKhaDungAsync(Guid id, CancellationToken cancellationToken)
    {
        var hoSo = await ApplyDataScope(_dbContext.DangKyXayDungVanBans.AsNoTracking().Where(x => !x.IsDeleted))
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (hoSo is null)
        {
            return Array.Empty<HanhDongKhaDungDto>();
        }

        var actions = await (
            from cauHinh in _dbContext.DangKyCauHinhChuyenTrangThais.AsNoTracking()
            join hanhDong in _dbContext.DangKyHanhDongXuLys.AsNoTracking()
                on cauHinh.HanhDongId equals hanhDong.Id
            where cauHinh.TrangThai
                && hanhDong.TrangThai
                && cauHinh.QuyTrinhSoanThaoId == hoSo.QuyTrinhSoanThaoId
                && cauHinh.BuocHienTaiId == hoSo.BuocHienTaiId
                && cauHinh.TrangThaiHienTaiId == hoSo.TrangThaiHoSoId
            orderby hanhDong.ThuTuSapXep
            select new HanhDongKhaDungDto(
                hanhDong.Id,
                hanhDong.MaHanhDong,
                hanhDong.TenHanhDong,
                hanhDong.LoaiHanhDong,
                cauHinh.YeuCauLyDo || hanhDong.YeuCauLyDo,
                cauHinh.YeuCauFileDinhKem || hanhDong.YeuCauFileDinhKem,
                cauHinh.BuocTiepTheoId,
                cauHinh.TrangThaiTiepTheoId))
            .ToListAsync(cancellationToken);

        return actions
            .Where(action => CanPerformAction(hoSo, action.HanhDongId))
            .ToList();
    }

    public async Task<IReadOnlyList<DangKyXayDungVanBanFileDto>?> GetFilesAsync(Guid id, CancellationToken cancellationToken)
    {
        var exists = await CanAccessHoSoAsync(id, cancellationToken);

        if (!exists)
        {
            return null;
        }

        return await _dbContext.DangKyXayDungVanBanFiles
            .AsNoTracking()
            .Where(x => x.DangKyXayDungVanBanId == id && !x.IsDeleted)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new DangKyXayDungVanBanFileDto(
                x.Id,
                x.DangKyXayDungVanBanId,
                x.LoaiFile,
                x.TenFile,
                x.DuongDanFile,
                x.DungLuong,
                x.MimeType,
                x.MoTa,
                x.CreatedAt,
                x.CreatedBy))
            .ToListAsync(cancellationToken);
    }

    public async Task<DangKyXayDungVanBanFileDto?> UploadFileAsync(Guid id, TaiFileDangKyXayDungVanBanRequest request, CancellationToken cancellationToken)
    {
        var hoSo = await ApplyDataScope(_dbContext.DangKyXayDungVanBans.AsNoTracking().Where(x => !x.IsDeleted))
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (hoSo is null)
        {
            return null;
        }

        if (!CanModifyDraft(hoSo))
        {
            throw new UnauthorizedAccessException("Nguoi dung khong duoc tai file len ho so nay.");
        }

        if (request.NoiDung.Length == 0)
        {
            throw new InvalidOperationException("File tải lên không có nội dung.");
        }

        var safeFileName = BuildSafeFileName(request.TenFile);
        var relativeDirectory = Path.Combine("uploads", "dang-ky-xay-dung-van-ban", id.ToString("N"));
        var storageRoot = Path.Combine(_environment.ContentRootPath, relativeDirectory);
        Directory.CreateDirectory(storageRoot);

        var storageFileName = $"{DateTime.UtcNow:yyyyMMddHHmmssfff}_{safeFileName}";
        var physicalPath = Path.Combine(storageRoot, storageFileName);

        await using (var output = System.IO.File.Create(physicalPath))
        {
            await request.NoiDung.CopyToAsync(output, cancellationToken);
        }

        var relativePath = Path.Combine(relativeDirectory, storageFileName).Replace('\\', '/');
        var entity = new DangKyXayDungVanBanFile
        {
            DangKyXayDungVanBanId = id,
            LoaiFile = request.LoaiFile,
            TenFile = request.TenFile,
            DuongDanFile = relativePath,
            DungLuong = request.NoiDung.Length,
            MimeType = request.MimeType,
            MoTa = request.MoTa,
            CreatedBy = CurrentUserIdText()
        };

        _dbContext.DangKyXayDungVanBanFiles.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new DangKyXayDungVanBanFileDto(
            entity.Id,
            entity.DangKyXayDungVanBanId,
            entity.LoaiFile,
            entity.TenFile,
            entity.DuongDanFile,
            entity.DungLuong,
            entity.MimeType,
            entity.MoTa,
            entity.CreatedAt,
            entity.CreatedBy);
    }

    public async Task<bool> DeleteFileAsync(Guid id, Guid fileId, CancellationToken cancellationToken)
    {
        var hoSo = await ApplyDataScope(_dbContext.DangKyXayDungVanBans.AsNoTracking().Where(x => !x.IsDeleted))
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (hoSo is null)
        {
            return false;
        }

        if (!CanModifyDraft(hoSo))
        {
            throw new UnauthorizedAccessException("Nguoi dung khong duoc xoa file cua ho so nay.");
        }

        var file = await _dbContext.DangKyXayDungVanBanFiles
            .FirstOrDefaultAsync(x => x.Id == fileId && x.DangKyXayDungVanBanId == id && !x.IsDeleted, cancellationToken);

        if (file is null)
        {
            return false;
        }

        file.IsDeleted = true;
        file.UpdatedAt = DateTime.UtcNow;
        file.UpdatedBy = CurrentUserIdText();

        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<DangKyXayDungVanBanDto?> XuLyAsync(Guid id, XuLyDangKyXayDungVanBanRequest request, CancellationToken cancellationToken)
    {
        var hoSo = await ApplyDataScope(_dbContext.DangKyXayDungVanBans.Where(x => !x.IsDeleted))
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (hoSo is null)
        {
            return null;
        }

        if (!CanPerformAction(hoSo, request.HanhDongId))
        {
            throw new UnauthorizedAccessException("Nguoi dung khong duoc thuc hien hanh dong nay tren ho so.");
        }

        var cauHinh = await _dbContext.DangKyCauHinhChuyenTrangThais
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.TrangThai
                && x.QuyTrinhSoanThaoId == hoSo.QuyTrinhSoanThaoId
                && x.BuocHienTaiId == hoSo.BuocHienTaiId
                && x.TrangThaiHienTaiId == hoSo.TrangThaiHoSoId
                && x.HanhDongId == request.HanhDongId,
                cancellationToken);

        if (cauHinh is null)
        {
            throw new InvalidOperationException("Hanh dong khong hop le voi buoc va trang thai hien tai cua ho so.");
        }

        var hanhDong = await _dbContext.DangKyHanhDongXuLys
            .AsNoTracking()
            .FirstAsync(x => x.Id == request.HanhDongId, cancellationToken);

        if ((cauHinh.YeuCauLyDo || hanhDong.YeuCauLyDo) && string.IsNullOrWhiteSpace(request.LyDoTraLai))
        {
            throw new InvalidOperationException("Hanh dong nay bat buoc nhap ly do.");
        }

        var buocTruoc = hoSo.BuocHienTaiId;
        var trangThaiTruoc = hoSo.TrangThaiHoSoId;

        hoSo.BuocHienTaiId = cauHinh.BuocTiepTheoId;
        hoSo.TrangThaiHoSoId = cauHinh.TrangThaiTiepTheoId;
        hoSo.UpdatedAt = DateTime.UtcNow;
        hoSo.UpdatedBy = CurrentUserIdText();

        _dbContext.DangKyXayDungVanBanLichSuXuLys.Add(new DangKyXayDungVanBanLichSuXuLy
        {
            DangKyXayDungVanBanId = hoSo.Id,
            TuBuocId = buocTruoc,
            DenBuocId = cauHinh.BuocTiepTheoId,
            ChuyenBuocId = cauHinh.ChuyenBuocId,
            TenBuocTu = request.TenBuocTu,
            TenBuocDen = request.TenBuocDen,
            HanhDongId = hanhDong.Id,
            MaHanhDongSnapshot = hanhDong.MaHanhDong,
            TenHanhDongSnapshot = hanhDong.TenHanhDong,
            TrangThaiTruocId = trangThaiTruoc,
            TrangThaiSauId = cauHinh.TrangThaiTiepTheoId,
            TenTrangThaiTruocSnapshot = request.TenTrangThaiTruoc,
            TenTrangThaiSauSnapshot = request.TenTrangThaiSau,
            NoiDungXuLy = request.NoiDungXuLy,
            LyDoTraLai = request.LyDoTraLai,
            NguoiXuLyId = CurrentUserId(),
            TenNguoiXuLy = ResolveUserDisplayName(request.TenNguoiXuLy),
            DonViXuLyId = CurrentDonViIdOrRequest(request.DonViXuLyId),
            TenDonViXuLy = request.TenDonViXuLy,
            CreatedBy = CurrentUserIdText()
        });

        await _dbContext.SaveChangesAsync(cancellationToken);

        return ToDto(hoSo);
    }

    public async Task<Guid?> CapNhatKetQuaPheDuyetAsync(Guid id, CapNhatKetQuaPheDuyetRequest request, CancellationToken cancellationToken)
    {
        var hoSo = await ApplyDataScope(_dbContext.DangKyXayDungVanBans.Where(x => !x.IsDeleted))
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (hoSo is null)
        {
            return null;
        }

        if (!CanPerformAction(hoSo, DangKySeedIds.HanhDong.CapNhatKetQua))
        {
            throw new UnauthorizedAccessException("Nguoi dung khong duoc cap nhat ket qua phe duyet cho ho so nay.");
        }

        var ketQua = new DangKyXayDungVanBanKetQuaPheDuyet
        {
            DangKyXayDungVanBanId = hoSo.Id,
            KetQua = request.KetQua,
            SoVanBan = request.SoVanBan,
            NgayVanBan = request.NgayVanBan,
            CoQuanPheDuyetId = request.CoQuanPheDuyetId,
            TenCoQuanPheDuyet = request.TenCoQuanPheDuyet,
            NguoiKy = request.NguoiKy,
            ChucVuNguoiKy = request.ChucVuNguoiKy,
            NoiDungKetQua = request.NoiDungKetQua,
            FileKetQuaId = request.FileKetQuaId
        };

        _dbContext.DangKyXayDungVanBanKetQuaPheDuyets.Add(ketQua);
        hoSo.KetQuaPheDuyetId = ketQua.Id;
        hoSo.TrangThaiHoSoId = DangKySeedIds.TrangThai.DaCapNhatKetQua;
        hoSo.BuocHienTaiId = DangKySeedIds.DanhMucBuocDangKyXayDungQppl.CapNhatKetQua;
        hoSo.UpdatedAt = DateTime.UtcNow;
        hoSo.UpdatedBy = CurrentUserIdText();

        await _dbContext.SaveChangesAsync(cancellationToken);

        return ketQua.Id;
    }

    public async Task<DangKyXayDungVanBanDto?> KhoiTaoQuyTrinhXayDungAsync(Guid id, KhoiTaoQuyTrinhXayDungRequest request, CancellationToken cancellationToken)
    {
        var hoSo = await ApplyDataScope(_dbContext.DangKyXayDungVanBans.Where(x => !x.IsDeleted))
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (hoSo is null)
        {
            return null;
        }

        if (!CanPerformAction(hoSo, DangKySeedIds.HanhDong.KhoiTaoQuyTrinhXayDung))
        {
            throw new UnauthorizedAccessException("Nguoi dung khong duoc khoi tao quy trinh xay dung tu ho so nay.");
        }

        if (hoSo.DaKhoiTaoQuyTrinhXayDung)
        {
            throw new InvalidOperationException("Ho so da khoi tao quy trinh xay dung van ban.");
        }

        var trangThaiTruoc = hoSo.TrangThaiHoSoId;

        hoSo.DaKhoiTaoQuyTrinhXayDung = true;
        hoSo.HoSoXayDungVanBanId = request.HoSoXayDungVanBanId;
        hoSo.QuyTrinhXayDungTiepTheoId = request.QuyTrinhXayDungId;
        hoSo.NgayKhoiTaoQuyTrinhXayDung = DateTime.UtcNow;
        hoSo.TrangThaiHoSoId = DangKySeedIds.TrangThai.DaChuyenQuyTrinhXayDung;
        hoSo.UpdatedAt = DateTime.UtcNow;
        hoSo.UpdatedBy = CurrentUserIdText();

        _dbContext.DangKyXayDungVanBanLienKetQuyTrinhs.Add(new DangKyXayDungVanBanLienKetQuyTrinh
        {
            DangKyXayDungVanBanId = hoSo.Id,
            HoSoXayDungVanBanId = request.HoSoXayDungVanBanId,
            LoaiVanBanId = hoSo.LoaiVanBanId,
            QuyTrinhXayDungId = request.QuyTrinhXayDungId,
            MaQuyTrinhXayDung = request.MaQuyTrinhXayDung,
            TenQuyTrinhXayDung = request.TenQuyTrinhXayDung,
            CreatedBy = CurrentUserIdText()
        });

        _dbContext.DangKyXayDungVanBanLichSuXuLys.Add(new DangKyXayDungVanBanLichSuXuLy
        {
            DangKyXayDungVanBanId = hoSo.Id,
            HanhDongId = DangKySeedIds.HanhDong.KhoiTaoQuyTrinhXayDung,
            MaHanhDongSnapshot = KhoiTaoQuyTrinhXayDungActionCode,
            TenHanhDongSnapshot = KhoiTaoQuyTrinhXayDungActionName,
            TrangThaiTruocId = trangThaiTruoc,
            TrangThaiSauId = hoSo.TrangThaiHoSoId,
            NoiDungXuLy = "Khoi tao quy trinh xay dung van ban tu ho so dang ky",
            NguoiXuLyId = CurrentUserId(),
            TenNguoiXuLy = ResolveUserDisplayName(request.TenNguoiXuLy),
            DonViXuLyId = CurrentDonViIdOrRequest(request.DonViXuLyId),
            TenDonViXuLy = request.TenDonViXuLy,
            CreatedBy = CurrentUserIdText()
        });

        await _dbContext.SaveChangesAsync(cancellationToken);

        return ToDto(hoSo);
    }

    private async Task<string> GenerateMaHoSoAsync(int namDangKy, CancellationToken cancellationToken)
    {
        var prefix = $"DKXDVBan-{namDangKy}-";
        var count = await _dbContext.DangKyXayDungVanBans
            .CountAsync(x => x.NamDangKy == namDangKy, cancellationToken);

        return $"{prefix}{count + 1:00000}";
    }

    private IQueryable<DangKyXayDungVanBan> ApplyDataScope(IQueryable<DangKyXayDungVanBan> query)
    {
        if (_currentUserContext.IsSSA)
        {
            return query;
        }

        if (!_currentUserContext.IsAuthenticated || _currentUserContext.UserId is null)
        {
            return query.Where(_ => false);
        }

        var userIdText = _currentUserContext.UserId.Value.ToString();
        if (_currentUserContext.DonViId is null)
        {
            return query.Where(x => x.CreatedBy == userIdText);
        }

        var donViId = _currentUserContext.DonViId.Value;
        return query.Where(x =>
            x.DonViSoanThaoId == donViId
            || x.DonViPheDuyetId == donViId
            || x.CreatedBy == userIdText);
    }

    private Task<bool> CanAccessHoSoAsync(Guid id, CancellationToken cancellationToken)
    {
        return ApplyDataScope(_dbContext.DangKyXayDungVanBans.AsNoTracking().Where(x => !x.IsDeleted))
            .AnyAsync(x => x.Id == id, cancellationToken);
    }

    private bool CanModifyDraft(DangKyXayDungVanBan hoSo)
    {
        return IsDraftEditable(hoSo)
            && (_currentUserContext.IsSSA || IsCreatedByCurrentUser(hoSo) || IsCurrentDonViSoanThao(hoSo));
    }

    private bool CanPerformAction(DangKyXayDungVanBan hoSo, Guid hanhDongId)
    {
        if (_currentUserContext.IsSSA)
        {
            return true;
        }

        if (!_currentUserContext.IsAuthenticated || _currentUserContext.UserId is null)
        {
            return false;
        }

        if (hanhDongId == DangKySeedIds.HanhDong.CapNhatHoSo
            || hanhDongId == DangKySeedIds.HanhDong.TrinhPheDuyet)
        {
            return CanModifyDraft(hoSo);
        }

        if (hanhDongId == DangKySeedIds.HanhDong.PheDuyet
            || hanhDongId == DangKySeedIds.HanhDong.TraLai
            || hanhDongId == DangKySeedIds.HanhDong.KhongPheDuyet)
        {
            return hoSo.TrangThaiHoSoId == DangKySeedIds.TrangThai.DaTrinhPheDuyet
                && IsCurrentDonViPheDuyet(hoSo);
        }

        if (hanhDongId == DangKySeedIds.HanhDong.CapNhatKetQua)
        {
            return hoSo.TrangThaiHoSoId == DangKySeedIds.TrangThai.DaPheDuyet
                && (IsCreatedByCurrentUser(hoSo) || IsCurrentDonViSoanThao(hoSo));
        }

        if (hanhDongId == DangKySeedIds.HanhDong.HoanThanh)
        {
            return hoSo.TrangThaiHoSoId == DangKySeedIds.TrangThai.DaCapNhatKetQua
                && (IsCreatedByCurrentUser(hoSo) || IsCurrentDonViSoanThao(hoSo));
        }

        if (hanhDongId == DangKySeedIds.HanhDong.KhoiTaoQuyTrinhXayDung)
        {
            return hoSo.TrangThaiHoSoId == DangKySeedIds.TrangThai.HoanThanh
                && !hoSo.DaKhoiTaoQuyTrinhXayDung
                && (IsCreatedByCurrentUser(hoSo) || IsCurrentDonViSoanThao(hoSo));
        }

        return IsCreatedByCurrentUser(hoSo)
            || IsCurrentDonViSoanThao(hoSo)
            || IsCurrentDonViPheDuyet(hoSo);
    }

    private static bool IsDraftEditable(DangKyXayDungVanBan hoSo)
    {
        return hoSo.TrangThaiHoSoId == DangKySeedIds.TrangThai.DangSoanThao
            || hoSo.TrangThaiHoSoId == DangKySeedIds.TrangThai.BiTraLai;
    }

    private bool IsCreatedByCurrentUser(DangKyXayDungVanBan hoSo)
    {
        return _currentUserContext.UserId is not null
            && hoSo.CreatedBy == _currentUserContext.UserId.Value.ToString();
    }

    private bool IsCurrentDonViSoanThao(DangKyXayDungVanBan hoSo)
    {
        return _currentUserContext.DonViId is not null
            && hoSo.DonViSoanThaoId == _currentUserContext.DonViId.Value;
    }

    private bool IsCurrentDonViPheDuyet(DangKyXayDungVanBan hoSo)
    {
        return _currentUserContext.DonViId is not null
            && hoSo.DonViPheDuyetId == _currentUserContext.DonViId.Value;
    }

    private CurrentUserInfo RequireCurrentUser()
    {
        if (!_currentUserContext.IsAuthenticated || _currentUserContext.UserId is null)
        {
            throw new UnauthorizedAccessException("Chua xac dinh duoc nguoi dung hien tai.");
        }

        return new CurrentUserInfo(
            _currentUserContext.UserId.Value,
            _currentUserContext.DonViId,
            _currentUserContext.IsSSA);
    }

    private Guid CurrentUserId()
    {
        return RequireCurrentUser().UserId;
    }

    private string CurrentUserIdText()
    {
        return CurrentUserId().ToString();
    }

    private Guid CurrentDonViIdOrRequest(Guid requestDonViId)
    {
        return _currentUserContext.DonViId ?? requestDonViId;
    }

    private string ResolveUserDisplayName(string? requestName)
    {
        return _currentUserContext.Username
            ?? requestName
            ?? CurrentUserIdText();
    }

    private static IQueryable<DangKyXayDungVanBan> ApplyListFilters(
        IQueryable<DangKyXayDungVanBan> query,
        DangKyXayDungVanBanListRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var keyword = request.Search.Trim();
            query = query.Where(x =>
                x.MaHoSo.Contains(keyword)
                || x.TenHoSo.Contains(keyword)
                || x.TenVanBanDuKien.Contains(keyword));
        }

        if (request.LoaiVanBanId.HasValue)
        {
            query = query.Where(x => x.LoaiVanBanId == request.LoaiVanBanId.Value);
        }

        if (request.BuocHienTaiId.HasValue)
        {
            query = query.Where(x => x.BuocHienTaiId == request.BuocHienTaiId.Value);
        }

        if (request.TrangThaiHoSoId.HasValue)
        {
            query = query.Where(x => x.TrangThaiHoSoId == request.TrangThaiHoSoId.Value);
        }

        if (request.DonViId.HasValue)
        {
            query = query.Where(x =>
                x.DonViSoanThaoId == request.DonViId.Value
                || x.DonViPheDuyetId == request.DonViId.Value);
        }

        if (request.DonViSoanThaoId.HasValue)
        {
            query = query.Where(x => x.DonViSoanThaoId == request.DonViSoanThaoId.Value);
        }

        if (request.DonViPheDuyetId.HasValue)
        {
            query = query.Where(x => x.DonViPheDuyetId == request.DonViPheDuyetId.Value);
        }

        if (request.NamDangKy.HasValue)
        {
            query = query.Where(x => x.NamDangKy == request.NamDangKy.Value);
        }

        return query;
    }

    private static string BuildSafeFileName(string fileName)
    {
        var name = Path.GetFileName(fileName);
        foreach (var invalidChar in Path.GetInvalidFileNameChars())
        {
            name = name.Replace(invalidChar, '_');
        }

        return string.IsNullOrWhiteSpace(name) ? "file" : name;
    }

    private static string? GetMaBuocHienTai(Guid buocId)
    {
        if (buocId == DangKySeedIds.DanhMucBuocDangKyXayDungQppl.LapHoSo) return "LAP_HO_SO";
        if (buocId == DangKySeedIds.DanhMucBuocDangKyXayDungQppl.TrinhHoSo) return "TRINH_HO_SO";
        if (buocId == DangKySeedIds.DanhMucBuocDangKyXayDungQppl.PheDuyet) return "PHE_DUYET";
        if (buocId == DangKySeedIds.DanhMucBuocDangKyXayDungQppl.CapNhatKetQua) return "CAP_NHAT_KET_QUA";
        if (buocId == DangKySeedIds.DanhMucBuocDangKyXayDungQppl.HoanThanh) return "HOAN_THANH";
        return null;
    }

    private static string? GetTenBuocHienTai(Guid buocId)
    {
        if (buocId == DangKySeedIds.DanhMucBuocDangKyXayDungQppl.LapHoSo) return "Lập hồ sơ đề nghị/đăng ký";
        if (buocId == DangKySeedIds.DanhMucBuocDangKyXayDungQppl.TrinhHoSo) return "Trình hồ sơ";
        if (buocId == DangKySeedIds.DanhMucBuocDangKyXayDungQppl.PheDuyet) return "Phê duyệt";
        if (buocId == DangKySeedIds.DanhMucBuocDangKyXayDungQppl.CapNhatKetQua) return "Cập nhật kết quả";
        if (buocId == DangKySeedIds.DanhMucBuocDangKyXayDungQppl.HoanThanh) return "Hoàn thành";
        return null;
    }

    private static DangKyXayDungVanBanDto ToDto(DangKyXayDungVanBan entity)
    {
        return new DangKyXayDungVanBanDto(
            entity.Id,
            entity.MaHoSo,
            entity.TenHoSo,
            entity.TenVanBanDuKien,
            entity.LoaiVanBanId,
            entity.QuyTrinhSoanThaoId,
            entity.BuocHienTaiId,
            entity.TrangThaiHoSoId,
            entity.DonViSoanThaoId,
            entity.DonViPheDuyetId,
            entity.NamDangKy,
            entity.DaKhoiTaoQuyTrinhXayDung,
            entity.HoSoXayDungVanBanId,
            entity.CreatedAt,
            null,
            null,
            null,
            GetMaBuocHienTai(entity.BuocHienTaiId),
            GetTenBuocHienTai(entity.BuocHienTaiId));
    }

    private sealed record CurrentUserInfo(Guid UserId, Guid? DonViId, bool IsSSA);
}
