using DanhMucService.Domain.Entities.DanhMuc;

namespace DanhMucService.Application.DTOs.DanhMuc;

public static class DanhMucVanBanMapper
{
    public static DanhMucVanBanDto ToDto(this DanhMucVanBanEntity entity)
    {
        return new DanhMucVanBanDto
        {
            Id = entity.Id,
            TenLoaiVanBan = entity.TenLoaiVanBan,
            CapChinhQuyen = entity.CapChinhQuyen,
            ChuTheBanHanh = entity.ChuTheBanHanh,
            KyHieuMau = entity.KyHieuMau,
            ThuTuSapXep = entity.ThuTuSapXep,
            TrangThai = entity.TrangThai,
            MoTa = entity.MoTa,
            GhiChu = entity.GhiChu
        };
    }

    public static DanhMucVanBanEntity ToEntity(this UpsertDanhMucVanBanRequest request)
    {
        return new DanhMucVanBanEntity
        {
            TenLoaiVanBan = request.TenLoaiVanBan.Trim(),
            CapChinhQuyen = request.CapChinhQuyen.Trim(),
            ChuTheBanHanh = request.ChuTheBanHanh.Trim(),
            KyHieuMau = request.KyHieuMau?.Trim(),
            ThuTuSapXep = request.ThuTuSapXep,
            TrangThai = request.TrangThai,
            MoTa = request.MoTa?.Trim(),
            GhiChu = request.GhiChu?.Trim()
        };
    }
}

