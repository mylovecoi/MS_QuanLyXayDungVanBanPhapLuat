using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using KhaoSatThiHanhPhapLuatService.Infrastructure.Persistence.Entities;

namespace KhaoSatThiHanhPhapLuatService.Application;

public static class SurveyReportWordWriter
{
    public static void Write(string path, BaoCaoKhaoSat report, IReadOnlyList<ChiTietBaoCaoKhaoSat> details)
    {
        using var document = WordprocessingDocument.Create(path, WordprocessingDocumentType.Document);
        var main = document.AddMainDocumentPart();
        main.Document = new Document(new Body());
        var body = main.Document.Body!;
        Add(body, report.SoKyHieu ?? "Số: .../BC", JustificationValues.Left, true);
        Add(body, "BÁO CÁO", JustificationValues.Center, true, 15);
        Add(body, report.TenBaoCao, JustificationValues.Center, true, 13);
        Add(body, $"Ngày báo cáo: {report.NgayBaoCao:dd/MM/yyyy}", JustificationValues.Center);
        Add(body, "I. KẾT QUẢ ĐIỀU TRA, KHẢO SÁT", JustificationValues.Left, true);
        foreach (var question in details.GroupBy(x => new { x.MaCauHoi, x.NoiDungCauHoi }).OrderBy(x => x.Key.MaCauHoi))
        {
            Add(body, $"Nội dung {question.Key.MaCauHoi}: {question.Key.NoiDungCauHoi}", JustificationValues.Left, true);
            foreach (var item in question.Where(x => !string.IsNullOrWhiteSpace(x.MaLuaChon))) Add(body, $"- {item.NoiDungLuaChon}: {item.SoLuong:0.##} ({item.TyLe:0.##}%)", JustificationValues.Left);
            foreach (var item in question.Where(x => !string.IsNullOrWhiteSpace(x.YKienTuDo))) Add(body, $"- Ý kiến: {item.YKienTuDo}", JustificationValues.Left);
        }
        AddSection(body, "II. ƯU ĐIỂM", report.UuDiem);
        AddSection(body, "III. HẠN CHẾ, TỒN TẠI", report.HanChe);
        AddSection(body, "IV. KIẾN NGHỊ, ĐỀ XUẤT", report.KienNghi);
        main.Document.Save();
    }

    private static void AddSection(Body body, string title, string? content) { Add(body, title, JustificationValues.Left, true); Add(body, string.IsNullOrWhiteSpace(content) ? "" : content, JustificationValues.Left); }
    private static void Add(Body body, string text, JustificationValues alignment, bool bold = false, int? size = null)
    {
        var runProperties = new RunProperties();
        if (bold) runProperties.Append(new Bold());
        if (size.HasValue) runProperties.Append(new FontSize { Val = (size.Value * 2).ToString() });
        body.Append(new Paragraph(new ParagraphProperties(new Justification { Val = alignment }, new SpacingBetweenLines { After = "120" }), new Run(runProperties, new Text(text) { Space = SpaceProcessingModeValues.Preserve })));
    }
}
