using QuanTriHeThongService.Application.Abstractions;
using QuanTriHeThongService.Application.DTOs.Systems;
using Microsoft.AspNetCore.Mvc;
using QuanTriHeThongService.Contracts.Requests.Systems;
using QuanTriHeThongService.Contracts.Responses;

namespace QuanTriHeThongService.Controllers.Systems;

[ApiController]
[Route("api/he-thong/danh-sach-chuc-nang")]
public class RoleActionController(IRoleActionAppService appService) : ControllerBase
{
    private readonly IRoleActionAppService _appService = appService;

    [HttpGet]
    public async Task<ActionResult<PagedApiResponse<RoleActionDto>>> GetPaged([FromQuery] string? search, [FromQuery] int pageSize = 10, [FromQuery] int pageCurrent = 1, CancellationToken cancellationToken = default)
    {
        var result = await _appService.GetPagedAsync(search, pageSize, pageCurrent, cancellationToken);
        return Ok(new PagedApiResponse<RoleActionDto>
        {
            IsSuccess = true,
            Message = "Lấy danh sách chức năng thành công.",
            Data = result.Items,
            TotalCount = result.TotalCount,
            PageSize = result.PageSize,
            PageCurrent = result.PageCurrent
        });
    }

    [HttpGet("all")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<RoleActionDto>>>> GetAll(CancellationToken cancellationToken = default)
    {
        var items = await _appService.GetAllAsync(cancellationToken);
        return Ok(new ApiResponse<IReadOnlyList<RoleActionDto>>
        {
            IsSuccess = true,
            Message = "Lấy toàn bộ chức năng thành công.",
            Data = items
        });
    }

    [HttpGet("group-options")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<RoleActionDto>>>> GetGroupOptions(CancellationToken cancellationToken = default)
    {
        var items = await _appService.GetGroupOptionsAsync(cancellationToken);
        return Ok(new ApiResponse<IReadOnlyList<RoleActionDto>>
        {
            IsSuccess = true,
            Message = "Lấy danh sách chức năng cha thành công.",
            Data = items
        });
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<RoleActionDto>>> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await _appService.GetByIdAsync(id, cancellationToken);
        if (item == null)
        {
            return NotFound(new ApiResponse<RoleActionDto> { IsSuccess = false, Message = "Không tìm thấy chức năng." });
        }

        return Ok(new ApiResponse<RoleActionDto> { IsSuccess = true, Message = "Lấy thông tin chức năng thành công.", Data = item });
    }

    [HttpGet("next-sort-order")]
    public async Task<ActionResult<ApiResponse<int>>> GetNextSortOrder([FromQuery] Guid? parentId, CancellationToken cancellationToken = default)
    {
        var nextValue = await _appService.GetNextSortOrderAsync(parentId, cancellationToken);
        return Ok(new ApiResponse<int>
        {
            IsSuccess = true,
            Message = "Lấy thứ tự sắp xếp tiếp theo thành công.",
            Data = nextValue
        });
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<RoleActionDto>>> Create([FromBody] RoleActionUpsertApiRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _appService.CreateAsync(ToApplicationRequest(request), cancellationToken);
        if (!result.IsSuccess)
        {
            return BadRequest(new ApiResponse<RoleActionDto> { IsSuccess = false, Message = result.Message });
        }

        return Ok(new ApiResponse<RoleActionDto> { IsSuccess = true, Message = result.Message, Data = result.Data });
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<RoleActionDto>>> Update(Guid id, [FromBody] RoleActionUpsertApiRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _appService.UpdateAsync(id, ToApplicationRequest(request), cancellationToken);
        if (!result.IsSuccess)
        {
            return BadRequest(new ApiResponse<RoleActionDto> { IsSuccess = false, Message = result.Message });
        }

        return Ok(new ApiResponse<RoleActionDto> { IsSuccess = true, Message = result.Message, Data = result.Data });
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse>> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _appService.DeleteAsync(id, cancellationToken);
        if (!result.IsSuccess)
        {
            return BadRequest(new ApiResponse { IsSuccess = false, Message = result.Message });
        }

        return Ok(new ApiResponse { IsSuccess = true, Message = result.Message });
    }

    private static QuanTriHeThongService.Application.DTOs.Systems.UpsertRoleActionRequest ToApplicationRequest(RoleActionUpsertApiRequest request)
    {
        return new QuanTriHeThongService.Application.DTOs.Systems.UpsertRoleActionRequest
        {
            Id = request.Id,
            STTSapXep = request.STTSapXep,
            PhanLoai = request.PhanLoai,
            Role = request.Role,
            ParentId = request.ParentId,
            Title = request.Title,
            Controller = request.Controller,
            Action = request.Action,
            Parameter = request.Parameter,
            Table = request.Table,
            Status = request.Status,
            UseGroup = request.UseGroup,
            FrontendPath = request.FrontendPath,
            IsVisibleInMenu = request.IsVisibleInMenu,
            ClientApp = request.ClientApp,
            MenuTitle = request.MenuTitle,
            MenuIcon = request.MenuIcon,
            Icon = request.Icon
        };
    }
}

