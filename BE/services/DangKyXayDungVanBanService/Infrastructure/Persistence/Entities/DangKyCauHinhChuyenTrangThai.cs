using System.ComponentModel.DataAnnotations;

namespace DangKyXayDungVanBanService.Infrastructure.Persistence.Entities;

public class DangKyCauHinhChuyenTrangThai : BaseEntity
{
    public Guid QuyTrinhSoanThaoId { get; set; }
    public Guid BuocHienTaiId { get; set; }
    public Guid TrangThaiHienTaiId { get; set; }
    public Guid HanhDongId { get; set; }
    public Guid BuocTiepTheoId { get; set; }
    public Guid TrangThaiTiepTheoId { get; set; }
    public Guid? ChuyenBuocId { get; set; }

    [Required]
    [MaxLength(100)]
    public string NhomNhapLieu { get; set; } = string.Empty;

    public bool YeuCauLyDo { get; set; }
    public bool YeuCauFileDinhKem { get; set; }
    public bool LaKetThuc { get; set; }
    public bool TrangThai { get; set; } = true;
}
