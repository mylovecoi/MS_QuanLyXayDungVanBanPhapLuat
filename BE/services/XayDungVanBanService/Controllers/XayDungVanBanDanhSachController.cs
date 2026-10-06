using BuildingBlocks.Abstractions;
using Microsoft.AspNetCore.Mvc;
using XayDungVanBanService.Application.Abstractions;
using XayDungVanBanService.Application.DTOs;
using XayDungVanBanService.Infrastructure.Authorization;

namespace XayDungVanBanService.Controllers;

[ApiController]
[Route("api/xay-dung-van-ban/danh-sach")]
public sealed class XayDungVanBanDanhSachController(
    ICurrentUserContext user,
    IQuanTriHeThongPermissionClient permissionClient,
    IXayDungVanBanHoSoQueryService queryService) : XayDungVanBanControllerBase(user, permissionClient)
{
    [HttpGet]
    public async Task<ActionResult<PagedResultDto<XayDungVanBanHoSoListItemDto>>> GetList(
        [FromQuery] XayDungVanBanHoSoListRequest request,
        CancellationToken cancellationToken)
    {
        var accessResult = await EnsurePermissionAsync("XayDungVanBanDanhSach", "Index", "Index", cancellationToken);
        return accessResult ?? Ok(await queryService.GetListAsync(request, cancellationToken));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<XayDungVanBanHoSoDetailDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var accessResult = await EnsurePermissionAsync("XayDungVanBanDanhSach", "Index", "Index", cancellationToken);
        if (accessResult is not null)
        {
            return accessResult;
        }

        var result = await queryService.GetByIdAsync(id, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("{id:guid}/timeline")]
    public async Task<ActionResult<IReadOnlyList<XayDungVanBanTimelineItemDto>>> GetTimeline(Guid id, CancellationToken cancellationToken)
    {
        var accessResult = await EnsurePermissionAsync("XayDungVanBanDanhSach", "Index", "Index", cancellationToken);
        if (accessResult is not null)
        {
            return accessResult;
        }

        var result = await queryService.GetTimelineAsync(id, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }
}
