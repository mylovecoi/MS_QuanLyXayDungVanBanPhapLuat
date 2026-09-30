using QuanTriHeThongService.Domain.Entities.Systems;

namespace QuanTriHeThongService.Domain.Interfaces.Repositories;

public interface IUserAccountRepository
{
    Task<(IReadOnlyList<UserAccountEntity> Items, int TotalCount)> GetPagedAsync(string? search, int pageSize, int pageCurrent, string? level, CancellationToken cancellationToken = default);
    Task<UserAccountEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OptionItemEntity>> GetGroupPermissionOptionsAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsByUsernameOrEmailAsync(string username, string email, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task<bool> GroupPermissionExistsAsync(Guid groupPermissionId, CancellationToken cancellationToken = default);
    Task UpdateAsync(UserAccountEntity entity, string? newPassword, CancellationToken cancellationToken = default);
    Task DuplicateAsync(Guid sourceId, string username, string name, string email, CancellationToken cancellationToken = default);
    Task ResetPasswordAsync(Guid id, CancellationToken cancellationToken = default);
    Task ChangeStatusAsync(Guid id, string status, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

