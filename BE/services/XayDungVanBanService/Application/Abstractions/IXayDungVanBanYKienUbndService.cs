using XayDungVanBanService.Application.DTOs;

namespace XayDungVanBanService.Application.Abstractions;

public interface IXayDungVanBanYKienUbndService
{
    Task<XayDungVanBanYKienUbndDto> CreateAsync(TaoHoSoYKienUbndRequest request, CancellationToken cancellationToken = default);
    Task<XayDungVanBanYKienUbndDto?> GetAsync(Guid hoSoId, CancellationToken cancellationToken = default);
    Task<XayDungVanBanYKienUbndDto?> UpdateAsync(Guid hoSoId, CapNhatYKienUbndRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<XayDungVanBanTaiLieuDto>?> GetTaiLieuAsync(Guid hoSoId, CancellationToken cancellationToken = default);
    Task<XayDungVanBanTaiLieuDto?> UploadTaiLieuAsync(Guid hoSoId, TaiTaiLieuYKienUbndRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteTaiLieuAsync(Guid hoSoId, Guid boHoSoTaiLieuId, CancellationToken cancellationToken = default);
    Task<DieuKienGuiYKienUbndDto?> KiemTraAsync(Guid hoSoId, CancellationToken cancellationToken = default);
    Task<XayDungVanBanYKienUbndDto?> GuiAsync(Guid hoSoId, GuiYKienUbndRequest request, CancellationToken cancellationToken = default);
    Task<bool> HuyAsync(Guid hoSoId, CancellationToken cancellationToken = default);
}
