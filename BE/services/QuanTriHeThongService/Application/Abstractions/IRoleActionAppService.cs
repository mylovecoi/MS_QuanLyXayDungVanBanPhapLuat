using QuanTriHeThongService.Application.Common.Models;
using QuanTriHeThongService.Application.DTOs.Systems;

namespace QuanTriHeThongService.Application.Abstractions;

public interface IRoleActionAppService
{
    Task<PagedResult<RoleActionDto>> GetPagedAsync(string? search, int pageSize, int pageCurrent, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RoleActionDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RoleActionDto>> GetGroupOptionsAsync(CancellationToken cancellationToken = default);
    Task<RoleActionDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> GetNextSortOrderAsync(Guid? parentId, CancellationToken cancellationToken = default);
    Task<(bool IsSuccess, string Message, RoleActionDto? Data)> CreateAsync(UpsertRoleActionRequest request, CancellationToken cancellationToken = default);
    Task<(bool IsSuccess, string Message, RoleActionDto? Data)> UpdateAsync(Guid id, UpsertRoleActionRequest request, CancellationToken cancellationToken = default);
    Task<(bool IsSuccess, string Message)> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

