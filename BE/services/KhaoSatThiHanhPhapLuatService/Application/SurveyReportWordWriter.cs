using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using KhaoSatThiHanhPhapLuatService.Infrastructure.Persistence.Entities;

namespace KhaoSatThiHanhPhapLuatService.Application;

public static class SurveyReportWordWriter
{
    public static void Write(string path, BaoCaoKhaoSat report, IReadOnlyList<ChiTietBaoCaoKhaoSat> details, IReadOnlyList<SurveyReportQuestionOrder> questionOrders)
    {
        using var document = WordprocessingDocument.Create(path, WordprocessingDocumentType.Document);
        var main = document.AddMainDocumentPart();
        main.Document = new Document(new Body());
        var body = main.Document.Body!;

        Add(body, report.SoKyHieu ?? "Số: .../BC", JustificationValues.Left, true, 11);
        Add(body, "BÁO CÁO", JustificationValues.Center, true, 16);
        Add(body, report.TenBaoCao, JustificationValues.Center, true, 13);
        Add(body, $"Ngày báo cáo: {report.NgayBaoCao:dd/MM/yyyy}", JustificationValues.Center, false, 11);
        Add(body, "I. KẾT QUẢ ĐIỀU TRA, KHẢO SÁT", JustificationValues.Left, true, 12);

        var detailLookup = details
            .GroupBy(x => x.CauHoiThongKeId)
            .ToDictionary(x => x.Key, x => x.OrderBy(y => y.MaLuaChon).ToList());
        var orderedQuestions = BuildOrderedQuestions(details, questionOrders);

        var groupIndex = 0;
        foreach (var group in orderedQuestions.GroupBy(x => new { x.NhomDoiTuongKhaoSatId, x.TenNhom }).OrderBy(x => x.Min(y => y.ThuTuNhom)))
        {
            groupIndex++;
            Add(body, $"{groupIndex}. {group.Key.TenNhom}", JustificationValues.Left, true, 12);
            foreach (var question in group.OrderBy(x => x.ThuTuCauHoi))
            {
                Add(body, $"Câu {question.SoThuTuHienThi}. {question.NoiDungCauHoi}", JustificationValues.Left, true, 11);
                var questionDetails = detailLookup.TryGetValue(question.CauHoiThongKeId, out var values) ? values : [];
                var choiceRows = questionDetails.Where(x => !string.IsNullOrWhiteSpace(x.MaLuaChon)).OrderBy(x => x.MaLuaChon).ToList();
                if (choiceRows.Count > 0)
                {
                    AddResultTable(body, choiceRows);
                }

                foreach (var item in questionDetails.Where(x => !string.IsNullOrWhiteSpace(x.YKienTuDo)))
                {
                    Add(body, $"Ý kiến: {item.YKienTuDo}", JustificationValues.Left, false, 11);
                }
            }
        }

        AddSection(body, "II. ƯU ĐIỂM", report.UuDiem);
        AddSection(body, "III. HẠN CHẾ, TỒN TẠI", report.HanChe);
        AddSection(body, "IV. KIẾN NGHỊ, ĐỀ XUẤT", report.KienNghi);
        main.Document.Save();
    }

    private static IReadOnlyList<SurveyReportOrderedQuestion> BuildOrderedQuestions(IReadOnlyList<ChiTietBaoCaoKhaoSat> details, IReadOnlyList<SurveyReportQuestionOrder> questionOrders)
    {
        var byQuestionId = questionOrders
            .GroupBy(x => x.CauHoiThongKeId)
            .ToDictionary(x => x.Key, x => x.OrderBy(y => y.ThuTuNhom).ThenBy(y => y.ThuTuCauHoi).First());
        var ordered = new List<SurveyReportOrderedQuestion>();
        foreach (var detailGroup in details.GroupBy(x => new { x.CauHoiThongKeId, x.MaCauHoi, x.NoiDungCauHoi }))
        {
            if (byQuestionId.TryGetValue(detailGroup.Key.CauHoiThongKeId, out var metadata))
            {
                ordered.Add(new SurveyReportOrderedQuestion(metadata.CauHoiThongKeId, metadata.NhomDoiTuongKhaoSatId, metadata.TenNhom, metadata.ThuTuNhom, metadata.ThuTuCauHoi, DisplayQuestionNumber(metadata.MaCauHoi, metadata.ThuTuCauHoi), metadata.NoiDungCauHoi));
                continue;
            }

            var fallbackOrder = ExtractQuestionNumber(detailGroup.Key.MaCauHoi) ?? int.MaxValue;
            ordered.Add(new SurveyReportOrderedQuestion(detailGroup.Key.CauHoiThongKeId, Guid.Empty, "Mẫu phiếu khảo sát", int.MaxValue, fallbackOrder, DisplayQuestionNumber(detailGroup.Key.MaCauHoi, fallbackOrder), detailGroup.Key.NoiDungCauHoi));
        }

        return ordered
            .GroupBy(x => x.CauHoiThongKeId)
            .Select(x => x.OrderBy(y => y.ThuTuNhom).ThenBy(y => y.ThuTuCauHoi).First())
            .OrderBy(x => x.ThuTuNhom)
            .ThenBy(x => x.ThuTuCauHoi)
            .ToList();
    }

    private static string DisplayQuestionNumber(string code, int fallback)
    {
        var number = ExtractQuestionNumber(code);
        return number.HasValue ? number.Value.ToString() : fallback == int.MaxValue ? code : fallback.ToString();
    }

    private static int? ExtractQuestionNumber(string code)
    {
        var digits = new string(code.Reverse().TakeWhile(char.IsDigit).Reverse().ToArray());
        return int.TryParse(digits, out var value) ? value : null;
    }

    private static void AddResultTable(Body body, IReadOnlyList<ChiTietBaoCaoKhaoSat> rows)
    {
        var table = new Table();
        table.AppendChild(new TableProperties(
            new TableWidth { Width = "5000", Type = TableWidthUnitValues.Pct },
            new TableBorders(
                new TopBorder { Val = BorderValues.Single, Color = "D9D9D9", Size = 4 },
                new BottomBorder { Val = BorderValues.Single, Color = "D9D9D9", Size = 4 },
                new LeftBorder { Val = BorderValues.Single, Color = "D9D9D9", Size = 4 },
                new RightBorder { Val = BorderValues.Single, Color = "D9D9D9", Size = 4 },
                new InsideHorizontalBorder { Val = BorderValues.Single, Color = "D9D9D9", Size = 4 },
                new InsideVerticalBorder { Val = BorderValues.Single, Color = "D9D9D9", Size = 4 })));

        table.Append(CreateRow(true, "Phương án trả lời", "Số lượng", "Tỷ lệ"));
        foreach (var row in rows)
        {
            table.Append(CreateRow(false, row.NoiDungLuaChon ?? row.MaLuaChon ?? string.Empty, row.SoLuong.ToString("0.##"), $"{row.TyLe:0.##}%"));
        }

        body.Append(table);
        body.Append(new Paragraph(new ParagraphProperties(new SpacingBetweenLines { After = "120" })));
    }

    private static TableRow CreateRow(bool header, string option, string count, string rate)
    {
        var row = new TableRow();
        row.Append(CreateCell(option, header, "7000", JustificationValues.Left));
        row.Append(CreateCell(count, header, "1400", JustificationValues.Center));
        row.Append(CreateCell(rate, header, "1600", JustificationValues.Center));
        return row;
    }

    private static TableCell CreateCell(string text, bool header, string width, JustificationValues alignment)
    {
        var shading = header ? new Shading { Fill = "1F4E79", Val = ShadingPatternValues.Clear } : null;
        var cellProperties = new TableCellProperties(new TableCellWidth { Width = width, Type = TableWidthUnitValues.Pct }, new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center });
        if (shading is not null) cellProperties.Append(shading);

        var runProperties = new RunProperties();
        if (header)
        {
            runProperties.Append(new Bold());
            runProperties.Append(new Color { Val = "FFFFFF" });
        }

        return new TableCell(cellProperties, new Paragraph(new ParagraphProperties(new Justification { Val = alignment }, new SpacingBetweenLines { After = "40" }), new Run(runProperties, new Text(text) { Space = SpaceProcessingModeValues.Preserve })));
    }

    private static void AddSection(Body body, string title, string? content) { Add(body, title, JustificationValues.Left, true, 12); Add(body, string.IsNullOrWhiteSpace(content) ? "" : content, JustificationValues.Left); }

    private static void Add(Body body, string text, JustificationValues alignment, bool bold = false, int? size = null)
    {
        var runProperties = new RunProperties();
        if (bold) runProperties.Append(new Bold());
        if (size.HasValue) runProperties.Append(new FontSize { Val = (size.Value * 2).ToString() });
        body.Append(new Paragraph(new ParagraphProperties(new Justification { Val = alignment }, new SpacingBetweenLines { After = "140" }), new Run(runProperties, new Text(text) { Space = SpaceProcessingModeValues.Preserve })));
    }
}

public sealed record SurveyReportQuestionOrder(Guid CauHoiThongKeId, Guid NhomDoiTuongKhaoSatId, string TenNhom, int ThuTuNhom, string MaCauHoi, string NoiDungCauHoi, int ThuTuCauHoi);

internal sealed record SurveyReportOrderedQuestion(Guid CauHoiThongKeId, Guid NhomDoiTuongKhaoSatId, string TenNhom, int ThuTuNhom, int ThuTuCauHoi, string SoThuTuHienThi, string NoiDungCauHoi);
