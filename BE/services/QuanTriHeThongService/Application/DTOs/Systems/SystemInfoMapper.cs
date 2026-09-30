using QuanTriHeThongService.Domain.Entities.Systems;

namespace QuanTriHeThongService.Application.DTOs.Systems;

public static class SystemInfoMapper
{
    public static SystemInfoDto ToDto(this SystemInfoEntity entity)
    {
        return new SystemInfoDto
        {
            Id = entity.Id,
            AppName = entity.AppName,
            Copyright = entity.Copyright,
            MfgDate = entity.MfgDate,
            ExpDate = entity.ExpDate,
            LoginLock = entity.LoginLock,
            Train = entity.Train,
            IsChatBot = entity.IsChatBot,
            IsOPT = entity.IsOPT
        };
    }
}

