using DanhMucService.Contracts.Requests.DanhMuc;
using DanhMucService.Contracts.Responses;
using DanhMucService.Application.Abstractions;
using DanhMucService.Application.DTOs.DanhMuc;
using DanhMucService.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace DanhMucService.Controllers.DanhMuc;

[ApiController]
[Route("api/danh-muc/can-bo")]
public class DanhMucCanBoController(IDanhMucCanBoAppService appService) : ControllerBase
{
    private readonly IDanhMucCanBoAppService _appService = appService;

    [HttpGet]
    public async Task<ActionResult<PagedApiResponse<DanhMucCanBoDto>>> GetPaged(
        [FromQuery] string? search,
        [FromQuery] int pageSize = 5,
        [FromQuery] int pageCurrent = 1,
        [FromQuery] Guid? donViId = null,
        [FromQuery] Guid? phongBanId = null,
        [FromQuery] LoaiLaoDongType? loaiLaoDong = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _appService.GetPagedAsync(search, pageSize, pageCurrent, donViId, phongBanId, loaiLaoDong, cancellationToken);
        return Ok(new PagedApiResponse<DanhMucCanBoDto>
        {
            IsSuccess = true,
            Message = "Lấy danh sách cán bộ thành công.",
            Data = result.Items,
            TotalCount = result.TotalCount,
            PageSize = result.PageSize,
            PageCurrent = result.PageCurrent
        });
    }

    [HttpGet("options")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<DanhMucCanBoDto>>>> GetAll(CancellationToken cancellationToken = default)
    {
        var items = await _appService.GetAllAsync(cancellationToken);
        return Ok(new ApiResponse<IReadOnlyList<DanhMucCanBoDto>>
        {
            IsSuccess = true,
            Message = "Lấy danh sách cán bộ thành công.",
            Data = items
        });
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<DanhMucCanBoDto>>> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _appService.GetByIdAsync(id, cancellationToken);
        if (result == null)
        {
            return NotFound(new ApiResponse { IsSuccess = false, Message = "Không tìm thấy cán bộ." });
        }

        return Ok(new ApiResponse<DanhMucCanBoDto>
        {
            IsSuccess = true,
            Message = "Lấy thông tin cán bộ thành công.",
            Data = result
        });
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<DanhMucCanBoDto>>> Create([FromBody] DanhMucCanBoUpsertApiRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _appService.CreateAsync(ToApplicationRequest(request), cancellationToken);
        if (!result.IsSuccess)
        {
            return BadRequest(new ApiResponse<DanhMucCanBoDto> { IsSuccess = false, Message = result.Message });
        }

        return Ok(new ApiResponse<DanhMucCanBoDto> { IsSuccess = true, Message = result.Message, Data = result.Data });
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<DanhMucCanBoDto>>> Update(Guid id, [FromBody] DanhMucCanBoUpsertApiRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _appService.UpdateAsync(id, ToApplicationRequest(request), cancellationToken);
        if (!result.IsSuccess)
        {
            return BadRequest(new ApiResponse<DanhMucCanBoDto> { IsSuccess = false, Message = result.Message });
        }

        return Ok(new ApiResponse<DanhMucCanBoDto> { IsSuccess = true, Message = result.Message, Data = result.Data });
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse>> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _appService.DeleteAsync(id, cancellationToken);
        if (!result.IsSuccess)
        {
            return NotFound(new ApiResponse { IsSuccess = false, Message = result.Message });
        }

        return Ok(new ApiResponse { IsSuccess = true, Message = result.Message });
    }

    private static UpsertDanhMucCanBoRequest ToApplicationRequest(DanhMucCanBoUpsertApiRequest request)
    {
        return new UpsertDanhMucCanBoRequest
        {
            Id = request.Id,
            DonViQuanLyId = request.DonViQuanLyId,
            TenCanBo = request.TenCanBo,
            NgaySinh = request.NgaySinh,
            UserId = request.UserId,
            PhongBanId = request.PhongBanId,
            GioiTinh = request.GioiTinh,
            TrinhDoChuyenMon = request.TrinhDoChuyenMon,
            LoaiLaoDong = request.LoaiLaoDong,
            SoTienBHXH = request.SoTienBHXH,
            SoTienBHYT = request.SoTienBHYT,
            SoQuyetDinhDung = request.SoQuyetDinhDung,
            NgayQuyetDinhDung = request.NgayQuyetDinhDung,
            GhiChu = request.GhiChu,
            SoQuyetDinhBoNhiem = request.SoQuyetDinhBoNhiem,
            NgayQuyetDinhBoNhiem = request.NgayQuyetDinhBoNhiem,
            SoQuyetDinhCapThe = request.SoQuyetDinhCapThe,
            NgayQuyetDinhCapThe = request.NgayQuyetDinhCapThe,
            SoTheCongChungVien = request.SoTheCongChungVien,
            ChucVu = request.ChucVu,
            MucPhiBaoHiemTrachNhiem = request.MucPhiBaoHiemTrachNhiem,
            ViTriViecLam = request.ViTriViecLam,
            NgayTuyenDung = request.NgayTuyenDung,
            SoHopDongLaoDong = request.SoHopDongLaoDong,
            NgayKyHopDongLaoDong = request.NgayKyHopDongLaoDong
        };
    }
}

