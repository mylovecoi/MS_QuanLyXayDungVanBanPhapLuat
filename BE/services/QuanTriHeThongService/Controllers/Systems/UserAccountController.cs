using QuanTriHeThongService.Application.Abstractions;
using QuanTriHeThongService.Application.DTOs.Systems;
using Microsoft.AspNetCore.Mvc;
using QuanTriHeThongService.Contracts.Requests.Systems;
using QuanTriHeThongService.Contracts.Responses;

namespace QuanTriHeThongService.Controllers.Systems;

[ApiController]
[Route("api/he-thong/tai-khoan-truy-cap")]
public class UserAccountController(IUserAccountAppService appService) : ControllerBase
{
    private readonly IUserAccountAppService _appService = appService;

    [HttpGet]
    public async Task<ActionResult<PagedApiResponse<UserAccountDto>>> GetPaged([FromQuery] string? search, [FromQuery] int pageSize = 10, [FromQuery] int pageCurrent = 1, [FromQuery] string? level = null, CancellationToken cancellationToken = default)
    {
        var result = await _appService.GetPagedAsync(search, pageSize, pageCurrent, level, cancellationToken);
        return Ok(new PagedApiResponse<UserAccountDto>
        {
            IsSuccess = true,
            Message = "Lấy danh sách tài khoản truy cập thành công.",
            Data = result.Items,
            TotalCount = result.TotalCount,
            PageSize = result.PageSize,
            PageCurrent = result.PageCurrent
        });
    }

    [HttpGet("group-permissions")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<OptionItemDto>>>> GetGroupPermissionOptions(CancellationToken cancellationToken = default)
    {
        var items = await _appService.GetGroupPermissionOptionsAsync(cancellationToken);
        return Ok(new ApiResponse<IReadOnlyList<OptionItemDto>>
        {
            IsSuccess = true,
            Message = "Lấy danh sách nhóm quyền thành công.",
            Data = items
        });
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<UserAccountDto>>> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await _appService.GetByIdAsync(id, cancellationToken);
        if (item == null)
        {
            return NotFound(new ApiResponse<UserAccountDto> { IsSuccess = false, Message = "Không tìm thấy tài khoản truy cập." });
        }

        return Ok(new ApiResponse<UserAccountDto> { IsSuccess = true, Message = "Lấy thông tin tài khoản truy cập thành công.", Data = item });
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<UserAccountDto>>> Update(Guid id, [FromBody] UpdateUserAccountApiRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _appService.UpdateAsync(id, new QuanTriHeThongService.Application.DTOs.Systems.UpdateUserAccountRequest
        {
            Name = request.Name,
            Email = request.Email,
            Status = request.Status,
            Password = request.Password,
            Content = request.Content,
            GroupPermissionId = request.GroupPermissionId
        }, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(new ApiResponse<UserAccountDto> { IsSuccess = false, Message = result.Message });
        }

        return Ok(new ApiResponse<UserAccountDto> { IsSuccess = true, Message = result.Message, Data = result.Data });
    }

    [HttpPost("duplicate")]
    public async Task<ActionResult<ApiResponse<UserAccountDto>>> Duplicate([FromBody] DuplicateUserAccountApiRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _appService.DuplicateAsync(new QuanTriHeThongService.Application.DTOs.Systems.DuplicateUserAccountRequest
        {
            SourceUserId = request.SourceUserId,
            Username = request.Username,
            Name = request.Name,
            Email = request.Email
        }, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(new ApiResponse<UserAccountDto> { IsSuccess = false, Message = result.Message });
        }

        return Ok(new ApiResponse<UserAccountDto> { IsSuccess = true, Message = result.Message, Data = result.Data });
    }

    [HttpPost("{id:guid}/reset-password")]
    public async Task<ActionResult<ApiResponse>> ResetPassword(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _appService.ResetPasswordAsync(id, cancellationToken);
        if (!result.IsSuccess)
        {
            return BadRequest(new ApiResponse { IsSuccess = false, Message = result.Message });
        }

        return Ok(new ApiResponse { IsSuccess = true, Message = result.Message });
    }

    [HttpPost("{id:guid}/change-status")]
    public async Task<ActionResult<ApiResponse>> ChangeStatus(Guid id, [FromBody] ChangeUserStatusApiRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _appService.ChangeStatusAsync(id, request.Status, cancellationToken);
        if (!result.IsSuccess)
        {
            return BadRequest(new ApiResponse { IsSuccess = false, Message = result.Message });
        }

        return Ok(new ApiResponse { IsSuccess = true, Message = result.Message });
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

    public class ChangeUserStatusApiRequest
    {
        public string Status { get; set; } = "Kích hoạt";
    }
}

