using System.ComponentModel.DataAnnotations;

namespace XayDungVanBanService.Infrastructure.Persistence.Entities;

public sealed class HoSoXayDungVanBanSoSanhDuThao : BaseEntity
{
    public Guid HoSoXayDungVanBanId { get; set; }
    public Guid FileGocId { get; set; }
    public Guid FileSoSanhId { get; set; }
    public int SoNoiDungThem { get; set; }
    public int SoNoiDungXoa { get; set; }
    public int SoNoiDungSua { get; set; }
    [Required] public string NoiDungSoSanhHtml { get; set; } = string.Empty;
}
