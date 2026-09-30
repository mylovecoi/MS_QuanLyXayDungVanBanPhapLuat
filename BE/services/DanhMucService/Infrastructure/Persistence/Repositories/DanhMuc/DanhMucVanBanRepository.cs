using DanhMucService.Domain.Entities.DanhMuc;
using DanhMucService.Domain.Interfaces.Repositories;
using DanhMucService.Infrastructure.Persistence;
using DanhMucService.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace DanhMucService.Infrastructure.Persistence.Repositories.DanhMuc;

public class DanhMucVanBanRepository(DanhMucDbContext dbContext) : IDanhMucVanBanRepository
{
    private readonly DanhMucDbContext _dbContext = dbContext;

    public async Task<(IReadOnlyList<DanhMucVanBanEntity> Items, int TotalCount)> GetPagedAsync(string? search, int pageSize, int pageCurrent, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.DanhMucVanBans.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x =>
                x.TenLoaiVanBan.Contains(search) ||
                x.CapChinhQuyen.Contains(search) ||
                x.ChuTheBanHanh.Contains(search) ||
                (x.KyHieuMau != null && x.KyHieuMau.Contains(search)) ||
                (x.MoTa != null && x.MoTa.Contains(search)) ||
                (x.GhiChu != null && x.GhiChu.Contains(search)));
        }

        query = query.OrderBy(x => x.ThuTuSapXep).ThenBy(x => x.TenLoaiVanBan);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pageCurrent - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new DanhMucVanBanEntity
            {
                Id = x.Id,
                TenLoaiVanBan = x.TenLoaiVanBan,
                CapChinhQuyen = x.CapChinhQuyen,
                ChuTheBanHanh = x.ChuTheBanHanh,
                KyHieuMau = x.KyHieuMau,
                ThuTuSapXep = x.ThuTuSapXep,
                TrangThai = x.TrangThai,
                MoTa = x.MoTa,
                GhiChu = x.GhiChu
            })
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<DanhMucVanBanEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.DanhMucVanBans
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new DanhMucVanBanEntity
            {
                Id = x.Id,
                TenLoaiVanBan = x.TenLoaiVanBan,
                CapChinhQuyen = x.CapChinhQuyen,
                ChuTheBanHanh = x.ChuTheBanHanh,
                KyHieuMau = x.KyHieuMau,
                ThuTuSapXep = x.ThuTuSapXep,
                TrangThai = x.TrangThai,
                MoTa = x.MoTa,
                GhiChu = x.GhiChu
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<bool> ExistsByNameAsync(string tenLoaiVanBan, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        return _dbContext.DanhMucVanBans.AnyAsync(
            x => x.TenLoaiVanBan == tenLoaiVanBan && (!excludeId.HasValue || x.Id != excludeId.Value),
            cancellationToken);
    }

    public async Task<int> GetNextSortOrderAsync(CancellationToken cancellationToken = default)
    {
        var maxValue = await _dbContext.DanhMucVanBans
            .AsNoTracking()
            .Select(x => (int?)x.ThuTuSapXep)
            .MaxAsync(cancellationToken);

        return (maxValue ?? 0) + 1;
    }

    public async Task<DanhMucVanBanEntity> AddAsync(DanhMucVanBanEntity entity, CancellationToken cancellationToken = default)
    {
        var dbEntity = new DanhMucVanBan
        {
            TenLoaiVanBan = entity.TenLoaiVanBan,
            CapChinhQuyen = entity.CapChinhQuyen,
            ChuTheBanHanh = entity.ChuTheBanHanh,
            KyHieuMau = entity.KyHieuMau,
            ThuTuSapXep = entity.ThuTuSapXep,
            TrangThai = entity.TrangThai,
            MoTa = entity.MoTa,
            GhiChu = entity.GhiChu
        };

        _dbContext.DanhMucVanBans.Add(dbEntity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        entity.Id = dbEntity.Id;
        return entity;
    }

    public async Task<DanhMucVanBanEntity> UpdateAsync(DanhMucVanBanEntity entity, CancellationToken cancellationToken = default)
    {
        var dbEntity = await _dbContext.DanhMucVanBans.FirstAsync(x => x.Id == entity.Id, cancellationToken);
        dbEntity.TenLoaiVanBan = entity.TenLoaiVanBan;
        dbEntity.CapChinhQuyen = entity.CapChinhQuyen;
        dbEntity.ChuTheBanHanh = entity.ChuTheBanHanh;
        dbEntity.KyHieuMau = entity.KyHieuMau;
        dbEntity.ThuTuSapXep = entity.ThuTuSapXep;
        dbEntity.TrangThai = entity.TrangThai;
        dbEntity.MoTa = entity.MoTa;
        dbEntity.GhiChu = entity.GhiChu;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var dbEntity = await _dbContext.DanhMucVanBans.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (dbEntity == null)
        {
            return false;
        }

        _dbContext.DanhMucVanBans.Remove(dbEntity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}

