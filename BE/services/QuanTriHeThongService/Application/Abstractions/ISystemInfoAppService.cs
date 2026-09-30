using QuanTriHeThongService.Application.DTOs.Systems;

namespace QuanTriHeThongService.Application.Abstractions;

public interface ISystemInfoAppService
{
    Task<SystemInfoDto> GetAsync(CancellationToken cancellationToken = default);
    Task<(bool IsSuccess, string Message, SystemInfoDto? Data)> SaveAsync(UpdateSystemInfoRequest request, CancellationToken cancellationToken = default);
}

