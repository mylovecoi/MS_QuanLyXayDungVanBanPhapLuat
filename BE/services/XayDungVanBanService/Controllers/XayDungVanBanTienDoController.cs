using BuildingBlocks.Abstractions;
using Microsoft.AspNetCore.Mvc;
using XayDungVanBanService.Application.Abstractions;
using XayDungVanBanService.Application.DTOs;
using XayDungVanBanService.Infrastructure.Authorization;

namespace XayDungVanBanService.Controllers;

[ApiController]
[Route("api/xay-dung-van-ban/tien-do")]
public sealed class XayDungVanBanTienDoController(
    ICurrentUserContext user,
    IQuanTriHeThongPermissionClient permissionClient,
    IXayDungVanBanTienDoService tienDoService) : XayDungVanBanControllerBase(user, permissionClient)
{
    [HttpGet]
    public async Task<ActionResult<PagedResultDto<XayDungVanBanTienDoListItemDto>>> GetList(
        [FromQuery] XayDungVanBanTienDoListRequest request,
        CancellationToken cancellationToken)
    {
        var accessResult = await EnsurePermissionAsync("XayDungVanBanTienDo", "Index", "Index", cancellationToken);
        return accessResult ?? Ok(await tienDoService.GetListAsync(request, cancellationToken));
    }

    [HttpGet("{hoSoId:guid}")]
    public async Task<ActionResult<XayDungVanBanTienDoDetailDto>> GetById(
        Guid hoSoId,
        CancellationToken cancellationToken)
    {
        var accessResult = await EnsurePermissionAsync("XayDungVanBanTienDo", "Index", "Index", cancellationToken);
        if (accessResult is not null)
        {
            return accessResult;
        }

        var result = await tienDoService.GetByIdAsync(hoSoId, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("{hoSoId:guid}/nhac-nho")]
    public async Task<ActionResult<IReadOnlyList<XayDungVanBanNhacTienDoDto>>> GetNhacNho(
        Guid hoSoId,
        CancellationToken cancellationToken)
    {
        var accessResult = await EnsurePermissionAsync("XayDungVanBanTienDo", "Index", "Index", cancellationToken);
        if (accessResult is not null)
        {
            return accessResult;
        }

        var result = await tienDoService.GetNhacNhoAsync(hoSoId, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("{hoSoId:guid}/nhac-nho")]
    public async Task<ActionResult<XayDungVanBanNhacTienDoDto>> TaoNhacNho(
        Guid hoSoId,
        [FromBody] TaoNhacTienDoRequest request,
        CancellationToken cancellationToken)
    {
        var accessResult = await EnsurePermissionAsync("XayDungVanBanTienDo", "Index", "Create", cancellationToken);
        if (accessResult is not null)
        {
            return accessResult;
        }

        try
        {
            var result = await tienDoService.TaoNhacNhoAsync(hoSoId, request, cancellationToken);
            return result is null
                ? NotFound()
                : CreatedAtAction(nameof(GetNhacNho), new { hoSoId }, result);
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(exception.Message);
        }
    }

    [HttpPut("{hoSoId:guid}/nhac-nho/{nhacNhoId:guid}/phan-hoi")]
    public async Task<ActionResult<XayDungVanBanNhacTienDoDto>> PhanHoi(
        Guid hoSoId,
        Guid nhacNhoId,
        [FromBody] CapNhatPhanHoiNhacTienDoRequest request,
        CancellationToken cancellationToken)
    {
        var accessResult = await EnsurePermissionAsync("XayDungVanBanTienDo", "Index", "Edit", cancellationToken);
        if (accessResult is not null)
        {
            return accessResult;
        }

        try
        {
            var result = await tienDoService.PhanHoiAsync(hoSoId, nhacNhoId, request, cancellationToken);
            return result is null ? NotFound() : Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(exception.Message);
        }
    }

    [HttpPost("{hoSoId:guid}/nhac-nho/{nhacNhoId:guid}/xac-nhan-xu-ly")]
    public async Task<ActionResult<XayDungVanBanNhacTienDoDto>> XacNhanXuLy(
        Guid hoSoId,
        Guid nhacNhoId,
        [FromBody] XacNhanXuLyNhacTienDoRequest request,
        CancellationToken cancellationToken)
    {
        var accessResult = await EnsurePermissionAsync("XayDungVanBanTienDo", "Index", "Approve", cancellationToken);
        if (accessResult is not null)
        {
            return accessResult;
        }

        try
        {
            var result = await tienDoService.XacNhanXuLyAsync(hoSoId, nhacNhoId, request, cancellationToken);
            return result is null ? NotFound() : Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }
}
