using XayDungVanBanService.Application.DTOs;

namespace XayDungVanBanService.Application.Abstractions;

public interface IXayDungVanBanBanHanhService
{
    Task<IReadOnlyList<HoSoBanHanhListItemDto>> GetListAsync(CancellationToken cancellationToken = default);
    Task<XayDungVanBanBanHanhDto?> GetAsync(Guid hoSoId, CancellationToken cancellationToken = default);
    Task<XayDungVanBanBanHanhDto?> UpdateAsync(Guid hoSoId, CapNhatBanHanhRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<XayDungVanBanTaiLieuDto>?> GetTaiLieuAsync(Guid hoSoId, CancellationToken cancellationToken = default);
    Task<XayDungVanBanTaiLieuDto?> UploadTaiLieuAsync(Guid hoSoId, TaiTaiLieuBanHanhRequest request, CancellationToken cancellationToken = default);
    Task<DieuKienHoanThanhBanHanhDto?> KiemTraAsync(Guid hoSoId, CancellationToken cancellationToken = default);
    Task<XayDungVanBanBanHanhDto?> HoanThanhAsync(Guid hoSoId, CancellationToken cancellationToken = default);
}
