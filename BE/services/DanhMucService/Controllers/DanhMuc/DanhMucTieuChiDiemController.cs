using DanhMucService.Contracts.Requests.DanhMuc;
using DanhMucService.Contracts.Responses;
using DanhMucService.Application.Abstractions;
using DanhMucService.Application.DTOs.DanhMuc;
using Microsoft.AspNetCore.Mvc;

namespace DanhMucService.Controllers.DanhMuc;

[ApiController]
[Route("api/danh-muc/tieu-chi-diem")]
public class DanhMucTieuChiDiemController(IDanhMucTieuChiDiemAppService appService) : ControllerBase
{
    private readonly IDanhMucTieuChiDiemAppService _appService = appService;

    [HttpGet]
    public async Task<ActionResult<PagedApiResponse<DanhMucTieuChiDiemDto>>> GetPaged([FromQuery] string? search, [FromQuery] int pageSize = 5, [FromQuery] int pageCurrent = 1, CancellationToken cancellationToken = default)
    {
        var result = await _appService.GetPagedAsync(search, pageSize, pageCurrent, cancellationToken);
        return Ok(new PagedApiResponse<DanhMucTieuChiDiemDto>
        {
            IsSuccess = true,
            Message = "Lấy danh sách tiêu chí điểm thành công.",
            Data = result.Items,
            TotalCount = result.TotalCount,
            PageSize = result.PageSize,
            PageCurrent = result.PageCurrent
        });
    }

    [HttpGet("options")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<DanhMucTieuChiDiemDto>>>> GetAll(CancellationToken cancellationToken = default)
    {
        var items = await _appService.GetAllAsync(cancellationToken);
        return Ok(new ApiResponse<IReadOnlyList<DanhMucTieuChiDiemDto>>
        {
            IsSuccess = true,
            Message = "Lấy danh sách tiêu chí điểm thành công.",
            Data = items
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

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<DanhMucTieuChiDiemDto>>> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _appService.GetByIdAsync(id, cancellationToken);
        if (result == null)
        {
            return NotFound(new ApiResponse { IsSuccess = false, Message = "Không tìm thấy tiêu chí điểm." });
        }

        return Ok(new ApiResponse<DanhMucTieuChiDiemDto>
        {
            IsSuccess = true,
            Message = "Lấy thông tin tiêu chí điểm thành công.",
            Data = result
        });
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<DanhMucTieuChiDiemDto>>> Create([FromBody] DanhMucTieuChiDiemUpsertApiRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _appService.CreateAsync(ToApplicationRequest(request), cancellationToken);
        if (!result.IsSuccess)
        {
            return BadRequest(new ApiResponse<DanhMucTieuChiDiemDto> { IsSuccess = false, Message = result.Message });
        }

        return Ok(new ApiResponse<DanhMucTieuChiDiemDto> { IsSuccess = true, Message = result.Message, Data = result.Data });
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<DanhMucTieuChiDiemDto>>> Update(Guid id, [FromBody] DanhMucTieuChiDiemUpsertApiRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _appService.UpdateAsync(id, ToApplicationRequest(request), cancellationToken);
        if (!result.IsSuccess)
        {
            return BadRequest(new ApiResponse<DanhMucTieuChiDiemDto> { IsSuccess = false, Message = result.Message });
        }

        return Ok(new ApiResponse<DanhMucTieuChiDiemDto> { IsSuccess = true, Message = result.Message, Data = result.Data });
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

    private static UpsertDanhMucTieuChiDiemRequest ToApplicationRequest(DanhMucTieuChiDiemUpsertApiRequest request)
    {
        return new UpsertDanhMucTieuChiDiemRequest
        {
            Id = request.Id,
            MaTieuChi = request.MaTieuChi,
            TenTieuChi = request.TenTieuChi,
            LoaiTieuChi = request.LoaiTieuChi,
            KieuGiaTri = request.KieuGiaTri,
            DonViGiaTri = request.DonViGiaTri,
            ThuTuSapXep = request.ThuTuSapXep,
            DiemToiDa = request.DiemToiDa,
            TrangThai = request.TrangThai,
            MoTa = request.MoTa,
            GhiChu = request.GhiChu,
            Mucs = request.Mucs.Select(x => new UpsertDanhMucTieuChiDiemMucRequest
            {
                Id = x.Id,
                TuGiaTri = x.TuGiaTri,
                DenGiaTri = x.DenGiaTri,
                BaoGomTuGiaTri = x.BaoGomTuGiaTri,
                BaoGomDenGiaTri = x.BaoGomDenGiaTri,
                Diem = x.Diem,
                NhanHienThi = x.NhanHienThi,
                ThuTuSapXep = x.ThuTuSapXep,
                TrangThai = x.TrangThai,
                GhiChu = x.GhiChu
            }).ToList()
        };
    }
}

