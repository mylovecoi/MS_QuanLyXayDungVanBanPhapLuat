using QuanTriHeThongService.Domain.Entities.Systems;

namespace QuanTriHeThongService.Domain.Interfaces.Repositories;

public interface ILogEntryRepository
{
    Task<(IReadOnlyList<LogEntryEntity> Items, int TotalCount)> GetPagedAsync(string? search, int pageSize, int pageCurrent, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
}

