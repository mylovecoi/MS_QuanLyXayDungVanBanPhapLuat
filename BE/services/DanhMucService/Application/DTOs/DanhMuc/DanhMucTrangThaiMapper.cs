using DanhMucService.Domain.Entities.DanhMuc;

namespace DanhMucService.Application.DTOs.DanhMuc;

public static class DanhMucTrangThaiMapper
{
    public static DanhMucTrangThaiDto ToDto(this DanhMucTrangThaiEntity entity)
    {
        return new DanhMucTrangThaiDto
        {
            Id = entity.Id,
            NhomTrangThai = entity.NhomTrangThai,
            MaTrangThai = entity.MaTrangThai,
            TenTrangThai = entity.TenTrangThai,
            MaMauHex = entity.MaMauHex,
            ThuTuSapXep = entity.ThuTuSapXep,
            TrangThai = entity.TrangThai,
            MoTa = entity.MoTa,
            GhiChu = entity.GhiChu
        };
    }

    public static DanhMucTrangThaiEntity ToEntity(this UpsertDanhMucTrangThaiRequest request)
    {
        return new DanhMucTrangThaiEntity
        {
            Id = request.Id ?? Guid.Empty,
            NhomTrangThai = request.NhomTrangThai.Trim(),
            MaTrangThai = request.MaTrangThai.Trim(),
            TenTrangThai = request.TenTrangThai.Trim(),
            MaMauHex = request.MaMauHex.Trim(),
            ThuTuSapXep = request.ThuTuSapXep,
            TrangThai = request.TrangThai,
            MoTa = request.MoTa?.Trim(),
            GhiChu = request.GhiChu?.Trim()
        };
    }
}

