using QuanTriHeThongService.Application.Common.Models;
using QuanTriHeThongService.Application.DTOs.Systems;

namespace QuanTriHeThongService.Application.Abstractions;

public interface IUserAccountAppService
{
    Task<PagedResult<UserAccountDto>> GetPagedAsync(string? search, int pageSize, int pageCurrent, string? level, CancellationToken cancellationToken = default);
    Task<UserAccountDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OptionItemDto>> GetGroupPermissionOptionsAsync(CancellationToken cancellationToken = default);
    Task<(bool IsSuccess, string Message, UserAccountDto? Data)> UpdateAsync(Guid id, UpdateUserAccountRequest request, CancellationToken cancellationToken = default);
    Task<(bool IsSuccess, string Message)> ResetPasswordAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(bool IsSuccess, string Message)> ChangeStatusAsync(Guid id, string status, CancellationToken cancellationToken = default);
    Task<(bool IsSuccess, string Message, UserAccountDto? Data)> DuplicateAsync(DuplicateUserAccountRequest request, CancellationToken cancellationToken = default);
    Task<(bool IsSuccess, string Message)> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

