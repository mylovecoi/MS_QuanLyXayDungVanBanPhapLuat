using BuildingBlocks.Abstractions;
using Microsoft.EntityFrameworkCore;
using XayDungVanBanService.Application.Abstractions;
using XayDungVanBanService.Application.DTOs;
using XayDungVanBanService.Infrastructure.Persistence;
using XayDungVanBanService.Infrastructure.Persistence.Entities;

namespace XayDungVanBanService.Application.Services;

public sealed class XayDungVanBanBanHanhService(
    XayDungVanBanDbContext db,
    ICurrentUserContext user,
    IWebHostEnvironment environment) : IXayDungVanBanBanHanhService
{
    public async Task<IReadOnlyList<HoSoBanHanhListItemDto>> GetListAsync(CancellationToken cancellationToken = default) =>
        await (from b in db.BoHoSoNghiepVus.AsNoTracking()
               join d in db.HoSoXayDungVanBanKetQuaBanHanhs.AsNoTracking() on b.Id equals d.BoHoSoNghiepVuId
               join h in db.HoSoXayDungVanBans.AsNoTracking() on b.HoSoXayDungVanBanId equals h.Id
               where b.LoaiBoHoSo == LoaiBoHoSo.BanHanh && !b.IsDeleted && !h.IsDeleted
               orderby b.CreatedAt descending
               select new HoSoBanHanhListItemDto(
                   h.Id,
                   b.Id,
                   h.MaHoSo,
                   h.TenHoSo,
                   h.TenDuThaoVanBan,
                   h.NamXayDung,
                   b.TrangThai.ToString(),
                   b.NgayTao,
                   h.ThoiGianDuKienHoanThanh,
                   b.NgayHoanThanh,
                   d.KetQua,
                   d.SoVanBan,
                   d.NgayBanHanh))
            .ToListAsync(cancellationToken);

    public async Task<XayDungVanBanBanHanhDto?> GetAsync(Guid hoSoId, CancellationToken cancellationToken = default)
    {
        var data = await LoadAsync(hoSoId, false, cancellationToken);
        return data is null ? null : ToDto(hoSoId, data.BoHoSo, data.KetQua);
    }

    public async Task<XayDungVanBanBanHanhDto?> UpdateAsync(Guid hoSoId, CapNhatBanHanhRequest request, CancellationToken cancellationToken = default)
    {
        var data = await LoadAsync(hoSoId, true, cancellationToken);
        if (data is null) return null;
        EnsureNhap(data.BoHoSo);

        data.KetQua.KetQua = request.KetQua?.Trim() ?? string.Empty;
        data.KetQua.SoVanBan = request.SoVanBan?.Trim();
        data.KetQua.NgayBanHanh = request.NgayBanHanh;
        data.KetQua.CoQuanBanHanhId = request.CoQuanBanHanhId;
        data.KetQua.NguoiKyId = request.NguoiKyId;
        data.KetQua.ChucVuNguoiKy = request.ChucVuNguoiKy?.Trim();
        data.KetQua.NgayCoHieuLuc = request.NgayCoHieuLuc;
        data.KetQua.NoiDungKetQua = request.NoiDungKetQua?.Trim();
        data.KetQua.LyDoKhongThongQua = request.LyDoKhongThongQua?.Trim();
        await db.SaveChangesAsync(cancellationToken);
        return ToDto(hoSoId, data.BoHoSo, data.KetQua);
    }

    public async Task<IReadOnlyList<XayDungVanBanTaiLieuDto>?> GetTaiLieuAsync(Guid hoSoId, CancellationToken cancellationToken = default)
    {
        var data = await LoadAsync(hoSoId, false, cancellationToken);
        if (data is null) return null;

        return await (from link in db.BoHoSoNghiepVuTaiLieus.AsNoTracking()
                      join file in db.HoSoXayDungVanBanFiles.AsNoTracking() on link.HoSoXayDungVanBanFileId equals file.Id
                      where link.BoHoSoNghiepVuId == data.BoHoSo.Id && !link.IsDeleted && !file.IsDeleted
                      orderby file.NgayTaiLen descending
                      select ToFileDto(file))
            .ToListAsync(cancellationToken);
    }

    public async Task<XayDungVanBanTaiLieuDto?> UploadTaiLieuAsync(Guid hoSoId, TaiTaiLieuBanHanhRequest request, CancellationToken cancellationToken = default)
    {
        var data = await LoadAsync(hoSoId, true, cancellationToken);
        if (data is null) return null;
        EnsureNhap(data.BoHoSo);
        var actor = GetActor(data.BoHoSo.DonViLapId);
        if (request.LoaiTaiLieuId == Guid.Empty || request.NoiDung.Length == 0) throw new InvalidOperationException("Thiếu tài liệu ban hành.");

        var name = Path.GetFileName(request.TenFile);
        var relativeDirectory = Path.Combine("uploads", "xay-dung-van-ban", hoSoId.ToString("N"));
        var directory = Path.Combine(environment.ContentRootPath, relativeDirectory);
        Directory.CreateDirectory(directory);
        var stored = $"{Guid.NewGuid():N}_{name}";
        var path = Path.Combine(directory, stored);
        await using (var output = File.Create(path))
            await request.NoiDung.CopyToAsync(output, cancellationToken);

        var nextVersion = (await db.HoSoXayDungVanBanFiles
            .Where(x => x.HoSoXayDungVanBanId == hoSoId && x.LoaiTaiLieuId == request.LoaiTaiLieuId && !x.IsDeleted)
            .Select(x => (int?)x.PhienBan)
            .MaxAsync(cancellationToken) ?? 0) + 1;
        var file = new HoSoXayDungVanBanFile
        {
            HoSoXayDungVanBanId = hoSoId,
            LoaiTaiLieuId = request.LoaiTaiLieuId,
            TenTaiLieu = request.TenTaiLieu,
            TenFile = name,
            DuongDanFile = Path.Combine(relativeDirectory, stored).Replace('\\', '/'),
            MimeType = request.MimeType,
            DungLuong = request.NoiDung.Length,
            PhienBan = nextVersion,
            NguoiTaiLenId = actor.UserId,
            CreatedBy = actor.UserId.ToString()
        };

        db.HoSoXayDungVanBanFiles.Add(file);
        db.BoHoSoNghiepVuTaiLieus.Add(new()
        {
            BoHoSoNghiepVuId = data.BoHoSo.Id,
            HoSoXayDungVanBanFileId = file.Id,
            LoaiTaiLieuId = file.LoaiTaiLieuId,
            HinhThucThem = "TaoMoi",
            CreatedBy = actor.UserId.ToString()
        });
        await db.SaveChangesAsync(cancellationToken);
        return ToFileDto(file);
    }

    public async Task<DieuKienHoanThanhBanHanhDto?> KiemTraAsync(Guid hoSoId, CancellationToken cancellationToken = default)
    {
        var data = await LoadAsync(hoSoId, false, cancellationToken);
        if (data is null) return null;
        var conditions = await ConditionsAsync(data, cancellationToken);
        return new(conditions.Count == 0, conditions);
    }

    public async Task<XayDungVanBanBanHanhDto?> HoanThanhAsync(Guid hoSoId, CancellationToken cancellationToken = default)
    {
        var data = await LoadAsync(hoSoId, true, cancellationToken);
        if (data is null) return null;
        EnsureNhap(data.BoHoSo);
        var conditions = await ConditionsAsync(data, cancellationToken);
        if (conditions.Count > 0) throw new InvalidOperationException(string.Join(" ", conditions));

        var actor = GetActor(data.BoHoSo.DonViLapId);
        var now = DateTime.UtcNow;
        var hoSo = await db.HoSoXayDungVanBans.FirstOrDefaultAsync(x => x.Id == hoSoId && !x.IsDeleted, cancellationToken);
        if (hoSo is not null)
        {
            hoSo.UpdatedAt = now;
            hoSo.UpdatedBy = actor.UserId.ToString();
        }
        data.BoHoSo.TrangThai = TrangThaiBoHoSo.DaHoanThanh;
        data.BoHoSo.NgayGui = now;
        data.BoHoSo.NgayHoanThanh = now;
        data.BoHoSo.UpdatedAt = now;
        data.BoHoSo.UpdatedBy = actor.UserId.ToString();
        db.HoSoXayDungVanBanLichSuXuLys.Add(new()
        {
            HoSoXayDungVanBanId = hoSoId,
            BoHoSoNghiepVuId = data.BoHoSo.Id,
            HanhDong = "HOAN_THANH_BAN_HANH",
            NoiDung = string.IsNullOrWhiteSpace(data.KetQua.SoVanBan)
                ? "Hoàn thành bước ban hành."
                : $"Hoàn thành bước ban hành. Số văn bản: {data.KetQua.SoVanBan}.",
            NguoiXuLyId = actor.UserId,
            DonViXuLyId = actor.DonViId,
            ThoiGianXuLy = now,
            BuocQuyTrinhTruocId = data.BoHoSo.BuocQuyTrinhId,
            BuocQuyTrinhSauId = data.BoHoSo.BuocQuyTrinhId,
            CreatedBy = actor.UserId.ToString()
        });
        await db.SaveChangesAsync(cancellationToken);
        return ToDto(hoSoId, data.BoHoSo, data.KetQua);
    }

    private async Task<List<string>> ConditionsAsync(Data data, CancellationToken cancellationToken)
    {
        var conditions = new List<string>();
        if (string.IsNullOrWhiteSpace(data.KetQua.KetQua)) conditions.Add("Chưa cập nhật kết quả ban hành.");
        if (string.Equals(data.KetQua.KetQua, "BAN_HANH", StringComparison.OrdinalIgnoreCase)
            || string.Equals(data.KetQua.KetQua, "DA_BAN_HANH", StringComparison.OrdinalIgnoreCase))
        {
            if (string.IsNullOrWhiteSpace(data.KetQua.SoVanBan)) conditions.Add("Chưa nhập số văn bản ban hành.");
            if (!data.KetQua.NgayBanHanh.HasValue) conditions.Add("Chưa nhập ngày ban hành.");
        }
        if (!await db.BoHoSoNghiepVuTaiLieus.AnyAsync(x => x.BoHoSoNghiepVuId == data.BoHoSo.Id && !x.IsDeleted, cancellationToken))
            conditions.Add("Chưa có tài liệu ban hành.");
        return conditions;
    }

    private async Task<Data?> LoadAsync(Guid hoSoId, bool tracking, CancellationToken cancellationToken)
    {
        var boHoSos = tracking ? db.BoHoSoNghiepVus : db.BoHoSoNghiepVus.AsNoTracking();
        var details = tracking ? db.HoSoXayDungVanBanKetQuaBanHanhs : db.HoSoXayDungVanBanKetQuaBanHanhs.AsNoTracking();
        return await (from boHoSo in boHoSos
                      join detail in details on boHoSo.Id equals detail.BoHoSoNghiepVuId
                      where boHoSo.HoSoXayDungVanBanId == hoSoId && boHoSo.LoaiBoHoSo == LoaiBoHoSo.BanHanh && !boHoSo.IsDeleted
                      orderby boHoSo.CreatedAt descending
                      select new Data(boHoSo, detail))
            .FirstOrDefaultAsync(cancellationToken);
    }

    private static void EnsureNhap(BoHoSoNghiepVu boHoSo)
    {
        if (boHoSo.TrangThai != TrangThaiBoHoSo.Nhap) throw new InvalidOperationException("Hồ sơ ban hành đã hoàn thành, không được cập nhật.");
    }

    private CurrentActor GetActor(Guid? fallbackDonViId = null)
    {
        if (!user.IsAuthenticated || user.UserId is not Guid userId || userId == Guid.Empty)
            throw new UnauthorizedAccessException("Người dùng chưa đăng nhập.");
        var donViId = user.DonViId is { } current && current != Guid.Empty ? current : fallbackDonViId.GetValueOrDefault();
        if (donViId == Guid.Empty) throw new UnauthorizedAccessException("Người dùng chưa có thông tin đơn vị xử lý.");
        return new(userId, donViId);
    }

    private static XayDungVanBanBanHanhDto ToDto(Guid hoSoId, BoHoSoNghiepVu boHoSo, HoSoXayDungVanBanKetQuaBanHanh detail) =>
        new(hoSoId, boHoSo.Id, boHoSo.TrangThai.ToString(), detail.KetQua, detail.SoVanBan, detail.NgayBanHanh, detail.CoQuanBanHanhId, detail.NguoiKyId, detail.ChucVuNguoiKy, detail.NgayCoHieuLuc, detail.NoiDungKetQua, detail.LyDoKhongThongQua);

    private static XayDungVanBanTaiLieuDto ToFileDto(HoSoXayDungVanBanFile file) =>
        new(file.Id, file.LoaiTaiLieuId, file.TenTaiLieu, file.PhienBan, file.TenFile, file.DuongDanFile, file.MimeType, file.DungLuong, file.IsCurrent, file.NgayTaiLen);

    private sealed record CurrentActor(Guid UserId, Guid DonViId);
    private sealed record Data(BoHoSoNghiepVu BoHoSo, HoSoXayDungVanBanKetQuaBanHanh KetQua);
}
