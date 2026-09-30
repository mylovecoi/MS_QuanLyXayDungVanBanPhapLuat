using QuanTriHeThongService.Domain.Entities.Systems;
using QuanTriHeThongService.Domain.Interfaces.Repositories;
using QuanTriHeThongService.Infrastructure.Persistence;
using QuanTriHeThongService.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace QuanTriHeThongService.Infrastructure.Persistence.Repositories.Systems;

public class GroupPermissionRepository(QuanTriHeThongDbContext dbContext) : IGroupPermissionRepository
{
    private readonly QuanTriHeThongDbContext _dbContext = dbContext;

    public async Task<(IReadOnlyList<GroupPermissionEntity> Items, int TotalCount)> GetPagedAsync(string? search, int pageSize, int pageCurrent, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.GroupsPermision.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x => x.Name.Contains(search) || (x.Description != null && x.Description.Contains(search)) || x.Status.Contains(search));
        }

        query = query.OrderBy(x => x.Name);
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.Skip((pageCurrent - 1) * pageSize).Take(pageSize)
            .Select(x => new GroupPermissionEntity
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Status = x.Status
            }).ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<GroupPermissionEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.GroupsPermision.AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new GroupPermissionEntity
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Status = x.Status
            }).FirstOrDefaultAsync(cancellationToken);
    }

    public Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        return _dbContext.GroupsPermision.AsNoTracking()
            .AnyAsync(x => x.Name == name && (!excludeId.HasValue || x.Id != excludeId.Value), cancellationToken);
    }

    public async Task AddAsync(GroupPermissionEntity entity, CancellationToken cancellationToken = default)
    {
        var dbEntity = new GroupPermision
        {
            Name = entity.Name,
            Description = entity.Description,
            Status = entity.Status
        };

        _dbContext.GroupsPermision.Add(dbEntity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        entity.Id = dbEntity.Id;
    }

    public async Task UpdateAsync(GroupPermissionEntity entity, CancellationToken cancellationToken = default)
    {
        var dbEntity = await _dbContext.GroupsPermision.FirstAsync(x => x.Id == entity.Id, cancellationToken);
        dbEntity.Name = entity.Name;
        dbEntity.Description = entity.Description;
        dbEntity.Status = entity.Status;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _dbContext.Permission.Where(x => x.GroupPermissionId == id).ExecuteDeleteAsync(cancellationToken);
        await _dbContext.GroupsPermision.Where(x => x.Id == id).ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<OptionItemEntity>> GetTemplateGroupsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.OptionDatas.AsNoTracking()
            .Where(x => x.Code == "NhomQuyen")
            .OrderBy(x => x.DisplayName)
            .Select(x => new OptionItemEntity
            {
                Value = x.Value,
                DisplayName = x.DisplayName
            }).ToListAsync(cancellationToken);
    }

    public async Task InitializePermissionsAsync(Guid groupId, string templateGroup, CancellationToken cancellationToken = default)
    {
        await _dbContext.Permission.Where(x => x.GroupPermissionId == groupId).ExecuteDeleteAsync(cancellationToken);

        var roleActions = await _dbContext.RoleActions.AsNoTracking()
            .Where(x => x.Status == "Kích hoạt" && x.UseGroup != null && x.UseGroup.Contains(templateGroup))
            .ToListAsync(cancellationToken);

        var permissions = roleActions.Select(item => new Permission
        {
            GroupPermissionId = groupId,
            RoleActionId = item.Id,
            Index = true,
            Create = item.PhanLoai != "Group" && !item.Role.Contains("Log"),
            Edit = item.PhanLoai != "Group" && !item.Role.Contains("Log"),
            Delete = item.PhanLoai != "Group" && !item.Role.Contains("Log"),
            Approve = item.PhanLoai != "Group" && !item.Role.Contains("Log") && !item.Role.Contains("Systems") && !item.Role.Contains("Settings"),
            Public = item.PhanLoai != "Group" && !item.Role.Contains("Log") && !item.Role.Contains("Systems") && !item.Role.Contains("Settings"),
            Status = "XD"
        }).ToList();

        if (permissions.Count > 0)
        {
            _dbContext.Permission.AddRange(permissions);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<(IReadOnlyList<PermissionEntity> Items, int TotalCount)> GetPermissionsAsync(Guid groupId, string? search, int pageSize, int pageCurrent, CancellationToken cancellationToken = default)
    {
        var permissions = await BuildPermissionTreeAsync(groupId, search, cancellationToken);
        var totalCount = permissions.Count;
        var items = permissions.Skip((pageCurrent - 1) * pageSize).Take(pageSize).ToList();
        return (items, totalCount);
    }

    public async Task<PermissionEntity?> GetPermissionByIdAsync(Guid permissionId, CancellationToken cancellationToken = default)
    {
        return await (from per in _dbContext.Permission.AsNoTracking()
                      join role in _dbContext.RoleActions.AsNoTracking() on per.RoleActionId equals role.Id
                      where per.Id == permissionId
                      select new PermissionEntity
                      {
                          Id = per.Id,
                          GroupPermissionId = per.GroupPermissionId,
                          RoleActionId = per.RoleActionId,
                          Status = per.Status,
                          Index = per.Index,
                          Create = per.Create,
                          Edit = per.Edit,
                          Delete = per.Delete,
                          Approve = per.Approve,
                          Public = per.Public,
                          PhanLoai = role.PhanLoai,
                          RoleActionGroupId = role.RoleGroupId == Guid.Empty ? null : role.RoleGroupId,
                          Level = role.Level,
                          STTSapXep = role.STTSapXep,
                          Title = role.Title,
                          Role = role.Role,
                          Controller = role.Controller,
                          ActionName = role.Action,
                          Table = role.Table,
                          Icon = role.Icon
                      }).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task UpdatePermissionAsync(PermissionEntity entity, CancellationToken cancellationToken = default)
    {
        var dbEntity = await _dbContext.Permission.FirstAsync(x => x.Id == entity.Id, cancellationToken);
        dbEntity.Index = entity.Index;
        dbEntity.Create = entity.Create;
        dbEntity.Edit = entity.Edit;
        dbEntity.Delete = entity.Delete;
        dbEntity.Approve = entity.Approve;
        dbEntity.Public = entity.Public;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<List<PermissionEntity>> BuildPermissionTreeAsync(Guid groupId, string? search, CancellationToken cancellationToken)
    {
        var query = from per in _dbContext.Permission.AsNoTracking().Where(x => x.GroupPermissionId == groupId)
                    join role in _dbContext.RoleActions.AsNoTracking() on per.RoleActionId equals role.Id
                    select new PermissionEntity
                    {
                        Id = per.Id,
                        GroupPermissionId = per.GroupPermissionId,
                        RoleActionId = per.RoleActionId,
                        Status = per.Status,
                        Index = per.Index,
                        Create = per.Create,
                        Edit = per.Edit,
                        Delete = per.Delete,
                        Approve = per.Approve,
                        Public = per.Public,
                        PhanLoai = role.PhanLoai,
                        RoleActionGroupId = role.RoleGroupId == Guid.Empty ? null : role.RoleGroupId,
                        Level = role.Level,
                        STTSapXep = role.STTSapXep,
                        Title = role.Title,
                        Role = role.Role,
                        Controller = role.Controller,
                        ActionName = role.Action,
                        Table = role.Table,
                        Icon = role.Icon
                    };

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x => (x.Title != null && x.Title.Contains(search)) || (x.Role != null && x.Role.Contains(search)));
        }

        var all = await query.ToListAsync(cancellationToken);
        var roots = all.Where(x => x.Level == 0).OrderBy(x => x.STTSapXep).ToList();
        var ordered = new List<PermissionEntity>();

        foreach (var root in roots)
        {
            ordered.Add(root);
            if (root.PhanLoai == "Group" && root.Index)
            {
                AppendChildren(ordered, all, root.RoleActionId);
            }
        }

        return ordered;
    }

    private static void AppendChildren(List<PermissionEntity> target, List<PermissionEntity> all, Guid parentRoleActionId)
    {
        var children = all.Where(x => x.RoleActionGroupId == parentRoleActionId).OrderBy(x => x.STTSapXep).ToList();
        foreach (var child in children)
        {
            target.Add(child);
            if (child.PhanLoai == "Group" && child.Index)
            {
                AppendChildren(target, all, child.RoleActionId);
            }
        }
    }
}

