using QuanTriHeThongService.Application.Abstractions;
using QuanTriHeThongService.Application.DTOs.Systems;
using QuanTriHeThongService.Domain.Entities.Systems;
using QuanTriHeThongService.Domain.Interfaces.Repositories;

namespace QuanTriHeThongService.Application.Features.Systems;

public class SystemInfoAppService(ISystemInfoRepository repository) : ISystemInfoAppService
{
    private readonly ISystemInfoRepository _repository = repository;

    public async Task<SystemInfoDto> GetAsync(CancellationToken cancellationToken = default)
        => (await _repository.GetAsync(cancellationToken)).ToDto();

    public async Task<(bool IsSuccess, string Message, SystemInfoDto? Data)> SaveAsync(UpdateSystemInfoRequest request, CancellationToken cancellationToken = default)
    {
        if (request.LoginLock < 0)
        {
            return (false, "Số lần khóa đăng nhập không hợp lệ.", null);
        }

        if (request.ExpDate < request.MfgDate)
        {
            return (false, "Ngày hết hạn phải lớn hơn hoặc bằng ngày sản xuất.", null);
        }

        var entity = new SystemInfoEntity
        {
            Id = request.Id,
            AppName = request.AppName?.Trim(),
            Copyright = request.Copyright?.Trim(),
            MfgDate = request.MfgDate,
            ExpDate = request.ExpDate,
            LoginLock = request.LoginLock,
            Train = request.Train,
            IsChatBot = request.IsChatBot,
            IsOPT = request.IsOPT
        };

        var saved = await _repository.SaveAsync(entity, cancellationToken);
        return (true, "Cập nhật cấu hình hệ thống thành công. Vui lòng đăng nhập lại để áp dụng đầy đủ thay đổi.", saved.ToDto());
    }
}

