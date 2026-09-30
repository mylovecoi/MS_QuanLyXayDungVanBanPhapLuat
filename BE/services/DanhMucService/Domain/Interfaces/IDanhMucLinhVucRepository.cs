using DanhMucService.Domain.Entities.DanhMuc;

namespace DanhMucService.Domain.Interfaces.Repositories;

public interface IDanhMucLinhVucRepository
{
    Task<(IReadOnlyList<DanhMucLinhVucEntity> Items, int TotalCount)> GetPagedAsync(string? search, int pageSize, int pageCurrent, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DanhMucLinhVucEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<DanhMucLinhVucEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCodeAsync(string maLinhVuc, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task<int> GetNextSortOrderAsync(CancellationToken cancellationToken = default);
    Task<DanhMucLinhVucEntity> AddAsync(DanhMucLinhVucEntity entity, CancellationToken cancellationToken = default);
    Task<DanhMucLinhVucEntity> UpdateAsync(DanhMucLinhVucEntity entity, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

