using DanhMucService.Application.Common.Models;
using DanhMucService.Application.DTOs.DanhMuc;

namespace DanhMucService.Application.Abstractions;

public interface IDanhMucTieuChiDiemAppService
{
    Task<PagedResult<DanhMucTieuChiDiemDto>> GetPagedAsync(string? search, int pageSize, int pageCurrent, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DanhMucTieuChiDiemDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<DanhMucTieuChiDiemDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> GetNextSortOrderAsync(CancellationToken cancellationToken = default);
    Task<(bool IsSuccess, string Message, DanhMucTieuChiDiemDto? Data)> CreateAsync(UpsertDanhMucTieuChiDiemRequest request, CancellationToken cancellationToken = default);
    Task<(bool IsSuccess, string Message, DanhMucTieuChiDiemDto? Data)> UpdateAsync(Guid id, UpsertDanhMucTieuChiDiemRequest request, CancellationToken cancellationToken = default);
    Task<(bool IsSuccess, string Message)> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

