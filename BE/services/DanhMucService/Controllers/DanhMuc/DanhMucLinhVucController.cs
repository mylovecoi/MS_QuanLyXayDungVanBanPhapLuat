using DanhMucService.Contracts.Requests.DanhMuc;
using DanhMucService.Contracts.Responses;
using DanhMucService.Application.Abstractions;
using DanhMucService.Application.DTOs.DanhMuc;
using Microsoft.AspNetCore.Mvc;

namespace DanhMucService.Controllers.DanhMuc;

[ApiController]
[Route("api/danh-muc/linh-vuc")]
public class DanhMucLinhVucController(IDanhMucLinhVucAppService appService) : ControllerBase
{
    private readonly IDanhMucLinhVucAppService _appService = appService;

    [HttpGet]
    public async Task<ActionResult<PagedApiResponse<DanhMucLinhVucDto>>> GetPaged([FromQuery] string? search, [FromQuery] int pageSize = 5, [FromQuery] int pageCurrent = 1, CancellationToken cancellationToken = default)
    {
        var result = await _appService.GetPagedAsync(search, pageSize, pageCurrent, cancellationToken);
        return Ok(new PagedApiResponse<DanhMucLinhVucDto> { IsSuccess = true, Message = "Lấy danh sách lĩnh vực thành công.", Data = result.Items, TotalCount = result.TotalCount, PageSize = result.PageSize, PageCurrent = result.PageCurrent });
    }

    [HttpGet("options")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<DanhMucLinhVucDto>>>> GetAll(CancellationToken cancellationToken = default)
    {
        var items = await _appService.GetAllAsync(cancellationToken);
        return Ok(new ApiResponse<IReadOnlyList<DanhMucLinhVucDto>> { IsSuccess = true, Message = "Lấy danh sách lĩnh vực thành công.", Data = items });
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<DanhMucLinhVucDto>>> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _appService.GetByIdAsync(id, cancellationToken);
        if (result == null) return NotFound(new ApiResponse { IsSuccess = false, Message = "Không tìm thấy lĩnh vực." });
        return Ok(new ApiResponse<DanhMucLinhVucDto> { IsSuccess = true, Message = "Lấy thông tin lĩnh vực thành công.", Data = result });
    }

    [HttpGet("next-sort-order")]
    public async Task<ActionResult<ApiResponse<int>>> GetNextSortOrder(CancellationToken cancellationToken = default)
    {
        var next = await _appService.GetNextSortOrderAsync(cancellationToken);
        return Ok(new ApiResponse<int> { IsSuccess = true, Message = "Lấy thứ tự sắp xếp tiếp theo thành công.", Data = next });
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<DanhMucLinhVucDto>>> Create([FromBody] DanhMucLinhVucUpsertApiRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _appService.CreateAsync(ToApplicationRequest(request), cancellationToken);
        if (!result.IsSuccess) return BadRequest(new ApiResponse<DanhMucLinhVucDto> { IsSuccess = false, Message = result.Message });
        return Ok(new ApiResponse<DanhMucLinhVucDto> { IsSuccess = true, Message = result.Message, Data = result.Data });
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<DanhMucLinhVucDto>>> Update(Guid id, [FromBody] DanhMucLinhVucUpsertApiRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _appService.UpdateAsync(id, ToApplicationRequest(request), cancellationToken);
        if (!result.IsSuccess) return BadRequest(new ApiResponse<DanhMucLinhVucDto> { IsSuccess = false, Message = result.Message });
        return Ok(new ApiResponse<DanhMucLinhVucDto> { IsSuccess = true, Message = result.Message, Data = result.Data });
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse>> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _appService.DeleteAsync(id, cancellationToken);
        if (!result.IsSuccess) return NotFound(new ApiResponse { IsSuccess = false, Message = result.Message });
        return Ok(new ApiResponse { IsSuccess = true, Message = result.Message });
    }

    private static UpsertDanhMucLinhVucRequest ToApplicationRequest(DanhMucLinhVucUpsertApiRequest request)
        => new()
        {
            MaLinhVuc = request.MaLinhVuc,
            TenLinhVuc = request.TenLinhVuc,
            ThuTuSapXep = request.ThuTuSapXep,
            TrangThai = request.TrangThai,
            MoTa = request.MoTa,
            GhiChu = request.GhiChu
        };
}

