using System.Globalization;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace KhaoSatThiHanhPhapLuatService.Application;

public interface IWordSurveyParser
{
    Task<WordSurveyParseResult> ParseAsync(Stream document, CancellationToken cancellationToken = default);
}

public sealed class WordSurveyParser : IWordSurveyParser
{
    private static readonly XNamespace W = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
    private static readonly string[] RequiredHeaders = ["macauhoi", "noidung", "loai"];

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
            var answerCodeIndex = headers.FindIndex(x => x == "madapan");
            var answerIndex = headers.FindIndex(x => x == "dapan");
            var valueIndex = headers.FindIndex(x => x is "giatritraloi" or "traloi" or "giatri");
            var countIndex = headers.FindIndex(x => x is "soluong" or "soluongtraloi");
            var totalIndex = headers.FindIndex(x => x is "tongsotraloi" or "tongso");
            var requiredIndex = headers.FindIndex(x => x is "batbuoc" or "cobatbuoc");
            var multiSelectIndex = headers.FindIndex(x => x is "chophepnhieuluachon" or "nhieuluachon");
            var denominatorIndex = headers.FindIndex(x => x is "mausotyle" or "cachtinhtyle");
            var freeTextIndex = headers.FindIndex(x => x is "coykientudo" or "ykientudo");
            for (var rowIndex = 1; rowIndex < tableRows.Count; rowIndex++)
            {
                var row = tableRows[rowIndex];
                string ValueOf(string header) => indexes[header] < row.Count ? row[indexes[header]].Trim() : string.Empty;
                var questionCode = ValueOf("macauhoi");
                if (string.IsNullOrWhiteSpace(questionCode)) continue;
                rows.Add(new WordSurveyRow(rowIndex + 1, questionCode, ValueOf("noidung"), ValueOf("loai").ToUpperInvariant(), At(answerCodeIndex, row), At(answerIndex, row), valueIndex >= 0 && valueIndex < row.Count ? row[valueIndex].Trim() : string.Empty, AtDecimal(countIndex, row), AtDecimal(totalIndex, row), AtBool(requiredIndex, row), AtBool(multiSelectIndex, row), At(denominatorIndex, row), AtBool(freeTextIndex, row)));
            }
        }
        if (!found) rows.AddRange(ParseParagraphs(xml));
        if (!found && rows.Count == 0) throw new InvalidOperationException("Không nhận diện được câu hỏi. File cần dùng bảng chuẩn hoặc đánh số Câu 1, Câu 2 và phương án a), b), c).");
        if (rows.Count == 0) throw new InvalidOperationException("Mẫu Word không có câu hỏi hợp lệ.");
        return new WordSurveyParseResult(rows);
    }
    private static decimal? AtDecimal(int index, IReadOnlyList<string> row) => index >= 0 && index < row.Count && decimal.TryParse(row[index], NumberStyles.Number, CultureInfo.InvariantCulture, out var value) ? value : null;
    private static bool AtBool(int index, IReadOnlyList<string> row) => index >= 0 && index < row.Count && Normalize(row[index]) is "1" or "true" or "co" or "yes" or "x";
    private static string At(int index, IReadOnlyList<string> row) => index >= 0 && index < row.Count ? row[index].Trim() : string.Empty;

    private static List<string> ReadCells(XElement row) => row.Elements(W + "tc").Select(cell => string.Concat(cell.Descendants(W + "t").Select(x => x.Value))).ToList();
    private static IReadOnlyList<WordSurveyRow> ParseParagraphs(XDocument xml)
    {
        var result = new List<WordSurveyRow>();
        var questionPattern = new Regex(@"^\s*(?:câu|câu hỏi)\s*(\d+)\s*[:.\-]?\s*(.+)$", RegexOptions.IgnoreCase);
        var optionPattern = new Regex(@"^\s*(?:[☐□\-]\s*)?([a-z])\s*[\).:\-]\s*(.+)$", RegexOptions.IgnoreCase);
        string? code = null, content = string.Empty; var order = 0; var multi = false; var freeText = false;
        foreach (var paragraph in xml.Descendants(W + "p"))
        {
            var text = string.Concat(paragraph.Descendants(W + "t").Select(x => x.Value)).Trim(); if (string.IsNullOrWhiteSpace(text)) continue;
            var question = questionPattern.Match(text);
            if (question.Success) { code = $"Q{question.Groups[1].Value}"; content = question.Groups[2].Value.Trim(); order++; multi = Normalize(content).Contains("cothechonmothoacnhieu"); freeText = Normalize(content).Contains("kiennghi") || Normalize(content).Contains("ykien"); continue; }
            var option = optionPattern.Match(text);
            if (code is not null && option.Success) result.Add(new WordSurveyRow(order, code, content, "LUA_CHON", option.Groups[1].Value.ToUpperInvariant(), option.Groups[2].Value.Trim(), string.Empty, null, null, false, multi, "PHIEU_HOP_LE", freeText));
        }
        if (code is not null && !result.Any(x => x.MaCauHoi == code)) result.Add(new WordSurveyRow(order, code, content, freeText ? "VAN_BAN" : "LUA_CHON", string.Empty, string.Empty, string.Empty, null, null, false, multi, "PHIEU_HOP_LE", freeText));
        return result;
    }
    private static string Normalize(string value)
    {
        var decomposed = value.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
        return string.Concat(decomposed.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark && char.IsLetterOrDigit(c)));
    }
}

public sealed record WordSurveyParseResult(IReadOnlyList<WordSurveyRow> Rows);
public sealed record WordSurveyRow(int RowNumber, string MaCauHoi, string NoiDung, string LoaiCauHoi, string MaDapAn, string DapAn, string GiaTriTraLoi, decimal? SoLuong, decimal? TongSoTraLoi, bool BatBuoc, bool ChoPhepNhieuLuaChon, string MauSoTyLe, bool CoYKienTuDo);
