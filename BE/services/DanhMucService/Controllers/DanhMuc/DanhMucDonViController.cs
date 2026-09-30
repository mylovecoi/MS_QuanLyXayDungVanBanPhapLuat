using DanhMucService.Contracts.Requests.DanhMuc;
using DanhMucService.Contracts.Responses;
using DanhMucService.Application.Abstractions;
using DanhMucService.Application.DTOs.DanhMuc;
using Microsoft.AspNetCore.Mvc;

namespace DanhMucService.Controllers.DanhMuc;

[ApiController]
[Route("api/danh-muc/don-vi")]
public class DanhMucDonViController(IDanhMucDonViAppService appService) : ControllerBase
{
    private readonly IDanhMucDonViAppService _appService = appService;

    [HttpGet]
    public async Task<ActionResult<PagedApiResponse<DanhMucDonViDto>>> GetPaged([FromQuery] string? search, [FromQuery] int pageSize = 5, [FromQuery] int pageCurrent = 1, CancellationToken cancellationToken = default)
    {
        var result = await _appService.GetPagedAsync(search, pageSize, pageCurrent, cancellationToken);
        return Ok(new PagedApiResponse<DanhMucDonViDto> { IsSuccess = true, Message = "Lấy danh sách đơn vị thành công.", Data = result.Items, TotalCount = result.TotalCount, PageSize = result.PageSize, PageCurrent = result.PageCurrent });
    }

    [HttpGet("options")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<DanhMucDonViDto>>>> GetAll(CancellationToken cancellationToken = default)
    {
        var items = await _appService.GetAllAsync(cancellationToken);
        return Ok(new ApiResponse<IReadOnlyList<DanhMucDonViDto>> { IsSuccess = true, Message = "Lấy danh sách đơn vị thành công.", Data = items });
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<DanhMucDonViDto>>> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _appService.GetByIdAsync(id, cancellationToken);
        if (result == null) return NotFound(new ApiResponse { IsSuccess = false, Message = "Không tìm thấy đơn vị." });
        return Ok(new ApiResponse<DanhMucDonViDto> { IsSuccess = true, Message = "Lấy thông tin đơn vị thành công.", Data = result });
    }

    [HttpGet("next-sort-order")]
    public async Task<ActionResult<ApiResponse<int>>> GetNextSortOrder([FromQuery] Guid donViChuQuanId, CancellationToken cancellationToken = default)
    {
        var next = await _appService.GetNextSortOrderAsync(donViChuQuanId, cancellationToken);
        return Ok(new ApiResponse<int> { IsSuccess = true, Message = "Lấy thứ tự sắp xếp tiếp theo thành công.", Data = next });
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<DanhMucDonViDto>>> Create([FromBody] DanhMucDonViUpsertApiRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _appService.CreateAsync(ToApplicationRequest(request), cancellationToken);
        if (!result.IsSuccess) return BadRequest(new ApiResponse<DanhMucDonViDto> { IsSuccess = false, Message = result.Message });
        return Ok(new ApiResponse<DanhMucDonViDto> { IsSuccess = true, Message = result.Message, Data = result.Data });
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<DanhMucDonViDto>>> Update(Guid id, [FromBody] DanhMucDonViUpsertApiRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _appService.UpdateAsync(id, ToApplicationRequest(request), cancellationToken);
        if (!result.IsSuccess) return BadRequest(new ApiResponse<DanhMucDonViDto> { IsSuccess = false, Message = result.Message });
        return Ok(new ApiResponse<DanhMucDonViDto> { IsSuccess = true, Message = result.Message, Data = result.Data });
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse>> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _appService.DeleteAsync(id, cancellationToken);
        if (!result.IsSuccess) return NotFound(new ApiResponse { IsSuccess = false, Message = result.Message });
        return Ok(new ApiResponse { IsSuccess = true, Message = result.Message });
    }

    private static UpsertDanhMucDonViRequest ToApplicationRequest(DanhMucDonViUpsertApiRequest request)
        => new()
        {
            TenDonVi = request.TenDonVi,
            Level = request.Level,
            STTSapXep = request.STTSapXep,
            DonViChuQuanId = request.DonViChuQuanId,
            DiaChi = request.DiaChi,
            MaQHNS = request.MaQHNS,
            SoDienThoai = request.SoDienThoai,
            ChucDanhQuanLy = request.ChucDanhQuanLy,
            HoVaTenNguoiQuanLy = request.HoVaTenNguoiQuanLy,
            PhanLoaiDonVi = request.PhanLoaiDonVi,
            TinhNangThanhToan = request.TinhNangThanhToan
        };
}

