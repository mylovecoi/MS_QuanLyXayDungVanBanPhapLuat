using DanhMucService.Domain.Entities.DanhMuc;
using DanhMucService.Domain.Enums;
using DanhMucService.Domain.Interfaces.Repositories;
using DanhMucService.Infrastructure.Persistence;
using DanhMucService.Infrastructure.Persistence.Entities;
using DanhMucService.Infrastructure.Persistence.Enums;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DanhMucService.Infrastructure.Persistence.Repositories.DanhMuc;

public class DanhMucCanBoRepository(DanhMucDbContext dbContext) : IDanhMucCanBoRepository
{
    private readonly DanhMucDbContext _dbContext = dbContext;

    public async Task<(IReadOnlyList<DanhMucCanBoEntity> Items, int TotalCount)> GetPagedAsync(
        string? search,
        int pageSize,
        int pageCurrent,
        Guid? donViId,
        Guid? phongBanId,
        LoaiLaoDongType? loaiLaoDong,
        CancellationToken cancellationToken = default)
    {
        var query = BuildBaseQuery();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x =>
                EF.Functions.Like(x.TenCanBo, $"%{search}%") ||
                (x.ChucVu != null && EF.Functions.Like(x.ChucVu, $"%{search}%")) ||
                (x.ViTriViecLam != null && EF.Functions.Like(x.ViTriViecLam, $"%{search}%")));
        }

        if (donViId.HasValue && donViId.Value != Guid.Empty)
        {
            query = query.Where(x => x.DonViQuanLyId == donViId.Value);
        }

        if (phongBanId.HasValue && phongBanId.Value != Guid.Empty)
        {
            query = query.Where(x => x.PhongBanId == phongBanId.Value);
        }

        if (loaiLaoDong.HasValue)
        {
            query = query.Where(x => x.LoaiLaoDong == (LoaiLaoDong)loaiLaoDong.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(x => x.UpdatedDate)
            .Skip((pageCurrent - 1) * pageSize)
            .Take(pageSize)
            .Select(MapProjection())
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<IReadOnlyList<DanhMucCanBoEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await BuildBaseQuery()
            .OrderBy(x => x.TenCanBo)
            .Select(MapProjection())
            .ToListAsync(cancellationToken);
    }

    public async Task<DanhMucCanBoEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await BuildBaseQuery()
            .Where(x => x.Id == id)
            .Select(MapProjection())
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<bool> IsPhongBanThuocDonViAsync(Guid phongBanId, Guid donViId, CancellationToken cancellationToken = default)
    {
        return _dbContext.DanhMucPhongBans.AnyAsync(
            x => x.Id == phongBanId && x.DanhMucDonViId == donViId,
            cancellationToken);
    }

    public async Task AddAsync(DanhMucCanBoEntity entity, CancellationToken cancellationToken = default)
    {
        var dataEntity = new DanhMucCanBo
        {
            Id = entity.Id == Guid.Empty ? Guid.NewGuid() : entity.Id,
            DonViQuanLyId = entity.DonViQuanLyId,
            TenCanBo = entity.TenCanBo,
            NgaySinh = entity.NgaySinh,
            UserId = entity.UserId,
            PhongBanId = entity.PhongBanId,
            GioiTinh = entity.GioiTinh,
            TrinhDoChuyenMon = entity.TrinhDoChuyenMon,
            LoaiLaoDong = (LoaiLaoDong)entity.LoaiLaoDong,
            SoTienBHXH = entity.SoTienBHXH,
            SoTienBHYT = entity.SoTienBHYT,
            SoQuyetDinhDung = entity.SoQuyetDinhDung,
            NgayQuyetDinhDung = entity.NgayQuyetDinhDung,
            GhiChu = entity.GhiChu,
            SoQuyetDinhBoNhiem = entity.SoQuyetDinhBoNhiem,
            NgayQuyetDinhBoNhiem = entity.NgayQuyetDinhBoNhiem,
            SoQuyetDinhCapThe = entity.SoQuyetDinhCapThe,
            NgayQuyetDinhCapThe = entity.NgayQuyetDinhCapThe,
            SoTheCongChungVien = entity.SoTheCongChungVien,
            ChucVu = entity.ChucVu,
            MucPhiBaoHiemTrachNhiem = entity.MucPhiBaoHiemTrachNhiem,
            ViTriViecLam = entity.ViTriViecLam,
            NgayTuyenDung = entity.NgayTuyenDung,
            SoHopDongLaoDong = entity.SoHopDongLaoDong,
            NgayKyHopDongLaoDong = entity.NgayKyHopDongLaoDong
        };

        _dbContext.DanhMucCanBos.Add(dataEntity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        entity.Id = dataEntity.Id;
    }

    public async Task UpdateAsync(DanhMucCanBoEntity entity, CancellationToken cancellationToken = default)
    {
        var dataEntity = await _dbContext.DanhMucCanBos.FirstAsync(x => x.Id == entity.Id, cancellationToken);
        dataEntity.DonViQuanLyId = entity.DonViQuanLyId;
        dataEntity.TenCanBo = entity.TenCanBo;
        dataEntity.NgaySinh = entity.NgaySinh;
        dataEntity.UserId = entity.UserId;
        dataEntity.PhongBanId = entity.PhongBanId;
        dataEntity.GioiTinh = entity.GioiTinh;
        dataEntity.TrinhDoChuyenMon = entity.TrinhDoChuyenMon;
        dataEntity.LoaiLaoDong = (LoaiLaoDong)entity.LoaiLaoDong;
        dataEntity.SoTienBHXH = entity.SoTienBHXH;
        dataEntity.SoTienBHYT = entity.SoTienBHYT;
        dataEntity.SoQuyetDinhDung = entity.SoQuyetDinhDung;
        dataEntity.NgayQuyetDinhDung = entity.NgayQuyetDinhDung;
        dataEntity.GhiChu = entity.GhiChu;
        dataEntity.SoQuyetDinhBoNhiem = entity.SoQuyetDinhBoNhiem;
        dataEntity.NgayQuyetDinhBoNhiem = entity.NgayQuyetDinhBoNhiem;
        dataEntity.SoQuyetDinhCapThe = entity.SoQuyetDinhCapThe;
        dataEntity.NgayQuyetDinhCapThe = entity.NgayQuyetDinhCapThe;
        dataEntity.SoTheCongChungVien = entity.SoTheCongChungVien;
        dataEntity.ChucVu = entity.ChucVu;
        dataEntity.MucPhiBaoHiemTrachNhiem = entity.MucPhiBaoHiemTrachNhiem;
        dataEntity.ViTriViecLam = entity.ViTriViecLam;
        dataEntity.NgayTuyenDung = entity.NgayTuyenDung;
        dataEntity.SoHopDongLaoDong = entity.SoHopDongLaoDong;
        dataEntity.NgayKyHopDongLaoDong = entity.NgayKyHopDongLaoDong;

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var dataEntity = await _dbContext.DanhMucCanBos.FirstAsync(x => x.Id == id, cancellationToken);
        _dbContext.DanhMucCanBos.Remove(dataEntity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private IQueryable<DanhMucCanBo> BuildBaseQuery()
    {
        return _dbContext.DanhMucCanBos
            .Include(x => x.DonViQuanLy)
            .Include(x => x.PhongBan)
            .AsNoTracking()
            .AsQueryable();
    }

    private static Expression<Func<DanhMucCanBo, DanhMucCanBoEntity>> MapProjection()
    {
        return x => new DanhMucCanBoEntity
        {
            Id = x.Id,
            DonViQuanLyId = x.DonViQuanLyId,
            TenDonViQuanLy = x.DonViQuanLy != null ? x.DonViQuanLy.TenDonVi : null,
            TenCanBo = x.TenCanBo,
            NgaySinh = x.NgaySinh,
            UserId = x.UserId,
            PhongBanId = x.PhongBanId,
            TenPhongBan = x.PhongBan != null ? x.PhongBan.TenPhongBan : null,
            GioiTinh = x.GioiTinh,
            TrinhDoChuyenMon = x.TrinhDoChuyenMon,
            LoaiLaoDong = (LoaiLaoDongType)x.LoaiLaoDong,
            SoTienBHXH = x.SoTienBHXH,
            SoTienBHYT = x.SoTienBHYT,
            SoQuyetDinhDung = x.SoQuyetDinhDung,
            NgayQuyetDinhDung = x.NgayQuyetDinhDung,
            GhiChu = x.GhiChu,
            SoQuyetDinhBoNhiem = x.SoQuyetDinhBoNhiem,
            NgayQuyetDinhBoNhiem = x.NgayQuyetDinhBoNhiem,
            SoQuyetDinhCapThe = x.SoQuyetDinhCapThe,
            NgayQuyetDinhCapThe = x.NgayQuyetDinhCapThe,
            SoTheCongChungVien = x.SoTheCongChungVien,
            ChucVu = x.ChucVu,
            MucPhiBaoHiemTrachNhiem = x.MucPhiBaoHiemTrachNhiem,
            ViTriViecLam = x.ViTriViecLam,
            NgayTuyenDung = x.NgayTuyenDung,
            SoHopDongLaoDong = x.SoHopDongLaoDong,
            NgayKyHopDongLaoDong = x.NgayKyHopDongLaoDong
        };
    }
}

