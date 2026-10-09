using XayDungVanBanService.Application.DTOs;

namespace XayDungVanBanService.Application.Abstractions;

public interface IXayDungVanBanThamTraHdndService
{
    Task<IReadOnlyList<HoSoThamTraHdndListItemDto>> GetListAsync(CancellationToken cancellationToken = default);
    Task<XayDungVanBanThamTraHdndDto?> GetAsync(Guid hoSoId, CancellationToken cancellationToken = default);
    Task<XayDungVanBanThamTraHdndDto?> UpdateAsync(Guid hoSoId, CapNhatThamTraHdndRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<XayDungVanBanTaiLieuDto>?> GetTaiLieuAsync(Guid hoSoId, CancellationToken cancellationToken = default);
    Task<XayDungVanBanTaiLieuDto?> UploadTaiLieuAsync(Guid hoSoId, TaiTaiLieuThamTraHdndRequest request, CancellationToken cancellationToken = default);
    Task<DieuKienGuiThamTraHdndDto?> KiemTraAsync(Guid hoSoId, CancellationToken cancellationToken = default);
    Task<XayDungVanBanThamTraHdndDto?> GuiAsync(Guid hoSoId, GuiKetQuaThamTraHdndRequest request, CancellationToken cancellationToken = default);
}
