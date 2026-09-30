using DanhMucService.Contracts.Requests.DanhMuc;
using DanhMucService.Contracts.Responses;
using DanhMucService.Application.Abstractions;
using DanhMucService.Application.DTOs.DanhMuc;
using Microsoft.AspNetCore.Mvc;

namespace DanhMucService.Controllers.DanhMuc;

[ApiController]
[Route("api/danh-muc/van-ban")]
public class DanhMucVanBanController(IDanhMucVanBanAppService appService) : ControllerBase
{
    private readonly IDanhMucVanBanAppService _appService = appService;

    [HttpGet]
    public async Task<ActionResult<PagedApiResponse<DanhMucVanBanDto>>> GetPaged([FromQuery] string? search, [FromQuery] int pageSize = 5, [FromQuery] int pageCurrent = 1, CancellationToken cancellationToken = default)
    {
        var result = await _appService.GetPagedAsync(search, pageSize, pageCurrent, cancellationToken);
        return Ok(new PagedApiResponse<DanhMucVanBanDto>
        {
            IsSuccess = true,
            Message = "Lấy danh sách loại văn bản thành công.",
            Data = result.Items,
            TotalCount = result.TotalCount,
            PageSize = result.PageSize,
            PageCurrent = result.PageCurrent
        });
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<DanhMucVanBanDto>>> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _appService.GetByIdAsync(id, cancellationToken);
        if (result == null)
        {
            return NotFound(new ApiResponse { IsSuccess = false, Message = "Không tìm thấy loại văn bản." });
        }

        return Ok(new ApiResponse<DanhMucVanBanDto>
        {
            IsSuccess = true,
            Message = "Lấy thông tin loại văn bản thành công.",
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
    public async Task<ActionResult<ApiResponse<DanhMucVanBanDto>>> Create([FromBody] DanhMucVanBanUpsertApiRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _appService.CreateAsync(ToApplicationRequest(request), cancellationToken);
        if (!result.IsSuccess)
        {
            return BadRequest(new ApiResponse<DanhMucVanBanDto> { IsSuccess = false, Message = result.Message });
        }

        return Ok(new ApiResponse<DanhMucVanBanDto>
        {
            IsSuccess = true,
            Message = result.Message,
            Data = result.Data
        });
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<DanhMucVanBanDto>>> Update(Guid id, [FromBody] DanhMucVanBanUpsertApiRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _appService.UpdateAsync(id, ToApplicationRequest(request), cancellationToken);
        if (!result.IsSuccess)
        {
            return BadRequest(new ApiResponse<DanhMucVanBanDto> { IsSuccess = false, Message = result.Message });
        }

        return Ok(new ApiResponse<DanhMucVanBanDto>
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

    private static UpsertDanhMucVanBanRequest ToApplicationRequest(DanhMucVanBanUpsertApiRequest request)
    {
        return new UpsertDanhMucVanBanRequest
        {
            TenLoaiVanBan = request.TenLoaiVanBan,
            CapChinhQuyen = request.CapChinhQuyen,
            ChuTheBanHanh = request.ChuTheBanHanh,
            KyHieuMau = request.KyHieuMau,
            ThuTuSapXep = request.ThuTuSapXep,
            TrangThai = request.TrangThai,
            MoTa = request.MoTa,
            GhiChu = request.GhiChu
        };
    }
}

