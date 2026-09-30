using DanhMucService.Domain.Entities.DanhMuc;
using DanhMucService.Domain.Interfaces.Repositories;
using DanhMucService.Infrastructure.Persistence;
using DanhMucService.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace DanhMucService.Infrastructure.Persistence.Repositories.DanhMuc;

public class DanhMucLinhVucRepository(DanhMucDbContext dbContext) : IDanhMucLinhVucRepository
{
    private readonly DanhMucDbContext _dbContext = dbContext;

    public async Task<(IReadOnlyList<DanhMucLinhVucEntity> Items, int TotalCount)> GetPagedAsync(string? search, int pageSize, int pageCurrent, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.DanhMucLinhVucs.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x => EF.Functions.Like(x.MaLinhVuc, $"%{search}%") || EF.Functions.Like(x.TenLinhVuc, $"%{search}%"));
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query.OrderBy(x => x.ThuTuSapXep).ThenBy(x => x.TenLinhVuc)
            .Skip((pageCurrent - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new DanhMucLinhVucEntity
            {
                Id = x.Id,
                MaLinhVuc = x.MaLinhVuc,
                TenLinhVuc = x.TenLinhVuc,
                ThuTuSapXep = x.ThuTuSapXep,
                TrangThai = x.TrangThai,
                MoTa = x.MoTa,
                GhiChu = x.GhiChu
            }).ToListAsync(cancellationToken);

        return (items, total);
    }

    public async Task<IReadOnlyList<DanhMucLinhVucEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.DanhMucLinhVucs.AsNoTracking().OrderBy(x => x.ThuTuSapXep).ThenBy(x => x.TenLinhVuc)
            .Select(x => new DanhMucLinhVucEntity
            {
                Id = x.Id,
                MaLinhVuc = x.MaLinhVuc,
                TenLinhVuc = x.TenLinhVuc,
                ThuTuSapXep = x.ThuTuSapXep,
                TrangThai = x.TrangThai,
                MoTa = x.MoTa,
                GhiChu = x.GhiChu
            }).ToListAsync(cancellationToken);
    }

    public async Task<DanhMucLinhVucEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.DanhMucLinhVucs.AsNoTracking().Where(x => x.Id == id)
            .Select(x => new DanhMucLinhVucEntity
            {
                Id = x.Id,
                MaLinhVuc = x.MaLinhVuc,
                TenLinhVuc = x.TenLinhVuc,
                ThuTuSapXep = x.ThuTuSapXep,
                TrangThai = x.TrangThai,
                MoTa = x.MoTa,
                GhiChu = x.GhiChu
            }).FirstOrDefaultAsync(cancellationToken);
    }

    public Task<bool> ExistsByCodeAsync(string maLinhVuc, Guid? excludeId = null, CancellationToken cancellationToken = default)
        => _dbContext.DanhMucLinhVucs.AnyAsync(x => x.MaLinhVuc == maLinhVuc && (!excludeId.HasValue || x.Id != excludeId.Value), cancellationToken);

    public async Task<int> GetNextSortOrderAsync(CancellationToken cancellationToken = default)
    {
        var max = await _dbContext.DanhMucLinhVucs.MaxAsync(x => (int?)x.ThuTuSapXep, cancellationToken);
        return (max ?? 0) + 1;
    }

    public async Task<DanhMucLinhVucEntity> AddAsync(DanhMucLinhVucEntity entity, CancellationToken cancellationToken = default)
    {
        var data = new DanhMucLinhVuc
        {
            Id = entity.Id == Guid.Empty ? Guid.NewGuid() : entity.Id,
            MaLinhVuc = entity.MaLinhVuc,
            TenLinhVuc = entity.TenLinhVuc,
            ThuTuSapXep = entity.ThuTuSapXep,
            TrangThai = entity.TrangThai,
            MoTa = entity.MoTa,
            GhiChu = entity.GhiChu
        };

        _dbContext.DanhMucLinhVucs.Add(data);
        await _dbContext.SaveChangesAsync(cancellationToken);
        entity.Id = data.Id;
        return entity;
    }

    public async Task<DanhMucLinhVucEntity> UpdateAsync(DanhMucLinhVucEntity entity, CancellationToken cancellationToken = default)
    {
        var data = await _dbContext.DanhMucLinhVucs.FirstAsync(x => x.Id == entity.Id, cancellationToken);
        data.MaLinhVuc = entity.MaLinhVuc;
        data.TenLinhVuc = entity.TenLinhVuc;
        data.ThuTuSapXep = entity.ThuTuSapXep;
        data.TrangThai = entity.TrangThai;
        data.MoTa = entity.MoTa;
        data.GhiChu = entity.GhiChu;
        await _dbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var data = await _dbContext.DanhMucLinhVucs.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (data == null)
        {
            return false;
        }

        _dbContext.DanhMucLinhVucs.Remove(data);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}

