using QuanTriHeThongService.Domain.Entities.Systems;

namespace QuanTriHeThongService.Domain.Interfaces.Repositories;

public interface IRoleActionRepository
{
    Task<(IReadOnlyList<RoleActionEntity> Items, int TotalCount)> GetPagedAsync(string? search, int pageSize, int pageCurrent, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RoleActionEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RoleActionEntity>> GetGroupOptionsAsync(CancellationToken cancellationToken = default);
    Task<RoleActionEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByRoleAsync(string role, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task<int> GetNextSortOrderAsync(Guid? parentId, CancellationToken cancellationToken = default);
    Task<bool> HasChildrenAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> IsDescendantAsync(Guid currentId, Guid candidateParentId, CancellationToken cancellationToken = default);
    Task AddAsync(RoleActionEntity entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(RoleActionEntity entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

