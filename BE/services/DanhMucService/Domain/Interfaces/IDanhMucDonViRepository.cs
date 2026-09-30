using DanhMucService.Domain.Entities.DanhMuc;

namespace DanhMucService.Domain.Interfaces.Repositories;

public interface IDanhMucDonViRepository
{
    Task<(IReadOnlyList<DanhMucDonViEntity> Items, int TotalCount)> GetPagedAsync(string? search, int pageSize, int pageCurrent, CancellationToken cancellationToken = default);
    Task<DanhMucDonViEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DanhMucDonViEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(string tenDonVi, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task<int> GetNextSortOrderAsync(Guid donViChuQuanId, CancellationToken cancellationToken = default);
    Task<DanhMucDonViEntity> AddAsync(DanhMucDonViEntity entity, CancellationToken cancellationToken = default);
    Task<DanhMucDonViEntity> UpdateAsync(DanhMucDonViEntity entity, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

