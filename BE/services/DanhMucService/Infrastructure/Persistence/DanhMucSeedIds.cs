namespace DanhMucService.Infrastructure.Persistence;

public static class DanhMucSeedIds
{
    public static class QuyTrinh
    {
        public static readonly Guid DeXuatDangKyXayDungQppl = Guid.Parse("33333333-3333-3333-3333-333333333301");
    }

    public static class BuocDangKyXayDungQppl
    {
        public static readonly Guid DeXuatDangKy = Guid.Parse("33333333-3333-3333-3333-333333333311");
        public static readonly Guid PheDuyetCapNhatKetQua = Guid.Parse("33333333-3333-3333-3333-333333333313");
        public static readonly Guid LapHoSo = DeXuatDangKy;
        public static readonly Guid TrinhHoSo = Guid.Parse("33333333-3333-3333-3333-333333333312");
        public static readonly Guid PheDuyet = PheDuyetCapNhatKetQua;
        public static readonly Guid CapNhatKetQua = Guid.Parse("33333333-3333-3333-3333-333333333314");
        public static readonly Guid HoanThanh = Guid.Parse("33333333-3333-3333-3333-333333333315");
    }

    public static class ChuyenBuocDangKyXayDungQppl
    {
        public static readonly Guid DeXuatDangKyToPheDuyetCapNhatKetQua = Guid.Parse("33333333-3333-3333-3333-333333333321");
        public static readonly Guid PheDuyetCapNhatKetQuaToDeXuatDangKy = Guid.Parse("33333333-3333-3333-3333-333333333324");
        public static readonly Guid PheDuyetCapNhatKetQuaToHoanThanh = Guid.Parse("33333333-3333-3333-3333-333333333326");
        public static readonly Guid LapHoSoToTrinhHoSo = DeXuatDangKyToPheDuyetCapNhatKetQua;
        public static readonly Guid TrinhHoSoToPheDuyet = Guid.Parse("33333333-3333-3333-3333-333333333322");
        public static readonly Guid PheDuyetToCapNhatKetQua = Guid.Parse("33333333-3333-3333-3333-333333333323");
        public static readonly Guid PheDuyetToLapHoSo = PheDuyetCapNhatKetQuaToDeXuatDangKy;
        public static readonly Guid PheDuyetToHoanThanh = Guid.Parse("33333333-3333-3333-3333-333333333325");
        public static readonly Guid CapNhatKetQuaToHoanThanh = PheDuyetCapNhatKetQuaToHoanThanh;
    }
}
