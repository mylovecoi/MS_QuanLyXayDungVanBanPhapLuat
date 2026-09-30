using QuanTriHeThongService.Application.Abstractions;
using QuanTriHeThongService.Application.DTOs.Systems;
using Microsoft.AspNetCore.Mvc;
using QuanTriHeThongService.Contracts.Requests.Systems;
using QuanTriHeThongService.Contracts.Responses;

namespace QuanTriHeThongService.Controllers.Systems;

[ApiController]
[Route("api/he-thong/cau-hinh-he-thong")]
public class SystemInfoController(ISystemInfoAppService appService) : ControllerBase
{
    private readonly ISystemInfoAppService _appService = appService;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<SystemInfoDto>>> Get(CancellationToken cancellationToken = default)
    {
        var item = await _appService.GetAsync(cancellationToken);
        return Ok(new ApiResponse<SystemInfoDto>
        {
            IsSuccess = true,
            Message = "Lấy cấu hình hệ thống thành công.",
            Data = item
        });
    }

    [HttpPut]
    public async Task<ActionResult<ApiResponse<SystemInfoDto>>> Save([FromBody] UpdateSystemInfoApiRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _appService.SaveAsync(new QuanTriHeThongService.Application.DTOs.Systems.UpdateSystemInfoRequest
        {
            Id = request.Id,
            AppName = request.AppName,
            Copyright = request.Copyright,
            MfgDate = request.MfgDate,
            ExpDate = request.ExpDate,
            LoginLock = request.LoginLock,
            Train = request.Train,
            IsChatBot = request.IsChatBot,
            IsOPT = request.IsOPT
        }, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(new ApiResponse<SystemInfoDto> { IsSuccess = false, Message = result.Message });
        }

        return Ok(new ApiResponse<SystemInfoDto> { IsSuccess = true, Message = result.Message, Data = result.Data });
    }
}

