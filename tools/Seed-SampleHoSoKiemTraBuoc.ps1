param(
    [string]$Server = "14.248.85.58",
    [string]$Database = "CSDLXayDungVanBan_MS",
    [string]$Username = "sa",
    [string]$Password,
    [switch]$Execute,
    [string]$OutputSqlPath = "tools/seed_sample_ho_so_kiem_tra_buoc.generated.sql"
)

$ErrorActionPreference = "Stop"

function New-StableGuid {
    param([string]$Key)

    $md5 = [System.Security.Cryptography.MD5]::Create()
    try {
        return [Guid]::new($md5.ComputeHash([System.Text.Encoding]::UTF8.GetBytes($Key)))
    }
    finally {
        $md5.Dispose()
    }
}

function SqlGuid { param([Guid]$Value) "'$($Value.ToString())'" }
function SqlDate { param([datetime]$Value) "'$($Value.ToString("yyyy-MM-ddTHH:mm:ss"))'" }
function SqlText { param([string]$Value) "N'$($Value.Replace("'", "''"))'" }
function Add-Line { param([System.Text.StringBuilder]$Builder, [string]$Text = "") [void]$Builder.AppendLine($Text) }

function Split-SqlBatches {
    param([string]$Text)

    $batches = [System.Collections.Generic.List[string]]::new()
    $current = [System.Text.StringBuilder]::new()
    foreach ($line in ($Text -split "\r?\n")) {
        if ($line.Trim().ToUpperInvariant() -eq "GO") {
            $batch = $current.ToString().Trim()
            if (-not [string]::IsNullOrWhiteSpace($batch)) { $batches.Add($batch) }
            $current.Clear() | Out-Null
            continue
        }

        $current.AppendLine($line) | Out-Null
    }

    $last = $current.ToString().Trim()
    if (-not [string]::IsNullOrWhiteSpace($last)) { $batches.Add($last) }
    return $batches
}

function New-SampleFile {
    param(
        [string]$Service,
        [string]$RelativeDirectory,
        [string]$FileName,
        [string]$Title
    )

    $serviceRoot = Join-Path (Get-Location) "BE/services/$Service"
    $directory = Join-Path $serviceRoot $RelativeDirectory
    New-Item -ItemType Directory -Path $directory -Force | Out-Null
    $fullPath = Join-Path $directory $FileName
    Set-Content -LiteralPath $fullPath -Encoding UTF8 -Value @(
        $Title
        "GeneratedAt: 2026-10-11"
        "Purpose: Ho so mau dung de kiem tra tung buoc va nhanh xu ly quy trinh."
    )

    [pscustomobject]@{
        RelativePath = (Join-Path $RelativeDirectory $FileName).Replace("\", "/")
        FileName = $FileName
        Size = (Get-Item -LiteralPath $fullPath).Length
    }
}

$actorId = [Guid]"00000000-0000-0000-0000-000000000001"
$qdWorkflow = [Guid]"44444444-4444-4444-4444-444444444101"
$nqWorkflow = [Guid]"44444444-4444-4444-4444-444444444201"
$dkWorkflow = [Guid]"33333333-3333-3333-3333-333333333301"
$thplStatusNhap = [Guid]"55555555-5555-5555-5555-555555555401"
$thplStatusDang = [Guid]"55555555-5555-5555-5555-555555555402"
$thplStatusBoSung = [Guid]"55555555-5555-5555-5555-555555555403"
$thplStatusDanhGia = [Guid]"55555555-5555-5555-5555-555555555404"
$thplStatusHoanThanh = [Guid]"55555555-5555-5555-5555-555555555405"
$ksStatusNhap = [Guid]"55555555-5555-5555-5555-555555555501"
$ksStatusPhatHanh = [Guid]"55555555-5555-5555-5555-555555555502"
$ksStatusDangTraLoi = [Guid]"55555555-5555-5555-5555-555555555503"
$ksStatusBoSung = [Guid]"55555555-5555-5555-5555-555555555504"
$ksStatusDanhGia = [Guid]"55555555-5555-5555-5555-555555555505"
$ksStatusHoanThanh = [Guid]"55555555-5555-5555-5555-555555555506"

$dangKyCases = @(
    @{ Code = "DK-STEP-01"; StepId = [Guid]"33333333-3333-3333-3333-333333333311"; StatusId = [Guid]"11111111-1111-1111-1111-111111111102"; Name = "Đang lập hồ sơ đăng ký"; FileType = "DE_XUAT"; ActionId = [Guid]"22222222-2222-2222-2222-222222222201"; Action = "TAO_MOI" },
    @{ Code = "DK-STEP-02"; StepId = [Guid]"33333333-3333-3333-3333-333333333312"; StatusId = [Guid]"11111111-1111-1111-1111-111111111103"; Name = "Đã trình phê duyệt"; FileType = "TO_TRINH"; ActionId = [Guid]"22222222-2222-2222-2222-222222222203"; Action = "TRINH_PHE_DUYET" },
    @{ Code = "DK-STEP-03"; StepId = [Guid]"33333333-3333-3333-3333-333333333313"; StatusId = [Guid]"11111111-1111-1111-1111-111111111103"; Name = "Đang phê duyệt"; FileType = "PHIEU_PHE_DUYET"; ActionId = [Guid]"22222222-2222-2222-2222-222222222204"; Action = "PHE_DUYET" },
    @{ Code = "DK-STEP-04"; StepId = [Guid]"33333333-3333-3333-3333-333333333314"; StatusId = [Guid]"11111111-1111-1111-1111-111111111104"; Name = "Chờ cập nhật kết quả"; FileType = "KET_QUA"; ActionId = [Guid]"22222222-2222-2222-2222-222222222207"; Action = "CAP_NHAT_KET_QUA" },
    @{ Code = "DK-STEP-05"; StepId = [Guid]"33333333-3333-3333-3333-333333333315"; StatusId = [Guid]"11111111-1111-1111-1111-111111111108"; Name = "Hoàn thành đăng ký"; FileType = "BIEN_BAN_HOAN_THANH"; ActionId = [Guid]"22222222-2222-2222-2222-222222222208"; Action = "HOAN_THANH" },
    @{ Code = "DK-RETURN-01"; StepId = [Guid]"33333333-3333-3333-3333-333333333311"; StatusId = [Guid]"11111111-1111-1111-1111-111111111105"; Name = "Bị trả lại để bổ sung"; FileType = "YEU_CAU_BO_SUNG"; ActionId = [Guid]"22222222-2222-2222-2222-222222222205"; Action = "TRA_LAI"; ReturnReason = "Bổ sung căn cứ pháp lý và dự kiến nguồn lực." }
)

$qdSteps = @(
    @{ Code = "XDQD-STEP-01"; StepId = [Guid]"44444444-4444-4444-4444-444444444111"; StatusId = [Guid]"55555555-5555-5555-5555-555555555201"; Name = "Đang soạn thảo"; FileType = "DU_THAO"; BoType = 1; BoStatus = 1 },
    @{ Code = "XDQD-STEP-02"; StepId = [Guid]"44444444-4444-4444-4444-444444444112"; StatusId = [Guid]"55555555-5555-5555-5555-555555555202"; Name = "Đã gửi thẩm định"; FileType = "TO_TRINH_THAM_DINH"; BoType = 2; BoStatus = 2 },
    @{ Code = "XDQD-STEP-03"; StepId = [Guid]"44444444-4444-4444-4444-444444444113"; StatusId = [Guid]"55555555-5555-5555-5555-555555555203"; Name = "Đang thẩm định"; FileType = "BAO_CAO_THAM_DINH"; BoType = 3; BoStatus = 3 },
    @{ Code = "XDQD-STEP-04"; StepId = [Guid]"44444444-4444-4444-4444-444444444114"; StatusId = [Guid]"55555555-5555-5555-5555-555555555205"; Name = "Đã thẩm định, chờ trình UBND"; FileType = "TO_TRINH_UBND"; BoType = 4; BoStatus = 1 },
    @{ Code = "XDQD-STEP-05"; StepId = [Guid]"44444444-4444-4444-4444-444444444115"; StatusId = [Guid]"55555555-5555-5555-5555-555555555206"; Name = "Chờ ý kiến UBND"; FileType = "TONG_HOP_Y_KIEN_UBND"; BoType = 5; BoStatus = 1 },
    @{ Code = "XDQD-STEP-06"; StepId = [Guid]"44444444-4444-4444-4444-444444444116"; StatusId = [Guid]"55555555-5555-5555-5555-555555555207"; Name = "Đã ban hành quyết định"; FileType = "QUYET_DINH_BAN_HANH"; BoType = 7; BoStatus = 5 },
    @{ Code = "XDQD-BOSUNG-01"; StepId = [Guid]"44444444-4444-4444-4444-444444444111"; StatusId = [Guid]"55555555-5555-5555-5555-555555555204"; Name = "Thẩm định yêu cầu bổ sung"; FileType = "YEU_CAU_BO_SUNG"; BoType = 3; BoStatus = 4; ReturnReason = "Thiếu báo cáo đánh giá tác động." }
)

$nqSteps = @(
    @{ Code = "XDNQ-STEP-01"; StepId = [Guid]"44444444-4444-4444-4444-444444444211"; StatusId = [Guid]"55555555-5555-5555-5555-555555555301"; Name = "Đang soạn thảo"; FileType = "DU_THAO"; BoType = 1; BoStatus = 1 },
    @{ Code = "XDNQ-STEP-02"; StepId = [Guid]"44444444-4444-4444-4444-444444444212"; StatusId = [Guid]"55555555-5555-5555-5555-555555555302"; Name = "Đã gửi thẩm định"; FileType = "TO_TRINH_THAM_DINH"; BoType = 2; BoStatus = 2 },
    @{ Code = "XDNQ-STEP-03"; StepId = [Guid]"44444444-4444-4444-4444-444444444213"; StatusId = [Guid]"55555555-5555-5555-5555-555555555303"; Name = "Đang thẩm định"; FileType = "BAO_CAO_THAM_DINH"; BoType = 3; BoStatus = 3 },
    @{ Code = "XDNQ-STEP-04"; StepId = [Guid]"44444444-4444-4444-4444-444444444214"; StatusId = [Guid]"55555555-5555-5555-5555-555555555305"; Name = "Chờ trình UBND cho ý kiến"; FileType = "TO_TRINH_UBND"; BoType = 4; BoStatus = 1 },
    @{ Code = "XDNQ-STEP-05"; StepId = [Guid]"44444444-4444-4444-4444-444444444215"; StatusId = [Guid]"55555555-5555-5555-5555-555555555306"; Name = "Chờ ý kiến UBND"; FileType = "TONG_HOP_Y_KIEN_UBND"; BoType = 5; BoStatus = 1 },
    @{ Code = "XDNQ-STEP-06"; StepId = [Guid]"44444444-4444-4444-4444-444444444216"; StatusId = [Guid]"55555555-5555-5555-5555-555555555307"; Name = "Chờ thẩm tra HĐND"; FileType = "BAO_CAO_THAM_TRA"; BoType = 6; BoStatus = 1 },
    @{ Code = "XDNQ-STEP-07"; StepId = [Guid]"44444444-4444-4444-4444-444444444217"; StatusId = [Guid]"55555555-5555-5555-5555-555555555308"; Name = "Đã thông qua ban hành"; FileType = "NGHI_QUYET_BAN_HANH"; BoType = 7; BoStatus = 5 },
    @{ Code = "XDNQ-HDND-TRALAI-01"; StepId = [Guid]"44444444-4444-4444-4444-444444444211"; StatusId = [Guid]"55555555-5555-5555-5555-555555555304"; Name = "HĐND yêu cầu hoàn thiện"; FileType = "YEU_CAU_HOAN_THIEN"; BoType = 6; BoStatus = 4; ReturnReason = "Bổ sung giải trình sau thẩm tra HĐND." }
)

$sql = [System.Text.StringBuilder]::new()
Add-Line $sql "SET NOCOUNT ON;"
Add-Line $sql "SET XACT_ABORT ON;"
Add-Line $sql "BEGIN TRANSACTION;"
Add-Line $sql "DECLARE @Now datetime2 = SYSUTCDATETIME();"
Add-Line $sql "DECLARE @Actor uniqueidentifier = '$actorId';"
Add-Line $sql "DECLARE @DonViSoanThao uniqueidentifier = '00000000-0000-0000-0000-000000000101';"
Add-Line $sql "DECLARE @DonViPheDuyet uniqueidentifier = '00000000-0000-0000-0000-000000000102';"
Add-Line $sql "DECLARE @CanBo uniqueidentifier = '00000000-0000-0000-0000-000000000201';"
Add-Line $sql "DECLARE @LoaiVanBan uniqueidentifier = '00000000-0000-0000-0000-000000000301';"
Add-Line $sql "DECLARE @LinhVuc uniqueidentifier = '00000000-0000-0000-0000-000000000401';"
Add-Line $sql "SELECT TOP (1) @DonViSoanThao = [Id] FROM [dm].[DanhMucDonVis] ORDER BY [CreatedDate], [Id];"
Add-Line $sql "SELECT TOP (1) @DonViPheDuyet = [Id] FROM [dm].[DanhMucDonVis] WHERE [Id] <> @DonViSoanThao ORDER BY [CreatedDate], [Id];"
Add-Line $sql "SET @DonViPheDuyet = COALESCE(@DonViPheDuyet, @DonViSoanThao);"
Add-Line $sql "SELECT TOP (1) @CanBo = [Id] FROM [dm].[DanhMucCanBos] ORDER BY [CreatedDate], [Id];"
Add-Line $sql "SELECT TOP (1) @LoaiVanBan = [Id] FROM [dm].[DanhMucVanBans] WHERE [TrangThai] = 1 ORDER BY [CreatedDate], [Id];"
Add-Line $sql "SELECT TOP (1) @LinhVuc = [Id] FROM [dm].[DanhMucLinhVucs] WHERE [TrangThai] = 1 ORDER BY [CreatedDate], [Id];"
Add-Line $sql ""

foreach ($case in $dangKyCases) {
    $id = New-StableGuid "$($case.Code)-ho-so"
    $file = New-SampleFile -Service "DangKyXayDungVanBanService" -RelativeDirectory "uploads/sample-data/kiem-tra-buoc/dang-ky/$($case.Code.ToLowerInvariant())" -FileName "$($case.Code.ToLowerInvariant()).txt" -Title "$($case.Code) - $($case.Name)"
    $fileId = New-StableGuid "$($case.Code)-file"
    $historyId = New-StableGuid "$($case.Code)-history"
    $reason = if ($case.ReturnReason) { SqlText $case.ReturnReason } else { "NULL" }
    Add-Line $sql "IF NOT EXISTS (SELECT 1 FROM [dbo].[DangKyXayDungVanBans] WHERE [MaHoSo] = N'$($case.Code)')"
    Add-Line $sql "BEGIN"
    Add-Line $sql "    INSERT INTO [dbo].[DangKyXayDungVanBans] ([Id],[MaHoSo],[TenHoSo],[TenVanBanDuKien],[LoaiVanBanId],[QuyTrinhSoanThaoId],[BuocHienTaiId],[TrangThaiHoSoId],[DonViSoanThaoId],[DonViPheDuyetId],[NamDangKy],[CanCuDeXuat],[SuCanThiet],[NoiDungChinhSach],[DuKienThoiGianTrinh],[DaKhoiTaoQuyTrinhXayDung],[CreatedAt],[CreatedBy],[IsDeleted]) VALUES ($(SqlGuid $id),N'$($case.Code)',N'$($case.Name)',N'Dự thảo kiểm tra bước $($case.Code)',@LoaiVanBan,$(SqlGuid $dkWorkflow),$(SqlGuid $case.StepId),$(SqlGuid $case.StatusId),@DonViSoanThao,@DonViPheDuyet,2026,N'Căn cứ mẫu',N'Sự cần thiết mẫu',N'Nội dung chính sách mẫu','2026-09-30T00:00:00',0,@Now,CONVERT(nvarchar(64),@Actor),0);"
    Add-Line $sql "    INSERT INTO [dbo].[DangKyXayDungVanBanFiles] ([Id],[DangKyXayDungVanBanId],[LoaiFile],[TenFile],[DuongDanFile],[DungLuong],[MimeType],[MoTa],[CreatedAt],[CreatedBy],[IsDeleted]) VALUES ($(SqlGuid $fileId),$(SqlGuid $id),N'$($case.FileType)',N'$($file.FileName)',N'$($file.RelativePath)',$($file.Size),N'text/plain',N'$($case.Name)',@Now,CONVERT(nvarchar(64),@Actor),0);"
    Add-Line $sql "    INSERT INTO [dbo].[DangKyXayDungVanBanLichSuXuLys] ([Id],[DangKyXayDungVanBanId],[DenBuocId],[HanhDongId],[MaHanhDongSnapshot],[TenHanhDongSnapshot],[TrangThaiSauId],[TenTrangThaiSauSnapshot],[NoiDungXuLy],[LyDoTraLai],[NguoiXuLyId],[TenNguoiXuLy],[DonViXuLyId],[TenDonViXuLy],[NgayXuLy],[CreatedAt],[CreatedBy],[IsDeleted]) VALUES ($(SqlGuid $historyId),$(SqlGuid $id),$(SqlGuid $case.StepId),$(SqlGuid $case.ActionId),N'$($case.Action)',N'$($case.Name)',$(SqlGuid $case.StatusId),N'$($case.Name)',N'Hồ sơ kiểm tra bước: $($case.Name)',$reason,@Actor,N'Người dùng mẫu',@DonViSoanThao,N'Đơn vị mẫu',@Now,@Now,CONVERT(nvarchar(64),@Actor),0);"
    Add-Line $sql "END"
}

function Add-XayDungCaseSql {
    param(
        [hashtable]$Case,
        [Guid]$WorkflowId,
        [string]$ServicePrefix,
        [string]$Folder
    )

    $id = New-StableGuid "$($Case.Code)-ho-so"
    $boId = New-StableGuid "$($Case.Code)-bo"
    $fileId = New-StableGuid "$($Case.Code)-file"
    $linkId = New-StableGuid "$($Case.Code)-link"
    $historyId = New-StableGuid "$($Case.Code)-history"
    $loaiTaiLieuId = New-StableGuid "loai-tai-lieu-$($Case.FileType)"
    $file = New-SampleFile -Service "XayDungVanBanService" -RelativeDirectory "uploads/sample-data/kiem-tra-buoc/$Folder/$($Case.Code.ToLowerInvariant())" -FileName "$($Case.Code.ToLowerInvariant()).txt" -Title "$($Case.Code) - $($Case.Name)"
    $ngayHoanThanh = if ($Case.BoStatus -eq 5 -or $Case.BoStatus -eq 4) { "'2026-10-01T00:00:00'" } else { "NULL" }
    $reason = if ($Case.ReturnReason) { SqlText $Case.ReturnReason } else { "NULL" }

    Add-Line $sql "IF NOT EXISTS (SELECT 1 FROM [dbo].[HoSoXayDungVanBans] WHERE [MaHoSo] = N'$($Case.Code)')"
    Add-Line $sql "BEGIN"
    Add-Line $sql "    INSERT INTO [dbo].[HoSoXayDungVanBans] ([Id],[MaHoSo],[TenHoSo],[TenDuThaoVanBan],[DanhMucVanBanId],[QuyTrinhSoanThaoId],[BuocHienTaiId],[TrangThaiHoSoId],[DonViChuTriSoanThaoId],[NguoiPhuTrachId],[NamXayDung],[ThoiGianDuKienBatDau],[ThoiGianDuKienHoanThanh],[MoTa],[CreatedAt],[CreatedBy],[IsDeleted]) VALUES ($(SqlGuid $id),N'$($Case.Code)',N'$($Case.Name)',N'Dự thảo kiểm tra bước $($Case.Code)',@LoaiVanBan,$(SqlGuid $WorkflowId),$(SqlGuid $Case.StepId),$(SqlGuid $Case.StatusId),@DonViSoanThao,@CanBo,2026,'2026-01-01T00:00:00','2026-12-31T00:00:00',N'Hồ sơ test dừng tại bước quy trình.',@Now,CONVERT(nvarchar(64),@Actor),0);"
    Add-Line $sql "    INSERT INTO [dbo].[BoHoSoNghiepVus] ([Id],[HoSoXayDungVanBanId],[BuocQuyTrinhId],[LoaiBoHoSo],[TrangThai],[LanXuLy],[NgayTao],[NgayGui],[NgayHoanThanh],[NguoiLapId],[DonViLapId],[LyDoTraLai],[NoiDungGhiChu],[CreatedAt],[CreatedBy],[IsDeleted]) VALUES ($(SqlGuid $boId),$(SqlGuid $id),$(SqlGuid $Case.StepId),$($Case.BoType),$($Case.BoStatus),1,'2026-09-01T00:00:00','2026-09-02T00:00:00',$ngayHoanThanh,@Actor,@DonViSoanThao,$reason,N'$($Case.Name)',@Now,CONVERT(nvarchar(64),@Actor),0);"
    Add-Line $sql "    INSERT INTO [dbo].[HoSoXayDungVanBanFiles] ([Id],[HoSoXayDungVanBanId],[LoaiTaiLieuId],[TenTaiLieu],[PhienBan],[TenFile],[DuongDanFile],[MimeType],[DungLuong],[IsCurrent],[NguoiTaiLenId],[NgayTaiLen],[CreatedAt],[CreatedBy],[IsDeleted]) VALUES ($(SqlGuid $fileId),$(SqlGuid $id),$(SqlGuid $loaiTaiLieuId),N'$($Case.Name)',1,N'$($file.FileName)',N'$($file.RelativePath)',N'text/plain',$($file.Size),1,@Actor,'2026-09-01T00:00:00',@Now,CONVERT(nvarchar(64),@Actor),0);"
    Add-Line $sql "    INSERT INTO [dbo].[BoHoSoNghiepVuTaiLieus] ([Id],[BoHoSoNghiepVuId],[HoSoXayDungVanBanFileId],[LoaiTaiLieuId],[HinhThucThem],[BatBuoc],[DaKiemTra],[GhiChu],[ThuTu],[CreatedAt],[CreatedBy],[IsDeleted]) VALUES ($(SqlGuid $linkId),$(SqlGuid $boId),$(SqlGuid $fileId),$(SqlGuid $loaiTaiLieuId),N'TaoMoi',1,1,N'Tài liệu kiểm tra bước',1,@Now,CONVERT(nvarchar(64),@Actor),0);"
    Add-Line $sql "    INSERT INTO [dbo].[HoSoXayDungVanBanLichSuXuLys] ([Id],[HoSoXayDungVanBanId],[BoHoSoNghiepVuId],[BuocQuyTrinhSauId],[TrangThaiSauId],[TenBuocSau],[HanhDong],[NoiDung],[LyDo],[NguoiXuLyId],[DonViXuLyId],[ThoiGianXuLy],[HoSoXayDungVanBanFileId],[BoHoSoNghiepVuTaiLieuId],[CreatedAt],[CreatedBy],[IsDeleted]) VALUES ($(SqlGuid $historyId),$(SqlGuid $id),$(SqlGuid $boId),$(SqlGuid $Case.StepId),$(SqlGuid $Case.StatusId),N'$($Case.Name)',N'KIEM_TRA_BUOC',N'Hồ sơ kiểm tra bước: $($Case.Name)',$reason,@Actor,@DonViSoanThao,@Now,$(SqlGuid $fileId),$(SqlGuid $linkId),@Now,CONVERT(nvarchar(64),@Actor),0);"

    if ($Case.BoType -eq 1) {
        Add-Line $sql "    INSERT INTO [dbo].[HoSoXayDungVanBanSoanThaos] ([BoHoSoNghiepVuId],[CanCuXayDung],[PhamViDieuChinh],[NoiDungChinhSach],[HinhThucLayYKien],[TuNgayLayYKien],[DenNgayLayYKien],[TongSoYKien],[NoiDungTongHopTiepThuGiaiTrinh]) VALUES ($(SqlGuid $boId),N'Căn cứ test',N'Phạm vi test',N'Chính sách test',N'Lấy ý kiến test','2026-09-01T00:00:00','2026-09-10T00:00:00',5,N'Tổng hợp test');"
    }
    elseif ($Case.BoType -eq 2) {
        Add-Line $sql "    INSERT INTO [dbo].[HoSoXayDungVanBanTrinhThamDinhs] ([BoHoSoNghiepVuId],[FileDuThaoId],[SoToTrinh],[NgayToTrinh],[NgayGuiThamDinh],[DonViNhanThamDinhId],[NoiDungDeNghiThamDinh],[HanDeNghiTraKetQua]) VALUES ($(SqlGuid $boId),$(SqlGuid $fileId),N'$ServicePrefix/TTr-STP','2026-09-02T00:00:00','2026-09-03T00:00:00',@DonViPheDuyet,N'Đề nghị thẩm định hồ sơ test','2026-09-15T00:00:00');"
    }
    elseif ($Case.BoType -eq 3) {
        Add-Line $sql "    INSERT INTO [dbo].[HoSoXayDungVanBanThamDinhs] ([BoHoSoNghiepVuId],[NgayTiepNhan],[HinhThucThamDinh],[NgayThamDinh],[KetQuaThamDinh],[NoiDungKetLuan],[NoiDungYeuCauBoSung],[HanBoSung],[NgayGuiKetQua],[NguoiKetLuanId]) VALUES ($(SqlGuid $boId),'2026-09-04T00:00:00',N'Thẩm định hồ sơ','2026-09-08T00:00:00',N'DANG_XU_LY',N'Kết luận test',$reason,'2026-09-20T00:00:00','2026-09-09T00:00:00',@CanBo);"
    }
    elseif ($Case.BoType -eq 4) {
        Add-Line $sql "    INSERT INTO [dbo].[HoSoXayDungVanBanTrinhPheDuyets] ([BoHoSoNghiepVuId],[CapTrinh],[MucDichTrinh],[SoToTrinh],[NgayToTrinh],[NgayTrinh],[NoiDungTrinh],[DonViDongGuiId]) VALUES ($(SqlGuid $boId),N'UBND tỉnh',N'Trình kiểm tra bước',N'$ServicePrefix/TTr-UBND','2026-09-10T00:00:00','2026-09-11T00:00:00',N'Nội dung trình test',@DonViPheDuyet);"
    }
    elseif ($Case.BoType -eq 5) {
        Add-Line $sql "    INSERT INTO [dbo].[HoSoXayDungVanBanYKienUbnds] ([BoHoSoNghiepVuId],[NgayNhanYKien],[TongSoThanhVienDuocLayYKien],[SoDongY],[SoKhongDongY],[SoYKienKhac],[KetLuanTongHop],[NoiDungTongHop],[NoiDungGiaiTrinh],[CanBoSungHoSo]) VALUES ($(SqlGuid $boId),'2026-09-15T00:00:00',15,10,2,3,N'Đang tổng hợp',N'Tổng hợp ý kiến test',N'Giải trình test',0);"
    }
    elseif ($Case.BoType -eq 6) {
        Add-Line $sql "    INSERT INTO [dbo].[HoSoXayDungVanBanThamTraHdnds] ([BoHoSoNghiepVuId],[BanHdndThamTraId],[NgayTrinhThamTra],[NgayNhanKetQuaThamTra],[KetQuaThamTra],[NoiDungKienNghi],[NoiDungTiepThuGiaiTrinh]) VALUES ($(SqlGuid $boId),@DonViPheDuyet,'2026-09-20T00:00:00','2026-09-25T00:00:00',N'Đang thẩm tra',N'Kiến nghị test',N'Tiếp thu test');"
    }
    elseif ($Case.BoType -eq 7) {
        Add-Line $sql "    INSERT INTO [dbo].[HoSoXayDungVanBanKetQuaBanHanhs] ([BoHoSoNghiepVuId],[KetQua],[SoVanBan],[NgayBanHanh],[CoQuanBanHanhId],[NguoiKyId],[ChucVuNguoiKy],[NgayCoHieuLuc],[NoiDungKetQua]) VALUES ($(SqlGuid $boId),N'BAN_HANH',N'$ServicePrefix/2026/QPPL','2026-10-01T00:00:00',@DonViPheDuyet,@CanBo,N'Chủ tịch','2026-10-10T00:00:00',N'Kết quả ban hành test');"
    }

    Add-Line $sql "END"
}

foreach ($case in $qdSteps) { Add-XayDungCaseSql -Case $case -WorkflowId $qdWorkflow -ServicePrefix "QD" -Folder "quyet-dinh-ubnd" }
foreach ($case in $nqSteps) { Add-XayDungCaseSql -Case $case -WorkflowId $nqWorkflow -ServicePrefix "NQ" -Folder "nghi-quyet-hdnd" }

$thiHanhCases = @(
    @{ Code = "THPL-STEP-01"; Status = $thplStatusNhap; Name = "Tạo kế hoạch" },
    @{ Code = "THPL-STEP-02"; Status = $thplStatusDang; Name = "Đang thực hiện nội dung" },
    @{ Code = "THPL-STEP-03"; Status = $thplStatusDanhGia; Name = "Chờ đánh giá kết quả" },
    @{ Code = "THPL-STEP-04"; Status = $thplStatusHoanThanh; Name = "Tổng hợp hoàn thành" },
    @{ Code = "THPL-BOSUNG-01"; Status = $thplStatusBoSung; Name = "Yêu cầu bổ sung kết quả" }
)

foreach ($case in $thiHanhCases) {
    $khId = New-StableGuid "$($case.Code)-ke-hoach"
    $ndId = New-StableGuid "$($case.Code)-noi-dung"
    $pcId = New-StableGuid "$($case.Code)-phan-cong"
    $bcId = New-StableGuid "$($case.Code)-bao-cao"
    $tepId = New-StableGuid "$($case.Code)-tep"
    $file = New-SampleFile -Service "ThiHanhPhapLuatService" -RelativeDirectory "uploads/sample-data/kiem-tra-buoc/thi-hanh/$($case.Code.ToLowerInvariant())" -FileName "$($case.Code.ToLowerInvariant()).txt" -Title "$($case.Code) - $($case.Name)"
    Add-Line $sql "IF NOT EXISTS (SELECT 1 FROM [thpl].[KeHoachThiHanhPhapLuats] WHERE [MaKeHoach] = N'$($case.Code)')"
    Add-Line $sql "BEGIN"
    Add-Line $sql "    INSERT INTO [thpl].[KeHoachThiHanhPhapLuats] ([Id],[MaKeHoach],[TenKeHoach],[Nam],[TuNgay],[DenNgay],[DonViChuTriId],[NguoiPhuTrachId],[TrangThaiId],[PhamVi],[MucTieu],[CreatedAt],[CreatedBy],[IsDeleted]) VALUES ($(SqlGuid $khId),N'$($case.Code)',N'$($case.Name)',2026,'2026-01-01','2026-12-31',@DonViSoanThao,@CanBo,$(SqlGuid $case.Status),N'Phạm vi test',N'Mục tiêu test',@Now,CONVERT(nvarchar(64),@Actor),0);"
    Add-Line $sql "    INSERT INTO [thpl].[NoiDungKeHoachs] ([Id],[KeHoachId],[MaNoiDung],[TenNoiDung],[NoiDung],[ChiTieu],[DonViTinh],[HanHoanThanh],[ThuTu],[TrangThaiId],[TyLeHoanThanh],[CreatedAt],[CreatedBy],[IsDeleted]) VALUES ($(SqlGuid $ndId),$(SqlGuid $khId),N'$($case.Code)-ND',N'Nội dung $($case.Name)',N'Nội dung test',100,N'%', '2026-11-30',1,$(SqlGuid $case.Status),50,@Now,CONVERT(nvarchar(64),@Actor),0);"
    Add-Line $sql "    INSERT INTO [thpl].[PhanCongThiHanhs] ([Id],[NoiDungKeHoachId],[DonViDuocGiaoId],[CanBoDuocGiaoId],[VaiTro],[HanThucHien],[MucDoUuTien],[CreatedAt],[CreatedBy],[IsDeleted]) VALUES ($(SqlGuid $pcId),$(SqlGuid $ndId),@DonViPheDuyet,@CanBo,N'Phối hợp','2026-10-30',2,@Now,CONVERT(nvarchar(64),@Actor),0);"
    Add-Line $sql "    INSERT INTO [thpl].[BaoCaoTienDoThiHanhs] ([Id],[NoiDungKeHoachId],[PhanCongThiHanhId],[KyBaoCao],[TyLeHoanThanh],[KetQua],[KhoKhan],[KienNghi],[TrangThaiId],[NgayBaoCao],[CreatedAt],[CreatedBy],[IsDeleted]) VALUES ($(SqlGuid $bcId),$(SqlGuid $ndId),$(SqlGuid $pcId),N'Quý IV/2026',50,N'Kết quả test',N'Khó khăn test',N'Kiến nghị test',$(SqlGuid $case.Status),'2026-10-01T00:00:00',@Now,CONVERT(nvarchar(64),@Actor),0);"
    Add-Line $sql "    INSERT INTO [thpl].[TepDinhKemThiHanhs] ([Id],[KeHoachId],[NoiDungKeHoachId],[BaoCaoTienDoThiHanhId],[LoaiTaiLieu],[TenTep],[DuongDan],[PhienBan],[IsCurrent],[CreatedAt],[CreatedBy],[IsDeleted]) VALUES ($(SqlGuid $tepId),$(SqlGuid $khId),$(SqlGuid $ndId),$(SqlGuid $bcId),N'KIEM_TRA_BUOC',N'$($file.FileName)',N'$($file.RelativePath)',1,1,@Now,CONVERT(nvarchar(64),@Actor),0);"
    Add-Line $sql "END"
}

$khaoSatCases = @(
    @{ Code = "KSTHPL-STEP-01"; Status = $ksStatusNhap; Name = "Tạo cuộc khảo sát" },
    @{ Code = "KSTHPL-STEP-02"; Status = $ksStatusPhatHanh; Name = "Đã phát hành khảo sát" },
    @{ Code = "KSTHPL-STEP-03"; Status = $ksStatusDangTraLoi; Name = "Đang trả lời khảo sát" },
    @{ Code = "KSTHPL-STEP-04"; Status = $ksStatusDanhGia; Name = "Đã đánh giá kết quả" },
    @{ Code = "KSTHPL-STEP-05"; Status = $ksStatusHoanThanh; Name = "Hoàn thành báo cáo" },
    @{ Code = "KSTHPL-BOSUNG-01"; Status = $ksStatusBoSung; Name = "Yêu cầu bổ sung phiếu" }
)

foreach ($case in $khaoSatCases) {
    $cuocId = New-StableGuid "$($case.Code)-cuoc"
    $nhomId = New-StableGuid "$($case.Code)-nhom"
    $mauId = New-StableGuid "$($case.Code)-mau"
    $dtId = New-StableGuid "$($case.Code)-doi-tuong"
    $phieuId = New-StableGuid "$($case.Code)-phieu"
    $bcId = New-StableGuid "$($case.Code)-bao-cao"
    $file = New-SampleFile -Service "KhaoSatThiHanhPhapLuatService" -RelativeDirectory "uploads/sample-data/kiem-tra-buoc/khao-sat/$($case.Code.ToLowerInvariant())" -FileName "$($case.Code.ToLowerInvariant()).txt" -Title "$($case.Code) - $($case.Name)"
    Add-Line $sql "IF NOT EXISTS (SELECT 1 FROM [kspl].[CuocKhaoSats] WHERE [MaCuocKhaoSat] = N'$($case.Code)')"
    Add-Line $sql "BEGIN"
    Add-Line $sql "    INSERT INTO [kspl].[CuocKhaoSats] ([Id],[MaCuocKhaoSat],[TenCuocKhaoSat],[MucDich],[PhamVi],[LinhVucId],[DonViChuTriId],[TuNgay],[DenNgay],[TrangThaiId],[CreatedAt],[CreatedBy],[IsDeleted]) VALUES ($(SqlGuid $cuocId),N'$($case.Code)',N'$($case.Name)',N'Mục đích test',N'Phạm vi test',@LinhVuc,@DonViSoanThao,'2026-01-01','2026-12-31',$(SqlGuid $case.Status),@Now,CONVERT(nvarchar(64),@Actor),0);"
    Add-Line $sql "    INSERT INTO [kspl].[NhomDoiTuongKhaoSats] ([Id],[CuocKhaoSatId],[MaNhom],[TenNhom],[ThuTu],[CreatedAt],[CreatedBy],[IsDeleted]) VALUES ($(SqlGuid $nhomId),$(SqlGuid $cuocId),N'NHOM-01',N'Nhóm test',1,@Now,CONVERT(nvarchar(64),@Actor),0);"
    Add-Line $sql "    INSERT INTO [kspl].[MauPhieuKhaoSats] ([Id],[CuocKhaoSatId],[NhomDoiTuongKhaoSatId],[PhienBan],[TenFile],[DuongDanFile],[MaHash],[DaPhatHanh],[TrangThaiMauPhieu],[NgayHieuLuc],[NgayHetHieuLuc],[CreatedAt],[CreatedBy],[IsDeleted]) VALUES ($(SqlGuid $mauId),$(SqlGuid $cuocId),$(SqlGuid $nhomId),1,N'$($file.FileName)',N'$($file.RelativePath)',N'$((New-StableGuid "$($case.Code)-hash").ToString("N"))',1,N'DANG_SU_DUNG','2026-01-01T00:00:00','2026-12-31T00:00:00',@Now,CONVERT(nvarchar(64),@Actor),0);"
    Add-Line $sql "    INSERT INTO [kspl].[DoiTuongKhaoSats] ([Id],[CuocKhaoSatId],[NhomDoiTuongKhaoSatId],[MauPhieuKhaoSatId],[DonViId],[CanBoId],[HanNop],[TrangThaiId],[CreatedAt],[CreatedBy],[IsDeleted]) VALUES ($(SqlGuid $dtId),$(SqlGuid $cuocId),$(SqlGuid $nhomId),$(SqlGuid $mauId),@DonViPheDuyet,@CanBo,'2026-10-30',$(SqlGuid $case.Status),@Now,CONVERT(nvarchar(64),@Actor),0);"
    Add-Line $sql "    INSERT INTO [kspl].[PhieuNopKhaoSats] ([Id],[CuocKhaoSatId],[NhomDoiTuongKhaoSatId],[DoiTuongKhaoSatId],[MauPhieuKhaoSatId],[TenFile],[DuongDanFile],[MaHash],[TrangThaiId],[NgayImport],[TenPhanMem],[PhienBanPhanMem],[CreatedAt],[CreatedBy],[IsDeleted]) VALUES ($(SqlGuid $phieuId),$(SqlGuid $cuocId),$(SqlGuid $nhomId),$(SqlGuid $dtId),$(SqlGuid $mauId),N'$($file.FileName)',N'$($file.RelativePath)',N'$((New-StableGuid "$($case.Code)-phieu").ToString("N"))',$(SqlGuid $case.Status),'2026-10-01T00:00:00',N'Seed test',N'1.0',@Now,CONVERT(nvarchar(64),@Actor),0);"
    Add-Line $sql "    INSERT INTO [kspl].[BaoCaoKhaoSats] ([Id],[CuocKhaoSatId],[TenBaoCao],[SoKyHieu],[NgayBaoCao],[TrangThai],[UuDiem],[HanChe],[KienNghi],[DuongDanFileXuat],[TenFileXuat],[CreatedAt],[CreatedBy],[IsDeleted]) VALUES ($(SqlGuid $bcId),$(SqlGuid $cuocId),N'Báo cáo $($case.Name)',N'$($case.Code)/BC','2026-12-15',N'NHAP',N'Ưu điểm test',N'Hạn chế test',N'Kiến nghị test',N'$($file.RelativePath)',N'$($file.FileName)',@Now,CONVERT(nvarchar(64),@Actor),0);"
    Add-Line $sql "END"
}

Add-Line $sql "COMMIT TRANSACTION;"
Add-Line $sql ""
Add-Line $sql "SELECT N'DK_STEP_RETURN', COUNT(1) FROM [dbo].[DangKyXayDungVanBans] WHERE [MaHoSo] LIKE N'DK-STEP-%' OR [MaHoSo] LIKE N'DK-RETURN-%'"
Add-Line $sql "UNION ALL SELECT N'XDQD_STEP_BRANCH', COUNT(1) FROM [dbo].[HoSoXayDungVanBans] WHERE [MaHoSo] LIKE N'XDQD-STEP-%' OR [MaHoSo] LIKE N'XDQD-BOSUNG-%'"
Add-Line $sql "UNION ALL SELECT N'XDNQ_STEP_BRANCH', COUNT(1) FROM [dbo].[HoSoXayDungVanBans] WHERE [MaHoSo] LIKE N'XDNQ-STEP-%' OR [MaHoSo] LIKE N'XDNQ-HDND-TRALAI-%'"
Add-Line $sql "UNION ALL SELECT N'THPL_STEP_BRANCH', COUNT(1) FROM [thpl].[KeHoachThiHanhPhapLuats] WHERE [MaKeHoach] LIKE N'THPL-STEP-%' OR [MaKeHoach] LIKE N'THPL-BOSUNG-%'"
Add-Line $sql "UNION ALL SELECT N'KSTHPL_STEP_BRANCH', COUNT(1) FROM [kspl].[CuocKhaoSats] WHERE [MaCuocKhaoSat] LIKE N'KSTHPL-STEP-%' OR [MaCuocKhaoSat] LIKE N'KSTHPL-BOSUNG-%';"

$fullSqlPath = Join-Path (Get-Location) $OutputSqlPath
Set-Content -LiteralPath $fullSqlPath -Value $sql.ToString() -Encoding UTF8
Write-Output "Created step-test SQL seed: $fullSqlPath"

if (-not $Execute) {
    Write-Output "Dry run only. Re-run with -Execute -Password <password> to apply the generated SQL."
    return
}

if ([string]::IsNullOrWhiteSpace($Password)) { throw "Password is required when -Execute is used." }

$connectionString = "Server=$Server;Database=$Database;User ID=$Username;Password=$Password;MultipleActiveResultSets=true;TrustServerCertificate=True;"
$connection = [System.Data.SqlClient.SqlConnection]::new($connectionString)
$connection.Open()
try {
    foreach ($batch in (Split-SqlBatches $sql.ToString())) {
        $command = $connection.CreateCommand()
        $command.CommandTimeout = 180
        $command.CommandText = $batch
        $reader = $command.ExecuteReader()
        try {
            while ($reader.Read()) {
                if ($reader.FieldCount -ge 2) {
                    Write-Output ("{0}: {1}" -f $reader.GetValue(0), $reader.GetValue(1))
                }
            }
        }
        finally {
            $reader.Close()
        }
    }
}
finally {
    $connection.Close()
}
