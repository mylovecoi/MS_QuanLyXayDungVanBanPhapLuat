using DanhMucService.Domain.Entities.DanhMuc;
using DanhMucService.Domain.Enums;

namespace DanhMucService.Application.DTOs.DanhMuc;

public static class DanhMucCanBoMapper
{
    public static DanhMucCanBoDto ToDto(this DanhMucCanBoEntity entity)
    {
        return new DanhMucCanBoDto
        {
            Id = entity.Id,
            DonViQuanLyId = entity.DonViQuanLyId,
            TenDonViQuanLy = entity.TenDonViQuanLy,
            TenCanBo = entity.TenCanBo,
            NgaySinh = entity.NgaySinh,
            UserId = entity.UserId,
            PhongBanId = entity.PhongBanId,
            TenPhongBan = entity.TenPhongBan,
            GioiTinh = entity.GioiTinh,
            TrinhDoChuyenMon = entity.TrinhDoChuyenMon,
            LoaiLaoDong = entity.LoaiLaoDong,
            TenLoaiLaoDong = ToDisplayName(entity.LoaiLaoDong),
            SoTienBHXH = entity.SoTienBHXH,
            SoTienBHYT = entity.SoTienBHYT,
            SoQuyetDinhDung = entity.SoQuyetDinhDung,
            NgayQuyetDinhDung = entity.NgayQuyetDinhDung,
            GhiChu = entity.GhiChu,
            SoQuyetDinhBoNhiem = entity.SoQuyetDinhBoNhiem,
            NgayQuyetDinhBoNhiem = entity.NgayQuyetDinhBoNhiem,
            SoQuyetDinhCapThe = entity.SoQuyetDinhCapThe,
            NgayQuyetDinhCapThe = entity.NgayQuyetDinhCapThe,
            SoTheCongChungVien = entity.SoTheCongChungVien,
            ChucVu = entity.ChucVu,
            MucPhiBaoHiemTrachNhiem = entity.MucPhiBaoHiemTrachNhiem,
            ViTriViecLam = entity.ViTriViecLam,
            NgayTuyenDung = entity.NgayTuyenDung,
            SoHopDongLaoDong = entity.SoHopDongLaoDong,
            NgayKyHopDongLaoDong = entity.NgayKyHopDongLaoDong
        };
    }

    public static DanhMucCanBoEntity ToEntity(this UpsertDanhMucCanBoRequest request)
    {
        return new DanhMucCanBoEntity
        {
            Id = request.Id ?? Guid.NewGuid(),
            DonViQuanLyId = request.DonViQuanLyId,
            TenCanBo = request.TenCanBo.Trim(),
            NgaySinh = request.NgaySinh,
            UserId = request.UserId,
            PhongBanId = request.PhongBanId,
            GioiTinh = request.GioiTinh,
            TrinhDoChuyenMon = request.TrinhDoChuyenMon.Trim(),
            LoaiLaoDong = request.LoaiLaoDong,
            SoTienBHXH = request.SoTienBHXH,
            SoTienBHYT = request.SoTienBHYT,
            SoQuyetDinhDung = request.SoQuyetDinhDung,
            NgayQuyetDinhDung = request.NgayQuyetDinhDung,
            GhiChu = request.GhiChu,
            SoQuyetDinhBoNhiem = request.SoQuyetDinhBoNhiem,
            NgayQuyetDinhBoNhiem = request.NgayQuyetDinhBoNhiem,
            SoQuyetDinhCapThe = request.SoQuyetDinhCapThe,
            NgayQuyetDinhCapThe = request.NgayQuyetDinhCapThe,
            SoTheCongChungVien = request.SoTheCongChungVien,
            ChucVu = request.ChucVu,
            MucPhiBaoHiemTrachNhiem = request.MucPhiBaoHiemTrachNhiem,
            ViTriViecLam = request.ViTriViecLam,
            NgayTuyenDung = request.NgayTuyenDung,
            SoHopDongLaoDong = request.SoHopDongLaoDong,
            NgayKyHopDongLaoDong = request.NgayKyHopDongLaoDong
        };
    }

    public static string ToDisplayName(LoaiLaoDongType value)
    {
        return value switch
        {
            LoaiLaoDongType.CongChungVien => "Công chứng viên",
            LoaiLaoDongType.NhanVienNghiepVu => "Nhân viên nghiệp vụ",
            LoaiLaoDongType.NhanVienKhac => "Nhân viên khác",
            _ => value.ToString()
        };
    }
}

