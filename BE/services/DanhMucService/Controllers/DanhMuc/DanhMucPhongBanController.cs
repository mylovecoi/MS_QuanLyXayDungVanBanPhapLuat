using DanhMucService.Contracts.Requests.DanhMuc;
using DanhMucService.Contracts.Responses;
using DanhMucService.Application.Abstractions;
using DanhMucService.Application.DTOs.DanhMuc;
using Microsoft.AspNetCore.Mvc;

namespace DanhMucService.Controllers.DanhMuc;

[ApiController]
[Route("api/danh-muc/phong-ban")]
public class DanhMucPhongBanController(IDanhMucPhongBanAppService appService) : ControllerBase
{
    private readonly IDanhMucPhongBanAppService _appService = appService;

    [HttpGet]
    public async Task<ActionResult<PagedApiResponse<DanhMucPhongBanDto>>> GetPaged(
        [FromQuery] string? search,
        [FromQuery] int pageSize = 5,
        [FromQuery] int pageCurrent = 1,
        [FromQuery] Guid? donViId = null,
        [FromQuery] int? loaiPhongBan = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _appService.GetPagedAsync(search, pageSize, pageCurrent, donViId, loaiPhongBan, cancellationToken);
        return Ok(new PagedApiResponse<DanhMucPhongBanDto>
        {
            IsSuccess = true,
            Message = "Lấy danh sách phòng ban thành công.",
            Data = result.Items,
            TotalCount = result.TotalCount,
            PageSize = result.PageSize,
            PageCurrent = result.PageCurrent
        });
    }

    [HttpGet("options")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<DanhMucPhongBanDto>>>> GetAll(CancellationToken cancellationToken = default)
    {
        var items = await _appService.GetAllAsync(cancellationToken);
        return Ok(new ApiResponse<IReadOnlyList<DanhMucPhongBanDto>>
        {
            IsSuccess = true,
            Message = "Lấy danh sách phòng ban thành công.",
            Data = items
        });
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<DanhMucPhongBanDto>>> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _appService.GetByIdAsync(id, cancellationToken);
        if (result == null)
        {
            return NotFound(new ApiResponse { IsSuccess = false, Message = "Không tìm thấy phòng ban." });
        }

        return Ok(new ApiResponse<DanhMucPhongBanDto>
        {
            IsSuccess = true,
            Message = "Lấy thông tin phòng ban thành công.",
            Data = result
        });
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<DanhMucPhongBanDto>>> Create([FromBody] DanhMucPhongBanUpsertApiRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _appService.CreateAsync(ToApplicationRequest(request), cancellationToken);
        if (!result.IsSuccess)
        {
            return BadRequest(new ApiResponse<DanhMucPhongBanDto> { IsSuccess = false, Message = result.Message });
        }

        return Ok(new ApiResponse<DanhMucPhongBanDto> { IsSuccess = true, Message = result.Message, Data = result.Data });
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<DanhMucPhongBanDto>>> Update(Guid id, [FromBody] DanhMucPhongBanUpsertApiRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _appService.UpdateAsync(id, ToApplicationRequest(request), cancellationToken);
        if (!result.IsSuccess)
        {
            return BadRequest(new ApiResponse<DanhMucPhongBanDto> { IsSuccess = false, Message = result.Message });
        }

        return Ok(new ApiResponse<DanhMucPhongBanDto> { IsSuccess = true, Message = result.Message, Data = result.Data });
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

    private static UpsertDanhMucPhongBanRequest ToApplicationRequest(DanhMucPhongBanUpsertApiRequest request)
    {
        return new UpsertDanhMucPhongBanRequest
        {
            TenPhongBan = request.TenPhongBan,
            MaPhongBan = request.MaPhongBan,
            LoaiPhongBan = request.LoaiPhongBan,
            DanhMucDonViId = request.DanhMucDonViId
        };
    }
}

