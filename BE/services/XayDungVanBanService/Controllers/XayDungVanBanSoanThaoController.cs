using BuildingBlocks.Abstractions;
using Microsoft.AspNetCore.Mvc;
using XayDungVanBanService.Application.Abstractions;
using XayDungVanBanService.Application.DTOs;
using XayDungVanBanService.Infrastructure.Authorization;

namespace XayDungVanBanService.Controllers;

[ApiController]
[Route("api/xay-dung-van-ban/soan-thao")]
public sealed class XayDungVanBanSoanThaoController(
    ICurrentUserContext user,
    IQuanTriHeThongPermissionClient permissionClient,
    IXayDungVanBanSoanThaoService soanThaoService) : XayDungVanBanControllerBase(user, permissionClient)
{
    [HttpGet]
    public Task<ActionResult> Get(CancellationToken cancellationToken) =>
        IndexAsync("XayDungVanBanSoanThao", "VanBanQPPL.XayDungVanBan.SoanThao", cancellationToken);

    [HttpPost]
    public async Task<ActionResult<XayDungVanBanSoanThaoDto>> Create(
        [FromBody] TaoHoSoSoanThaoRequest request,
        CancellationToken cancellationToken)
    {
        var accessResult = await EnsurePermissionAsync("XayDungVanBanSoanThao", "Create", "Create", cancellationToken);
        if (accessResult is not null)
        {
            return accessResult;
        }

        try
        {
            var result = await soanThaoService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { hoSoId = result.HoSoId }, result);
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(exception.Message);
        }
    }

    [HttpGet("{hoSoId:guid}")]
    public async Task<ActionResult<XayDungVanBanSoanThaoDto>> GetById(Guid hoSoId, CancellationToken cancellationToken)
    {
        var accessResult = await EnsurePermissionAsync("XayDungVanBanSoanThao", "Index", "Index", cancellationToken);
        if (accessResult is not null)
        {
            return accessResult;
        }

        var result = await soanThaoService.GetByIdAsync(hoSoId, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPut("{hoSoId:guid}")]
    public async Task<ActionResult<XayDungVanBanSoanThaoDto>> Update(
        Guid hoSoId,
        [FromBody] CapNhatHoSoSoanThaoRequest request,
        CancellationToken cancellationToken)
    {
        var accessResult = await EnsurePermissionAsync("XayDungVanBanSoanThao", "Edit", "Edit", cancellationToken);
        if (accessResult is not null)
        {
            return accessResult;
        }

        try
        {
            var result = await soanThaoService.UpdateAsync(hoSoId, request, cancellationToken);
            return result is null ? NotFound() : Ok(result);
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(exception.Message);
        }
    }

    [HttpDelete("{hoSoId:guid}")]
    public async Task<IActionResult> Delete(Guid hoSoId, CancellationToken cancellationToken)
    {
        var accessResult = await EnsurePermissionAsync("XayDungVanBanSoanThao", "Delete", "Delete", cancellationToken);
        if (accessResult is not null)
        {
            return accessResult;
        }

        try
        {
            return await soanThaoService.DeleteAsync(hoSoId, cancellationToken) ? NoContent() : NotFound();
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(exception.Message);
        }
    }

    [HttpGet("{hoSoId:guid}/y-kien-don-vi")]
    public async Task<ActionResult<IReadOnlyList<XayDungVanBanYKienDonViDto>>> GetYKienDonVi(Guid hoSoId, CancellationToken cancellationToken)
    {
        var accessResult = await EnsurePermissionAsync("XayDungVanBanSoanThao", "Index", "Index", cancellationToken);
        if (accessResult is not null)
        {
            return accessResult;
        }

        var result = await soanThaoService.GetYKienDonViAsync(hoSoId, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("{hoSoId:guid}/y-kien-don-vi")]
    public async Task<ActionResult<XayDungVanBanYKienDonViDto>> CreateYKienDonVi(
        Guid hoSoId,
        [FromBody] TaoYKienDonViRequest request,
        CancellationToken cancellationToken)
    {
        var accessResult = await EnsurePermissionAsync("XayDungVanBanSoanThao", "Create", "Create", cancellationToken);
        if (accessResult is not null)
        {
            return accessResult;
        }

        try
        {
            var result = await soanThaoService.CreateYKienDonViAsync(hoSoId, request, cancellationToken);
            return result is null
                ? NotFound()
                : CreatedAtAction(nameof(GetYKienDonVi), new { hoSoId }, result);
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(exception.Message);
        }
    }

    [HttpPut("{hoSoId:guid}/y-kien-don-vi/{id:guid}")]
    public async Task<ActionResult<XayDungVanBanYKienDonViDto>> UpdateYKienDonVi(
        Guid hoSoId,
        Guid id,
        [FromBody] CapNhatYKienDonViRequest request,
        CancellationToken cancellationToken)
    {
        var accessResult = await EnsurePermissionAsync("XayDungVanBanSoanThao", "Edit", "Edit", cancellationToken);
        if (accessResult is not null)
        {
            return accessResult;
        }

        try
        {
            var result = await soanThaoService.UpdateYKienDonViAsync(hoSoId, id, request, cancellationToken);
            return result is null ? NotFound() : Ok(result);
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(exception.Message);
        }
    }

    [HttpDelete("{hoSoId:guid}/y-kien-don-vi/{id:guid}")]
    public async Task<IActionResult> DeleteYKienDonVi(Guid hoSoId, Guid id, CancellationToken cancellationToken)
    {
        var accessResult = await EnsurePermissionAsync("XayDungVanBanSoanThao", "Delete", "Delete", cancellationToken);
        if (accessResult is not null)
        {
            return accessResult;
        }

        try
        {
            return await soanThaoService.DeleteYKienDonViAsync(hoSoId, id, cancellationToken) ? NoContent() : NotFound();
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(exception.Message);
        }
    }

    [HttpGet("{hoSoId:guid}/tong-hop-y-kien")]
    public async Task<ActionResult<XayDungVanBanTongHopYKienDto>> GetTongHopYKien(Guid hoSoId, CancellationToken cancellationToken)
    {
        var accessResult = await EnsurePermissionAsync("XayDungVanBanSoanThao", "Index", "Index", cancellationToken);
        if (accessResult is not null)
        {
            return accessResult;
        }

        var result = await soanThaoService.GetTongHopYKienAsync(hoSoId, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPut("{hoSoId:guid}/tong-hop-y-kien")]
    public async Task<ActionResult<XayDungVanBanTongHopYKienDto>> UpdateTongHopYKien(
        Guid hoSoId,
        [FromBody] CapNhatTongHopYKienRequest request,
        CancellationToken cancellationToken)
    {
        var accessResult = await EnsurePermissionAsync("XayDungVanBanSoanThao", "Edit", "Edit", cancellationToken);
        if (accessResult is not null)
        {
            return accessResult;
        }

        try
        {
            var result = await soanThaoService.UpdateTongHopYKienAsync(hoSoId, request, cancellationToken);
            return result is null ? NotFound() : Ok(result);
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(exception.Message);
        }
    }

    [HttpGet("{hoSoId:guid}/file-tong-hop-y-kien")]
    public async Task<ActionResult<IReadOnlyList<XayDungVanBanTaiLieuDto>>> GetFileTongHopYKien(Guid hoSoId, CancellationToken cancellationToken)
    {
        var accessResult = await EnsurePermissionAsync("XayDungVanBanSoanThao", "Index", "Index", cancellationToken);
        if (accessResult is not null)
        {
            return accessResult;
        }

        var result = await soanThaoService.GetFileTongHopYKienAsync(hoSoId, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("{hoSoId:guid}/file-tong-hop-y-kien")]
    [RequestSizeLimit(100_000_000)]
    public async Task<ActionResult<XayDungVanBanTaiLieuDto>> UploadFileTongHopYKien(
        Guid hoSoId,
        [FromForm] IFormFile file,
        [FromForm] Guid loaiTaiLieuId,
        CancellationToken cancellationToken)
    {
        if (file.Length == 0)
        {
            return BadRequest("File tải lên không có nội dung.");
        }

        var accessResult = await EnsurePermissionAsync("XayDungVanBanSoanThao", "Create", "Create", cancellationToken);
        if (accessResult is not null)
        {
            return accessResult;
        }

        try
        {
            await using var stream = file.OpenReadStream();
            var result = await soanThaoService.UploadFileTongHopYKienAsync(
                hoSoId,
                new TaiFileTongHopYKienRequest(loaiTaiLieuId, file.FileName, file.ContentType, stream),
                cancellationToken);
            return result is null ? NotFound() : Ok(result);
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(exception.Message);
        }
    }

    [HttpGet("{hoSoId:guid}/tai-lieu")]
    public async Task<ActionResult<IReadOnlyList<XayDungVanBanTaiLieuDto>>> GetTaiLieu(Guid hoSoId, CancellationToken cancellationToken)
    {
        var accessResult = await EnsurePermissionAsync("XayDungVanBanSoanThao", "Index", "Index", cancellationToken);
        if (accessResult is not null) return accessResult;

        var result = await soanThaoService.GetTaiLieuAsync(hoSoId, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("{hoSoId:guid}/tai-lieu")]
    [RequestSizeLimit(100_000_000)]
    public async Task<ActionResult<XayDungVanBanTaiLieuDto>> UploadTaiLieu(
        Guid hoSoId,
        [FromForm] IFormFile file,
        [FromForm] Guid loaiTaiLieuId,
        [FromForm] string tenTaiLieu,
        CancellationToken cancellationToken)
    {
        if (file.Length == 0) return BadRequest("File tải lên không có nội dung.");

        var accessResult = await EnsurePermissionAsync("XayDungVanBanSoanThao", "Create", "Create", cancellationToken);
        if (accessResult is not null) return accessResult;

        try
        {
            await using var stream = file.OpenReadStream();
            var result = await soanThaoService.UploadTaiLieuAsync(
                hoSoId,
                new TaiTaiLieuSoanThaoRequest(loaiTaiLieuId, tenTaiLieu, file.FileName, file.ContentType, stream),
                cancellationToken);
            return result is null ? NotFound() : Ok(result);
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(exception.Message);
        }
    }

    [HttpDelete("{hoSoId:guid}/tai-lieu/{fileId:guid}")]
    public async Task<IActionResult> DeleteTaiLieu(Guid hoSoId, Guid fileId, CancellationToken cancellationToken)
    {
        var accessResult = await EnsurePermissionAsync("XayDungVanBanSoanThao", "Delete", "Delete", cancellationToken);
        if (accessResult is not null) return accessResult;

        try
        {
            return await soanThaoService.DeleteTaiLieuAsync(hoSoId, fileId, cancellationToken) ? NoContent() : NotFound();
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(exception.Message);
        }
    }

    [HttpGet("{hoSoId:guid}/kiem-tra-truoc-trinh-tham-dinh")]
    public async Task<ActionResult<DieuKienTrinhThamDinhDto>> KiemTraTruocTrinhThamDinh(Guid hoSoId, CancellationToken cancellationToken)
    {
        var accessResult = await EnsurePermissionAsync("XayDungVanBanSoanThao", "Index", "Index", cancellationToken);
        if (accessResult is not null) return accessResult;

        var result = await soanThaoService.KiemTraTruocTrinhThamDinhAsync(hoSoId, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("{hoSoId:guid}/trinh-tham-dinh")]
    public async Task<ActionResult<TrinhThamDinhDto>> TrinhThamDinh(
        Guid hoSoId,
        [FromBody] TrinhThamDinhRequest request,
        CancellationToken cancellationToken)
    {
        var accessResult = await EnsurePermissionAsync("XayDungVanBanSoanThao", "Approve", "Approve", cancellationToken);
        if (accessResult is not null) return accessResult;

        try
        {
            var result = await soanThaoService.TrinhThamDinhAsync(hoSoId, request, cancellationToken);
            return result is null ? NotFound() : Ok(result);
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(exception.Message);
        }
    }

}
