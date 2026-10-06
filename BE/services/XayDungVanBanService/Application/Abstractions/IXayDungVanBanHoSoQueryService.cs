using XayDungVanBanService.Application.DTOs;

namespace XayDungVanBanService.Application.Abstractions;

public interface IXayDungVanBanHoSoQueryService
{
    Task<PagedResultDto<XayDungVanBanHoSoListItemDto>> GetListAsync(
        XayDungVanBanHoSoListRequest request,
        CancellationToken cancellationToken = default);

    Task<XayDungVanBanHoSoDetailDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<XayDungVanBanTimelineItemDto>?> GetTimelineAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}
