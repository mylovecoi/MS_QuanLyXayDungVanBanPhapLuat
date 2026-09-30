using QuanTriHeThongService.Application.Abstractions;
using QuanTriHeThongService.Application.DTOs.Systems;
using Microsoft.AspNetCore.Mvc;
using QuanTriHeThongService.Contracts.Responses;

namespace QuanTriHeThongService.Controllers.Systems;

[ApiController]
[Route("api/he-thong/nhat-ky-he-thong")]
public class LogEntryController(ILogEntryAppService appService) : ControllerBase
{
    private readonly ILogEntryAppService _appService = appService;

    [HttpGet]
    public async Task<ActionResult<PagedApiResponse<LogEntryDto>>> GetPaged(
        [FromQuery] string? search,
        [FromQuery] int pageSize = 10,
        [FromQuery] int pageCurrent = 1,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _appService.GetPagedAsync(search, pageSize, pageCurrent, fromDate, toDate, cancellationToken);
        return Ok(new PagedApiResponse<LogEntryDto>
        {
            IsSuccess = true,
            Message = "Lấy nhật ký hệ thống thành công.",
            Data = result.Items,
            TotalCount = result.TotalCount,
            PageSize = result.PageSize,
            PageCurrent = result.PageCurrent
        });
    }
}

