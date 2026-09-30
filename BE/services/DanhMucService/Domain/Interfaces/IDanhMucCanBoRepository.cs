using DanhMucService.Domain.Entities.DanhMuc;
using DanhMucService.Domain.Enums;

namespace DanhMucService.Domain.Interfaces.Repositories;

public interface IDanhMucCanBoRepository
{
    Task<(IReadOnlyList<DanhMucCanBoEntity> Items, int TotalCount)> GetPagedAsync(
        string? search,
        int pageSize,
        int pageCurrent,
        Guid? donViId,
        Guid? phongBanId,
        LoaiLaoDongType? loaiLaoDong,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DanhMucCanBoEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<DanhMucCanBoEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> IsPhongBanThuocDonViAsync(Guid phongBanId, Guid donViId, CancellationToken cancellationToken = default);

    Task AddAsync(DanhMucCanBoEntity entity, CancellationToken cancellationToken = default);

    Task UpdateAsync(DanhMucCanBoEntity entity, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

