namespace DanhMucService.Infrastructure.Persistence.Entities;

public class HoSoVanBanUsage
{
    public Guid Id { get; set; }
    public Guid QuyTrinhSoanThaoId { get; set; }
    public Guid? BuocHienTaiId { get; set; }
}
