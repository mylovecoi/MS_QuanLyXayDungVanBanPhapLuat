using DanhMucService.Contracts.Requests.DanhMuc;
using DanhMucService.Contracts.Responses;
using DanhMucService.Application.Abstractions;
using DanhMucService.Application.DTOs.DanhMuc;
using Microsoft.AspNetCore.Mvc;

namespace DanhMucService.Controllers.DanhMuc;

[ApiController]
[Route("api/danh-muc/dia-danh")]
public class DanhMucDiaDanhController(IDanhMucDiaDanhAppService appService) : ControllerBase
{
    private readonly IDanhMucDiaDanhAppService _appService = appService;

    [HttpGet]
    public async Task<ActionResult<PagedApiResponse<DanhMucDiaDanhDto>>> GetPaged([FromQuery] string? search, [FromQuery] int pageSize = 5, [FromQuery] int pageCurrent = 1, CancellationToken cancellationToken = default)
    {
        var result = await _appService.GetPagedAsync(search, pageSize, pageCurrent, cancellationToken);
        return Ok(new PagedApiResponse<DanhMucDiaDanhDto> { IsSuccess = true, Message = "Lấy danh sách địa danh thành công.", Data = result.Items, TotalCount = result.TotalCount, PageSize = result.PageSize, PageCurrent = result.PageCurrent });
    }

    [HttpGet("options")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<DanhMucDiaDanhDto>>>> GetAll(CancellationToken cancellationToken = default)
    {
        var items = await _appService.GetAllAsync(cancellationToken);
        return Ok(new ApiResponse<IReadOnlyList<DanhMucDiaDanhDto>> { IsSuccess = true, Message = "Lấy danh sách địa danh thành công.", Data = items });
    }

    [HttpGet("children/{parentId:guid}")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<DanhMucDiaDanhDto>>>> GetChildren(Guid parentId, CancellationToken cancellationToken = default)
    {
        var items = await _appService.GetChildrenAsync(parentId, cancellationToken);
        return Ok(new ApiResponse<IReadOnlyList<DanhMucDiaDanhDto>> { IsSuccess = true, Message = "Lấy danh sách địa danh con thành công.", Data = items });
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<DanhMucDiaDanhDto>>> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _appService.GetByIdAsync(id, cancellationToken);
        if (result == null) return NotFound(new ApiResponse { IsSuccess = false, Message = "Không tìm thấy địa danh." });
        return Ok(new ApiResponse<DanhMucDiaDanhDto> { IsSuccess = true, Message = "Lấy thông tin địa danh thành công.", Data = result });
    }

    [HttpGet("next-sort-order")]
    public async Task<ActionResult<ApiResponse<int>>> GetNextSortOrder([FromQuery] Guid parentId, CancellationToken cancellationToken = default)
    {
        var next = await _appService.GetNextSortOrderAsync(parentId, cancellationToken);
        return Ok(new ApiResponse<int> { IsSuccess = true, Message = "Lấy thứ tự sắp xếp tiếp theo thành công.", Data = next });
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<DanhMucDiaDanhDto>>> Create([FromBody] DanhMucDiaDanhUpsertApiRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _appService.CreateAsync(ToApplicationRequest(request), cancellationToken);
        if (!result.IsSuccess) return BadRequest(new ApiResponse<DanhMucDiaDanhDto> { IsSuccess = false, Message = result.Message });
        return Ok(new ApiResponse<DanhMucDiaDanhDto> { IsSuccess = true, Message = result.Message, Data = result.Data });
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<DanhMucDiaDanhDto>>> Update(Guid id, [FromBody] DanhMucDiaDanhUpsertApiRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _appService.UpdateAsync(id, ToApplicationRequest(request), cancellationToken);
        if (!result.IsSuccess) return BadRequest(new ApiResponse<DanhMucDiaDanhDto> { IsSuccess = false, Message = result.Message });
        return Ok(new ApiResponse<DanhMucDiaDanhDto> { IsSuccess = true, Message = result.Message, Data = result.Data });
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse>> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _appService.DeleteAsync(id, cancellationToken);
        if (!result.IsSuccess) return NotFound(new ApiResponse { IsSuccess = false, Message = result.Message });
        return Ok(new ApiResponse { IsSuccess = true, Message = result.Message });
    }

    private static UpsertDanhMucDiaDanhRequest ToApplicationRequest(DanhMucDiaDanhUpsertApiRequest request)
        => new()
        {
            TenDiaDanh = request.TenDiaDanh,
            Level = request.Level,
            STTSapXep = request.STTSapXep,
            DiaDanhCapTrenId = request.DiaDanhCapTrenId
        };
}

