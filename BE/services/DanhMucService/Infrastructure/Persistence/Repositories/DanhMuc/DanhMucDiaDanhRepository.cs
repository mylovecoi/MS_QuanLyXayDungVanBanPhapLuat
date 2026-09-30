using DanhMucService.Domain.Entities.DanhMuc;
using DanhMucService.Domain.Interfaces.Repositories;
using DanhMucService.Infrastructure.Persistence;
using DanhMucService.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace DanhMucService.Infrastructure.Persistence.Repositories.DanhMuc;

public class DanhMucDiaDanhRepository(DanhMucDbContext dbContext) : IDanhMucDiaDanhRepository
{
    private readonly DanhMucDbContext _dbContext = dbContext;

    public async Task<(IReadOnlyList<DanhMucDiaDanhEntity> Items, int TotalCount)> GetPagedAsync(string? search, int pageSize, int pageCurrent, CancellationToken cancellationToken = default)
    {
        var baseQuery = _dbContext.DanhMucDiaDanhs.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            baseQuery = baseQuery.Where(x => EF.Functions.Like(x.TenDiaDanh, $"%{search}%"));
        }

        var filtered = await baseQuery.OrderBy(x => x.Level).ThenBy(x => x.STTSapXep).ToListAsync(cancellationToken);
        var all = await _dbContext.DanhMucDiaDanhs.AsNoTracking().ToListAsync(cancellationToken);
        var mapped = BuildTree(filtered, all);
        var total = mapped.Count;
        var paged = mapped.Skip((pageCurrent - 1) * pageSize).Take(pageSize).ToList();
        return (paged, total);
    }

    public async Task<IReadOnlyList<DanhMucDiaDanhEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var all = await _dbContext.DanhMucDiaDanhs.AsNoTracking().OrderBy(x => x.Level).ThenBy(x => x.STTSapXep).ToListAsync(cancellationToken);
        return BuildTree(all, all);
    }

    public async Task<DanhMucDiaDanhEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var all = await _dbContext.DanhMucDiaDanhs.AsNoTracking().ToListAsync(cancellationToken);
        var item = all.FirstOrDefault(x => x.Id == id);
        return item == null ? null : Map(item, all);
    }

    public async Task<IReadOnlyList<DanhMucDiaDanhEntity>> GetChildrenAsync(Guid parentId, CancellationToken cancellationToken = default)
    {
        var all = await _dbContext.DanhMucDiaDanhs.AsNoTracking().ToListAsync(cancellationToken);
        return all.Where(x => x.DiaDanhCapTrenId == parentId).OrderBy(x => x.STTSapXep).Select(x => Map(x, all)).ToList();
    }

    public async Task<int> GetNextSortOrderAsync(Guid parentId, CancellationToken cancellationToken = default)
    {
        var max = await _dbContext.DanhMucDiaDanhs
            .Where(x => x.DiaDanhCapTrenId == parentId)
            .MaxAsync(x => (int?)x.STTSapXep, cancellationToken);
        return (max ?? 0) + 1;
    }

    public async Task<DanhMucDiaDanhEntity> AddAsync(DanhMucDiaDanhEntity entity, CancellationToken cancellationToken = default)
    {
        var data = new DanhMucDiaDanh
        {
            Id = entity.Id == Guid.Empty ? Guid.NewGuid() : entity.Id,
            TenDiaDanh = entity.TenDiaDanh,
            Level = entity.Level,
            STTSapXep = entity.STTSapXep,
            DiaDanhCapTrenId = entity.DiaDanhCapTrenId
        };

        _dbContext.DanhMucDiaDanhs.Add(data);
        await _dbContext.SaveChangesAsync(cancellationToken);
        entity.Id = data.Id;
        return entity;
    }

    public async Task<DanhMucDiaDanhEntity> UpdateAsync(DanhMucDiaDanhEntity entity, CancellationToken cancellationToken = default)
    {
        var data = await _dbContext.DanhMucDiaDanhs.FirstAsync(x => x.Id == entity.Id, cancellationToken);
        data.TenDiaDanh = entity.TenDiaDanh;
        data.Level = entity.Level;
        data.STTSapXep = entity.STTSapXep;
        data.DiaDanhCapTrenId = entity.DiaDanhCapTrenId;
        await _dbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<bool> DeleteCascadeAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var all = await _dbContext.DanhMucDiaDanhs.AsNoTracking().ToListAsync(cancellationToken);
        var root = all.FirstOrDefault(x => x.Id == id);
        if (root == null)
        {
            return false;
        }

        var ids = new HashSet<Guid>();
        CollectIds(id, all, ids);
        ids.Add(id);

        var tracked = await _dbContext.DanhMucDiaDanhs.Where(x => ids.Contains(x.Id)).ToListAsync(cancellationToken);
        _dbContext.DanhMucDiaDanhs.RemoveRange(tracked);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static List<DanhMucDiaDanhEntity> BuildTree(IReadOnlyList<DanhMucDiaDanh> filtered, IReadOnlyList<DanhMucDiaDanh> all)
    {
        var result = new List<DanhMucDiaDanhEntity>();
        var added = new HashSet<Guid>();
        foreach (var item in filtered)
        {
            if (added.Add(item.Id))
            {
                result.Add(Map(item, all));
                AddChildren(item.Id, all, result, added);
            }
        }

        return result;
    }

    private static void AddChildren(Guid parentId, IReadOnlyList<DanhMucDiaDanh> all, List<DanhMucDiaDanhEntity> result, HashSet<Guid> added)
    {
        var children = all.Where(x => x.DiaDanhCapTrenId == parentId).OrderBy(x => x.STTSapXep).ToList();
        foreach (var child in children)
        {
            if (added.Add(child.Id))
            {
                result.Add(Map(child, all));
                AddChildren(child.Id, all, result, added);
            }
        }
    }

    private static void CollectIds(Guid parentId, IReadOnlyList<DanhMucDiaDanh> all, HashSet<Guid> ids)
    {
        var children = all.Where(x => x.DiaDanhCapTrenId == parentId).ToList();
        foreach (var child in children)
        {
            if (ids.Add(child.Id))
            {
                CollectIds(child.Id, all, ids);
            }
        }
    }

    private static DanhMucDiaDanhEntity Map(DanhMucDiaDanh item, IReadOnlyList<DanhMucDiaDanh> all)
    {
        var parentName = all.FirstOrDefault(x => x.Id == item.DiaDanhCapTrenId)?.TenDiaDanh;
        return new DanhMucDiaDanhEntity
        {
            Id = item.Id,
            TenDiaDanh = item.TenDiaDanh,
            Level = item.Level,
            STTSapXep = item.STTSapXep,
            DiaDanhCapTrenId = item.DiaDanhCapTrenId,
            TenDiaDanhChuQuan = parentName
        };
    }
}

