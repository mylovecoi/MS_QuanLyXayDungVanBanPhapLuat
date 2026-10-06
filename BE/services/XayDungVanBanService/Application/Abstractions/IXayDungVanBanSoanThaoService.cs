using XayDungVanBanService.Application.DTOs;

namespace XayDungVanBanService.Application.Abstractions;

public interface IXayDungVanBanSoanThaoService
{
    Task<XayDungVanBanSoanThaoDto> CreateAsync(
        TaoHoSoSoanThaoRequest request,
        CancellationToken cancellationToken = default);

    Task<XayDungVanBanSoanThaoDto?> GetByIdAsync(
        Guid hoSoId,
        CancellationToken cancellationToken = default);

    Task<XayDungVanBanSoanThaoDto?> UpdateAsync(
        Guid hoSoId,
        CapNhatHoSoSoanThaoRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid hoSoId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<XayDungVanBanYKienDonViDto>?> GetYKienDonViAsync(
        Guid hoSoId,
        CancellationToken cancellationToken = default);

    Task<XayDungVanBanYKienDonViDto?> CreateYKienDonViAsync(
        Guid hoSoId,
        TaoYKienDonViRequest request,
        CancellationToken cancellationToken = default);

    Task<XayDungVanBanYKienDonViDto?> UpdateYKienDonViAsync(
        Guid hoSoId,
        Guid id,
        CapNhatYKienDonViRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteYKienDonViAsync(
        Guid hoSoId,
        Guid id,
        CancellationToken cancellationToken = default);

    Task<XayDungVanBanTongHopYKienDto?> GetTongHopYKienAsync(
        Guid hoSoId,
        CancellationToken cancellationToken = default);

    Task<XayDungVanBanTongHopYKienDto?> UpdateTongHopYKienAsync(
        Guid hoSoId,
        CapNhatTongHopYKienRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<XayDungVanBanTaiLieuDto>?> GetFileTongHopYKienAsync(
        Guid hoSoId,
        CancellationToken cancellationToken = default);

    Task<XayDungVanBanTaiLieuDto?> UploadFileTongHopYKienAsync(
        Guid hoSoId,
        TaiFileTongHopYKienRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<XayDungVanBanTaiLieuDto>?> GetTaiLieuAsync(
        Guid hoSoId,
        CancellationToken cancellationToken = default);

    Task<XayDungVanBanTaiLieuDto?> UploadTaiLieuAsync(
        Guid hoSoId,
        TaiTaiLieuSoanThaoRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteTaiLieuAsync(
        Guid hoSoId,
        Guid fileId,
        CancellationToken cancellationToken = default);

    Task<DieuKienTrinhThamDinhDto?> KiemTraTruocTrinhThamDinhAsync(
        Guid hoSoId,
        CancellationToken cancellationToken = default);

    Task<TrinhThamDinhDto?> TrinhThamDinhAsync(
        Guid hoSoId,
        TrinhThamDinhRequest request,
        CancellationToken cancellationToken = default);
}
