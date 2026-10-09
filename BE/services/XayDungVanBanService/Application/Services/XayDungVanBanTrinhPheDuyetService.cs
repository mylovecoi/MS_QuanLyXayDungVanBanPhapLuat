using BuildingBlocks.Abstractions;
using Microsoft.EntityFrameworkCore;
using XayDungVanBanService.Application.Abstractions;
using XayDungVanBanService.Application.DTOs;
using XayDungVanBanService.Infrastructure.Persistence;
using XayDungVanBanService.Infrastructure.Persistence.Entities;

namespace XayDungVanBanService.Application.Services;

public sealed class XayDungVanBanTrinhPheDuyetService(
    XayDungVanBanDbContext db,
    ICurrentUserContext user,
    IWebHostEnvironment env) : IXayDungVanBanTrinhPheDuyetService
{
    public async Task<IReadOnlyList<HoSoTrinhPheDuyetListItemDto>> GetListAsync(CancellationToken ct = default)
    {
        var sources = await (
            from b in db.BoHoSoNghiepVus.AsNoTracking()
            join d in db.HoSoXayDungVanBanThamDinhs.AsNoTracking() on b.Id equals d.BoHoSoNghiepVuId
            join h in db.HoSoXayDungVanBans.AsNoTracking() on b.HoSoXayDungVanBanId equals h.Id
            where b.LoaiBoHoSo == LoaiBoHoSo.ThamDinh
                && b.TrangThai == TrangThaiBoHoSo.DaHoanThanh
                && d.NgayGuiKetQua != null
                && !b.IsDeleted
                && !h.IsDeleted
            select new { h.Id, h.MaHoSo, h.TenHoSo, h.TenDuThaoVanBan, h.NamXayDung, d.NgayThamDinh, b.CreatedAt })
            .ToListAsync(ct);

        var ids = sources.Select(x => x.Id).ToList();
        var approvals = await (
            from b in db.BoHoSoNghiepVus.AsNoTracking()
            join d in db.HoSoXayDungVanBanTrinhPheDuyets.AsNoTracking() on b.Id equals d.BoHoSoNghiepVuId
            where ids.Contains(b.HoSoXayDungVanBanId)
                && b.LoaiBoHoSo == LoaiBoHoSo.TrinhPheDuyet
                && !b.IsDeleted
            select new { b.HoSoXayDungVanBanId, b.Id, b.TrangThai, d.NgayTrinh, b.CreatedAt })
            .ToListAsync(ct);

        var latest = approvals
            .GroupBy(x => x.HoSoXayDungVanBanId)
            .ToDictionary(x => x.Key, x => x.OrderByDescending(y => y.CreatedAt).First());

        return sources
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => latest.TryGetValue(x.Id, out var a)
                ? new HoSoTrinhPheDuyetListItemDto(x.Id, a.Id, x.MaHoSo, x.TenHoSo, x.TenDuThaoVanBan, x.NamXayDung, a.TrangThai.ToString(), x.NgayThamDinh, a.NgayTrinh)
                : new HoSoTrinhPheDuyetListItemDto(x.Id, null, x.MaHoSo, x.TenHoSo, x.TenDuThaoVanBan, x.NamXayDung, "ChoLapHoSo", x.NgayThamDinh, null))
            .ToList();
    }

    public async Task<XayDungVanBanTrinhPheDuyetDto> CreateAsync(TaoHoSoTrinhPheDuyetRequest r, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(r.CapTrinh) || string.IsNullOrWhiteSpace(r.MucDichTrinh))
            throw new InvalidOperationException("Thiếu cấp hoặc mục đích trình.");

        var source = await db.BoHoSoNghiepVus
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync(x => x.HoSoXayDungVanBanId == r.HoSoId
                && x.LoaiBoHoSo == LoaiBoHoSo.ThamDinh
                && x.TrangThai == TrangThaiBoHoSo.DaHoanThanh
                && !x.IsDeleted, ct)
            ?? throw new InvalidOperationException("Chưa có kết quả thẩm định đã gửi.");

        if (await Load(r.HoSoId, true, ct) != null)
            throw new InvalidOperationException("Đã có hồ sơ trình lấy ý kiến UBND.");

        var hoSo = await db.HoSoXayDungVanBans.AsNoTracking().FirstAsync(x => x.Id == r.HoSoId, ct);
        var a = Actor(hoSo.DonViChuTriSoanThaoId);
        var lanXuLy = (await db.BoHoSoNghiepVus
            .Where(x => x.HoSoXayDungVanBanId == r.HoSoId && x.BuocQuyTrinhId == hoSo.BuocHienTaiId && !x.IsDeleted)
            .Select(x => (int?)x.LanXuLy)
            .MaxAsync(ct) ?? 0) + 1;

        await using var tx = await db.Database.BeginTransactionAsync(ct);
        var b = new BoHoSoNghiepVu
        {
            HoSoXayDungVanBanId = r.HoSoId,
            BuocQuyTrinhId = hoSo.BuocHienTaiId,
            LoaiBoHoSo = LoaiBoHoSo.TrinhPheDuyet,
            TrangThai = TrangThaiBoHoSo.Nhap,
            LanXuLy = lanXuLy,
            BoHoSoNguonId = source.Id,
            NguoiLapId = a.U,
            DonViLapId = a.D,
            CreatedBy = a.U.ToString()
        };
        var d = new HoSoXayDungVanBanTrinhPheDuyet
        {
            BoHoSoNghiepVuId = b.Id,
            CapTrinh = r.CapTrinh.Trim(),
            MucDichTrinh = r.MucDichTrinh.Trim()
        };

        db.BoHoSoNghiepVus.Add(b);
        db.HoSoXayDungVanBanTrinhPheDuyets.Add(d);

        var links = await db.BoHoSoNghiepVuTaiLieus
            .Where(x => x.BoHoSoNghiepVuId == source.Id && !x.IsDeleted)
            .ToListAsync(ct);

        foreach (var x in links)
            db.BoHoSoNghiepVuTaiLieus.Add(new()
            {
                BoHoSoNghiepVuId = b.Id,
                HoSoXayDungVanBanFileId = x.HoSoXayDungVanBanFileId,
                LoaiTaiLieuId = x.LoaiTaiLieuId,
                HinhThucThem = "KeThua",
                BoHoSoTaiLieuNguonId = x.Id,
                CreatedBy = a.U.ToString()
            });

        Log(r.HoSoId, b.Id, "TAO_HO_SO_TRINH_Y_KIEN_UBND", "Tạo hồ sơ trình lấy ý kiến UBND", a);
        await db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
        return ToDto(r.HoSoId, b, d);
    }

    public async Task<XayDungVanBanTrinhPheDuyetDto?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var x = await Load(id, false, ct);
        return x == null ? null : ToDto(id, x.B, x.D);
    }

    public async Task<XayDungVanBanTrinhPheDuyetDto?> UpdateAsync(Guid id, CapNhatHoSoTrinhPheDuyetRequest r, CancellationToken ct = default)
    {
        var x = await Load(id, true, ct);
        if (x == null) return null;
        var a = Actor(x.B.DonViLapId);
        Nhap(x.B);
        if (string.IsNullOrWhiteSpace(r.CapTrinh) || string.IsNullOrWhiteSpace(r.MucDichTrinh))
            throw new InvalidOperationException("Thiếu cấp hoặc mục đích trình.");

        x.D.CapTrinh = r.CapTrinh.Trim();
        x.D.MucDichTrinh = r.MucDichTrinh.Trim();
        x.D.SoToTrinh = r.SoToTrinh;
        x.D.NgayToTrinh = r.NgayToTrinh;
        x.D.NoiDungTrinh = r.NoiDungTrinh;
        x.D.DonViDongGuiId = r.DonViDongGuiId;
        Log(id, x.B.Id, "CAP_NHAT_HO_SO_TRINH_Y_KIEN_UBND", "Cập nhật hồ sơ trình lấy ý kiến UBND", a);
        await db.SaveChangesAsync(ct);
        return ToDto(id, x.B, x.D);
    }

    public async Task<DieuKienGuiPheDuyetDto?> KiemTraAsync(Guid id, CancellationToken ct = default)
    {
        var x = await Load(id, false, ct);
        if (x == null) return null;
        var c = await Conditions(x, ct);
        return new(c.Count == 0, c);
    }

    public async Task<XayDungVanBanTrinhPheDuyetDto?> GuiAsync(Guid id, GuiPheDuyetRequest r, CancellationToken ct = default)
    {
        var x = await Load(id, true, ct);
        if (x == null) return null;
        var a = Actor(x.B.DonViLapId);
        Nhap(x.B);
        var c = await Conditions(x, ct);
        if (c.Count > 0) throw new InvalidOperationException(string.Join(" ", c));
        if (r.BuocQuyTrinhTiepTheoId == Guid.Empty || r.TrangThaiHoSoTiepTheoId == Guid.Empty)
            throw new InvalidOperationException("Thiếu bước hoặc trạng thái tiếp theo.");

        await using var tx = await db.Database.BeginTransactionAsync(ct);
        x.B.TrangThai = TrangThaiBoHoSo.DaGui;
        x.B.NgayGui = DateTime.UtcNow;
        x.D.NgayTrinh = r.NgayTrinh ?? DateTime.UtcNow;
        var h = await db.HoSoXayDungVanBans.FirstAsync(z => z.Id == id, ct);
        h.BuocHienTaiId = r.BuocQuyTrinhTiepTheoId;
        h.TrangThaiHoSoId = r.TrangThaiHoSoTiepTheoId;
        if (r.HanXuLy.HasValue)
            h.ThoiGianDuKienHoanThanh = r.HanXuLy.Value;
        if (r.ThoiGianCanhBao.HasValue)
        {
            db.HoSoXayDungVanBanNhacTienDos.Add(new()
            {
                HoSoXayDungVanBanId = id,
                LoaiNhacNho = "CANH_BAO_SAP_HAN",
                TrangThaiXuLy = "DA_GUI",
                NoiDungNhacNho = $"Cảnh báo sắp đến hạn xử lý bước tiếp theo. Hạn xử lý: {r.HanXuLy:dd/MM/yyyy}.",
                NguoiGuiId = a.U,
                DonViGuiId = a.D == Guid.Empty ? null : a.D,
                NgayGui = r.ThoiGianCanhBao.Value,
                CreatedBy = a.U.ToString()
            });
        }
        Log(id, x.B.Id, "GUI_TRINH_Y_KIEN_UBND", "Gửi hồ sơ trình lấy ý kiến UBND", a);
        await db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
        return ToDto(id, x.B, x.D);
    }

    public async Task<bool> HuyAsync(Guid id, CancellationToken ct = default)
    {
        var x = await Load(id, true, ct);
        if (x == null) return false;
        var a = Actor(x.B.DonViLapId);
        Nhap(x.B);
        x.B.IsDeleted = true;
        Log(id, x.B.Id, "HUY_TRINH_Y_KIEN_UBND", "Hủy hồ sơ trình lấy ý kiến UBND", a);
        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<IReadOnlyList<XayDungVanBanTaiLieuDto>?> GetTaiLieuAsync(Guid id, CancellationToken ct = default)
    {
        var x = await Load(id, false, ct);
        if (x == null) return null;
        return await (
            from l in db.BoHoSoNghiepVuTaiLieus
            join f in db.HoSoXayDungVanBanFiles on l.HoSoXayDungVanBanFileId equals f.Id
            where l.BoHoSoNghiepVuId == x.B.Id && !l.IsDeleted && !f.IsDeleted
            select FD(f))
            .ToListAsync(ct);
    }

    public async Task<XayDungVanBanTaiLieuDto?> UploadTaiLieuAsync(Guid id, TaiTaiLieuTrinhPheDuyetRequest r, CancellationToken ct = default)
    {
        var x = await Load(id, true, ct);
        if (x == null) return null;
        var a = Actor(x.B.DonViLapId);
        Nhap(x.B);
        if (r.LoaiTaiLieuId == Guid.Empty || r.NoiDung.Length == 0)
            throw new InvalidOperationException("Thiếu file hoặc loại tài liệu.");

        var name = Path.GetFileName(r.TenFile);
        var rel = Path.Combine("uploads", "xay-dung-van-ban", id.ToString("N"));
        var dir = Path.Combine(env.ContentRootPath, rel);
        Directory.CreateDirectory(dir);
        var store = $"{Guid.NewGuid():N}_{name}";
        var path = Path.Combine(dir, store);
        await using (var o = File.Create(path))
            await r.NoiDung.CopyToAsync(o, ct);

        var f = new HoSoXayDungVanBanFile
        {
            HoSoXayDungVanBanId = id,
            LoaiTaiLieuId = r.LoaiTaiLieuId,
            TenTaiLieu = r.TenTaiLieu,
            TenFile = name,
            DuongDanFile = Path.Combine(rel, store).Replace('\\', '/'),
            MimeType = r.MimeType,
            DungLuong = r.NoiDung.Length,
            NguoiTaiLenId = a.U,
            CreatedBy = a.U.ToString()
        };

        db.HoSoXayDungVanBanFiles.Add(f);
        db.BoHoSoNghiepVuTaiLieus.Add(new()
        {
            BoHoSoNghiepVuId = x.B.Id,
            HoSoXayDungVanBanFileId = f.Id,
            LoaiTaiLieuId = f.LoaiTaiLieuId,
            HinhThucThem = "TaoMoi",
            CreatedBy = a.U.ToString()
        });
        Log(id, x.B.Id, "TAI_TAI_LIEU_TRINH_Y_KIEN_UBND", $"Tải tài liệu {f.TenTaiLieu}", a);
        await db.SaveChangesAsync(ct);
        return FD(f);
    }

    public async Task<bool> DeleteTaiLieuAsync(Guid id, Guid linkId, CancellationToken ct = default)
    {
        var x = await Load(id, true, ct);
        if (x == null) return false;
        var a = Actor(x.B.DonViLapId);
        Nhap(x.B);
        var l = await db.BoHoSoNghiepVuTaiLieus.FirstOrDefaultAsync(z => z.Id == linkId && z.BoHoSoNghiepVuId == x.B.Id && !z.IsDeleted, ct);
        if (l == null) return false;
        if (l.HinhThucThem == "KeThua") throw new InvalidOperationException("Không thể gỡ tài liệu kế thừa.");
        l.IsDeleted = true;
        Log(id, x.B.Id, "GO_TAI_LIEU_TRINH_Y_KIEN_UBND", "Gỡ tài liệu bổ sung", a);
        await db.SaveChangesAsync(ct);
        return true;
    }

    private async Task<List<string>> Conditions(Data x, CancellationToken ct)
    {
        var c = new List<string>();
        if (string.IsNullOrWhiteSpace(x.D.NoiDungTrinh)) c.Add("Chưa có nội dung trình.");
        if (!await db.BoHoSoNghiepVuTaiLieus.AnyAsync(z => z.BoHoSoNghiepVuId == x.B.Id && !z.IsDeleted, ct))
            c.Add("Chưa có tờ trình hoặc tài liệu đính kèm.");
        return c;
    }

    private async Task<Data?> Load(Guid id, bool tracked, CancellationToken ct)
    {
        var b = tracked ? db.BoHoSoNghiepVus : db.BoHoSoNghiepVus.AsNoTracking();
        var d = tracked ? db.HoSoXayDungVanBanTrinhPheDuyets : db.HoSoXayDungVanBanTrinhPheDuyets.AsNoTracking();
        return await (
            from x in b
            join y in d on x.Id equals y.BoHoSoNghiepVuId
            where x.HoSoXayDungVanBanId == id
                && x.LoaiBoHoSo == LoaiBoHoSo.TrinhPheDuyet
                && !x.IsDeleted
            select new Data(x, y))
            .FirstOrDefaultAsync(ct);
    }

    private static void Nhap(BoHoSoNghiepVu b)
    {
        if (b.TrangThai != TrangThaiBoHoSo.Nhap)
            throw new InvalidOperationException("Hồ sơ không ở trạng thái nhập.");
    }

    private A Actor(Guid? fallbackDonViId = null)
    {
        if (!user.IsAuthenticated || user.UserId is not Guid u || u == Guid.Empty)
            throw new UnauthorizedAccessException();

        var d = user.DonViId is { } userDonViId && userDonViId != Guid.Empty
            ? userDonViId
            : fallbackDonViId.GetValueOrDefault();
        if (d == Guid.Empty)
            throw new UnauthorizedAccessException("Người dùng chưa có thông tin đơn vị xử lý.");

        return new(u, d);
    }

    private void Log(Guid h, Guid b, string ac, string n, A a) =>
        db.HoSoXayDungVanBanLichSuXuLys.Add(new()
        {
            HoSoXayDungVanBanId = h,
            BoHoSoNghiepVuId = b,
            HanhDong = ac,
            NoiDung = n,
            NguoiXuLyId = a.U,
            DonViXuLyId = a.D,
            CreatedBy = a.U.ToString()
        });

    private static XayDungVanBanTrinhPheDuyetDto ToDto(Guid h, BoHoSoNghiepVu b, HoSoXayDungVanBanTrinhPheDuyet d) =>
        new(h, b.Id, b.TrangThai.ToString(), d.CapTrinh, d.MucDichTrinh, d.SoToTrinh, d.NgayToTrinh, d.NgayTrinh, d.NoiDungTrinh, d.DonViDongGuiId);

    private static XayDungVanBanTaiLieuDto FD(HoSoXayDungVanBanFile f) =>
        new(f.Id, f.LoaiTaiLieuId, f.TenTaiLieu, f.PhienBan, f.TenFile, f.DuongDanFile, f.MimeType, f.DungLuong, f.IsCurrent, f.NgayTaiLen);

    private sealed record Data(BoHoSoNghiepVu B, HoSoXayDungVanBanTrinhPheDuyet D);
    private sealed record A(Guid U, Guid D);
}
