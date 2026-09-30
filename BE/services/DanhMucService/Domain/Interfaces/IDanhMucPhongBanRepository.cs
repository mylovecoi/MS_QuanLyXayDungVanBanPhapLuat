using DanhMucService.Domain.Entities.DanhMuc;

namespace DanhMucService.Domain.Interfaces.Repositories;

public interface IDanhMucPhongBanRepository
{
    Task<(IReadOnlyList<DanhMucPhongBanEntity> Items, int TotalCount)> GetPagedAsync(
        string? search,
        int pageSize,
        int pageCurrent,
        Guid? donViId,
        int? loaiPhongBan,
        CancellationToken cancellationToken = default);

    Task<DanhMucPhongBanEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DanhMucPhongBanEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(string maPhongBan, Guid? ignoreId = null, CancellationToken cancellationToken = default);

    Task AddAsync(DanhMucPhongBanEntity entity, CancellationToken cancellationToken = default);

    Task UpdateAsync(DanhMucPhongBanEntity entity, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

