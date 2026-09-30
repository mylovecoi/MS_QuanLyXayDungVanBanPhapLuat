using QuanTriHeThongService.Application.Common.Models;
using QuanTriHeThongService.Application.DTOs.Systems;

namespace QuanTriHeThongService.Application.Abstractions;

public interface ILogEntryAppService
{
    Task<PagedResult<LogEntryDto>> GetPagedAsync(string? search, int pageSize, int pageCurrent, DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken = default);
}

