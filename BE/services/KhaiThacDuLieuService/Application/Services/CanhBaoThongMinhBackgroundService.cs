using KhaiThacDuLieuService.Application.Abstractions;

namespace KhaiThacDuLieuService.Application.Services;

public sealed class CanhBaoThongMinhBackgroundService(
    IServiceScopeFactory scopeFactory,
    ILogger<CanhBaoThongMinhBackgroundService> logger,
    IConfiguration configuration) : BackgroundService
{
    private readonly TimeSpan interval = TimeSpan.FromMinutes(
        Math.Clamp(configuration.GetValue("CanhBaoThongMinh:IntervalMinutes", 60), 5, 1440));

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await RunOnceAsync(stoppingToken);

        using var timer = new PeriodicTimer(interval);
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await RunOnceAsync(stoppingToken);
        }
    }

    private async Task RunOnceAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var scope = scopeFactory.CreateScope();
            var generator = scope.ServiceProvider.GetRequiredService<ICanhBaoThongMinhGeneratorService>();
            var result = await generator.SinhCanhBaoTuDongAsync(stoppingToken);
            logger.LogInformation(
                "Đã quét cảnh báo thông minh: {CheckedObjects} đối tượng, tạo {Created}, cập nhật {Updated}, tự đóng {Closed}, chuyển quá hạn {OverdueReminders} nhắc việc.",
                result.SoDoiTuongDuocKiemTra,
                result.SoCanhBaoTaoMoi,
                result.SoCanhBaoCapNhat,
                result.SoCanhBaoTuDongDong,
                result.SoNhacViecChuyenQuaHan);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Không thể quét cảnh báo thông minh.");
        }
    }
}
