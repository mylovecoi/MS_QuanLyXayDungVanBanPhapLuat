using DangKyXayDungVanBanService.Application.Abstractions;
using DangKyXayDungVanBanService.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace DangKyXayDungVanBanService.Controllers;

[ApiController]
[Route("api/dang-ky-xay-dung-van-ban/ket-qua")]
public class DangKyXayDungVanBanKetQuaController(IDangKyXayDungVanBanAppService appService) : ControllerBase
{
    private readonly IDangKyXayDungVanBanAppService _appService = appService;

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<DangKyXayDungVanBanDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _appService.GetByIdAsync(id, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("{id:guid}/timeline")]
    public async Task<ActionResult<IReadOnlyList<DangKyXayDungVanBanTimelineDto>>> GetTimeline(
        Guid id,
        CancellationToken cancellationToken)
    {
        return Ok(await _appService.GetTimelineAsync(id, cancellationToken));
    }

    [HttpGet("{id:guid}/hanh-dong-kha-dung")]
    public async Task<ActionResult<IReadOnlyList<HanhDongKhaDungDto>>> GetHanhDongKhaDung(
        Guid id,
        CancellationToken cancellationToken)
    {
        return Ok(await _appService.GetHanhDongKhaDungAsync(id, cancellationToken));
    }

    [HttpGet("{id:guid}/files")]
    public async Task<ActionResult<IReadOnlyList<DangKyXayDungVanBanFileDto>>> GetFiles(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _appService.GetFilesAsync(id, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("{id:guid}/xu-ly")]
    public async Task<ActionResult<DangKyXayDungVanBanDto>> XuLy(
        Guid id,
        XuLyDangKyXayDungVanBanRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _appService.XuLyAsync(id, request, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("{id:guid}/ket-qua-phe-duyet")]
    public async Task<ActionResult<Guid>> CapNhatKetQuaPheDuyet(
        Guid id,
        CapNhatKetQuaPheDuyetRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _appService.CapNhatKetQuaPheDuyetAsync(id, request, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }
}
