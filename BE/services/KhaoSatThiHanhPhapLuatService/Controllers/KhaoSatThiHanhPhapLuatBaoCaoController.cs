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
        var submissions = await (from submission in db.PhieuNopKhaoSats.AsNoTracking()
                                 join subject in db.DoiTuongKhaoSats.AsNoTracking() on submission.DoiTuongKhaoSatId equals subject.Id
                                 where !submission.IsDeleted && !subject.IsDeleted && subject.CuocKhaoSatId == cuocKhaoSatId && submission.TrangThaiId == trangThaiPhieuHopLeId
                                 select new { submission, subject }).ToListAsync(ct);
        var latest = submissions.GroupBy(x => x.subject.Id).Select(x => x.OrderByDescending(y => y.submission.CreatedAt).First()).ToList();
        var submissionIds = latest.Select(x => x.submission.Id).ToList();
        var questions = await db.CauHoiThongKes.AsNoTracking().Where(x => x.CuocKhaoSatId == cuocKhaoSatId && !x.IsDeleted).OrderBy(x => x.MaCauHoiThongKe).ToListAsync(ct);
        var answers = await db.CauTraLoiKhaoSats.AsNoTracking().Where(x => submissionIds.Contains(x.PhieuNopKhaoSatId) && !x.IsDeleted).ToListAsync(ct);
        var optionRows = await (from option in db.LuaChonTraLois.AsNoTracking()
                                join question in db.CauHoiMauPhieus.AsNoTracking() on option.CauHoiMauPhieuId equals question.Id
                                where !option.IsDeleted && !question.IsDeleted && questions.Select(q => q.Id).Contains(question.CauHoiThongKeId)
                                select new { option, question.CauHoiThongKeId }).ToListAsync(ct);
        var result = questions.Select(question =>
        {
            var questionAnswers = answers.Where(x => x.CauHoiThongKeId == question.Id).ToList();
            var denominator = questionAnswers.GroupBy(x => x.PhieuNopKhaoSatId).Sum(x => x.Select(y => y.TongSoTraLoi).FirstOrDefault() ?? 0);
            var options = optionRows.Where(x => x.CauHoiThongKeId == question.Id).GroupBy(x => x.option.MaLuaChon).Select(group =>
            {
                var optionIds = group.Select(x => x.option.Id).ToList(); var count = questionAnswers.Where(x => x.LuaChonTraLoiId.HasValue && optionIds.Contains(x.LuaChonTraLoiId.Value)).Sum(x => x.SoLuong ?? 0);
                return new { MaDapAn = group.Key, NoiDung = group.First().option.NoiDung, SoLuong = count, TyLe = denominator == 0 ? 0 : Math.Round(count * 100m / denominator, 2) };
            });
            return new { question.MaCauHoiThongKe, question.NoiDung, question.LoaiCauHoi, SoPhieuHopLe = denominator, DapAn = options, YKienTuDo = questionAnswers.Where(x => !string.IsNullOrWhiteSpace(x.GiaTriText)).Select(x => x.GiaTriText) };
        });
        return Ok(new { TongSoPhieuHopLe = latest.Count, CauHoi = result });
    }
}
