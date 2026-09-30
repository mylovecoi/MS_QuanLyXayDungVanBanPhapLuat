using DanhMucService.Contracts.Requests.DanhMuc;
using DanhMucService.Contracts.Responses;
using DanhMucService.Application.Abstractions;
using DanhMucService.Application.DTOs.DanhMuc;
using Microsoft.AspNetCore.Mvc;

namespace DanhMucService.Controllers.DanhMuc;

[ApiController]
[Route("api/danh-muc/trang-thai")]
public class DanhMucTrangThaiController(IDanhMucTrangThaiAppService appService) : ControllerBase
{
    private readonly IDanhMucTrangThaiAppService _appService = appService;

    [HttpGet]
    public async Task<ActionResult<PagedApiResponse<DanhMucTrangThaiDto>>> GetPaged(
        [FromQuery] string? search,
        [FromQuery] int pageSize = 5,
        [FromQuery] int pageCurrent = 1,
        CancellationToken cancellationToken = default)
    {
        var result = await _appService.GetPagedAsync(search, pageSize, pageCurrent, cancellationToken);
        return Ok(new PagedApiResponse<DanhMucTrangThaiDto>
        {
            IsSuccess = true,
            Message = "Lấy danh sách trạng thái thành công.",
            Data = result.Items,
            TotalCount = result.TotalCount,
            PageSize = result.PageSize,
            PageCurrent = result.PageCurrent
        });
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<DanhMucTrangThaiDto>>> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _appService.GetByIdAsync(id, cancellationToken);
        if (result == null)
        {
            return NotFound(new ApiResponse { IsSuccess = false, Message = "Không tìm thấy trạng thái." });
        }

        return Ok(new ApiResponse<DanhMucTrangThaiDto>
        {
            IsSuccess = true,
            Message = "Lấy thông tin trạng thái thành công.",
            Data = result
        });
    }

    [HttpGet("next-sort-order")]
    public async Task<ActionResult<ApiResponse<int>>> GetNextSortOrder(CancellationToken cancellationToken = default)
    {
        var nextValue = await _appService.GetNextSortOrderAsync(cancellationToken);
        return Ok(new ApiResponse<int>
        {
            IsSuccess = true,
            Message = "Lấy thứ tự sắp xếp tiếp theo thành công.",
            Data = nextValue
        });
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<DanhMucTrangThaiDto>>> Create(
        [FromBody] DanhMucTrangThaiUpsertApiRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await _appService.CreateAsync(ToApplicationRequest(request), cancellationToken);
        if (!result.IsSuccess)
        {
            return BadRequest(new ApiResponse<DanhMucTrangThaiDto> { IsSuccess = false, Message = result.Message });
        }

        return Ok(new ApiResponse<DanhMucTrangThaiDto>
        {
            IsSuccess = true,
            Message = result.Message,
            Data = result.Data
        });
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<DanhMucTrangThaiDto>>> Update(
        Guid id,
        [FromBody] DanhMucTrangThaiUpsertApiRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await _appService.UpdateAsync(id, ToApplicationRequest(request), cancellationToken);
        if (!result.IsSuccess)
        {
            return BadRequest(new ApiResponse<DanhMucTrangThaiDto> { IsSuccess = false, Message = result.Message });
        }

        return Ok(new ApiResponse<DanhMucTrangThaiDto>
        {
            IsSuccess = true,
            Message = result.Message,
            Data = result.Data
        });
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

    private static UpsertDanhMucTrangThaiRequest ToApplicationRequest(DanhMucTrangThaiUpsertApiRequest request)
    {
        return new UpsertDanhMucTrangThaiRequest
        {
            MaTrangThai = request.MaTrangThai,
            TenTrangThai = request.TenTrangThai,
            MaMauHex = request.MaMauHex,
            ThuTuSapXep = request.ThuTuSapXep,
            TrangThai = request.TrangThai,
            MoTa = request.MoTa,
            GhiChu = request.GhiChu
        };
    }
}

