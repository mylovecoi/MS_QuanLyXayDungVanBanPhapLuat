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
    Task<IReadOnlyList<CanhBaoNhacViecDto>?> GetNhacViecAsync(Guid canhBaoId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CanhBaoNhacViecDto>> GetNhacViecCuaToiAsync(CancellationToken cancellationToken = default);
    Task<CanhBaoNhacViecDto?> TaoNhacViecAsync(Guid canhBaoId, TaoCanhBaoNhacViecRequest request, CancellationToken cancellationToken = default);
    Task<CanhBaoNhacViecDto?> DanhDauDaXemNhacViecAsync(Guid canhBaoId, Guid nhacViecId, CancellationToken cancellationToken = default);
    Task<CanhBaoNhacViecDto?> HoanThanhNhacViecAsync(Guid canhBaoId, Guid nhacViecId, HoanThanhCanhBaoNhacViecRequest request, CancellationToken cancellationToken = default);
    Task<CanhBaoNhacViecDto?> HuyNhacViecAsync(Guid canhBaoId, Guid nhacViecId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CanhBaoLichSuXuLyDto>?> GetLichSuAsync(Guid canhBaoId, CancellationToken cancellationToken = default);
}

public interface ICanhBaoThongMinhGeneratorService
{
    Task<SinhCanhBaoResultDto> SinhCanhBaoTuDongAsync(CancellationToken cancellationToken = default);
}

public interface ITraCuuKhaiThacDuLieuService
{
    Task<PagedResultDto<TraCuuTongHopItemDto>> SearchAsync(string nguonDuLieu, TraCuuRequest request, CancellationToken cancellationToken = default);
}

public interface IBaoCaoKhaiThacDuLieuService
{
    Task<BaoCaoTongHopDto> GetBaoCaoAsync(string loaiBaoCao, BaoCaoRequest request, CancellationToken cancellationToken = default);
}
