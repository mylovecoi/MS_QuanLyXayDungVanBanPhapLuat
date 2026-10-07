using System.Globalization;
using System.IO.Compression;
using System.Text;
using System.Xml.Linq;

namespace KhaoSatThiHanhPhapLuatService.Application;

public interface IWordSurveyParser
{
    Task<WordSurveyParseResult> ParseAsync(Stream document, CancellationToken cancellationToken = default);
}

public sealed class WordSurveyParser : IWordSurveyParser
{
    private static readonly XNamespace W = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
    private static readonly string[] RequiredHeaders = ["macauhoi", "noidung", "loai", "madapan", "dapan"];

    public async Task<WordSurveyParseResult> ParseAsync(Stream document, CancellationToken cancellationToken = default)
    {
        using var archive = new ZipArchive(document, ZipArchiveMode.Read, leaveOpen: true);
        var entry = archive.GetEntry("word/document.xml") ?? throw new InvalidOperationException("File Word không có nội dung document.xml hợp lệ.");
        await using var xmlStream = entry.Open();
        var xml = await XDocument.LoadAsync(xmlStream, LoadOptions.None, cancellationToken);
        var rows = new List<WordSurveyRow>();
        var found = false;
        foreach (var table in xml.Descendants(W + "tbl"))
        {
            var tableRows = table.Elements(W + "tr").Select(ReadCells).Where(x => x.Count > 0).ToList();
            if (tableRows.Count < 2) continue;
            var headers = tableRows[0].Select(Normalize).ToList();
            var indexes = RequiredHeaders.ToDictionary(header => header, header => headers.FindIndex(x => x == header));
            if (indexes.Values.Any(x => x < 0)) continue;
            found = true;
            var valueIndex = headers.FindIndex(x => x is "giatritraloi" or "traloi" or "giatri");
            var countIndex = headers.FindIndex(x => x is "soluong" or "soluongtraloi");
            var totalIndex = headers.FindIndex(x => x is "tongsotraloi" or "tongso");
            for (var rowIndex = 1; rowIndex < tableRows.Count; rowIndex++)
            {
                var row = tableRows[rowIndex];
                string At(string header) => indexes[header] < row.Count ? row[indexes[header]].Trim() : string.Empty;
                var questionCode = At("macauhoi");
                if (string.IsNullOrWhiteSpace(questionCode)) continue;
                rows.Add(new WordSurveyRow(rowIndex + 1, questionCode, At("noidung"), At("loai").ToUpperInvariant(), At("madapan"), At("dapan"), valueIndex >= 0 && valueIndex < row.Count ? row[valueIndex].Trim() : string.Empty, AtDecimal(countIndex, row), AtDecimal(totalIndex, row)));
            }
        }
        if (!found) throw new InvalidOperationException("Không tìm thấy bảng Word chuẩn. Bảng phải có các cột: Mã câu hỏi, Nội dung, Loại, Mã đáp án, Đáp án.");
        if (rows.Count == 0) throw new InvalidOperationException("Mẫu Word không có câu hỏi hợp lệ.");
        return new WordSurveyParseResult(rows);
    }
    private static decimal? AtDecimal(int index, IReadOnlyList<string> row) => index >= 0 && index < row.Count && decimal.TryParse(row[index], NumberStyles.Number, CultureInfo.InvariantCulture, out var value) ? value : null;

    private static List<string> ReadCells(XElement row) => row.Elements(W + "tc").Select(cell => string.Concat(cell.Descendants(W + "t").Select(x => x.Value))).ToList();
    private static string Normalize(string value)
    {
        var decomposed = value.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
        return string.Concat(decomposed.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark && char.IsLetterOrDigit(c)));
    }
}

public sealed record WordSurveyParseResult(IReadOnlyList<WordSurveyRow> Rows);
public sealed record WordSurveyRow(int RowNumber, string MaCauHoi, string NoiDung, string LoaiCauHoi, string MaDapAn, string DapAn, string GiaTriTraLoi, decimal? SoLuong, decimal? TongSoTraLoi);
