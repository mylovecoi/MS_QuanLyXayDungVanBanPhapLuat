namespace DanhMucService.Infrastructure.Persistence;

public static class DanhMucSeedIds
{
    public static class QuyTrinh
    {
        public static readonly Guid DeXuatDangKyXayDungQppl = Guid.Parse("33333333-3333-3333-3333-333333333301");
    }

    public static class BuocDangKyXayDungQppl
    {
        public static readonly Guid LapHoSo = Guid.Parse("33333333-3333-3333-3333-333333333311");
        public static readonly Guid TrinhHoSo = Guid.Parse("33333333-3333-3333-3333-333333333312");
        public static readonly Guid PheDuyet = Guid.Parse("33333333-3333-3333-3333-333333333313");
        public static readonly Guid CapNhatKetQua = Guid.Parse("33333333-3333-3333-3333-333333333314");
        public static readonly Guid HoanThanh = Guid.Parse("33333333-3333-3333-3333-333333333315");
    }

    public static class ChuyenBuocDangKyXayDungQppl
    {
        public static readonly Guid LapHoSoToTrinhHoSo = Guid.Parse("33333333-3333-3333-3333-333333333321");
        public static readonly Guid TrinhHoSoToPheDuyet = Guid.Parse("33333333-3333-3333-3333-333333333322");
        public static readonly Guid PheDuyetToCapNhatKetQua = Guid.Parse("33333333-3333-3333-3333-333333333323");
        public static readonly Guid PheDuyetToLapHoSo = Guid.Parse("33333333-3333-3333-3333-333333333324");
        public static readonly Guid PheDuyetToHoanThanh = Guid.Parse("33333333-3333-3333-3333-333333333325");
        public static readonly Guid CapNhatKetQuaToHoanThanh = Guid.Parse("33333333-3333-3333-3333-333333333326");
    }
}
