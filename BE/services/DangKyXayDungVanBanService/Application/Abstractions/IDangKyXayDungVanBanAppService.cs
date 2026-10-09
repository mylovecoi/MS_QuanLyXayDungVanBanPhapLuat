using DangKyXayDungVanBanService.Application.DTOs;

namespace DangKyXayDungVanBanService.Application.Abstractions;

public interface IDangKyXayDungVanBanAppService
{
    Task<PagedResultDto<DangKyXayDungVanBanDto>> GetListAsync(DangKyXayDungVanBanListRequest request, CancellationToken cancellationToken);
    Task<PagedResultDto<DangKyXayDungVanBanKetQuaListItemDto>> GetKetQuaListAsync(DangKyXayDungVanBanListRequest request, CancellationToken cancellationToken);
    Task<DangKyXayDungVanBanDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<DangKyXayDungVanBanDto> CreateAsync(TaoDangKyXayDungVanBanRequest request, CancellationToken cancellationToken);
    Task<DangKyXayDungVanBanDto?> UpdateAsync(Guid id, CapNhatDangKyXayDungVanBanRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<DangKyXayDungVanBanTimelineDto>> GetTimelineAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<HanhDongKhaDungDto>> GetHanhDongKhaDungAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<DangKyXayDungVanBanFileDto>?> GetFilesAsync(Guid id, CancellationToken cancellationToken);
    Task<DangKyXayDungVanBanFileDto?> UploadFileAsync(Guid id, TaiFileDangKyXayDungVanBanRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteFileAsync(Guid id, Guid fileId, CancellationToken cancellationToken);
    Task<DangKyXayDungVanBanDto?> XuLyAsync(Guid id, XuLyDangKyXayDungVanBanRequest request, CancellationToken cancellationToken);
    Task<Guid?> CapNhatKetQuaPheDuyetAsync(Guid id, CapNhatKetQuaPheDuyetRequest request, CancellationToken cancellationToken);
    Task<DangKyXayDungVanBanDto?> KhoiTaoQuyTrinhXayDungAsync(Guid id, KhoiTaoQuyTrinhXayDungRequest request, CancellationToken cancellationToken);
}
