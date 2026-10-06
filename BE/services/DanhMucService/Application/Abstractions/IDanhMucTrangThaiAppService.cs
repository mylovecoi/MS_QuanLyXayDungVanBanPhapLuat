using DanhMucService.Application.Common.Models;
using DanhMucService.Application.DTOs.DanhMuc;

namespace DanhMucService.Application.Abstractions;

public interface IDanhMucTrangThaiAppService
{
    Task<PagedResult<DanhMucTrangThaiDto>> GetPagedAsync(string? search, string? nhomTrangThai, int pageSize, int pageCurrent, CancellationToken cancellationToken = default);
    Task<DanhMucTrangThaiDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> GetNextSortOrderAsync(CancellationToken cancellationToken = default);
    Task<(bool IsSuccess, string Message, DanhMucTrangThaiDto? Data)> CreateAsync(UpsertDanhMucTrangThaiRequest request, CancellationToken cancellationToken = default);
    Task<(bool IsSuccess, string Message, DanhMucTrangThaiDto? Data)> UpdateAsync(Guid id, UpsertDanhMucTrangThaiRequest request, CancellationToken cancellationToken = default);
    Task<(bool IsSuccess, string Message)> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

