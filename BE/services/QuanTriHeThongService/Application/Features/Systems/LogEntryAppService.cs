using QuanTriHeThongService.Application.Abstractions;
using QuanTriHeThongService.Application.Common.Models;
using QuanTriHeThongService.Application.DTOs.Systems;
using QuanTriHeThongService.Domain.Interfaces.Repositories;

namespace QuanTriHeThongService.Application.Features.Systems;

public class LogEntryAppService(ILogEntryRepository repository) : ILogEntryAppService
{
    private readonly ILogEntryRepository _repository = repository;

    public async Task<PagedResult<LogEntryDto>> GetPagedAsync(string? search, int pageSize, int pageCurrent, DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken = default)
    {
        pageCurrent = pageCurrent < 1 ? 1 : pageCurrent;
        pageSize = pageSize < 5 ? 5 : pageSize > 100 ? 100 : pageSize;

        var resolvedFromDate = fromDate?.Date ?? new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        var resolvedToDate = toDate?.Date ?? DateTime.Today;
        if (resolvedToDate < resolvedFromDate)
        {
            resolvedToDate = resolvedFromDate;
        }

        var result = await _repository.GetPagedAsync(search?.Trim(), pageSize, pageCurrent, resolvedFromDate, resolvedToDate, cancellationToken);
        return new PagedResult<LogEntryDto>
        {
            Items = result.Items.Select(x => x.ToDto()).ToList(),
            TotalCount = result.TotalCount,
            PageSize = pageSize,
            PageCurrent = pageCurrent
        };
    }
}

