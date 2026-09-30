using QuanTriHeThongService.Application.Common.Models;
using QuanTriHeThongService.Application.DTOs.Systems;

namespace QuanTriHeThongService.Application.Abstractions;

public interface IGroupPermissionAppService
{
    Task<PagedResult<GroupPermissionDto>> GetPagedAsync(string? search, int pageSize, int pageCurrent, CancellationToken cancellationToken = default);
    Task<GroupPermissionDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OptionItemDto>> GetTemplateGroupsAsync(CancellationToken cancellationToken = default);
    Task<(bool IsSuccess, string Message, GroupPermissionDto? Data)> CreateAsync(UpsertGroupPermissionRequest request, CancellationToken cancellationToken = default);
    Task<(bool IsSuccess, string Message, GroupPermissionDto? Data)> UpdateAsync(Guid id, UpsertGroupPermissionRequest request, CancellationToken cancellationToken = default);
    Task<(bool IsSuccess, string Message)> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResult<PermissionDto>> GetPermissionsAsync(Guid groupId, string? search, int pageSize, int pageCurrent, CancellationToken cancellationToken = default);
    Task<PermissionDto?> GetPermissionByIdAsync(Guid groupId, Guid permissionId, CancellationToken cancellationToken = default);
    Task<(bool IsSuccess, string Message, PermissionDto? Data)> UpdatePermissionAsync(Guid groupId, Guid permissionId, UpdatePermissionRequest request, CancellationToken cancellationToken = default);
}

