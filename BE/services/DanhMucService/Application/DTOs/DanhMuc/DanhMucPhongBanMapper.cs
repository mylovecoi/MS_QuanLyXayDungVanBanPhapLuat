using DanhMucService.Domain.Entities.DanhMuc;

namespace DanhMucService.Application.DTOs.DanhMuc;

public static class DanhMucPhongBanMapper
{
    public static DanhMucPhongBanDto ToDto(this DanhMucPhongBanEntity entity)
    {
        return new DanhMucPhongBanDto
        {
            Id = entity.Id,
            TenPhongBan = entity.TenPhongBan,
            MaPhongBan = entity.MaPhongBan,
            LoaiPhongBan = entity.LoaiPhongBan,
            TenLoaiPhongBan = GetLoaiPhongBanName(entity.LoaiPhongBan),
            DanhMucDonViId = entity.DanhMucDonViId,
            TenDonVi = entity.TenDonVi
        };
    }

    public static DanhMucPhongBanEntity ToEntity(this UpsertDanhMucPhongBanRequest request)
    {
        return new DanhMucPhongBanEntity
        {
            TenPhongBan = request.TenPhongBan.Trim(),
            MaPhongBan = request.MaPhongBan.Trim(),
            LoaiPhongBan = request.LoaiPhongBan,
            DanhMucDonViId = request.DanhMucDonViId
        };
    }

    public static string GetLoaiPhongBanName(int loaiPhongBan) => loaiPhongBan switch
    {
        0 => "Lưu trữ",
        1 => "Soạn thảo",
        2 => "Công chứng",
        _ => "Khác"
    };
}

