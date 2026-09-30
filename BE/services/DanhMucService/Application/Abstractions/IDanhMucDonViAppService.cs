using DanhMucService.Application.Common.Models;
using DanhMucService.Application.DTOs.DanhMuc;

namespace DanhMucService.Application.Abstractions;

public interface IDanhMucDonViAppService
{
    Task<PagedResult<DanhMucDonViDto>> GetPagedAsync(string? search, int pageSize, int pageCurrent, CancellationToken cancellationToken = default);
    Task<DanhMucDonViDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DanhMucDonViDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<int> GetNextSortOrderAsync(Guid donViChuQuanId, CancellationToken cancellationToken = default);
    Task<(bool IsSuccess, string Message, DanhMucDonViDto? Data)> CreateAsync(UpsertDanhMucDonViRequest request, CancellationToken cancellationToken = default);
    Task<(bool IsSuccess, string Message, DanhMucDonViDto? Data)> UpdateAsync(Guid id, UpsertDanhMucDonViRequest request, CancellationToken cancellationToken = default);
    Task<(bool IsSuccess, string Message)> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

