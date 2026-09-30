using DanhMucService.Domain.Entities.DanhMuc;

namespace DanhMucService.Application.DTOs.DanhMuc;

public static class DanhMucLinhVucMapper
{
    public static DanhMucLinhVucDto ToDto(this DanhMucLinhVucEntity entity)
    {
        return new DanhMucLinhVucDto
        {
            Id = entity.Id,
            MaLinhVuc = entity.MaLinhVuc,
            TenLinhVuc = entity.TenLinhVuc,
            ThuTuSapXep = entity.ThuTuSapXep,
            TrangThai = entity.TrangThai,
            MoTa = entity.MoTa,
            GhiChu = entity.GhiChu
        };
    }

    public static DanhMucLinhVucEntity ToEntity(this UpsertDanhMucLinhVucRequest request)
    {
        return new DanhMucLinhVucEntity
        {
            MaLinhVuc = request.MaLinhVuc.Trim(),
            TenLinhVuc = request.TenLinhVuc.Trim(),
            ThuTuSapXep = request.ThuTuSapXep,
            TrangThai = request.TrangThai,
            MoTa = request.MoTa?.Trim(),
            GhiChu = request.GhiChu?.Trim()
        };
    }
}

