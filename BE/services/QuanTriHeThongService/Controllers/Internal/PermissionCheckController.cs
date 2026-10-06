using BuildingBlocks.Abstractions;
using Microsoft.AspNetCore.Mvc;
using QuanTriHeThongService.Application.Common.Interfaces;
using QuanTriHeThongService.Contracts.Requests.Internal;
using QuanTriHeThongService.Contracts.Responses;
using QuanTriHeThongService.Contracts.Responses.Internal;

namespace QuanTriHeThongService.Controllers.Internal;

[ApiController]
[Route("api/internal/permissions")]
public sealed class PermissionCheckController(
    ICurrentUserContext currentUserContext,
    IPermissionChecker permissionChecker) : ControllerBase
{
    [HttpPost("check")]
    public async Task<ActionResult<ApiResponse<PermissionCheckApiResponse>>> CheckAsync(
        [FromBody] PermissionCheckApiRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!currentUserContext.IsAuthenticated || currentUserContext.UserId is null)
        {
            return Unauthorized(new ApiResponse<PermissionCheckApiResponse>
            {
                IsSuccess = false,
                Message = "Người dùng chưa được xác thực."
            });
        }

        var allowed = await permissionChecker.HasPermissionAsync(
            request.Controller,
            request.Action,
            request.PermissionType,
            cancellationToken);

        return Ok(new ApiResponse<PermissionCheckApiResponse>
        {
            IsSuccess = true,
            Message = allowed ? "Được phép truy cập." : "Không có quyền truy cập.",
            Data = new PermissionCheckApiResponse { Allowed = allowed }
        });
    }
}
