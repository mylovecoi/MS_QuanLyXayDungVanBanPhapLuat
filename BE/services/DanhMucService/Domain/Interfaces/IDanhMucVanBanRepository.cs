using DanhMucService.Domain.Entities.DanhMuc;

namespace DanhMucService.Domain.Interfaces.Repositories;

public interface IDanhMucVanBanRepository
{
    Task<(IReadOnlyList<DanhMucVanBanEntity> Items, int TotalCount)> GetPagedAsync(
        string? search,
        int pageSize,
        int pageCurrent,
        CancellationToken cancellationToken = default);

    Task<DanhMucVanBanEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(string tenLoaiVanBan, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task<int> GetNextSortOrderAsync(CancellationToken cancellationToken = default);
    Task<DanhMucVanBanEntity> AddAsync(DanhMucVanBanEntity entity, CancellationToken cancellationToken = default);
    Task<DanhMucVanBanEntity> UpdateAsync(DanhMucVanBanEntity entity, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

