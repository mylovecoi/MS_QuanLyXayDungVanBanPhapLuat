using QuanTriHeThongService.Domain.Entities.Systems;
using QuanTriHeThongService.Domain.Interfaces.Repositories;
using QuanTriHeThongService.Infrastructure.Persistence;
using QuanTriHeThongService.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace QuanTriHeThongService.Infrastructure.Persistence.Repositories.Systems;

public class SystemInfoRepository(QuanTriHeThongDbContext dbContext) : ISystemInfoRepository
{
    private readonly QuanTriHeThongDbContext _dbContext = dbContext;

    public async Task<SystemInfoEntity> GetAsync(CancellationToken cancellationToken = default)
    {
        var item = await _dbContext.SystemInfo.AsNoTracking()
            .OrderBy(x => x.Id)
            .Select(x => new SystemInfoEntity
            {
                Id = x.Id,
                AppName = x.AppName,
                Copyright = x.Copyright,
                MfgDate = x.MfgDate,
                ExpDate = x.ExpDate,
                LoginLock = x.LoginLock,
                Train = x.Train,
                IsChatBot = x.IsChatBot,
                IsOPT = x.IsOPT
            })
            .FirstOrDefaultAsync(cancellationToken);

        return item ?? new SystemInfoEntity();
    }

    public async Task<SystemInfoEntity> SaveAsync(SystemInfoEntity entity, CancellationToken cancellationToken = default)
    {
        if (entity.Id == Guid.Empty)
        {
            var dbEntity = new SystemInfo
            {
                AppName = entity.AppName,
                Copyright = entity.Copyright,
                MfgDate = entity.MfgDate,
                ExpDate = entity.ExpDate,
                LoginLock = entity.LoginLock,
                Train = entity.Train,
                IsChatBot = entity.IsChatBot,
                IsOPT = entity.IsOPT
            };

            _dbContext.SystemInfo.Add(dbEntity);
            await _dbContext.SaveChangesAsync(cancellationToken);
            entity.Id = dbEntity.Id;
            return entity;
        }

        var current = await _dbContext.SystemInfo.FirstOrDefaultAsync(x => x.Id == entity.Id, cancellationToken);
        if (current == null)
        {
            current = new SystemInfo
            {
                Id = entity.Id
            };
            _dbContext.SystemInfo.Add(current);
        }

        current.AppName = entity.AppName;
        current.Copyright = entity.Copyright;
        current.MfgDate = entity.MfgDate;
        current.ExpDate = entity.ExpDate;
        current.LoginLock = entity.LoginLock;
        current.Train = entity.Train;
        current.IsChatBot = entity.IsChatBot;
        current.IsOPT = entity.IsOPT;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }
}

