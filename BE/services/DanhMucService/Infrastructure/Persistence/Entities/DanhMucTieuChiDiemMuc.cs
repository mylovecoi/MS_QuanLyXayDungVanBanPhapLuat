using System.ComponentModel.DataAnnotations;

namespace DanhMucService.Infrastructure.Persistence.Entities
{
    public class DanhMucTieuChiDiemMuc : BaseEntity
    {
        public Guid DanhMucTieuChiDiemId { get; set; }

        public decimal? TuGiaTri { get; set; }

        public decimal? DenGiaTri { get; set; }

        public bool BaoGomTuGiaTri { get; set; } = true;

        public bool BaoGomDenGiaTri { get; set; } = true;

        public decimal Diem { get; set; } = 0;

        [StringLength(250)]
        public string? NhanHienThi { get; set; }

        public int ThuTuSapXep { get; set; } = 1;

        public bool TrangThai { get; set; } = true;

        public string? GhiChu { get; set; }
    }
}
