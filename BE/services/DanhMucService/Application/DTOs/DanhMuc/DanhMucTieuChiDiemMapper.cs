using DanhMucService.Domain.Entities.DanhMuc;

namespace DanhMucService.Application.DTOs.DanhMuc;

public static class DanhMucTieuChiDiemMapper
{
    public static DanhMucTieuChiDiemDto ToDto(this DanhMucTieuChiDiemEntity entity)
    {
        return new DanhMucTieuChiDiemDto
        {
            Id = entity.Id,
            MaTieuChi = entity.MaTieuChi,
            TenTieuChi = entity.TenTieuChi,
            LoaiTieuChi = entity.LoaiTieuChi,
            KieuGiaTri = entity.KieuGiaTri,
            DonViGiaTri = entity.DonViGiaTri,
            ThuTuSapXep = entity.ThuTuSapXep,
            DiemToiDa = entity.DiemToiDa,
            TrangThai = entity.TrangThai,
            MoTa = entity.MoTa,
            GhiChu = entity.GhiChu,
            Mucs = entity.Mucs
                .OrderBy(x => x.ThuTuSapXep)
                .ThenBy(x => x.TuGiaTri)
                .Select(x => x.ToDto())
                .ToArray()
        };
    }

    public static DanhMucTieuChiDiemEntity ToEntity(this UpsertDanhMucTieuChiDiemRequest request)
    {
        return new DanhMucTieuChiDiemEntity
        {
            Id = request.Id ?? Guid.NewGuid(),
            MaTieuChi = request.MaTieuChi.Trim().ToUpperInvariant(),
            TenTieuChi = request.TenTieuChi.Trim(),
            LoaiTieuChi = request.LoaiTieuChi.Trim().ToUpperInvariant(),
            KieuGiaTri = request.KieuGiaTri.Trim().ToUpperInvariant(),
            DonViGiaTri = request.DonViGiaTri.Trim().ToUpperInvariant(),
            ThuTuSapXep = request.ThuTuSapXep,
            DiemToiDa = request.DiemToiDa,
            TrangThai = request.TrangThai,
            MoTa = string.IsNullOrWhiteSpace(request.MoTa) ? null : request.MoTa.Trim(),
            GhiChu = string.IsNullOrWhiteSpace(request.GhiChu) ? null : request.GhiChu.Trim(),
            Mucs = request.Mucs.Select(x => x.ToEntity()).ToList()
        };
    }

    public static DanhMucTieuChiDiemMucDto ToDto(this DanhMucTieuChiDiemMucEntity entity)
    {
        return new DanhMucTieuChiDiemMucDto
        {
            Id = entity.Id,
            DanhMucTieuChiDiemId = entity.DanhMucTieuChiDiemId,
            TuGiaTri = entity.TuGiaTri,
            DenGiaTri = entity.DenGiaTri,
            BaoGomTuGiaTri = entity.BaoGomTuGiaTri,
            BaoGomDenGiaTri = entity.BaoGomDenGiaTri,
            Diem = entity.Diem,
            NhanHienThi = entity.NhanHienThi,
            ThuTuSapXep = entity.ThuTuSapXep,
            TrangThai = entity.TrangThai,
            GhiChu = entity.GhiChu
        };
    }

    public static DanhMucTieuChiDiemMucEntity ToEntity(this UpsertDanhMucTieuChiDiemMucRequest request)
    {
        return new DanhMucTieuChiDiemMucEntity
        {
            Id = request.Id ?? Guid.NewGuid(),
            TuGiaTri = request.TuGiaTri,
            DenGiaTri = request.DenGiaTri,
            BaoGomTuGiaTri = request.BaoGomTuGiaTri,
            BaoGomDenGiaTri = request.BaoGomDenGiaTri,
            Diem = request.Diem,
            NhanHienThi = string.IsNullOrWhiteSpace(request.NhanHienThi) ? null : request.NhanHienThi.Trim(),
            ThuTuSapXep = request.ThuTuSapXep,
            TrangThai = request.TrangThai,
            GhiChu = string.IsNullOrWhiteSpace(request.GhiChu) ? null : request.GhiChu.Trim()
        };
    }
}

