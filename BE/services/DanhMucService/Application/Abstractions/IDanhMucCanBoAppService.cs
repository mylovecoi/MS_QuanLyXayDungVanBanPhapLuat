using DanhMucService.Application.Common.Models;
using DanhMucService.Application.DTOs.DanhMuc;
using DanhMucService.Domain.Enums;

namespace DanhMucService.Application.Abstractions;

public interface IDanhMucCanBoAppService
{
    Task<PagedResult<DanhMucCanBoDto>> GetPagedAsync(
        string? search,
        int pageSize,
        int pageCurrent,
        Guid? donViId,
        Guid? phongBanId,
        LoaiLaoDongType? loaiLaoDong,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DanhMucCanBoDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<DanhMucCanBoDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<(bool IsSuccess, string Message, DanhMucCanBoDto? Data)> CreateAsync(UpsertDanhMucCanBoRequest request, CancellationToken cancellationToken = default);

    Task<(bool IsSuccess, string Message, DanhMucCanBoDto? Data)> UpdateAsync(Guid id, UpsertDanhMucCanBoRequest request, CancellationToken cancellationToken = default);

    Task<(bool IsSuccess, string Message)> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

