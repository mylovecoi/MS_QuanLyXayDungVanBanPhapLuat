using DanhMucService.Application.Common.Models;
using DanhMucService.Application.DTOs.DanhMuc;

namespace DanhMucService.Application.Abstractions;

public interface IDanhMucPhongBanAppService
{
    Task<PagedResult<DanhMucPhongBanDto>> GetPagedAsync(
        string? search,
        int pageSize,
        int pageCurrent,
        Guid? donViId,
        int? loaiPhongBan,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DanhMucPhongBanDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<DanhMucPhongBanDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<(bool IsSuccess, string Message, DanhMucPhongBanDto? Data)> CreateAsync(UpsertDanhMucPhongBanRequest request, CancellationToken cancellationToken = default);

    Task<(bool IsSuccess, string Message, DanhMucPhongBanDto? Data)> UpdateAsync(Guid id, UpsertDanhMucPhongBanRequest request, CancellationToken cancellationToken = default);

    Task<(bool IsSuccess, string Message)> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

