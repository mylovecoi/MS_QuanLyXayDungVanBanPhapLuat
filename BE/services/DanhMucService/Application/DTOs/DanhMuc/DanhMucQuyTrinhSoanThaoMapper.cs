using DanhMucService.Domain.Entities.DanhMuc;

namespace DanhMucService.Application.DTOs.DanhMuc;

public static class DanhMucQuyTrinhSoanThaoMapper
{
    public static DanhMucQuyTrinhSoanThaoDto ToDto(this DanhMucQuyTrinhSoanThaoEntity entity)
    {
        return new DanhMucQuyTrinhSoanThaoDto
        {
            Id = entity.Id,
            MaQuyTrinh = entity.MaQuyTrinh,
            TenQuyTrinh = entity.TenQuyTrinh,
            LoaiQuyTrinh = entity.LoaiQuyTrinh,
            TenLoaiQuyTrinh = entity.TenLoaiQuyTrinh,
            DanhMucVanBanId = entity.DanhMucVanBanId,
            DanhMucVanBanIds = entity.DanhMucVanBanIds.ToArray(),
            TenLoaiVanBan = entity.TenLoaiVanBan,
            CapApDung = entity.CapApDung,
            CapApDungs = entity.CapApDungs.ToArray(),
            PhienBan = entity.PhienBan,
            TrangThai = entity.TrangThai,
            MoTa = entity.MoTa,
            GhiChu = entity.GhiChu,
            SoBuoc = entity.SoBuoc,
            SoNhanhChuyen = entity.SoNhanhChuyen,
            BuocQuyTrinhs = entity.BuocQuyTrinhs.Select(x => x.ToDto()).ToArray(),
            ChuyenBuocs = entity.ChuyenBuocs.Select(x => x.ToDto()).ToArray()
        };
    }

    public static DanhMucBuocQuyTrinhDto ToDto(this DanhMucBuocQuyTrinhEntity entity)
    {
        return new DanhMucBuocQuyTrinhDto
        {
            Id = entity.Id,
            QuyTrinhSoanThaoId = entity.QuyTrinhSoanThaoId,
            MaBuoc = entity.MaBuoc,
            TenBuoc = entity.TenBuoc,
            ThuTuSapXep = entity.ThuTuSapXep,
            LoaiBuoc = entity.LoaiBuoc,
            BatBuoc = entity.BatBuoc,
            ChoPhepBoQua = entity.ChoPhepBoQua,
            ChoPhepQuayLui = entity.ChoPhepQuayLui,
            CachHoanThanh = entity.CachHoanThanh,
            SoLuongPhanHoiToiThieu = entity.SoLuongPhanHoiToiThieu,
            YeuCauFileDinhKem = entity.YeuCauFileDinhKem,
            SoLanTraLaiToiDa = entity.SoLanTraLaiToiDa,
            SoNgayXuLyTieuChuan = entity.SoNgayXuLyTieuChuan,
            SoNgayCanhBaoSapHan = entity.SoNgayCanhBaoSapHan,
            DonViTiepNhanMacDinhId = entity.DonViTiepNhanMacDinhId,
            TenDonViTiepNhanMacDinh = entity.TenDonViTiepNhanMacDinh,
            MoTa = entity.MoTa,
            GhiChu = entity.GhiChu
        };
    }

    public static DanhMucChuyenBuocQuyTrinhDto ToDto(this DanhMucChuyenBuocQuyTrinhEntity entity)
    {
        return new DanhMucChuyenBuocQuyTrinhDto
        {
            Id = entity.Id,
            QuyTrinhSoanThaoId = entity.QuyTrinhSoanThaoId,
            TuBuocId = entity.TuBuocId,
            DenBuocId = entity.DenBuocId,
            TuBuocMa = entity.TuBuocMa,
            DenBuocMa = entity.DenBuocMa,
            DieuKienKetQua = entity.DieuKienKetQua,
            LoaiChuyenBuoc = entity.LoaiChuyenBuoc,
            LaNhanhMacDinh = entity.LaNhanhMacDinh,
            YeuCauNhapLyDo = entity.YeuCauNhapLyDo,
            IsKetThuc = entity.IsKetThuc,
            MoTa = entity.MoTa,
            GhiChu = entity.GhiChu
        };
    }

    public static DanhMucLookupDto ToDto(this DanhMucLookupEntity entity)
    {
        return new DanhMucLookupDto
        {
            Id = entity.Id,
            Ma = entity.Ma,
            Ten = entity.Ten
        };
    }

    public static DanhMucQuyTrinhSoanThaoEntity ToEntity(this UpsertDanhMucQuyTrinhSoanThaoRequest request)
    {
        return new DanhMucQuyTrinhSoanThaoEntity
        {
            Id = request.Id,
            MaQuyTrinh = request.MaQuyTrinh,
            TenQuyTrinh = request.TenQuyTrinh,
            LoaiQuyTrinh = request.LoaiQuyTrinh,
            DanhMucVanBanId = request.DanhMucVanBanId,
            DanhMucVanBanIds = request.DanhMucVanBanIds.ToList(),
            CapApDung = request.CapApDung,
            CapApDungs = request.CapApDungs.ToList(),
            PhienBan = request.PhienBan,
            TrangThai = request.TrangThai,
            MoTa = request.MoTa,
            GhiChu = request.GhiChu,
            BuocQuyTrinhs = request.BuocQuyTrinhs.Select(x => x.ToEntity()).ToList(),
            ChuyenBuocs = request.ChuyenBuocs.Select(x => x.ToEntity()).ToList()
        };
    }

    public static DanhMucBuocQuyTrinhEntity ToEntity(this UpsertDanhMucBuocQuyTrinhRequest request)
    {
        return new DanhMucBuocQuyTrinhEntity
        {
            Id = request.Id,
            MaBuoc = request.MaBuoc,
            TenBuoc = request.TenBuoc,
            ThuTuSapXep = request.ThuTuSapXep,
            LoaiBuoc = request.LoaiBuoc,
            BatBuoc = request.BatBuoc,
            ChoPhepBoQua = request.ChoPhepBoQua,
            ChoPhepQuayLui = request.ChoPhepQuayLui,
            CachHoanThanh = request.CachHoanThanh,
            SoLuongPhanHoiToiThieu = request.SoLuongPhanHoiToiThieu,
            YeuCauFileDinhKem = request.YeuCauFileDinhKem,
            SoLanTraLaiToiDa = request.SoLanTraLaiToiDa,
            SoNgayXuLyTieuChuan = request.SoNgayXuLyTieuChuan,
            SoNgayCanhBaoSapHan = request.SoNgayCanhBaoSapHan,
            DonViTiepNhanMacDinhId = request.DonViTiepNhanMacDinhId,
            MoTa = request.MoTa,
            GhiChu = request.GhiChu
        };
    }

    public static DanhMucChuyenBuocQuyTrinhEntity ToEntity(this UpsertDanhMucChuyenBuocQuyTrinhRequest request)
    {
        return new DanhMucChuyenBuocQuyTrinhEntity
        {
            Id = request.Id,
            TuBuocMa = request.TuBuocMa,
            DenBuocMa = request.DenBuocMa,
            DieuKienKetQua = request.DieuKienKetQua,
            LoaiChuyenBuoc = request.LoaiChuyenBuoc,
            LaNhanhMacDinh = request.LaNhanhMacDinh,
            YeuCauNhapLyDo = request.YeuCauNhapLyDo,
            IsKetThuc = request.IsKetThuc,
            MoTa = request.MoTa,
            GhiChu = request.GhiChu
        };
    }
}

