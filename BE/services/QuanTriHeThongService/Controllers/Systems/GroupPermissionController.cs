using QuanTriHeThongService.Application.Abstractions;
using QuanTriHeThongService.Application.DTOs.Systems;
using Microsoft.AspNetCore.Mvc;
using QuanTriHeThongService.Contracts.Requests.Systems;
using QuanTriHeThongService.Contracts.Responses;

namespace QuanTriHeThongService.Controllers.Systems;

[ApiController]
[Route("api/he-thong/nhom-quyen-truy-cap")]
public class GroupPermissionController(IGroupPermissionAppService appService) : ControllerBase
{
    private readonly IGroupPermissionAppService _appService = appService;

    [HttpGet]
    public async Task<ActionResult<PagedApiResponse<GroupPermissionDto>>> GetPaged([FromQuery] string? search, [FromQuery] int pageSize = 10, [FromQuery] int pageCurrent = 1, CancellationToken cancellationToken = default)
    {
        var result = await _appService.GetPagedAsync(search, pageSize, pageCurrent, cancellationToken);
        return Ok(new PagedApiResponse<GroupPermissionDto>
        {
            IsSuccess = true,
            Message = "Lấy danh sách nhóm quyền thành công.",
            Data = result.Items,
            TotalCount = result.TotalCount,
            PageSize = result.PageSize,
            PageCurrent = result.PageCurrent
        });
    }

    [HttpGet("template-groups")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<OptionItemDto>>>> GetTemplateGroups(CancellationToken cancellationToken = default)
    {
        var items = await _appService.GetTemplateGroupsAsync(cancellationToken);
        return Ok(new ApiResponse<IReadOnlyList<OptionItemDto>>
        {
            IsSuccess = true,
            Message = "Lấy danh sách nhóm quyền mẫu thành công.",
            Data = items
        });
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<GroupPermissionDto>>> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await _appService.GetByIdAsync(id, cancellationToken);
        if (item == null)
        {
            return NotFound(new ApiResponse<GroupPermissionDto> { IsSuccess = false, Message = "Không tìm thấy nhóm quyền." });
        }

        return Ok(new ApiResponse<GroupPermissionDto> { IsSuccess = true, Message = "Lấy thông tin nhóm quyền thành công.", Data = item });
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<GroupPermissionDto>>> Create([FromBody] GroupPermissionUpsertApiRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _appService.CreateAsync(ToUpsertRequest(request), cancellationToken);
        if (!result.IsSuccess)
        {
            return BadRequest(new ApiResponse<GroupPermissionDto> { IsSuccess = false, Message = result.Message });
        }

        return Ok(new ApiResponse<GroupPermissionDto> { IsSuccess = true, Message = result.Message, Data = result.Data });
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<GroupPermissionDto>>> Update(Guid id, [FromBody] GroupPermissionUpsertApiRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _appService.UpdateAsync(id, ToUpsertRequest(request), cancellationToken);
        if (!result.IsSuccess)
        {
            return BadRequest(new ApiResponse<GroupPermissionDto> { IsSuccess = false, Message = result.Message });
        }

        return Ok(new ApiResponse<GroupPermissionDto> { IsSuccess = true, Message = result.Message, Data = result.Data });
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

    [HttpGet("{id:guid}/permissions")]
    public async Task<ActionResult<PagedApiResponse<PermissionDto>>> GetPermissions(Guid id, [FromQuery] string? search, [FromQuery] int pageSize = 10, [FromQuery] int pageCurrent = 1, CancellationToken cancellationToken = default)
    {
        var result = await _appService.GetPermissionsAsync(id, search, pageSize, pageCurrent, cancellationToken);
        return Ok(new PagedApiResponse<PermissionDto>
        {
            IsSuccess = true,
            Message = "Lấy danh sách quyền chi tiết thành công.",
            Data = result.Items,
            TotalCount = result.TotalCount,
            PageSize = result.PageSize,
            PageCurrent = result.PageCurrent
        });
    }

    [HttpGet("{groupId:guid}/permissions/{permissionId:guid}")]
    public async Task<ActionResult<ApiResponse<PermissionDto>>> GetPermissionById(Guid groupId, Guid permissionId, CancellationToken cancellationToken = default)
    {
        var item = await _appService.GetPermissionByIdAsync(groupId, permissionId, cancellationToken);
        if (item == null)
        {
            return NotFound(new ApiResponse<PermissionDto> { IsSuccess = false, Message = "Không tìm thấy quyền chi tiết." });
        }

        return Ok(new ApiResponse<PermissionDto> { IsSuccess = true, Message = "Lấy quyền chi tiết thành công.", Data = item });
    }

    [HttpPut("{groupId:guid}/permissions/{permissionId:guid}")]
    public async Task<ActionResult<ApiResponse<PermissionDto>>> UpdatePermission(Guid groupId, Guid permissionId, [FromBody] PermissionUpdateApiRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _appService.UpdatePermissionAsync(groupId, permissionId, new QuanTriHeThongService.Application.DTOs.Systems.UpdatePermissionRequest
        {
            Index = request.Index,
            Create = request.Create,
            Edit = request.Edit,
            Delete = request.Delete,
            Approve = request.Approve,
            Public = request.Public
        }, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(new ApiResponse<PermissionDto> { IsSuccess = false, Message = result.Message });
        }

        return Ok(new ApiResponse<PermissionDto> { IsSuccess = true, Message = result.Message, Data = result.Data });
    }

    private static QuanTriHeThongService.Application.DTOs.Systems.UpsertGroupPermissionRequest ToUpsertRequest(GroupPermissionUpsertApiRequest request)
    {
        return new QuanTriHeThongService.Application.DTOs.Systems.UpsertGroupPermissionRequest
        {
            Name = request.Name,
            Description = request.Description,
            Status = request.Status,
            TemplateGroup = request.TemplateGroup
        };
    }
}

