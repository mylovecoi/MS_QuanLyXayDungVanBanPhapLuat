using DanhMucService.Domain.Entities.DanhMuc;

namespace DanhMucService.Domain.Interfaces.Repositories;

public interface IDanhMucTrangThaiRepository
{
    Task<(IReadOnlyList<DanhMucTrangThaiEntity> Items, int TotalCount)> GetPagedAsync(
        string? search,
        int pageSize,
        int pageCurrent,
        CancellationToken cancellationToken = default);

    Task<DanhMucTrangThaiEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCodeAsync(string maTrangThai, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task<int> GetNextSortOrderAsync(CancellationToken cancellationToken = default);
    Task<DanhMucTrangThaiEntity> AddAsync(DanhMucTrangThaiEntity entity, CancellationToken cancellationToken = default);
    Task<DanhMucTrangThaiEntity> UpdateAsync(DanhMucTrangThaiEntity entity, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

