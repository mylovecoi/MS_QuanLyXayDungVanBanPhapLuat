using DanhMucService.Domain.Entities.DanhMuc;

namespace DanhMucService.Application.DTOs.DanhMuc;

public static class DanhMucDonViMapper
{
    public static DanhMucDonViDto ToDto(this DanhMucDonViEntity entity)
    {
        return new DanhMucDonViDto
        {
            Id = entity.Id,
            TenDonVi = entity.TenDonVi,
            Level = entity.Level,
            STTSapXep = entity.STTSapXep,
            DonViChuQuanId = entity.DonViChuQuanId,
            TenDonViChuQuan = entity.TenDonViChuQuan,
            DiaChi = entity.DiaChi,
            MaQHNS = entity.MaQHNS,
            SoDienThoai = entity.SoDienThoai,
            ChucDanhQuanLy = entity.ChucDanhQuanLy,
            HoVaTenNguoiQuanLy = entity.HoVaTenNguoiQuanLy,
            PhanLoaiDonVi = entity.PhanLoaiDonVi,
            TinhNangThanhToan = entity.TinhNangThanhToan
        };
    }

    public static DanhMucDonViEntity ToEntity(this UpsertDanhMucDonViRequest request)
    {
        return new DanhMucDonViEntity
        {
            TenDonVi = request.TenDonVi.Trim(),
            Level = request.Level,
            STTSapXep = request.STTSapXep,
            DonViChuQuanId = request.DonViChuQuanId,
            DiaChi = request.DiaChi?.Trim(),
            MaQHNS = request.MaQHNS?.Trim(),
            SoDienThoai = request.SoDienThoai?.Trim(),
            ChucDanhQuanLy = request.ChucDanhQuanLy?.Trim(),
            HoVaTenNguoiQuanLy = request.HoVaTenNguoiQuanLy?.Trim(),
            PhanLoaiDonVi = request.PhanLoaiDonVi?.Trim(),
            TinhNangThanhToan = request.TinhNangThanhToan
        };
    }
}

