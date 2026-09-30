using QuanTriHeThongService.Domain.Entities.Systems;
using QuanTriHeThongService.Domain.Interfaces.Repositories;
using QuanTriHeThongService.Infrastructure.Persistence;
using QuanTriHeThongService.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace QuanTriHeThongService.Infrastructure.Persistence.Repositories.Systems;

public class LogEntryRepository(QuanTriHeThongDbContext dbContext) : ILogEntryRepository
{
    private readonly QuanTriHeThongDbContext _dbContext = dbContext;

    public async Task<(IReadOnlyList<LogEntryEntity> Items, int TotalCount)> GetPagedAsync(string? search, int pageSize, int pageCurrent, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
    {
        var from = fromDate.Date;
        var to = toDate.Date.AddDays(1).AddTicks(-1);

        var query = _dbContext.Logs.AsNoTracking()
            .Where(x => x.CreatedDate >= from && x.CreatedDate <= to);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x =>
                (x.Username != null && x.Username.Contains(search)) ||
                (x.Url != null && x.Url.Contains(search)) ||
                (x.Method != null && x.Method.Contains(search)) ||
                (x.IpAddress != null && x.IpAddress.Contains(search)));
        }

        query = query.OrderByDescending(x => x.CreatedDate);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pageCurrent - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new LogEntryEntity
            {
                Id = x.Id,
                CreatedDate = x.CreatedDate,
                Username = x.Username,
                IpAddress = x.IpAddress,
                Url = x.Url,
                Method = x.Method,
                Request = x.Request
            })
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}

