using KhaiThacDuLieuService.Application.DTOs;

namespace KhaiThacDuLieuService.Application.Abstractions;

public interface IDashboardKhaiThacDuLieuService
{
    Task<DashboardTongQuanDto> GetTongQuanAsync(CancellationToken cancellationToken = default);
}

public interface ICanhBaoKhaiThacDuLieuService
{
    Task<PagedResultDto<CanhBaoDto>> GetListAsync(CanhBaoListRequest request, CancellationToken cancellationToken = default);
    Task<CanhBaoDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CanhBaoDto> CreateAsync(TaoCanhBaoRequest request, CancellationToken cancellationToken = default);
    Task<CanhBaoDto?> DanhDauDaXemAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CanhBaoDto?> XacNhanXuLyAsync(Guid id, XacNhanXuLyCanhBaoRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CauHinhCanhBaoDto>> GetCauHinhAsync(CancellationToken cancellationToken = default);
    Task<CauHinhCanhBaoDto?> UpdateCauHinhAsync(Guid id, CapNhatCauHinhCanhBaoRequest request, CancellationToken cancellationToken = default);
}

public interface ITraCuuKhaiThacDuLieuService
{
    Task<PagedResultDto<TraCuuTongHopItemDto>> SearchAsync(string nguonDuLieu, TraCuuRequest request, CancellationToken cancellationToken = default);
}

public interface IBaoCaoKhaiThacDuLieuService
{
    Task<BaoCaoTongHopDto> GetBaoCaoAsync(string loaiBaoCao, BaoCaoRequest request, CancellationToken cancellationToken = default);
}
