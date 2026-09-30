namespace DanhMucService.Domain.Enums;

public enum TrangThaiHoSo
{
    DangXuLy = 1,
    HoanThanh = 2,
    HoanThanhDungHan = 3,
    HoanThanhQuaHan = 4
}

public static class TrangThaiHoSoExtensions
{
    public static string ToStorageValue(this TrangThaiHoSo value)
    {
        return value switch
        {
            TrangThaiHoSo.DangXuLy => "DANG_XU_LY",
            TrangThaiHoSo.HoanThanh => "HOAN_THANH",
            TrangThaiHoSo.HoanThanhDungHan => "HOAN_THANH_DUNG_HAN",
            TrangThaiHoSo.HoanThanhQuaHan => "HOAN_THANH_QUA_HAN",
            _ => "DANG_XU_LY"
        };
    }

    public static bool TryParseStorageValue(string? value, out TrangThaiHoSo result)
    {
        switch ((value ?? string.Empty).Trim().ToUpperInvariant())
        {
            case "DANG_XU_LY":
                result = TrangThaiHoSo.DangXuLy;
                return true;
            case "HOAN_THANH":
                result = TrangThaiHoSo.HoanThanh;
                return true;
            case "HOAN_THANH_DUNG_HAN":
                result = TrangThaiHoSo.HoanThanhDungHan;
                return true;
            case "HOAN_THANH_QUA_HAN":
                result = TrangThaiHoSo.HoanThanhQuaHan;
                return true;
            default:
                result = TrangThaiHoSo.DangXuLy;
                return false;
        }
    }
}

