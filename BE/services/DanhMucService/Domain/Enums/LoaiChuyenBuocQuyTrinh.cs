namespace DanhMucService.Domain.Enums;

public enum LoaiChuyenBuocQuyTrinh
{
    Forward = 1,
    ReturnStep = 2,
    Finish = 3
}

public static class LoaiChuyenBuocQuyTrinhExtensions
{
    public static string ToStorageValue(this LoaiChuyenBuocQuyTrinh value)
    {
        return value switch
        {
            LoaiChuyenBuocQuyTrinh.Forward => "Forward",
            LoaiChuyenBuocQuyTrinh.ReturnStep => "Return",
            LoaiChuyenBuocQuyTrinh.Finish => "Finish",
            _ => "Forward"
        };
    }

    public static bool TryParseStorageValue(string? value, out LoaiChuyenBuocQuyTrinh result)
    {
        switch ((value ?? string.Empty).Trim().ToUpperInvariant())
        {
            case "FORWARD":
                result = LoaiChuyenBuocQuyTrinh.Forward;
                return true;
            case "RETURN":
                result = LoaiChuyenBuocQuyTrinh.ReturnStep;
                return true;
            case "FINISH":
                result = LoaiChuyenBuocQuyTrinh.Finish;
                return true;
            default:
                result = LoaiChuyenBuocQuyTrinh.Forward;
                return false;
        }
    }
}

