using XayDungVanBanService.Application.DTOs;

namespace XayDungVanBanService.Application.Abstractions;

public interface IXayDungVanBanTienDoService
{
    Task<PagedResultDto<XayDungVanBanTienDoListItemDto>> GetListAsync(
        XayDungVanBanTienDoListRequest request,
        CancellationToken cancellationToken = default);

    Task<XayDungVanBanTienDoDetailDto?> GetByIdAsync(
        Guid hoSoId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<XayDungVanBanNhacTienDoDto>?> GetNhacNhoAsync(
        Guid hoSoId,
        CancellationToken cancellationToken = default);

    Task<XayDungVanBanNhacTienDoDto?> TaoNhacNhoAsync(
        Guid hoSoId,
        TaoNhacTienDoRequest request,
        CancellationToken cancellationToken = default);

    Task<XayDungVanBanNhacTienDoDto?> PhanHoiAsync(
        Guid hoSoId,
        Guid nhacNhoId,
        CapNhatPhanHoiNhacTienDoRequest request,
        CancellationToken cancellationToken = default);

    Task<XayDungVanBanNhacTienDoDto?> XacNhanXuLyAsync(
        Guid hoSoId,
        Guid nhacNhoId,
        XacNhanXuLyNhacTienDoRequest request,
        CancellationToken cancellationToken = default);
}
