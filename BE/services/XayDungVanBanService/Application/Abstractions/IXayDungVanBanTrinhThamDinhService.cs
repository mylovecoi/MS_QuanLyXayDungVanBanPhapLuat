using XayDungVanBanService.Application.DTOs;

namespace XayDungVanBanService.Application.Abstractions;

public interface IXayDungVanBanTrinhThamDinhService
{
    Task<IReadOnlyList<HoSoTrinhThamDinhListItemDto>> GetListAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<HoSoNguonTrinhThamDinhDto>> GetNguonKeThuaAsync(CancellationToken cancellationToken = default);
    Task<XayDungVanBanTrinhThamDinhDto> CreateAsync(TaoHoSoTrinhThamDinhRequest request, CancellationToken cancellationToken = default);
    Task<XayDungVanBanTrinhThamDinhDto?> GetByHoSoIdAsync(Guid hoSoId, CancellationToken cancellationToken = default);
    Task<XayDungVanBanTrinhThamDinhDto?> UpdateAsync(Guid hoSoId, CapNhatHoSoTrinhThamDinhRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<XayDungVanBanTaiLieuDto>?> GetTaiLieuAsync(Guid hoSoId, CancellationToken cancellationToken = default);
    Task<XayDungVanBanTaiLieuDto?> UploadTaiLieuAsync(Guid hoSoId, TaiTaiLieuTrinhThamDinhRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteTaiLieuAsync(Guid hoSoId, Guid boHoSoTaiLieuId, CancellationToken cancellationToken = default);
    Task<DieuKienGuiThamDinhDto?> KiemTraTruocGuiAsync(Guid hoSoId, CancellationToken cancellationToken = default);
    Task<XayDungVanBanTrinhThamDinhDto?> GuiAsync(Guid hoSoId, GuiThamDinhRequest request, CancellationToken cancellationToken = default);
    Task<bool> HuyAsync(Guid hoSoId, CancellationToken cancellationToken = default);
}
