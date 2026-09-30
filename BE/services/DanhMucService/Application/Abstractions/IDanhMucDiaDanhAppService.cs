using DanhMucService.Application.Common.Models;
using DanhMucService.Application.DTOs.DanhMuc;

namespace DanhMucService.Application.Abstractions;

public interface IDanhMucDiaDanhAppService
{
    Task<PagedResult<DanhMucDiaDanhDto>> GetPagedAsync(string? search, int pageSize, int pageCurrent, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DanhMucDiaDanhDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DanhMucDiaDanhDto>> GetChildrenAsync(Guid parentId, CancellationToken cancellationToken = default);
    Task<DanhMucDiaDanhDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> GetNextSortOrderAsync(Guid parentId, CancellationToken cancellationToken = default);
    Task<(bool IsSuccess, string Message, DanhMucDiaDanhDto? Data)> CreateAsync(UpsertDanhMucDiaDanhRequest request, CancellationToken cancellationToken = default);
    Task<(bool IsSuccess, string Message, DanhMucDiaDanhDto? Data)> UpdateAsync(Guid id, UpsertDanhMucDiaDanhRequest request, CancellationToken cancellationToken = default);
    Task<(bool IsSuccess, string Message)> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

