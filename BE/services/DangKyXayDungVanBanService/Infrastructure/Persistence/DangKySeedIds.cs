namespace DangKyXayDungVanBanService.Infrastructure.Persistence;

public static class DangKySeedIds
{
    public static class TrangThai
    {
        public static readonly Guid MoiTao = Guid.Parse("11111111-1111-1111-1111-111111111101");
        public static readonly Guid DangSoanThao = Guid.Parse("11111111-1111-1111-1111-111111111102");
        public static readonly Guid DaTrinhPheDuyet = Guid.Parse("11111111-1111-1111-1111-111111111103");
        public static readonly Guid DaPheDuyet = Guid.Parse("11111111-1111-1111-1111-111111111104");
        public static readonly Guid BiTraLai = Guid.Parse("11111111-1111-1111-1111-111111111105");
        public static readonly Guid KhongPheDuyet = Guid.Parse("11111111-1111-1111-1111-111111111106");
        public static readonly Guid DaCapNhatKetQua = Guid.Parse("11111111-1111-1111-1111-111111111107");
        public static readonly Guid HoanThanh = Guid.Parse("11111111-1111-1111-1111-111111111108");
        public static readonly Guid DaChuyenQuyTrinhXayDung = Guid.Parse("11111111-1111-1111-1111-111111111109");
    }

    public static class HanhDong
    {
        public static readonly Guid TaoMoi = Guid.Parse("22222222-2222-2222-2222-222222222201");
        public static readonly Guid CapNhatHoSo = Guid.Parse("22222222-2222-2222-2222-222222222202");
        public static readonly Guid TrinhPheDuyet = Guid.Parse("22222222-2222-2222-2222-222222222203");
        public static readonly Guid PheDuyet = Guid.Parse("22222222-2222-2222-2222-222222222204");
        public static readonly Guid TraLai = Guid.Parse("22222222-2222-2222-2222-222222222205");
        public static readonly Guid KhongPheDuyet = Guid.Parse("22222222-2222-2222-2222-222222222206");
        public static readonly Guid CapNhatKetQua = Guid.Parse("22222222-2222-2222-2222-222222222207");
        public static readonly Guid HoanThanh = Guid.Parse("22222222-2222-2222-2222-222222222208");
        public static readonly Guid KhoiTaoQuyTrinhXayDung = Guid.Parse("22222222-2222-2222-2222-222222222209");
    }

    public static class DanhMucQuyTrinh
    {
        public static readonly Guid DeXuatDangKyXayDungQppl = Guid.Parse("33333333-3333-3333-3333-333333333301");
    }

    public static class DanhMucBuocDangKyXayDungQppl
    {
        public static readonly Guid LapHoSo = Guid.Parse("33333333-3333-3333-3333-333333333311");
        public static readonly Guid TrinhHoSo = Guid.Parse("33333333-3333-3333-3333-333333333312");
        public static readonly Guid PheDuyet = Guid.Parse("33333333-3333-3333-3333-333333333313");
        public static readonly Guid CapNhatKetQua = Guid.Parse("33333333-3333-3333-3333-333333333314");
        public static readonly Guid HoanThanh = Guid.Parse("33333333-3333-3333-3333-333333333315");
    }

    public static class DanhMucChuyenBuocDangKyXayDungQppl
    {
        public static readonly Guid LapHoSoToTrinhHoSo = Guid.Parse("33333333-3333-3333-3333-333333333321");
        public static readonly Guid TrinhHoSoToPheDuyet = Guid.Parse("33333333-3333-3333-3333-333333333322");
        public static readonly Guid PheDuyetToCapNhatKetQua = Guid.Parse("33333333-3333-3333-3333-333333333323");
        public static readonly Guid TrinhHoSoToLapHoSo = Guid.Parse("33333333-3333-3333-3333-333333333324");
        public static readonly Guid TrinhHoSoToHoanThanh = Guid.Parse("33333333-3333-3333-3333-333333333325");
        public static readonly Guid CapNhatKetQuaToHoanThanh = Guid.Parse("33333333-3333-3333-3333-333333333326");
    }

    public static class CauHinhChuyenTrangThai
    {
        public static readonly Guid CapNhatHoSoDangSoanThao = Guid.Parse("44444444-4444-4444-4444-444444444401");
        public static readonly Guid CapNhatHoSoBiTraLai = Guid.Parse("44444444-4444-4444-4444-444444444402");
        public static readonly Guid TrinhPheDuyetDangSoanThao = Guid.Parse("44444444-4444-4444-4444-444444444403");
        public static readonly Guid TrinhPheDuyetBiTraLai = Guid.Parse("44444444-4444-4444-4444-444444444404");
        public static readonly Guid PheDuyet = Guid.Parse("44444444-4444-4444-4444-444444444405");
        public static readonly Guid TraLai = Guid.Parse("44444444-4444-4444-4444-444444444406");
        public static readonly Guid KhongPheDuyet = Guid.Parse("44444444-4444-4444-4444-444444444407");
        public static readonly Guid CapNhatKetQua = Guid.Parse("44444444-4444-4444-4444-444444444408");
        public static readonly Guid HoanThanh = Guid.Parse("44444444-4444-4444-4444-444444444409");
        public static readonly Guid KhoiTaoQuyTrinhXayDung = Guid.Parse("44444444-4444-4444-4444-444444444410");
    }
}
