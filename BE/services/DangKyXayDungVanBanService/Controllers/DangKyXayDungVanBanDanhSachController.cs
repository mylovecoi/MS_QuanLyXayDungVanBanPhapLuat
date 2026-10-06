using DangKyXayDungVanBanService.Application.Abstractions;
using DangKyXayDungVanBanService.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace DangKyXayDungVanBanService.Controllers;

[ApiController]
[Route("api/dang-ky-xay-dung-van-ban/danh-sach")]
public class DangKyXayDungVanBanDanhSachController : ControllerBase
{
    private readonly IDangKyXayDungVanBanAppService _appService;

    public DangKyXayDungVanBanDanhSachController(IDangKyXayDungVanBanAppService appService)
    {
        _appService = appService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResultDto<DangKyXayDungVanBanDto>>> GetList(
        [FromQuery] DangKyXayDungVanBanListRequest request,
        CancellationToken cancellationToken)
    {
        return Ok(await _appService.GetListAsync(request, cancellationToken));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<DangKyXayDungVanBanDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _appService.GetByIdAsync(id, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }
}
