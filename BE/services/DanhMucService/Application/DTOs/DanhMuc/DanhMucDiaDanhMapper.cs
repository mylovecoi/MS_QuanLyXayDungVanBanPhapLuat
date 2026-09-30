using DanhMucService.Domain.Entities.DanhMuc;

namespace DanhMucService.Application.DTOs.DanhMuc;

public static class DanhMucDiaDanhMapper
{
    public static DanhMucDiaDanhDto ToDto(this DanhMucDiaDanhEntity entity)
    {
        return new DanhMucDiaDanhDto
        {
            Id = entity.Id,
            TenDiaDanh = entity.TenDiaDanh,
            Level = entity.Level,
            STTSapXep = entity.STTSapXep,
            DiaDanhCapTrenId = entity.DiaDanhCapTrenId,
            TenDiaDanhChuQuan = entity.TenDiaDanhChuQuan
        };
    }

    public static DanhMucDiaDanhEntity ToEntity(this UpsertDanhMucDiaDanhRequest request)
    {
        return new DanhMucDiaDanhEntity
        {
            TenDiaDanh = request.TenDiaDanh.Trim(),
            Level = request.Level,
            STTSapXep = request.STTSapXep,
            DiaDanhCapTrenId = request.DiaDanhCapTrenId
        };
    }
}

