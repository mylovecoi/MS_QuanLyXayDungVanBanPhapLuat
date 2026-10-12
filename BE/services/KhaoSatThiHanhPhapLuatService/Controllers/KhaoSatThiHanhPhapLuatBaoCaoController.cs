using KhaoSatThiHanhPhapLuatService.Application;
using KhaoSatThiHanhPhapLuatService.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KhaoSatThiHanhPhapLuatService.Controllers;

[Authorize]
[ApiController]
[Route("api/khao-sat-thi-hanh-phap-luat/bao-cao")]
public sealed class KhaoSatThiHanhPhapLuatBaoCaoController(KhaoSatThiHanhPhapLuatDbContext db) : ControllerBase
{
    [HttpGet("thong-ke")]
    public async Task<ActionResult> Statistics(Guid cuocKhaoSatId, Guid trangThaiPhieuHopLeId, CancellationToken ct)
    {
        var submissions = await db.PhieuNopKhaoSats.AsNoTracking().Where(x => x.CuocKhaoSatId == cuocKhaoSatId && x.TrangThaiId == trangThaiPhieuHopLeId && !x.IsDeleted).ToListAsync(ct);
        var latest = submissions.GroupBy(x => new { x.NhomDoiTuongKhaoSatId, x.MauPhieuKhaoSatId }).Select(x => x.OrderByDescending(y => y.CreatedAt).First()).ToList();
        var submissionIds = latest.Select(x => x.Id).ToList();
        var questionOrders = await BuildQuestionOrdersAsync(cuocKhaoSatId, ct);
        var orderLookup = questionOrders.ToDictionary(x => x.CauHoiThongKeId);
        var questions = await db.CauHoiThongKes.AsNoTracking().Where(x => x.CuocKhaoSatId == cuocKhaoSatId && !x.IsDeleted).ToListAsync(ct);
        questions = questions
            .OrderBy(x => orderLookup.TryGetValue(x.Id, out var order) ? order.ThuTuNhom : int.MaxValue)
            .ThenBy(x => orderLookup.TryGetValue(x.Id, out var order) ? order.ThuTuCauHoi : ExtractQuestionNumber(x.MaCauHoiThongKe) ?? int.MaxValue)
            .ToList();
        var answers = await db.CauTraLoiKhaoSats.AsNoTracking().Where(x => submissionIds.Contains(x.PhieuNopKhaoSatId) && !x.IsDeleted).ToListAsync(ct);
        var optionRows = await (from option in db.LuaChonTraLois.AsNoTracking()
                                join question in db.CauHoiMauPhieus.AsNoTracking() on option.CauHoiMauPhieuId equals question.Id
                                where !option.IsDeleted && !question.IsDeleted && questions.Select(q => q.Id).Contains(question.CauHoiThongKeId)
                                select new { option, question.CauHoiThongKeId, question.ThuTu }).ToListAsync(ct);
        var result = questions.Select(question =>
        {
            orderLookup.TryGetValue(question.Id, out var questionOrder);
            var questionAnswers = answers.Where(x => x.CauHoiThongKeId == question.Id).ToList();
            var theoPhieu = questionAnswers.GroupBy(x => x.PhieuNopKhaoSatId).Select(group => new
            {
                TongSoPhieuHopLe = group.Where(x => x.TongSoTraLoi.HasValue).Select(x => x.TongSoTraLoi!.Value).DefaultIfEmpty().Max(),
                TongLuotChon = group.Where(x => x.LuaChonTraLoiId.HasValue).Sum(x => x.SoLuong ?? 0)
            }).ToList();
            var denominator = string.Equals(question.MauSoTyLe, "TONG_LUOT_CHON", StringComparison.OrdinalIgnoreCase)
                ? theoPhieu.Sum(x => x.TongLuotChon)
                : theoPhieu.Sum(x => x.TongSoPhieuHopLe);
            var options = optionRows.Where(x => x.CauHoiThongKeId == question.Id).GroupBy(x => x.option.MaLuaChon).Select(group =>
            {
                var optionIds = group.Select(x => x.option.Id).ToList(); var count = questionAnswers.Where(x => x.LuaChonTraLoiId.HasValue && optionIds.Contains(x.LuaChonTraLoiId.Value)).Sum(x => x.SoLuong ?? 0);
                return new { MaDapAn = group.Key, NoiDung = group.First().option.NoiDung, SoLuong = count, TyLe = denominator == 0 ? 0 : Math.Round(count * 100m / denominator, 2) };
            }).OrderBy(x => x.MaDapAn);
            return new { NhomDoiTuongKhaoSatId = questionOrder?.NhomDoiTuongKhaoSatId, TenNhomDoiTuong = questionOrder?.TenNhom, ThuTuNhom = questionOrder?.ThuTuNhom ?? int.MaxValue, ThuTuCauHoi = questionOrder?.ThuTuCauHoi ?? ExtractQuestionNumber(question.MaCauHoiThongKe) ?? int.MaxValue, question.MaCauHoiThongKe, MaCauHoi = questionOrder?.MaCauHoi, question.NoiDung, question.LoaiCauHoi, question.ChoPhepNhieuLuaChon, question.MauSoTyLe, SoPhieuHopLe = theoPhieu.Sum(x => x.TongSoPhieuHopLe), MauSoTinhTyLe = denominator, DapAn = options, YKienTuDo = questionAnswers.Where(x => !string.IsNullOrWhiteSpace(x.GiaTriText)).Select(x => x.GiaTriText) };
        });
        return Ok(new { SoFileKetQuaHopLe = latest.Count, Nhom = result.GroupBy(x => new { x.NhomDoiTuongKhaoSatId, x.TenNhomDoiTuong, x.ThuTuNhom }).OrderBy(x => x.Key.ThuTuNhom).Select(x => new { x.Key.NhomDoiTuongKhaoSatId, x.Key.TenNhomDoiTuong, CauHoi = x.OrderBy(y => y.ThuTuCauHoi) }), CauHoi = result });
    }

    private async Task<List<SurveyReportQuestionOrder>> BuildQuestionOrdersAsync(Guid cuocKhaoSatId, CancellationToken ct)
    {
        var groups = await db.NhomDoiTuongKhaoSats.AsNoTracking().Where(x => x.CuocKhaoSatId == cuocKhaoSatId && !x.IsDeleted).ToListAsync(ct);
        var templates = await db.MauPhieuKhaoSats.AsNoTracking().Where(x => x.CuocKhaoSatId == cuocKhaoSatId && !x.IsDeleted).ToListAsync(ct);
        var selectedTemplateIds = templates
            .GroupBy(x => x.NhomDoiTuongKhaoSatId)
            .Select(x => x.OrderByDescending(y => y.TrangThaiMauPhieu == "DANG_SU_DUNG").ThenByDescending(y => y.PhienBan).ThenByDescending(y => y.CreatedAt).First().Id)
            .ToList();
        var questions = await db.CauHoiMauPhieus.AsNoTracking().Where(x => selectedTemplateIds.Contains(x.MauPhieuKhaoSatId) && !x.IsDeleted).ToListAsync(ct);
        return (from template in templates.Where(x => selectedTemplateIds.Contains(x.Id))
                join groupInfo in groups on template.NhomDoiTuongKhaoSatId equals groupInfo.Id
                join question in questions on template.Id equals question.MauPhieuKhaoSatId
                orderby groupInfo.ThuTu, question.ThuTu
                select new SurveyReportQuestionOrder(question.CauHoiThongKeId, groupInfo.Id, groupInfo.TenNhom, groupInfo.ThuTu, question.MaCauHoi, question.NoiDung, question.ThuTu)).ToList();
    }

    private static int? ExtractQuestionNumber(string code)
    {
        var digits = new string(code.Reverse().TakeWhile(char.IsDigit).Reverse().ToArray());
        return int.TryParse(digits, out var value) ? value : null;
    }
}
