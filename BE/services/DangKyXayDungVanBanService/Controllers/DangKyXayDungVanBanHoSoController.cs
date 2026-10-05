using DangKyXayDungVanBanService.Application.Abstractions;
using DangKyXayDungVanBanService.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace DangKyXayDungVanBanService.Controllers;

[ApiController]
[Route("api/dang-ky-xay-dung-van-ban/ho-so")]
public class DangKyXayDungVanBanHoSoController(IDangKyXayDungVanBanAppService appService) : ControllerBase
{
    private readonly IDangKyXayDungVanBanAppService _appService = appService;

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<DangKyXayDungVanBanDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _appService.GetByIdAsync(id, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<DangKyXayDungVanBanDto>> Create(
        TaoDangKyXayDungVanBanRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _appService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<DangKyXayDungVanBanDto>> Update(
        Guid id,
        CapNhatDangKyXayDungVanBanRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _appService.UpdateAsync(id, request, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _appService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
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

    [HttpPost("{id:guid}/files")]
    [RequestSizeLimit(100_000_000)]
    public async Task<ActionResult<DangKyXayDungVanBanFileDto>> UploadFile(
        Guid id,
        IFormFile file,
        [FromForm] string loaiFile,
        [FromForm] string? moTa,
        CancellationToken cancellationToken)
    {
        if (file.Length == 0)
        {
            return BadRequest("File tải lên không có nội dung.");
        }

        await using var stream = file.OpenReadStream();
        var request = new TaiFileDangKyXayDungVanBanRequest(
            loaiFile,
            file.FileName,
            file.ContentType,
            moTa,
            stream);

        var result = await _appService.UploadFileAsync(id, request, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:guid}/files/{fileId:guid}")]
    public async Task<IActionResult> DeleteFile(Guid id, Guid fileId, CancellationToken cancellationToken)
    {
        var deleted = await _appService.DeleteFileAsync(id, fileId, cancellationToken);
        return deleted ? NoContent() : NotFound();
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

    [HttpPost("{id:guid}/khoi-tao-quy-trinh-xay-dung")]
    public async Task<ActionResult<DangKyXayDungVanBanDto>> KhoiTaoQuyTrinhXayDung(
        Guid id,
        KhoiTaoQuyTrinhXayDungRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _appService.KhoiTaoQuyTrinhXayDungAsync(id, request, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }
}
