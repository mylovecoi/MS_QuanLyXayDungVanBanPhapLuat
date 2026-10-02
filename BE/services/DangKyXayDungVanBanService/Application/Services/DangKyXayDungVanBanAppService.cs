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

    public DangKyXayDungVanBanAppService(DangKyXayDungVanBanDbContext dbContext, IWebHostEnvironment environment)
    {
        _dbContext = dbContext;
        _environment = environment;
    }

    public async Task<IReadOnlyList<DangKyXayDungVanBanDto>> GetListAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.DangKyXayDungVanBans
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new DangKyXayDungVanBanDto(
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
                x.CreatedAt))
            .ToListAsync(cancellationToken);
    }

    public async Task<DangKyXayDungVanBanDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.DangKyXayDungVanBans
            .AsNoTracking()
            .Where(x => x.Id == id && !x.IsDeleted)
            .Select(x => new DangKyXayDungVanBanDto(
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
                x.CreatedAt))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<DangKyXayDungVanBanDto> CreateAsync(TaoDangKyXayDungVanBanRequest request, CancellationToken cancellationToken)
    {
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
            CreatedBy = request.NguoiXuLyId.ToString()
        };

        entity.LichSuXuLys.Add(new DangKyXayDungVanBanLichSuXuLy
        {
            DenBuocId = DangKySeedIds.DanhMucBuocDangKyXayDungQppl.LapHoSo,
            HanhDongId = DangKySeedIds.HanhDong.TaoMoi,
            MaHanhDongSnapshot = TaoMoiActionCode,
            TenHanhDongSnapshot = TaoMoiActionName,
            TrangThaiSauId = DangKySeedIds.TrangThai.DangSoanThao,
            NoiDungXuLy = "Tao moi ho so dang ky xay dung van ban",
            NguoiXuLyId = request.NguoiXuLyId,
            TenNguoiXuLy = request.TenNguoiXuLy,
            DonViXuLyId = request.DonViSoanThaoId,
            TenDonViXuLy = request.TenDonViXuLy,
            CreatedBy = request.NguoiXuLyId.ToString()
        });

        _dbContext.DangKyXayDungVanBans.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return ToDto(entity);
    }

    public async Task<DangKyXayDungVanBanDto?> UpdateAsync(Guid id, CapNhatDangKyXayDungVanBanRequest request, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.DangKyXayDungVanBans
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);

        if (entity is null)
        {
            return null;
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

        await _dbContext.SaveChangesAsync(cancellationToken);

        return ToDto(entity);
    }

    public async Task<IReadOnlyList<DangKyXayDungVanBanTimelineDto>> GetTimelineAsync(Guid id, CancellationToken cancellationToken)
    {
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
        var hoSo = await _dbContext.DangKyXayDungVanBans
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);

        if (hoSo is null)
        {
            return Array.Empty<HanhDongKhaDungDto>();
        }

        return await (
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
    }

    public async Task<IReadOnlyList<DangKyXayDungVanBanFileDto>?> GetFilesAsync(Guid id, CancellationToken cancellationToken)
    {
        var exists = await _dbContext.DangKyXayDungVanBans
            .AsNoTracking()
            .AnyAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);

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
        var exists = await _dbContext.DangKyXayDungVanBans
            .AsNoTracking()
            .AnyAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);

        if (!exists)
        {
            return null;
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
            CreatedBy = request.NguoiTaiLenId.ToString()
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

    public async Task<bool> DeleteFileAsync(Guid id, Guid fileId, Guid nguoiXoaId, CancellationToken cancellationToken)
    {
        var file = await _dbContext.DangKyXayDungVanBanFiles
            .FirstOrDefaultAsync(x => x.Id == fileId && x.DangKyXayDungVanBanId == id && !x.IsDeleted, cancellationToken);

        if (file is null)
        {
            return false;
        }

        file.IsDeleted = true;
        file.UpdatedAt = DateTime.UtcNow;
        file.UpdatedBy = nguoiXoaId.ToString();

        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<DangKyXayDungVanBanDto?> XuLyAsync(Guid id, XuLyDangKyXayDungVanBanRequest request, CancellationToken cancellationToken)
    {
        var hoSo = await _dbContext.DangKyXayDungVanBans
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);

        if (hoSo is null)
        {
            return null;
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
        hoSo.UpdatedBy = request.NguoiXuLyId.ToString();

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
            NguoiXuLyId = request.NguoiXuLyId,
            TenNguoiXuLy = request.TenNguoiXuLy,
            DonViXuLyId = request.DonViXuLyId,
            TenDonViXuLy = request.TenDonViXuLy,
            CreatedBy = request.NguoiXuLyId.ToString()
        });

        await _dbContext.SaveChangesAsync(cancellationToken);

        return ToDto(hoSo);
    }

    public async Task<Guid?> CapNhatKetQuaPheDuyetAsync(Guid id, CapNhatKetQuaPheDuyetRequest request, CancellationToken cancellationToken)
    {
        var hoSo = await _dbContext.DangKyXayDungVanBans
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);

        if (hoSo is null)
        {
            return null;
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
        hoSo.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return ketQua.Id;
    }

    public async Task<DangKyXayDungVanBanDto?> KhoiTaoQuyTrinhXayDungAsync(Guid id, KhoiTaoQuyTrinhXayDungRequest request, CancellationToken cancellationToken)
    {
        var hoSo = await _dbContext.DangKyXayDungVanBans
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);

        if (hoSo is null)
        {
            return null;
        }

        if (hoSo.DaKhoiTaoQuyTrinhXayDung)
        {
            throw new InvalidOperationException("Ho so da khoi tao quy trinh xay dung van ban.");
        }

        hoSo.DaKhoiTaoQuyTrinhXayDung = true;
        hoSo.HoSoXayDungVanBanId = request.HoSoXayDungVanBanId;
        hoSo.QuyTrinhXayDungTiepTheoId = request.QuyTrinhXayDungId;
        hoSo.NgayKhoiTaoQuyTrinhXayDung = DateTime.UtcNow;
        hoSo.UpdatedAt = DateTime.UtcNow;
        hoSo.UpdatedBy = request.NguoiXuLyId.ToString();

        _dbContext.DangKyXayDungVanBanLienKetQuyTrinhs.Add(new DangKyXayDungVanBanLienKetQuyTrinh
        {
            DangKyXayDungVanBanId = hoSo.Id,
            HoSoXayDungVanBanId = request.HoSoXayDungVanBanId,
            LoaiVanBanId = hoSo.LoaiVanBanId,
            QuyTrinhXayDungId = request.QuyTrinhXayDungId,
            MaQuyTrinhXayDung = request.MaQuyTrinhXayDung,
            TenQuyTrinhXayDung = request.TenQuyTrinhXayDung,
            CreatedBy = request.NguoiXuLyId.ToString()
        });

        _dbContext.DangKyXayDungVanBanLichSuXuLys.Add(new DangKyXayDungVanBanLichSuXuLy
        {
            DangKyXayDungVanBanId = hoSo.Id,
            HanhDongId = Guid.Empty,
            MaHanhDongSnapshot = KhoiTaoQuyTrinhXayDungActionCode,
            TenHanhDongSnapshot = KhoiTaoQuyTrinhXayDungActionName,
            TrangThaiTruocId = hoSo.TrangThaiHoSoId,
            TrangThaiSauId = hoSo.TrangThaiHoSoId,
            NoiDungXuLy = "Khoi tao quy trinh xay dung van ban tu ho so dang ky",
            NguoiXuLyId = request.NguoiXuLyId,
            TenNguoiXuLy = request.TenNguoiXuLy,
            DonViXuLyId = request.DonViXuLyId,
            TenDonViXuLy = request.TenDonViXuLy,
            CreatedBy = request.NguoiXuLyId.ToString()
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

    private static string BuildSafeFileName(string fileName)
    {
        var name = Path.GetFileName(fileName);
        foreach (var invalidChar in Path.GetInvalidFileNameChars())
        {
            name = name.Replace(invalidChar, '_');
        }

        return string.IsNullOrWhiteSpace(name) ? "file" : name;
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
            entity.CreatedAt);
    }
}
