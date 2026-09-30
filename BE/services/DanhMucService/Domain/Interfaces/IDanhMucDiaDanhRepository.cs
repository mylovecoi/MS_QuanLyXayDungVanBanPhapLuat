using DanhMucService.Domain.Entities.DanhMuc;

namespace DanhMucService.Domain.Interfaces.Repositories;

public interface IDanhMucDiaDanhRepository
{
    Task<(IReadOnlyList<DanhMucDiaDanhEntity> Items, int TotalCount)> GetPagedAsync(string? search, int pageSize, int pageCurrent, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DanhMucDiaDanhEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<DanhMucDiaDanhEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DanhMucDiaDanhEntity>> GetChildrenAsync(Guid parentId, CancellationToken cancellationToken = default);
    Task<int> GetNextSortOrderAsync(Guid parentId, CancellationToken cancellationToken = default);
    Task<DanhMucDiaDanhEntity> AddAsync(DanhMucDiaDanhEntity entity, CancellationToken cancellationToken = default);
    Task<DanhMucDiaDanhEntity> UpdateAsync(DanhMucDiaDanhEntity entity, CancellationToken cancellationToken = default);
    Task<bool> DeleteCascadeAsync(Guid id, CancellationToken cancellationToken = default);
}

