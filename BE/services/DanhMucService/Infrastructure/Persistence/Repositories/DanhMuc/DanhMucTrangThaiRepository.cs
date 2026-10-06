using DanhMucService.Domain.Entities.DanhMuc;
using DanhMucService.Domain.Interfaces.Repositories;
using DanhMucService.Infrastructure.Persistence;
using DanhMucService.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace DanhMucService.Infrastructure.Persistence.Repositories.DanhMuc;

public class DanhMucTrangThaiRepository(DanhMucDbContext dbContext) : IDanhMucTrangThaiRepository
{
    private readonly DanhMucDbContext _dbContext = dbContext;

    public async Task<(IReadOnlyList<DanhMucTrangThaiEntity> Items, int TotalCount)> GetPagedAsync(
        string? search,
        string? nhomTrangThai,
        int pageSize,
        int pageCurrent,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.DanhMucTrangThais.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(nhomTrangThai))
        {
            query = query.Where(x => x.NhomTrangThai == nhomTrangThai);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x =>
                x.MaTrangThai.Contains(search) ||
                x.TenTrangThai.Contains(search) ||
                x.MaMauHex.Contains(search) ||
                (x.MoTa != null && x.MoTa.Contains(search)) ||
                (x.GhiChu != null && x.GhiChu.Contains(search)));
        }

        query = query.OrderBy(x => x.ThuTuSapXep).ThenBy(x => x.TenTrangThai);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pageCurrent - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new DanhMucTrangThaiEntity
            {
                Id = x.Id,
                NhomTrangThai = x.NhomTrangThai,
                MaTrangThai = x.MaTrangThai,
                TenTrangThai = x.TenTrangThai,
                MaMauHex = x.MaMauHex,
                ThuTuSapXep = x.ThuTuSapXep,
                TrangThai = x.TrangThai,
                MoTa = x.MoTa,
                GhiChu = x.GhiChu
            })
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<DanhMucTrangThaiEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.DanhMucTrangThais
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new DanhMucTrangThaiEntity
            {
                Id = x.Id,
                NhomTrangThai = x.NhomTrangThai,
                MaTrangThai = x.MaTrangThai,
                TenTrangThai = x.TenTrangThai,
                MaMauHex = x.MaMauHex,
                ThuTuSapXep = x.ThuTuSapXep,
                TrangThai = x.TrangThai,
                MoTa = x.MoTa,
                GhiChu = x.GhiChu
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<bool> ExistsByCodeAsync(string nhomTrangThai, string maTrangThai, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        return _dbContext.DanhMucTrangThais.AnyAsync(
            x => x.NhomTrangThai == nhomTrangThai && x.MaTrangThai == maTrangThai && (!excludeId.HasValue || x.Id != excludeId.Value),
            cancellationToken);
    }

    public async Task<int> GetNextSortOrderAsync(CancellationToken cancellationToken = default)
    {
        var maxValue = await _dbContext.DanhMucTrangThais
            .AsNoTracking()
            .Select(x => (int?)x.ThuTuSapXep)
            .MaxAsync(cancellationToken);

        return (maxValue ?? 0) + 1;
    }

    public async Task<DanhMucTrangThaiEntity> AddAsync(DanhMucTrangThaiEntity entity, CancellationToken cancellationToken = default)
    {
        var dbEntity = new DanhMucTrangThai
        {
            NhomTrangThai = entity.NhomTrangThai,
            MaTrangThai = entity.MaTrangThai,
            TenTrangThai = entity.TenTrangThai,
            MaMauHex = entity.MaMauHex,
            ThuTuSapXep = entity.ThuTuSapXep,
            TrangThai = entity.TrangThai,
            MoTa = entity.MoTa,
            GhiChu = entity.GhiChu
        };

        _dbContext.DanhMucTrangThais.Add(dbEntity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        entity.Id = dbEntity.Id;
        return entity;
    }

    public async Task<DanhMucTrangThaiEntity> UpdateAsync(DanhMucTrangThaiEntity entity, CancellationToken cancellationToken = default)
    {
        var dbEntity = await _dbContext.DanhMucTrangThais.FirstAsync(x => x.Id == entity.Id, cancellationToken);
        dbEntity.NhomTrangThai = entity.NhomTrangThai;
        dbEntity.MaTrangThai = entity.MaTrangThai;
        dbEntity.TenTrangThai = entity.TenTrangThai;
        dbEntity.MaMauHex = entity.MaMauHex;
        dbEntity.ThuTuSapXep = entity.ThuTuSapXep;
        dbEntity.TrangThai = entity.TrangThai;
        dbEntity.MoTa = entity.MoTa;
        dbEntity.GhiChu = entity.GhiChu;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var dbEntity = await _dbContext.DanhMucTrangThais.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (dbEntity == null)
        {
            return false;
        }

        _dbContext.DanhMucTrangThais.Remove(dbEntity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}

