using DanhMucService.Application.Common.Models;
using DanhMucService.Application.DTOs.DanhMuc;

namespace DanhMucService.Application.Abstractions;

public interface IDanhMucQuyTrinhSoanThaoAppService
{
    Task<PagedResult<DanhMucQuyTrinhSoanThaoDto>> GetPagedAsync(string? search, int pageSize, int pageCurrent, CancellationToken cancellationToken = default);
    Task<DanhMucQuyTrinhSoanThaoDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DanhMucLookupDto>> GetDanhMucVanBanOptionsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DanhMucLookupDto>> GetDanhMucDonViOptionsAsync(CancellationToken cancellationToken = default);
    Task<(bool IsSuccess, string Message, DanhMucQuyTrinhSoanThaoDto? Data)> CreateAsync(UpsertDanhMucQuyTrinhSoanThaoRequest request, CancellationToken cancellationToken = default);
    Task<(bool IsSuccess, string Message, DanhMucQuyTrinhSoanThaoDto? Data)> UpdateAsync(Guid id, UpsertDanhMucQuyTrinhSoanThaoRequest request, CancellationToken cancellationToken = default);
    Task<(bool IsSuccess, string Message)> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

