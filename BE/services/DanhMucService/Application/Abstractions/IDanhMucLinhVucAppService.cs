using DanhMucService.Application.Common.Models;
using DanhMucService.Application.DTOs.DanhMuc;

namespace DanhMucService.Application.Abstractions;

public interface IDanhMucLinhVucAppService
{
    Task<PagedResult<DanhMucLinhVucDto>> GetPagedAsync(string? search, int pageSize, int pageCurrent, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DanhMucLinhVucDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<DanhMucLinhVucDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> GetNextSortOrderAsync(CancellationToken cancellationToken = default);
    Task<(bool IsSuccess, string Message, DanhMucLinhVucDto? Data)> CreateAsync(UpsertDanhMucLinhVucRequest request, CancellationToken cancellationToken = default);
    Task<(bool IsSuccess, string Message, DanhMucLinhVucDto? Data)> UpdateAsync(Guid id, UpsertDanhMucLinhVucRequest request, CancellationToken cancellationToken = default);
    Task<(bool IsSuccess, string Message)> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

