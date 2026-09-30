using DanhMucService.Application.Common.Models;
using DanhMucService.Application.DTOs.DanhMuc;

namespace DanhMucService.Application.Abstractions;

public interface IDanhMucVanBanAppService
{
    Task<PagedResult<DanhMucVanBanDto>> GetPagedAsync(string? search, int pageSize, int pageCurrent, CancellationToken cancellationToken = default);
    Task<DanhMucVanBanDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> GetNextSortOrderAsync(CancellationToken cancellationToken = default);
    Task<(bool IsSuccess, string Message, DanhMucVanBanDto? Data)> CreateAsync(UpsertDanhMucVanBanRequest request, CancellationToken cancellationToken = default);
    Task<(bool IsSuccess, string Message, DanhMucVanBanDto? Data)> UpdateAsync(Guid id, UpsertDanhMucVanBanRequest request, CancellationToken cancellationToken = default);
    Task<(bool IsSuccess, string Message)> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

