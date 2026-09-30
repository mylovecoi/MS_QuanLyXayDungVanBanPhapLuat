using DanhMucService.Domain.Entities.DanhMuc;

namespace DanhMucService.Domain.Interfaces.Repositories;

public interface IDanhMucTieuChiDiemRepository
{
    Task<(IReadOnlyList<DanhMucTieuChiDiemEntity> Items, int TotalCount)> GetPagedAsync(string? search, int pageSize, int pageCurrent, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DanhMucTieuChiDiemEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<DanhMucTieuChiDiemEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCodeAsync(string maTieuChi, Guid? ignoreId = null, CancellationToken cancellationToken = default);
    Task<int> GetNextSortOrderAsync(CancellationToken cancellationToken = default);
    Task AddAsync(DanhMucTieuChiDiemEntity entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(DanhMucTieuChiDiemEntity entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

