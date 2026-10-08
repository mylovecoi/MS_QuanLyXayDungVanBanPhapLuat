using DanhMucService.Contracts.Requests.DanhMuc;
using DanhMucService.Contracts.Responses;
using DanhMucService.Application.Abstractions;
using DanhMucService.Application.DTOs.DanhMuc;
using Microsoft.AspNetCore.Mvc;

namespace DanhMucService.Controllers.DanhMuc;

[ApiController]
[Route("api/danh-muc/quy-trinh-soan-thao")]
public class DanhMucQuyTrinhSoanThaoController(IDanhMucQuyTrinhSoanThaoAppService appService) : ControllerBase
{
    private readonly IDanhMucQuyTrinhSoanThaoAppService _appService = appService;

    [HttpGet]
    public async Task<ActionResult<PagedApiResponse<DanhMucQuyTrinhSoanThaoDto>>> GetPaged([FromQuery] string? search, [FromQuery] int pageSize = 5, [FromQuery] int pageCurrent = 1, CancellationToken cancellationToken = default)
    {
        var result = await _appService.GetPagedAsync(search, pageSize, pageCurrent, cancellationToken);
        return Ok(new PagedApiResponse<DanhMucQuyTrinhSoanThaoDto>
        {
            IsSuccess = true,
            Message = "Lấy danh sách quy trình soạn thảo thành công.",
            Data = result.Items,
            TotalCount = result.TotalCount,
            PageSize = result.PageSize,
            PageCurrent = result.PageCurrent
        });
    }

    [HttpGet("van-ban-options")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<DanhMucLookupDto>>>> GetVanBanOptions(CancellationToken cancellationToken = default)
    {
        var items = await _appService.GetDanhMucVanBanOptionsAsync(cancellationToken);
        return Ok(new ApiResponse<IReadOnlyList<DanhMucLookupDto>>
        {
            IsSuccess = true,
            Message = "Lấy danh sách loại văn bản thành công.",
            Data = items
        });
    }

    [HttpGet("don-vi-options")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<DanhMucLookupDto>>>> GetDonViOptions(CancellationToken cancellationToken = default)
    {
        var items = await _appService.GetDanhMucDonViOptionsAsync(cancellationToken);
        return Ok(new ApiResponse<IReadOnlyList<DanhMucLookupDto>>
        {
            IsSuccess = true,
            Message = "Lấy danh sách đơn vị thành công.",
            Data = items
        });
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<DanhMucQuyTrinhSoanThaoDto>>> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _appService.GetByIdAsync(id, cancellationToken);
        if (result == null)
        {
            return NotFound(new ApiResponse { IsSuccess = false, Message = "Không tìm thấy quy trình soạn thảo." });
        }

        return Ok(new ApiResponse<DanhMucQuyTrinhSoanThaoDto>
        {
            IsSuccess = true,
            Message = "Lấy thông tin quy trình soạn thảo thành công.",
            Data = result
        });
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<DanhMucQuyTrinhSoanThaoDto>>> Create([FromBody] DanhMucQuyTrinhSoanThaoUpsertApiRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _appService.CreateAsync(ToApplicationRequest(request), cancellationToken);
        if (!result.IsSuccess)
        {
            return BadRequest(new ApiResponse<DanhMucQuyTrinhSoanThaoDto> { IsSuccess = false, Message = result.Message });
        }

        return Ok(new ApiResponse<DanhMucQuyTrinhSoanThaoDto> { IsSuccess = true, Message = result.Message, Data = result.Data });
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<DanhMucQuyTrinhSoanThaoDto>>> Update(Guid id, [FromBody] DanhMucQuyTrinhSoanThaoUpsertApiRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _appService.UpdateAsync(id, ToApplicationRequest(request), cancellationToken);
        if (!result.IsSuccess)
        {
            return BadRequest(new ApiResponse<DanhMucQuyTrinhSoanThaoDto> { IsSuccess = false, Message = result.Message });
        }

        return Ok(new ApiResponse<DanhMucQuyTrinhSoanThaoDto> { IsSuccess = true, Message = result.Message, Data = result.Data });
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

    [HttpPut("{id:guid}/buoc/{stepId:guid}")]
    public async Task<ActionResult<ApiResponse<DanhMucQuyTrinhSoanThaoDto>>> UpdateStep(Guid id, Guid stepId, [FromBody] DanhMucBuocQuyTrinhUpsertApiRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _appService.UpdateStepAsync(id, stepId, ToApplicationRequest(request), cancellationToken);
        if (!result.IsSuccess)
        {
            return BadRequest(new ApiResponse<DanhMucQuyTrinhSoanThaoDto> { IsSuccess = false, Message = result.Message });
        }

        return Ok(new ApiResponse<DanhMucQuyTrinhSoanThaoDto> { IsSuccess = true, Message = result.Message, Data = result.Data });
    }

    [HttpDelete("{id:guid}/buoc/{stepId:guid}")]
    public async Task<ActionResult<ApiResponse>> DeleteStep(Guid id, Guid stepId, CancellationToken cancellationToken = default)
    {
        var result = await _appService.DeleteStepAsync(id, stepId, cancellationToken);
        if (!result.IsSuccess)
        {
            return BadRequest(new ApiResponse { IsSuccess = false, Message = result.Message });
        }

        return Ok(new ApiResponse { IsSuccess = true, Message = result.Message });
    }

    [HttpPut("{id:guid}/chuyen-buoc/{transitionId:guid}")]
    public async Task<ActionResult<ApiResponse<DanhMucQuyTrinhSoanThaoDto>>> UpdateTransition(Guid id, Guid transitionId, [FromBody] DanhMucChuyenBuocQuyTrinhUpsertApiRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _appService.UpdateTransitionAsync(id, transitionId, ToApplicationRequest(request), cancellationToken);
        if (!result.IsSuccess)
        {
            return BadRequest(new ApiResponse<DanhMucQuyTrinhSoanThaoDto> { IsSuccess = false, Message = result.Message });
        }

        return Ok(new ApiResponse<DanhMucQuyTrinhSoanThaoDto> { IsSuccess = true, Message = result.Message, Data = result.Data });
    }

    [HttpDelete("{id:guid}/chuyen-buoc/{transitionId:guid}")]
    public async Task<ActionResult<ApiResponse>> DeleteTransition(Guid id, Guid transitionId, CancellationToken cancellationToken = default)
    {
        var result = await _appService.DeleteTransitionAsync(id, transitionId, cancellationToken);
        if (!result.IsSuccess)
        {
            return BadRequest(new ApiResponse { IsSuccess = false, Message = result.Message });
        }

        return Ok(new ApiResponse { IsSuccess = true, Message = result.Message });
    }

    private static UpsertDanhMucQuyTrinhSoanThaoRequest ToApplicationRequest(DanhMucQuyTrinhSoanThaoUpsertApiRequest request)
    {
        return new UpsertDanhMucQuyTrinhSoanThaoRequest
        {
            Id = request.Id,
            MaQuyTrinh = request.MaQuyTrinh,
            TenQuyTrinh = request.TenQuyTrinh,
            LoaiQuyTrinh = request.LoaiQuyTrinh,
            DanhMucVanBanId = request.DanhMucVanBanId,
            DanhMucVanBanIds = request.DanhMucVanBanIds,
            CapApDung = request.CapApDung,
            CapApDungs = request.CapApDungs,
            PhienBan = request.PhienBan,
            TrangThai = request.TrangThai,
            MoTa = request.MoTa,
            GhiChu = request.GhiChu,
            BuocQuyTrinhs = request.BuocQuyTrinhs.Select(x => new UpsertDanhMucBuocQuyTrinhRequest
            {
                Id = x.Id,
                MaBuoc = x.MaBuoc,
                TenBuoc = x.TenBuoc,
                ThuTuSapXep = x.ThuTuSapXep,
                LoaiBuoc = x.LoaiBuoc,
                BatBuoc = x.BatBuoc,
                ChoPhepBoQua = x.ChoPhepBoQua,
                ChoPhepQuayLui = x.ChoPhepQuayLui,
                CachHoanThanh = x.CachHoanThanh,
                SoLuongPhanHoiToiThieu = x.SoLuongPhanHoiToiThieu,
                YeuCauFileDinhKem = x.YeuCauFileDinhKem,
                SoLanTraLaiToiDa = x.SoLanTraLaiToiDa,
                SoNgayXuLyTieuChuan = x.SoNgayXuLyTieuChuan,
                SoNgayCanhBaoSapHan = x.SoNgayCanhBaoSapHan,
                DonViTiepNhanMacDinhId = x.DonViTiepNhanMacDinhId,
                MoTa = x.MoTa,
                GhiChu = x.GhiChu
            }).ToList(),
            ChuyenBuocs = request.ChuyenBuocs.Select(x => new UpsertDanhMucChuyenBuocQuyTrinhRequest
            {
                Id = x.Id,
                TuBuocMa = x.TuBuocMa,
                DenBuocMa = x.DenBuocMa,
                DieuKienKetQua = x.DieuKienKetQua,
                LoaiChuyenBuoc = x.LoaiChuyenBuoc,
                LaNhanhMacDinh = x.LaNhanhMacDinh,
                YeuCauNhapLyDo = x.YeuCauNhapLyDo,
                IsKetThuc = x.IsKetThuc,
                MoTa = x.MoTa,
                GhiChu = x.GhiChu
            }).ToList()
        };
    }

    private static UpsertDanhMucBuocQuyTrinhRequest ToApplicationRequest(DanhMucBuocQuyTrinhUpsertApiRequest request)
    {
        return new UpsertDanhMucBuocQuyTrinhRequest
        {
            Id = request.Id,
            MaBuoc = request.MaBuoc,
            TenBuoc = request.TenBuoc,
            ThuTuSapXep = request.ThuTuSapXep,
            LoaiBuoc = request.LoaiBuoc,
            BatBuoc = request.BatBuoc,
            ChoPhepBoQua = request.ChoPhepBoQua,
            ChoPhepQuayLui = request.ChoPhepQuayLui,
            CachHoanThanh = request.CachHoanThanh,
            SoLuongPhanHoiToiThieu = request.SoLuongPhanHoiToiThieu,
            YeuCauFileDinhKem = request.YeuCauFileDinhKem,
            SoLanTraLaiToiDa = request.SoLanTraLaiToiDa,
            SoNgayXuLyTieuChuan = request.SoNgayXuLyTieuChuan,
            SoNgayCanhBaoSapHan = request.SoNgayCanhBaoSapHan,
            DonViTiepNhanMacDinhId = request.DonViTiepNhanMacDinhId,
            MoTa = request.MoTa,
            GhiChu = request.GhiChu
        };
    }

    private static UpsertDanhMucChuyenBuocQuyTrinhRequest ToApplicationRequest(DanhMucChuyenBuocQuyTrinhUpsertApiRequest request)
    {
        return new UpsertDanhMucChuyenBuocQuyTrinhRequest
        {
            Id = request.Id,
            TuBuocMa = request.TuBuocMa,
            DenBuocMa = request.DenBuocMa,
            DieuKienKetQua = request.DieuKienKetQua,
            LoaiChuyenBuoc = request.LoaiChuyenBuoc,
            LaNhanhMacDinh = request.LaNhanhMacDinh,
            YeuCauNhapLyDo = request.YeuCauNhapLyDo,
            IsKetThuc = request.IsKetThuc,
            MoTa = request.MoTa,
            GhiChu = request.GhiChu
        };
    }
}

