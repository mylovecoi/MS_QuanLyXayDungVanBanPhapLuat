using QuanTriHeThongService.Domain.Entities.Systems;

namespace QuanTriHeThongService.Domain.Interfaces.Repositories;

public interface IGroupPermissionRepository
{
    Task<(IReadOnlyList<GroupPermissionEntity> Items, int TotalCount)> GetPagedAsync(string? search, int pageSize, int pageCurrent, CancellationToken cancellationToken = default);
    Task<GroupPermissionEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task AddAsync(GroupPermissionEntity entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(GroupPermissionEntity entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OptionItemEntity>> GetTemplateGroupsAsync(CancellationToken cancellationToken = default);
    Task InitializePermissionsAsync(Guid groupId, string templateGroup, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<PermissionEntity> Items, int TotalCount)> GetPermissionsAsync(Guid groupId, string? search, int pageSize, int pageCurrent, CancellationToken cancellationToken = default);
    Task<PermissionEntity?> GetPermissionByIdAsync(Guid permissionId, CancellationToken cancellationToken = default);
    Task UpdatePermissionAsync(PermissionEntity entity, CancellationToken cancellationToken = default);
}

