using QuanTriHeThongService.Domain.Entities.Systems;
using QuanTriHeThongService.Domain.Interfaces.Repositories;
using QuanTriHeThongService.Infrastructure.Persistence;
using QuanTriHeThongService.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace QuanTriHeThongService.Infrastructure.Persistence.Repositories.Systems;

public class RoleActionRepository(QuanTriHeThongDbContext dbContext) : IRoleActionRepository
{
    private readonly QuanTriHeThongDbContext _dbContext = dbContext;

    public async Task<(IReadOnlyList<RoleActionEntity> Items, int TotalCount)> GetPagedAsync(string? search, int pageSize, int pageCurrent, CancellationToken cancellationToken = default)
    {
        var orderedItems = await GetFlattenedAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var keyword = search.Trim();
            orderedItems = orderedItems
                .Where(x =>
                    x.Role.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    (x.Title?.Contains(keyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (x.MenuTitle?.Contains(keyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (x.FrontendPath?.Contains(keyword, StringComparison.OrdinalIgnoreCase) ?? false))
                .ToList();
        }

        var totalCount = orderedItems.Count;
        var items = orderedItems
            .Skip((pageCurrent - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return (items, totalCount);
    }

    public Task<IReadOnlyList<RoleActionEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        => GetFlattenedAsync(cancellationToken).ContinueWith<IReadOnlyList<RoleActionEntity>>(task => task.Result, cancellationToken);

    public async Task<IReadOnlyList<RoleActionEntity>> GetGroupOptionsAsync(CancellationToken cancellationToken = default)
    {
        var items = await GetFlattenedAsync(cancellationToken);
        return items.Where(x => string.Equals(x.PhanLoai, "Group", StringComparison.OrdinalIgnoreCase)).ToList();
    }

    public async Task<RoleActionEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var all = await LoadAllAsync(cancellationToken);
        return all.FirstOrDefault(x => x.Id == id);
    }

    public Task<bool> ExistsByRoleAsync(string role, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        return _dbContext.RoleActions.AsNoTracking().AnyAsync(
            x => x.Role == role && (!excludeId.HasValue || x.Id != excludeId.Value),
            cancellationToken);
    }

    public async Task<int> GetNextSortOrderAsync(Guid? parentId, CancellationToken cancellationToken = default)
    {
        var dbParentId = parentId ?? Guid.Empty;
        var maxValue = await _dbContext.RoleActions.AsNoTracking()
            .Where(x => x.RoleGroupId == dbParentId)
            .Select(x => (int?)x.STTSapXep)
            .MaxAsync(cancellationToken);

        return (maxValue ?? 0) + 1;
    }

    public Task<bool> HasChildrenAsync(Guid id, CancellationToken cancellationToken = default)
        => _dbContext.RoleActions.AsNoTracking().AnyAsync(x => x.RoleGroupId == id, cancellationToken);

    public async Task<bool> IsDescendantAsync(Guid currentId, Guid candidateParentId, CancellationToken cancellationToken = default)
    {
        var all = await _dbContext.RoleActions.AsNoTracking()
            .Select(x => new { x.Id, x.RoleGroupId })
            .ToListAsync(cancellationToken);

        var childrenMap = all
            .Where(x => x.RoleGroupId != Guid.Empty)
            .GroupBy(x => x.RoleGroupId)
            .ToDictionary(g => g.Key, g => g.Select(x => x.Id).ToList());

        var stack = new Stack<Guid>();
        stack.Push(currentId);

        while (stack.Count > 0)
        {
            var parent = stack.Pop();
            if (!childrenMap.TryGetValue(parent, out var children))
            {
                continue;
            }

            foreach (var child in children)
            {
                if (child == candidateParentId)
                {
                    return true;
                }

                stack.Push(child);
            }
        }

        return false;
    }

    public async Task AddAsync(RoleActionEntity entity, CancellationToken cancellationToken = default)
    {
        var dbEntity = new RoleAction
        {
            STTSapXep = entity.STTSapXep,
            PhanLoai = entity.PhanLoai,
            Level = entity.Level,
            Role = entity.Role,
            RoleGroupId = entity.ParentId ?? Guid.Empty,
            Title = entity.Title,
            Controller = entity.Controller,
            Action = entity.Action,
            Parameter = entity.Parameter,
            Table = entity.Table,
            Status = entity.Status,
            UseGroup = entity.UseGroup,
            FrontendPath = entity.FrontendPath,
            IsVisibleInMenu = entity.IsVisibleInMenu,
            ClientApp = entity.ClientApp,
            MenuTitle = entity.MenuTitle,
            MenuIcon = entity.MenuIcon,
            Icon = entity.Icon
        };

        _dbContext.RoleActions.Add(dbEntity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        entity.Id = dbEntity.Id;
    }

    public async Task UpdateAsync(RoleActionEntity entity, CancellationToken cancellationToken = default)
    {
        var dbEntity = await _dbContext.RoleActions.FirstAsync(x => x.Id == entity.Id, cancellationToken);
        dbEntity.STTSapXep = entity.STTSapXep;
        dbEntity.PhanLoai = entity.PhanLoai;
        dbEntity.Level = entity.Level;
        dbEntity.Role = entity.Role;
        dbEntity.RoleGroupId = entity.ParentId ?? Guid.Empty;
        dbEntity.Title = entity.Title;
        dbEntity.Controller = entity.Controller;
        dbEntity.Action = entity.Action;
        dbEntity.Parameter = entity.Parameter;
        dbEntity.Table = entity.Table;
        dbEntity.Status = entity.Status;
        dbEntity.UseGroup = entity.UseGroup;
        dbEntity.FrontendPath = entity.FrontendPath;
        dbEntity.IsVisibleInMenu = entity.IsVisibleInMenu;
        dbEntity.ClientApp = entity.ClientApp;
        dbEntity.MenuTitle = entity.MenuTitle;
        dbEntity.MenuIcon = entity.MenuIcon;
        dbEntity.Icon = entity.Icon;

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var dbEntity = await _dbContext.RoleActions.FirstAsync(x => x.Id == id, cancellationToken);
        _dbContext.RoleActions.Remove(dbEntity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<List<RoleActionEntity>> GetFlattenedAsync(CancellationToken cancellationToken)
    {
        var all = await LoadAllAsync(cancellationToken);
        var roots = all
            .Where(x => !x.ParentId.HasValue)
            .OrderBy(x => x.STTSapXep)
            .ThenBy(x => x.Role, StringComparer.OrdinalIgnoreCase)
            .ToList();

        var byParent = all
            .Where(x => x.ParentId.HasValue)
            .GroupBy(x => x.ParentId!.Value)
            .ToDictionary(
                g => g.Key,
                g => g.OrderBy(x => x.STTSapXep).ThenBy(x => x.Role, StringComparer.OrdinalIgnoreCase).ToList());

        var ordered = new List<RoleActionEntity>();

        void Append(RoleActionEntity current)
        {
            ordered.Add(current);
            if (!byParent.TryGetValue(current.Id, out var children))
            {
                return;
            }

            foreach (var child in children)
            {
                Append(child);
            }
        }

        foreach (var root in roots)
        {
            Append(root);
        }

        return ordered;
    }

    private async Task<List<RoleActionEntity>> LoadAllAsync(CancellationToken cancellationToken)
    {
        return await (from role in _dbContext.RoleActions.AsNoTracking()
                      join parent in _dbContext.RoleActions.AsNoTracking() on role.RoleGroupId equals parent.Id into parentJoin
                      from parent in parentJoin.DefaultIfEmpty()
                      select new RoleActionEntity
                      {
                          Id = role.Id,
                          STTSapXep = role.STTSapXep,
                          PhanLoai = role.PhanLoai,
                          Level = role.Level,
                          Role = role.Role,
                          ParentId = role.RoleGroupId == Guid.Empty ? null : role.RoleGroupId,
                          ParentTitle = parent != null ? parent.Title : null,
                          Title = role.Title,
                          Controller = role.Controller,
                          Action = role.Action,
                          Parameter = role.Parameter,
                          Table = role.Table,
                          Status = role.Status,
                          UseGroup = role.UseGroup,
                          FrontendPath = role.FrontendPath,
                          IsVisibleInMenu = role.IsVisibleInMenu,
                          ClientApp = role.ClientApp,
                          MenuTitle = role.MenuTitle,
                          MenuIcon = role.MenuIcon,
                          Icon = role.Icon
                      }).ToListAsync(cancellationToken);
    }
}

