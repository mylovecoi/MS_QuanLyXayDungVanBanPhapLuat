using DanhMucService.Domain.Entities.DanhMuc;
using DanhMucService.Domain.Interfaces.Repositories;
using DanhMucService.Infrastructure.Persistence;
using DanhMucService.Infrastructure.Persistence.Entities;
using DanhMucService.Infrastructure.Persistence.Enums;
using Microsoft.EntityFrameworkCore;

namespace DanhMucService.Infrastructure.Persistence.Repositories.DanhMuc;

public class DanhMucPhongBanRepository(DanhMucDbContext dbContext) : IDanhMucPhongBanRepository
{
    private readonly DanhMucDbContext _dbContext = dbContext;

    public async Task<(IReadOnlyList<DanhMucPhongBanEntity> Items, int TotalCount)> GetPagedAsync(
        string? search,
        int pageSize,
        int pageCurrent,
        Guid? donViId,
        int? loaiPhongBan,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.DanhMucPhongBans
            .Include(x => x.DanhMucDonVi)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x =>
                EF.Functions.Like(x.TenPhongBan, $"%{search}%") ||
                EF.Functions.Like(x.MaPhongBan, $"%{search}%"));
        }

        if (donViId.HasValue && donViId.Value != Guid.Empty)
        {
            query = query.Where(x => x.DanhMucDonViId == donViId.Value);
        }

        if (loaiPhongBan.HasValue)
        {
            query = query.Where(x => (int)x.LoaiPhongBan == loaiPhongBan.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(x => x.UpdatedDate)
            .Skip((pageCurrent - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new DanhMucPhongBanEntity
            {
                Id = x.Id,
                TenPhongBan = x.TenPhongBan,
                MaPhongBan = x.MaPhongBan,
                LoaiPhongBan = (int)x.LoaiPhongBan,
                DanhMucDonViId = x.DanhMucDonViId,
                TenDonVi = x.DanhMucDonVi != null ? x.DanhMucDonVi.TenDonVi : null
            })
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<DanhMucPhongBanEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.DanhMucPhongBans
            .Include(x => x.DanhMucDonVi)
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new DanhMucPhongBanEntity
            {
                Id = x.Id,
                TenPhongBan = x.TenPhongBan,
                MaPhongBan = x.MaPhongBan,
                LoaiPhongBan = (int)x.LoaiPhongBan,
                DanhMucDonViId = x.DanhMucDonViId,
                TenDonVi = x.DanhMucDonVi != null ? x.DanhMucDonVi.TenDonVi : null
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<DanhMucPhongBanEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.DanhMucPhongBans
            .Include(x => x.DanhMucDonVi)
            .AsNoTracking()
            .OrderBy(x => x.TenPhongBan)
            .Select(x => new DanhMucPhongBanEntity
            {
                Id = x.Id,
                TenPhongBan = x.TenPhongBan,
                MaPhongBan = x.MaPhongBan,
                LoaiPhongBan = (int)x.LoaiPhongBan,
                DanhMucDonViId = x.DanhMucDonViId,
                TenDonVi = x.DanhMucDonVi != null ? x.DanhMucDonVi.TenDonVi : null
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByCodeAsync(string maPhongBan, Guid? ignoreId = null, CancellationToken cancellationToken = default)
    {
        return await _dbContext.DanhMucPhongBans.AnyAsync(
            x => x.MaPhongBan == maPhongBan && (!ignoreId.HasValue || x.Id != ignoreId.Value),
            cancellationToken);
    }

    public async Task AddAsync(DanhMucPhongBanEntity entity, CancellationToken cancellationToken = default)
    {
        var dataEntity = new DanhMucPhongBan
        {
            Id = entity.Id == Guid.Empty ? Guid.NewGuid() : entity.Id,
            TenPhongBan = entity.TenPhongBan,
            MaPhongBan = entity.MaPhongBan,
            LoaiPhongBan = (LoaiPhongBan)entity.LoaiPhongBan,
            DanhMucDonViId = entity.DanhMucDonViId
        };

        _dbContext.DanhMucPhongBans.Add(dataEntity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        entity.Id = dataEntity.Id;
    }

    public async Task UpdateAsync(DanhMucPhongBanEntity entity, CancellationToken cancellationToken = default)
    {
        var dataEntity = await _dbContext.DanhMucPhongBans.FirstAsync(x => x.Id == entity.Id, cancellationToken);
        dataEntity.TenPhongBan = entity.TenPhongBan;
        dataEntity.MaPhongBan = entity.MaPhongBan;
        dataEntity.LoaiPhongBan = (LoaiPhongBan)entity.LoaiPhongBan;
        dataEntity.DanhMucDonViId = entity.DanhMucDonViId;

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var dataEntity = await _dbContext.DanhMucPhongBans.FirstAsync(x => x.Id == id, cancellationToken);
        _dbContext.DanhMucPhongBans.Remove(dataEntity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}

