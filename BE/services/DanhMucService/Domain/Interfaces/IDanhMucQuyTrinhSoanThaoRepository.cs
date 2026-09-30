using DanhMucService.Domain.Entities.DanhMuc;

namespace DanhMucService.Domain.Interfaces.Repositories;

public interface IDanhMucQuyTrinhSoanThaoRepository
{
    Task<(IReadOnlyList<DanhMucQuyTrinhSoanThaoEntity> Items, int TotalCount)> GetPagedAsync(string? search, int pageSize, int pageCurrent, CancellationToken cancellationToken = default);
    Task<DanhMucQuyTrinhSoanThaoEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCodeAsync(string maQuyTrinh, Guid? ignoreId = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DanhMucLookupEntity>> GetDanhMucVanBanOptionsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DanhMucLookupEntity>> GetDanhMucDonViOptionsAsync(CancellationToken cancellationToken = default);
    Task<bool> IsWorkflowUsedAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> IsAnyStepInUseAsync(IReadOnlyCollection<Guid> stepIds, CancellationToken cancellationToken = default);
    Task AddAsync(DanhMucQuyTrinhSoanThaoEntity entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(DanhMucQuyTrinhSoanThaoEntity entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

