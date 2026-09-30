using QuanTriHeThongService.Domain.Entities.Systems;

namespace QuanTriHeThongService.Domain.Interfaces.Repositories;

public interface ISystemInfoRepository
{
    Task<SystemInfoEntity> GetAsync(CancellationToken cancellationToken = default);
    Task<SystemInfoEntity> SaveAsync(SystemInfoEntity entity, CancellationToken cancellationToken = default);
}

