param(
    [string]$Server = "14.248.85.58",
    [string]$Database = "CSDLXayDungVanBan_MS",
    [string]$Username = "sa",
    [string]$Password,
    [switch]$Execute,
    [string]$OutputSqlPath = "tools/seed_sample_ho_so_quy_trinh.generated.sql"
)

$ErrorActionPreference = "Stop"

function New-StableGuid {
    param([string]$Key)

    $md5 = [System.Security.Cryptography.MD5]::Create()
    try {
        $bytes = [System.Text.Encoding]::UTF8.GetBytes($Key)
        return [Guid]::new($md5.ComputeHash($bytes))
    }
    finally {
        $md5.Dispose()
    }
}

function SqlString {
    param([AllowNull()][string]$Value)

    if ($null -eq $Value) {
        return "NULL"
    }

    return "N'$($Value.Replace("'", "''"))'"
}

function SqlGuid {
    param([Guid]$Value)
    return "'$($Value.ToString())'"
}

function SqlDate {
    param([datetime]$Value)
    return "'$($Value.ToString("yyyy-MM-ddTHH:mm:ss"))'"
}

function Add-Line {
    param([System.Text.StringBuilder]$Builder, [string]$Text = "")
    [void]$Builder.AppendLine($Text)
}

function Split-SqlBatches {
    param([string]$Text)

    $batches = [System.Collections.Generic.List[string]]::new()
    $current = [System.Text.StringBuilder]::new()

    foreach ($line in ($Text -split "\r?\n")) {
        if ($line.Trim().ToUpperInvariant() -eq "GO") {
            $batch = $current.ToString().Trim()
            if (-not [string]::IsNullOrWhiteSpace($batch)) {
                $batches.Add($batch)
            }

            $current.Clear() | Out-Null
            continue
        }

        $current.AppendLine($line) | Out-Null
    }

    $lastBatch = $current.ToString().Trim()
    if (-not [string]::IsNullOrWhiteSpace($lastBatch)) {
        $batches.Add($lastBatch)
    }

    return $batches
}

function New-SampleFile {
    param(
        [string]$Service,
        [string]$RelativeDirectory,
        [string]$FileName,
        [string]$Title,
        [string[]]$Lines
    )

    $serviceRoot = Join-Path (Get-Location) "BE/services/$Service"
    $directory = Join-Path $serviceRoot $RelativeDirectory
    New-Item -ItemType Directory -Path $directory -Force | Out-Null

    $fullPath = Join-Path $directory $FileName
    $content = @(
        $Title
        "GeneratedAt: 2026-10-11"
        "Purpose: Du lieu mau phuc vu kiem thu quy trinh nghiep vu."
        ""
    ) + $Lines

    Set-Content -LiteralPath $fullPath -Value $content -Encoding UTF8

    return [pscustomobject]@{
        RelativePath = (Join-Path $RelativeDirectory $FileName).Replace("\", "/")
        FullPath = $fullPath
        Size = (Get-Item -LiteralPath $fullPath).Length
    }
}

$actorId = [Guid]"00000000-0000-0000-0000-000000000001"
$systemUser = $actorId.ToString()
$donViSoanThaoId = [Guid]"00000000-0000-0000-0000-000000000101"
$donViPheDuyetId = [Guid]"00000000-0000-0000-0000-000000000102"
$canBoId = [Guid]"00000000-0000-0000-0000-000000000201"
$loaiVanBanId = [Guid]"00000000-0000-0000-0000-000000000301"
$linhVucId = [Guid]"00000000-0000-0000-0000-000000000401"

$workflows = @(
    @{
        Code = "DXDM_DKXD_QPPL"
        Name = "De xuat danh muc / Dang ky xay dung QPPL"
        Kind = "DangKy"
        Service = "DangKyXayDungVanBanService"
        Prefix = "DK-MAU"
        WorkflowId = [Guid]"33333333-3333-3333-3333-333333333301"
        FinalStepId = [Guid]"33333333-3333-3333-3333-333333333315"
        FinalStatusId = [Guid]"11111111-1111-1111-1111-111111111109"
        Steps = @(
            @{ Code = "LAP_HO_SO"; Name = "Lap ho so de nghi/dang ky"; StepId = [Guid]"33333333-3333-3333-3333-333333333311"; ActionId = [Guid]"22222222-2222-2222-2222-222222222201"; ActionCode = "TAO_MOI"; ActionName = "Tao moi ho so"; FileType = "DE_XUAT" },
            @{ Code = "TRINH_HO_SO"; Name = "Trinh ho so"; StepId = [Guid]"33333333-3333-3333-3333-333333333312"; ActionId = [Guid]"22222222-2222-2222-2222-222222222203"; ActionCode = "TRINH_PHE_DUYET"; ActionName = "Trinh phe duyet"; FileType = "TO_TRINH" },
            @{ Code = "PHE_DUYET"; Name = "Phe duyet ho so"; StepId = [Guid]"33333333-3333-3333-3333-333333333313"; ActionId = [Guid]"22222222-2222-2222-2222-222222222204"; ActionCode = "PHE_DUYET"; ActionName = "Phe duyet"; FileType = "PHIEU_PHE_DUYET" },
            @{ Code = "CAP_NHAT_KET_QUA"; Name = "Cap nhat ket qua phe duyet"; StepId = [Guid]"33333333-3333-3333-3333-333333333314"; ActionId = [Guid]"22222222-2222-2222-2222-222222222207"; ActionCode = "CAP_NHAT_KET_QUA"; ActionName = "Cap nhat ket qua"; FileType = "KET_QUA" },
            @{ Code = "HOAN_THANH"; Name = "Hoan thanh"; StepId = [Guid]"33333333-3333-3333-3333-333333333315"; ActionId = [Guid]"22222222-2222-2222-2222-222222222209"; ActionCode = "KHOI_TAO_QUY_TRINH_XAY_DUNG"; ActionName = "Khoi tao quy trinh xay dung"; FileType = "BIEN_BAN_HOAN_THANH" }
        )
    },
    @{
        Code = "XD_QD_UBND_TINH"
        Name = "Xay dung van ban QPPL - Quyet dinh UBND tinh"
        Kind = "XayDung"
        Service = "XayDungVanBanService"
        Prefix = "XDQD-MAU"
        WorkflowId = [Guid]"44444444-4444-4444-4444-444444444101"
        FinalStepId = [Guid]"44444444-4444-4444-4444-444444444116"
        FinalStatusId = [Guid]"55555555-5555-5555-5555-555555555207"
        Steps = @(
            @{ Code = "SOAN_THAO_LAY_Y_KIEN"; Name = "Soan thao, lay y kien"; StepId = [Guid]"44444444-4444-4444-4444-444444444111"; FileType = "DU_THAO"; BoType = 1 },
            @{ Code = "GUI_THAM_DINH"; Name = "Gui ho so tham dinh"; StepId = [Guid]"44444444-4444-4444-4444-444444444112"; FileType = "TO_TRINH_THAM_DINH"; BoType = 2 },
            @{ Code = "THAM_DINH"; Name = "Tham dinh van ban"; StepId = [Guid]"44444444-4444-4444-4444-444444444113"; FileType = "BAO_CAO_THAM_DINH"; BoType = 3 },
            @{ Code = "TRINH_UBND"; Name = "Trinh UBND tinh"; StepId = [Guid]"44444444-4444-4444-4444-444444444114"; FileType = "TO_TRINH_UBND"; BoType = 4 },
            @{ Code = "LAY_Y_KIEN_UBND"; Name = "Lay y kien thanh vien UBND"; StepId = [Guid]"44444444-4444-4444-4444-444444444115"; FileType = "TONG_HOP_Y_KIEN_UBND"; BoType = 5 },
            @{ Code = "BAN_HANH"; Name = "Ban hanh Quyet dinh"; StepId = [Guid]"44444444-4444-4444-4444-444444444116"; FileType = "QUYET_DINH_BAN_HANH"; BoType = 7 }
        )
    },
    @{
        Code = "XD_NQ_HDND_TINH"
        Name = "Xay dung van ban QPPL - Nghi quyet HDND tinh"
        Kind = "XayDung"
        Service = "XayDungVanBanService"
        Prefix = "XDNQ-MAU"
        WorkflowId = [Guid]"44444444-4444-4444-4444-444444444201"
        FinalStepId = [Guid]"44444444-4444-4444-4444-444444444217"
        FinalStatusId = [Guid]"55555555-5555-5555-5555-555555555308"
        Steps = @(
            @{ Code = "SOAN_THAO_LAY_Y_KIEN"; Name = "Soan thao, lay y kien"; StepId = [Guid]"44444444-4444-4444-4444-444444444211"; FileType = "DU_THAO"; BoType = 1 },
            @{ Code = "GUI_THAM_DINH"; Name = "Gui ho so tham dinh"; StepId = [Guid]"44444444-4444-4444-4444-444444444212"; FileType = "TO_TRINH_THAM_DINH"; BoType = 2 },
            @{ Code = "THAM_DINH"; Name = "Tham dinh van ban"; StepId = [Guid]"44444444-4444-4444-4444-444444444213"; FileType = "BAO_CAO_THAM_DINH"; BoType = 3 },
            @{ Code = "TRINH_UBND_CHO_Y_KIEN"; Name = "Trinh UBND cho y kien"; StepId = [Guid]"44444444-4444-4444-4444-444444444214"; FileType = "TO_TRINH_UBND"; BoType = 4 },
            @{ Code = "LAY_Y_KIEN_UBND"; Name = "Lay y kien thanh vien UBND"; StepId = [Guid]"44444444-4444-4444-4444-444444444215"; FileType = "TONG_HOP_Y_KIEN_UBND"; BoType = 5 },
            @{ Code = "TRINH_HDND_THAM_TRA"; Name = "Trinh HDND tham tra"; StepId = [Guid]"44444444-4444-4444-4444-444444444216"; FileType = "BAO_CAO_THAM_TRA"; BoType = 6 },
            @{ Code = "THONG_QUA_BAN_HANH"; Name = "Thong qua va ban hanh"; StepId = [Guid]"44444444-4444-4444-4444-444444444217"; FileType = "NGHI_QUYET_BAN_HANH"; BoType = 7 }
        )
    },
    @{
        Code = "TC_THI_HANH_PHAP_LUAT"
        Name = "To chuc thi hanh phap luat"
        Kind = "ThiHanh"
        Service = "ThiHanhPhapLuatService"
        Prefix = "THPL-MAU"
        WorkflowId = [Guid]"44444444-4444-4444-4444-444444444301"
        FinalStatusId = [Guid]"55555555-5555-5555-5555-555555555405"
        Steps = @(
            @{ Code = "TAO_KE_HOACH"; Name = "Tao ke hoach"; StepId = [Guid]"44444444-4444-4444-4444-444444444311"; FileType = "KE_HOACH" },
            @{ Code = "THUC_HIEN_NOI_DUNG"; Name = "Thuc hien noi dung"; StepId = [Guid]"44444444-4444-4444-4444-444444444312"; FileType = "BAO_CAO_TIEN_DO" },
            @{ Code = "DANH_GIA_KET_QUA"; Name = "Danh gia ket qua"; StepId = [Guid]"44444444-4444-4444-4444-444444444313"; FileType = "PHIEU_DANH_GIA" },
            @{ Code = "TONG_HOP_KET_QUA"; Name = "Tong hop ket qua"; StepId = [Guid]"44444444-4444-4444-4444-444444444314"; FileType = "BAO_CAO_TONG_HOP" }
        )
    },
    @{
        Code = "KS_THI_HANH_PHAP_LUAT"
        Name = "Khao sat tinh hinh thi hanh phap luat"
        Kind = "KhaoSat"
        Service = "KhaoSatThiHanhPhapLuatService"
        Prefix = "KSTHPL-MAU"
        WorkflowId = [Guid]"44444444-4444-4444-4444-444444444401"
        FinalStatusId = [Guid]"55555555-5555-5555-5555-555555555506"
        Steps = @(
            @{ Code = "TAO_CUOC_KHAO_SAT"; Name = "Tao cuoc khao sat"; StepId = [Guid]"44444444-4444-4444-4444-444444444411"; FileType = "MAU_PHIEU" },
            @{ Code = "THUC_HIEN_KHAO_SAT"; Name = "Thuc hien khao sat"; StepId = [Guid]"44444444-4444-4444-4444-444444444412"; FileType = "PHIEU_NOP" },
            @{ Code = "DANH_GIA_KET_QUA"; Name = "Danh gia ket qua"; StepId = [Guid]"44444444-4444-4444-4444-444444444413"; FileType = "BIEN_BAN_RA_SOAT" },
            @{ Code = "TONG_HOP_BAO_CAO"; Name = "Tong hop bao cao"; StepId = [Guid]"44444444-4444-4444-4444-444444444414"; FileType = "BAO_CAO_KHAO_SAT" }
        )
    }
)

$sampleFiles = @{}
foreach ($workflow in $workflows) {
    for ($i = 1; $i -le 5; $i++) {
        foreach ($step in $workflow.Steps) {
            $directory = "uploads/sample-data/$($workflow.Code.ToLowerInvariant())/$($workflow.Prefix.ToLowerInvariant())-$('{0:00}' -f $i)/$($step.Code.ToLowerInvariant())"
            $fileName = "$($step.FileType.ToLowerInvariant())-$('{0:00}' -f $i).txt"
            $file = New-SampleFile `
                -Service $workflow.Service `
                -RelativeDirectory $directory `
                -FileName $fileName `
                -Title "$($workflow.Name) - $($step.Name) - Ho so $i" `
                -Lines @(
                    "MaQuyTrinh: $($workflow.Code)",
                    "MaHoSo: $($workflow.Prefix)-$('{0:00}' -f $i)",
                    "Buoc: $($step.Code)",
                    "NoiDung: Tep dinh kem mau cho buoc $($step.Name)."
                )
            $sampleFiles["$($workflow.Code)|$i|$($step.Code)"] = $file
        }
    }
}

$sql = [System.Text.StringBuilder]::new()
Add-Line $sql "/*"
Add-Line $sql "    Du lieu mau ho so hoan chinh cho cac quy trinh nghiep vu."
Add-Line $sql "    Generated by tools/Seed-SampleHoSoQuyTrinh.ps1"
Add-Line $sql "    Safe to re-run: inserts are guarded by business codes."
Add-Line $sql "*/"
Add-Line $sql "SET NOCOUNT ON;"
Add-Line $sql "SET XACT_ABORT ON;"
Add-Line $sql ""
Add-Line $sql "BEGIN TRANSACTION;"
Add-Line $sql "DECLARE @Now datetime2 = SYSUTCDATETIME();"
Add-Line $sql "DECLARE @Actor uniqueidentifier = '$actorId';"
Add-Line $sql "DECLARE @DonViSoanThao uniqueidentifier = '$donViSoanThaoId';"
Add-Line $sql "DECLARE @DonViPheDuyet uniqueidentifier = '$donViPheDuyetId';"
Add-Line $sql "DECLARE @CanBo uniqueidentifier = '$canBoId';"
Add-Line $sql "DECLARE @LoaiVanBan uniqueidentifier = '$loaiVanBanId';"
Add-Line $sql "DECLARE @LinhVuc uniqueidentifier = '$linhVucId';"
Add-Line $sql "SELECT TOP (1) @DonViSoanThao = [Id] FROM [dm].[DanhMucDonVis] WHERE [Id] <> '00000000-0000-0000-0000-000000000000' ORDER BY [CreatedDate], [Id];"
Add-Line $sql "SELECT TOP (1) @DonViPheDuyet = [Id] FROM [dm].[DanhMucDonVis] WHERE [Id] <> '00000000-0000-0000-0000-000000000000' AND [Id] <> @DonViSoanThao ORDER BY [CreatedDate], [Id];"
Add-Line $sql "SET @DonViPheDuyet = COALESCE(@DonViPheDuyet, @DonViSoanThao);"
Add-Line $sql "SELECT TOP (1) @CanBo = [Id] FROM [dm].[DanhMucCanBos] ORDER BY [CreatedDate], [Id];"
Add-Line $sql "SELECT TOP (1) @LoaiVanBan = [Id] FROM [dm].[DanhMucVanBans] WHERE [TrangThai] = 1 ORDER BY [CreatedDate], [Id];"
Add-Line $sql "SELECT TOP (1) @LinhVuc = [Id] FROM [dm].[DanhMucLinhVucs] WHERE [TrangThai] = 1 ORDER BY [CreatedDate], [Id];"
Add-Line $sql ""

foreach ($workflow in $workflows) {
    for ($i = 1; $i -le 5; $i++) {
        $indexText = "{0:00}" -f $i
        $code = "$($workflow.Prefix)-$indexText"
        $baseDate = [datetime]"2026-01-01"
        $date = $baseDate.AddDays(($i - 1) * 12)

        if ($workflow.Kind -eq "DangKy") {
            $hoSoId = New-StableGuid "$($workflow.Code)-$i-ho-so"
            Add-Line $sql "-- $($workflow.Code) / $code"
            Add-Line $sql "IF NOT EXISTS (SELECT 1 FROM [dbo].[DangKyXayDungVanBans] WHERE [MaHoSo] = N'$code')"
            Add-Line $sql "BEGIN"
            Add-Line $sql "    INSERT INTO [dbo].[DangKyXayDungVanBans] ([Id],[MaHoSo],[TenHoSo],[TenVanBanDuKien],[LoaiVanBanId],[QuyTrinhSoanThaoId],[BuocHienTaiId],[TrangThaiHoSoId],[DonViSoanThaoId],[DonViPheDuyetId],[NamDangKy],[CanCuDeXuat],[SuCanThiet],[NoiDungChinhSach],[DuKienThoiGianTrinh],[DaKhoiTaoQuyTrinhXayDung],[QuyTrinhXayDungTiepTheoId],[NgayKhoiTaoQuyTrinhXayDung],[CreatedAt],[CreatedBy],[IsDeleted])"
            Add-Line $sql "    VALUES ($(SqlGuid $hoSoId),N'$code',N'Hồ sơ mẫu $indexText - $($workflow.Name)',N'Dự thảo văn bản mẫu $indexText',@LoaiVanBan,$(SqlGuid $workflow.WorkflowId),$(SqlGuid $workflow.FinalStepId),$(SqlGuid $workflow.FinalStatusId),@DonViSoanThao,@DonViPheDuyet,2026,N'Căn cứ pháp lý mẫu',N'Sự cần thiết ban hành văn bản mẫu',N'Nội dung chính sách mẫu phục vụ kiểm thử.',$(SqlDate $date.AddDays(30)),1,$(SqlGuid ([Guid]"44444444-4444-4444-4444-444444444101")),$(SqlDate $date.AddDays(45)),@Now,CONVERT(nvarchar(64),@Actor),0);"

            $previousStep = $null
            foreach ($step in $workflow.Steps) {
                $file = $sampleFiles["$($workflow.Code)|$i|$($step.Code)"]
                $fileId = New-StableGuid "$($workflow.Code)-$i-$($step.Code)-file"
                $historyId = New-StableGuid "$($workflow.Code)-$i-$($step.Code)-history"
                $previousStepSql = if ($previousStep) { SqlGuid $previousStep } else { "NULL" }
                Add-Line $sql "    INSERT INTO [dbo].[DangKyXayDungVanBanFiles] ([Id],[DangKyXayDungVanBanId],[LoaiFile],[TenFile],[DuongDanFile],[DungLuong],[MimeType],[MoTa],[CreatedAt],[CreatedBy],[IsDeleted])"
                Add-Line $sql "    VALUES ($(SqlGuid $fileId),$(SqlGuid $hoSoId),N'$($step.FileType)',N'$([System.IO.Path]::GetFileName($file.RelativePath))',N'$($file.RelativePath)',$($file.Size),N'text/plain',N'$($step.Name)',@Now,CONVERT(nvarchar(64),@Actor),0);"
                Add-Line $sql "    INSERT INTO [dbo].[DangKyXayDungVanBanLichSuXuLys] ([Id],[DangKyXayDungVanBanId],[TuBuocId],[DenBuocId],[HanhDongId],[MaHanhDongSnapshot],[TenHanhDongSnapshot],[TrangThaiSauId],[TenTrangThaiSauSnapshot],[NoiDungXuLy],[NguoiXuLyId],[TenNguoiXuLy],[DonViXuLyId],[TenDonViXuLy],[NgayXuLy],[CreatedAt],[CreatedBy],[IsDeleted])"
                Add-Line $sql "    VALUES ($(SqlGuid $historyId),$(SqlGuid $hoSoId),$previousStepSql,$(SqlGuid $step.StepId),$(SqlGuid $step.ActionId),N'$($step.ActionCode)',N'$($step.ActionName)',$(SqlGuid $workflow.FinalStatusId),N'Hoàn thành',N'Hoàn thành bước $($step.Name)',@Actor,N'Người dùng mẫu',@DonViSoanThao,N'Đơn vị mẫu',$(SqlDate $date),@Now,CONVERT(nvarchar(64),@Actor),0);"
                $previousStep = $step.StepId
                $date = $date.AddDays(2)
            }

            $ketQuaId = New-StableGuid "$($workflow.Code)-$i-ket-qua"
            $ketQuaFileId = New-StableGuid "$($workflow.Code)-$i-CAP_NHAT_KET_QUA-file"
            Add-Line $sql "    INSERT INTO [dbo].[DangKyXayDungVanBanKetQuaPheDuyets] ([Id],[DangKyXayDungVanBanId],[KetQua],[SoVanBan],[NgayVanBan],[CoQuanPheDuyetId],[TenCoQuanPheDuyet],[NguoiKy],[ChucVuNguoiKy],[NoiDungKetQua],[FileKetQuaId],[CreatedAt],[CreatedBy],[IsDeleted])"
            Add-Line $sql "    VALUES ($(SqlGuid $ketQuaId),$(SqlGuid $hoSoId),N'PHE_DUYET',N'$($i)/QĐ-UBND',$(SqlDate $date),@DonViPheDuyet,N'UBND tỉnh',N'Người ký mẫu',N'Chủ tịch',N'Phê duyệt hồ sơ đăng ký mẫu.',$(SqlGuid $ketQuaFileId),@Now,CONVERT(nvarchar(64),@Actor),0);"
            Add-Line $sql "END"
            Add-Line $sql ""
            continue
        }

        if ($workflow.Kind -eq "XayDung") {
            $hoSoId = New-StableGuid "$($workflow.Code)-$i-ho-so"
            Add-Line $sql "-- $($workflow.Code) / $code"
            Add-Line $sql "IF NOT EXISTS (SELECT 1 FROM [dbo].[HoSoXayDungVanBans] WHERE [MaHoSo] = N'$code')"
            Add-Line $sql "BEGIN"
            Add-Line $sql "    INSERT INTO [dbo].[HoSoXayDungVanBans] ([Id],[MaHoSo],[TenHoSo],[TenDuThaoVanBan],[DanhMucVanBanId],[QuyTrinhSoanThaoId],[BuocHienTaiId],[TrangThaiHoSoId],[DonViChuTriSoanThaoId],[NguoiPhuTrachId],[NamXayDung],[ThoiGianDuKienBatDau],[ThoiGianDuKienHoanThanh],[MoTa],[CreatedAt],[CreatedBy],[IsDeleted])"
            Add-Line $sql "    VALUES ($(SqlGuid $hoSoId),N'$code',N'Hồ sơ mẫu $indexText - $($workflow.Name)',N'Dự thảo văn bản QPPL mẫu $indexText',@LoaiVanBan,$(SqlGuid $workflow.WorkflowId),$(SqlGuid $workflow.FinalStepId),$(SqlGuid $workflow.FinalStatusId),@DonViSoanThao,@CanBo,2026,$(SqlDate $date),$(SqlDate $date.AddDays(60)),N'Hồ sơ mẫu hoàn chỉnh có đủ bộ hồ sơ nghiệp vụ và tài liệu từng bước.',@Now,CONVERT(nvarchar(64),@Actor),0);"

            $sourceBoId = $null
            foreach ($step in $workflow.Steps) {
                $boId = New-StableGuid "$($workflow.Code)-$i-$($step.Code)-bo"
                $fileId = New-StableGuid "$($workflow.Code)-$i-$($step.Code)-file"
                $linkId = New-StableGuid "$($workflow.Code)-$i-$($step.Code)-link"
                $historyId = New-StableGuid "$($workflow.Code)-$i-$($step.Code)-history"
                $file = $sampleFiles["$($workflow.Code)|$i|$($step.Code)"]
                $sourceBoSql = if ($sourceBoId) { SqlGuid $sourceBoId } else { "NULL" }
                $loaiTaiLieuId = New-StableGuid "loai-tai-lieu-$($step.FileType)"
                $boStatus = if ($step.Code -eq "BAN_HANH" -or $step.Code -eq "THONG_QUA_BAN_HANH") { 5 } else { 2 }
                $ngayHoanThanh = if ($boStatus -eq 5) { SqlDate $date.AddDays(1) } else { "NULL" }
                Add-Line $sql "    INSERT INTO [dbo].[BoHoSoNghiepVus] ([Id],[HoSoXayDungVanBanId],[BuocQuyTrinhId],[LoaiBoHoSo],[TrangThai],[LanXuLy],[BoHoSoNguonId],[NgayTao],[NgayGui],[NgayHoanThanh],[NguoiLapId],[DonViLapId],[NoiDungGhiChu],[CreatedAt],[CreatedBy],[IsDeleted])"
                Add-Line $sql "    VALUES ($(SqlGuid $boId),$(SqlGuid $hoSoId),$(SqlGuid $step.StepId),$($step.BoType),$boStatus,1,$sourceBoSql,$(SqlDate $date),$(SqlDate $date.AddHours(8)),$ngayHoanThanh,@Actor,@DonViSoanThao,N'Bộ hồ sơ mẫu: $($step.Name)',@Now,CONVERT(nvarchar(64),@Actor),0);"
                Add-Line $sql "    INSERT INTO [dbo].[HoSoXayDungVanBanFiles] ([Id],[HoSoXayDungVanBanId],[LoaiTaiLieuId],[TenTaiLieu],[PhienBan],[TenFile],[DuongDanFile],[MimeType],[DungLuong],[IsCurrent],[NguoiTaiLenId],[NgayTaiLen],[CreatedAt],[CreatedBy],[IsDeleted])"
                Add-Line $sql "    VALUES ($(SqlGuid $fileId),$(SqlGuid $hoSoId),$(SqlGuid $loaiTaiLieuId),N'$($step.Name)',1,N'$([System.IO.Path]::GetFileName($file.RelativePath))',N'$($file.RelativePath)',N'text/plain',$($file.Size),1,@Actor,$(SqlDate $date),@Now,CONVERT(nvarchar(64),@Actor),0);"
                Add-Line $sql "    INSERT INTO [dbo].[BoHoSoNghiepVuTaiLieus] ([Id],[BoHoSoNghiepVuId],[HoSoXayDungVanBanFileId],[LoaiTaiLieuId],[HinhThucThem],[BatBuoc],[DaKiemTra],[GhiChu],[ThuTu],[CreatedAt],[CreatedBy],[IsDeleted])"
                Add-Line $sql "    VALUES ($(SqlGuid $linkId),$(SqlGuid $boId),$(SqlGuid $fileId),$(SqlGuid $loaiTaiLieuId),N'TaoMoi',1,1,N'Tài liệu mẫu cho bước $($step.Name)',1,@Now,CONVERT(nvarchar(64),@Actor),0);"
                Add-Line $sql "    INSERT INTO [dbo].[HoSoXayDungVanBanLichSuXuLys] ([Id],[HoSoXayDungVanBanId],[BoHoSoNghiepVuId],[BuocQuyTrinhSauId],[TrangThaiSauId],[TenBuocSau],[ThuTuBuocSau],[HanhDong],[NoiDung],[NguoiXuLyId],[DonViXuLyId],[ThoiGianXuLy],[HoSoXayDungVanBanFileId],[BoHoSoNghiepVuTaiLieuId],[CreatedAt],[CreatedBy],[IsDeleted])"
                Add-Line $sql "    VALUES ($(SqlGuid $historyId),$(SqlGuid $hoSoId),$(SqlGuid $boId),$(SqlGuid $step.StepId),$(SqlGuid $workflow.FinalStatusId),N'$($step.Name)',1,N'HOAN_THANH_BUOC',N'Hoàn thành bước $($step.Name)',@Actor,@DonViSoanThao,$(SqlDate $date.AddHours(9)),$(SqlGuid $fileId),$(SqlGuid $linkId),@Now,CONVERT(nvarchar(64),@Actor),0);"

                $dateSql = SqlDate $date
                $date1Sql = SqlDate ($date.AddDays(1))
                $date2Sql = SqlDate ($date.AddDays(2))
                $date3Sql = SqlDate ($date.AddDays(3))
                $date4Sql = SqlDate ($date.AddDays(4))
                $date5Sql = SqlDate ($date.AddDays(5))
                $date6Sql = SqlDate ($date.AddDays(6))
                $date7Sql = SqlDate ($date.AddDays(7))
                $date10Sql = SqlDate ($date.AddDays(10))
                $boIdSql = SqlGuid $boId
                $fileIdSql = SqlGuid $fileId
                if ($step.BoType -eq 1) {
                    Add-Line $sql ("    INSERT INTO [dbo].[HoSoXayDungVanBanSoanThaos] ([BoHoSoNghiepVuId],[CanCuXayDung],[PhamViDieuChinh],[NoiDungChinhSach],[HinhThucLayYKien],[TuNgayLayYKien],[DenNgayLayYKien],[TongSoYKien],[NoiDungTongHopTiepThuGiaiTrinh]) VALUES ({0},N'Căn cứ mẫu',N'Phạm vi điều chỉnh mẫu',N'Chính sách mẫu',N'Lấy ý kiến bằng văn bản và hệ thống',{1},{2},12,N'Tổng hợp tiếp thu, giải trình mẫu');" -f $boIdSql, $dateSql, $date7Sql)
                }
                elseif ($step.BoType -eq 2) {
                    Add-Line $sql ("    INSERT INTO [dbo].[HoSoXayDungVanBanTrinhThamDinhs] ([BoHoSoNghiepVuId],[FileDuThaoId],[SoToTrinh],[NgayToTrinh],[NgayGuiThamDinh],[DonViNhanThamDinhId],[NoiDungDeNghiThamDinh],[HanDeNghiTraKetQua]) VALUES ({0},{1},N'{2}/TTr-STP',{3},{4},@DonViPheDuyet,N'Đề nghị thẩm định hồ sơ mẫu.',{5});" -f $boIdSql, $fileIdSql, $indexText, $dateSql, $date1Sql, $date10Sql)
                }
                elseif ($step.BoType -eq 3) {
                    Add-Line $sql ("    INSERT INTO [dbo].[HoSoXayDungVanBanThamDinhs] ([BoHoSoNghiepVuId],[NgayTiepNhan],[HinhThucThamDinh],[NgayThamDinh],[KetQuaThamDinh],[NoiDungKetLuan],[NgayGuiKetQua],[NguoiKetLuanId]) VALUES ({0},{1},N'Thẩm định hồ sơ',{2},N'DAT',N'Hồ sơ đủ điều kiện trình cấp có thẩm quyền.',{3},@CanBo);" -f $boIdSql, $dateSql, $date3Sql, $date4Sql)
                }
                elseif ($step.BoType -eq 4) {
                    Add-Line $sql ("    INSERT INTO [dbo].[HoSoXayDungVanBanTrinhPheDuyets] ([BoHoSoNghiepVuId],[CapTrinh],[MucDichTrinh],[SoToTrinh],[NgayToTrinh],[NgayTrinh],[NoiDungTrinh],[DonViDongGuiId],[TinhTrangXuLyBenNgoaiHeThong],[NgayCapNhatTinhTrang]) VALUES ({0},N'UBND tỉnh',N'Trình xem xét hồ sơ mẫu',N'{1}/TTr-UBND',{2},{3},N'Nội dung trình mẫu.',@DonViPheDuyet,N'Đã trình',{4});" -f $boIdSql, $indexText, $dateSql, $date1Sql, $date2Sql)
                }
                elseif ($step.BoType -eq 5) {
                    Add-Line $sql ("    INSERT INTO [dbo].[HoSoXayDungVanBanYKienUbnds] ([BoHoSoNghiepVuId],[NgayNhanYKien],[TongSoThanhVienDuocLayYKien],[SoDongY],[SoKhongDongY],[SoYKienKhac],[KetLuanTongHop],[NoiDungTongHop],[NoiDungGiaiTrinh],[CanBoSungHoSo]) VALUES ({0},{1},15,15,0,0,N'Đồng ý',N'Tổng hợp ý kiến mẫu.',N'Không có nội dung phải giải trình thêm.',0);" -f $boIdSql, $dateSql)
                }
                elseif ($step.BoType -eq 6) {
                    Add-Line $sql ("    INSERT INTO [dbo].[HoSoXayDungVanBanThamTraHdnds] ([BoHoSoNghiepVuId],[BanHdndThamTraId],[NgayTrinhThamTra],[NgayNhanKetQuaThamTra],[KetQuaThamTra],[NoiDungKienNghi],[NoiDungTiepThuGiaiTrinh],[NgayNhanYKienThaoLuan],[NoiDungTongHopYKienThaoLuan]) VALUES ({0},@DonViPheDuyet,{1},{2},N'DAT',N'Kiến nghị mẫu',N'Tiếp thu đầy đủ.',{3},N'Tổng hợp ý kiến thảo luận mẫu.');" -f $boIdSql, $dateSql, $date5Sql, $date6Sql)
                }
                elseif ($step.BoType -eq 7) {
                    Add-Line $sql ("    INSERT INTO [dbo].[HoSoXayDungVanBanKetQuaBanHanhs] ([BoHoSoNghiepVuId],[KetQua],[SoVanBan],[NgayBanHanh],[CoQuanBanHanhId],[NguoiKyId],[ChucVuNguoiKy],[NgayCoHieuLuc],[NoiDungKetQua]) VALUES ({0},N'BAN_HANH',N'{1}/2026/QPPL',{2},@DonViPheDuyet,@CanBo,N'Chủ tịch',{3},N'Văn bản mẫu đã ban hành.');" -f $boIdSql, $indexText, $dateSql, $date10Sql)
                }

                $sourceBoId = $boId
                $date = $date.AddDays(5)
            }

            Add-Line $sql "END"
            Add-Line $sql ""
            continue
        }

        if ($workflow.Kind -eq "ThiHanh") {
            $keHoachId = New-StableGuid "$($workflow.Code)-$i-ke-hoach"
            $noiDungId = New-StableGuid "$($workflow.Code)-$i-noi-dung"
            $phanCongId = New-StableGuid "$($workflow.Code)-$i-phan-cong"
            $baoCaoId = New-StableGuid "$($workflow.Code)-$i-bao-cao"
            $tongHopId = New-StableGuid "$($workflow.Code)-$i-tong-hop"
            Add-Line $sql "-- $($workflow.Code) / $code"
            Add-Line $sql "IF NOT EXISTS (SELECT 1 FROM [thpl].[KeHoachThiHanhPhapLuats] WHERE [MaKeHoach] = N'$code')"
            Add-Line $sql "BEGIN"
            Add-Line $sql "    INSERT INTO [thpl].[KeHoachThiHanhPhapLuats] ([Id],[MaKeHoach],[TenKeHoach],[Nam],[TuNgay],[DenNgay],[DonViChuTriId],[NguoiPhuTrachId],[TrangThaiId],[PhamVi],[MucTieu],[CreatedAt],[CreatedBy],[IsDeleted]) VALUES ($(SqlGuid $keHoachId),N'$code',N'Kế hoạch mẫu $indexText - $($workflow.Name)',2026,'2026-01-01','2026-12-31',@DonViSoanThao,@CanBo,$(SqlGuid $workflow.FinalStatusId),N'Phạm vi mẫu',N'Mục tiêu mẫu',@Now,CONVERT(nvarchar(64),@Actor),0);"
            Add-Line $sql "    INSERT INTO [thpl].[NoiDungKeHoachs] ([Id],[KeHoachId],[MaNoiDung],[TenNoiDung],[NoiDung],[ChiTieu],[DonViTinh],[HanHoanThanh],[ThuTu],[TrangThaiId],[TyLeHoanThanh],[CreatedAt],[CreatedBy],[IsDeleted]) VALUES ($(SqlGuid $noiDungId),$(SqlGuid $keHoachId),N'$code-ND01',N'Nội dung thi hành mẫu',N'Nội dung công việc mẫu.',100,N'%', '2026-11-30',1,$(SqlGuid $workflow.FinalStatusId),100,@Now,CONVERT(nvarchar(64),@Actor),0);"
            Add-Line $sql "    INSERT INTO [thpl].[PhanCongThiHanhs] ([Id],[NoiDungKeHoachId],[DonViDuocGiaoId],[CanBoDuocGiaoId],[VaiTro],[HanThucHien],[MucDoUuTien],[CreatedAt],[CreatedBy],[IsDeleted]) VALUES ($(SqlGuid $phanCongId),$(SqlGuid $noiDungId),@DonViPheDuyet,@CanBo,N'Phối hợp','2026-10-30',2,@Now,CONVERT(nvarchar(64),@Actor),0);"
            Add-Line $sql "    INSERT INTO [thpl].[BaoCaoTienDoThiHanhs] ([Id],[NoiDungKeHoachId],[PhanCongThiHanhId],[KyBaoCao],[TyLeHoanThanh],[KetQua],[KhoKhan],[KienNghi],[TrangThaiId],[NgayBaoCao],[CreatedAt],[CreatedBy],[IsDeleted]) VALUES ($(SqlGuid $baoCaoId),$(SqlGuid $noiDungId),$(SqlGuid $phanCongId),N'Quý IV/2026',100,N'Hoàn thành nội dung mẫu.',N'Không',N'Không',$(SqlGuid $workflow.FinalStatusId),$(SqlDate $date),@Now,CONVERT(nvarchar(64),@Actor),0);"
            Add-Line $sql "    INSERT INTO [thpl].[BaoCaoTongHopThiHanhs] ([Id],[MaBaoCao],[KeHoachId],[KyBaoCao],[TuNgay],[DenNgay],[TrangThaiId],[SoLieuTongHopJson],[NgayChot],[NguoiChotId],[CreatedAt],[CreatedBy],[IsDeleted]) VALUES ($(SqlGuid $tongHopId),N'$code-BC',$(SqlGuid $keHoachId),N'Năm 2026','2026-01-01','2026-12-31',$(SqlGuid $workflow.FinalStatusId),N'{""tyLeHoanThanh"":100}',$(SqlDate $date.AddDays(10)),@CanBo,@Now,CONVERT(nvarchar(64),@Actor),0);"
            foreach ($step in $workflow.Steps) {
                $file = $sampleFiles["$($workflow.Code)|$i|$($step.Code)"]
                $tepId = New-StableGuid "$($workflow.Code)-$i-$($step.Code)-tep"
                $baoCaoSql = if ($step.Code -eq "THUC_HIEN_NOI_DUNG") { SqlGuid $baoCaoId } else { "NULL" }
                Add-Line $sql "    INSERT INTO [thpl].[TepDinhKemThiHanhs] ([Id],[KeHoachId],[NoiDungKeHoachId],[BaoCaoTienDoThiHanhId],[LoaiTaiLieu],[TenTep],[DuongDan],[PhienBan],[IsCurrent],[CreatedAt],[CreatedBy],[IsDeleted]) VALUES ($(SqlGuid $tepId),$(SqlGuid $keHoachId),$(SqlGuid $noiDungId),$baoCaoSql,N'$($step.FileType)',N'$([System.IO.Path]::GetFileName($file.RelativePath))',N'$($file.RelativePath)',1,1,@Now,CONVERT(nvarchar(64),@Actor),0);"
            }
            Add-Line $sql "END"
            Add-Line $sql ""
            continue
        }

        if ($workflow.Kind -eq "KhaoSat") {
            $cuocId = New-StableGuid "$($workflow.Code)-$i-cuoc"
            $nhomId = New-StableGuid "$($workflow.Code)-$i-nhom"
            $mauId = New-StableGuid "$($workflow.Code)-$i-mau"
            $doiTuongId = New-StableGuid "$($workflow.Code)-$i-doi-tuong"
            $phieuId = New-StableGuid "$($workflow.Code)-$i-phieu"
            $baoCaoId = New-StableGuid "$($workflow.Code)-$i-bao-cao"
            $mauFile = $sampleFiles["$($workflow.Code)|$i|TAO_CUOC_KHAO_SAT"]
            $phieuFile = $sampleFiles["$($workflow.Code)|$i|THUC_HIEN_KHAO_SAT"]
            $baoCaoFile = $sampleFiles["$($workflow.Code)|$i|TONG_HOP_BAO_CAO"]
            Add-Line $sql "-- $($workflow.Code) / $code"
            Add-Line $sql "IF NOT EXISTS (SELECT 1 FROM [kspl].[CuocKhaoSats] WHERE [MaCuocKhaoSat] = N'$code')"
            Add-Line $sql "BEGIN"
            Add-Line $sql "    INSERT INTO [kspl].[CuocKhaoSats] ([Id],[MaCuocKhaoSat],[TenCuocKhaoSat],[MucDich],[PhamVi],[LinhVucId],[DonViChuTriId],[TuNgay],[DenNgay],[TrangThaiId],[CreatedAt],[CreatedBy],[IsDeleted]) VALUES ($(SqlGuid $cuocId),N'$code',N'Cuộc khảo sát mẫu $indexText - $($workflow.Name)',N'Mục đích khảo sát mẫu',N'Phạm vi khảo sát mẫu',@LinhVuc,@DonViSoanThao,'2026-01-01','2026-12-31',$(SqlGuid $workflow.FinalStatusId),@Now,CONVERT(nvarchar(64),@Actor),0);"
            Add-Line $sql "    INSERT INTO [kspl].[NhomDoiTuongKhaoSats] ([Id],[CuocKhaoSatId],[MaNhom],[TenNhom],[ThuTu],[CreatedAt],[CreatedBy],[IsDeleted]) VALUES ($(SqlGuid $nhomId),$(SqlGuid $cuocId),N'NHOM-01',N'Nhóm đối tượng mẫu',1,@Now,CONVERT(nvarchar(64),@Actor),0);"
            Add-Line $sql "    INSERT INTO [kspl].[MauPhieuKhaoSats] ([Id],[CuocKhaoSatId],[NhomDoiTuongKhaoSatId],[PhienBan],[TenFile],[DuongDanFile],[MaHash],[DaPhatHanh],[TrangThaiMauPhieu],[NgayHieuLuc],[NgayHetHieuLuc],[CreatedAt],[CreatedBy],[IsDeleted]) VALUES ($(SqlGuid $mauId),$(SqlGuid $cuocId),$(SqlGuid $nhomId),1,N'$([System.IO.Path]::GetFileName($mauFile.RelativePath))',N'$($mauFile.RelativePath)',N'$((New-StableGuid "$code-mau-hash").ToString("N"))',1,N'DANG_SU_DUNG',$(SqlDate $date),$(SqlDate $date.AddMonths(6)),@Now,CONVERT(nvarchar(64),@Actor),0);"
            Add-Line $sql "    INSERT INTO [kspl].[DoiTuongKhaoSats] ([Id],[CuocKhaoSatId],[NhomDoiTuongKhaoSatId],[MauPhieuKhaoSatId],[DonViId],[CanBoId],[HanNop],[TrangThaiId],[CreatedAt],[CreatedBy],[IsDeleted]) VALUES ($(SqlGuid $doiTuongId),$(SqlGuid $cuocId),$(SqlGuid $nhomId),$(SqlGuid $mauId),@DonViPheDuyet,@CanBo,'2026-10-30',$(SqlGuid $workflow.FinalStatusId),@Now,CONVERT(nvarchar(64),@Actor),0);"
            Add-Line $sql "    INSERT INTO [kspl].[PhieuNopKhaoSats] ([Id],[CuocKhaoSatId],[NhomDoiTuongKhaoSatId],[DoiTuongKhaoSatId],[MauPhieuKhaoSatId],[TenFile],[DuongDanFile],[MaHash],[TrangThaiId],[NgayImport],[TenPhanMem],[PhienBanPhanMem],[GhiChuNguonDuLieu],[CreatedAt],[CreatedBy],[IsDeleted]) VALUES ($(SqlGuid $phieuId),$(SqlGuid $cuocId),$(SqlGuid $nhomId),$(SqlGuid $doiTuongId),$(SqlGuid $mauId),N'$([System.IO.Path]::GetFileName($phieuFile.RelativePath))',N'$($phieuFile.RelativePath)',N'$((New-StableGuid "$code-phieu-hash").ToString("N"))',$(SqlGuid $workflow.FinalStatusId),$(SqlDate $date),N'Seed mẫu',N'1.0',N'Dữ liệu mẫu đã hoàn thành.',@Now,CONVERT(nvarchar(64),@Actor),0);"
            Add-Line $sql "    INSERT INTO [kspl].[BaoCaoKhaoSats] ([Id],[CuocKhaoSatId],[TenBaoCao],[SoKyHieu],[NgayBaoCao],[TrangThai],[NgayChot],[UuDiem],[HanChe],[KienNghi],[DuongDanFileXuat],[TenFileXuat],[NgayXuat],[CreatedAt],[CreatedBy],[IsDeleted]) VALUES ($(SqlGuid $baoCaoId),$(SqlGuid $cuocId),N'Báo cáo khảo sát mẫu $indexText',N'$indexText/BC-KS','2026-12-15',N'DA_CHOT',$(SqlDate $date.AddDays(15)),N'Ưu điểm mẫu',N'Hạn chế mẫu',N'Kiến nghị mẫu',N'$($baoCaoFile.RelativePath)',N'$([System.IO.Path]::GetFileName($baoCaoFile.RelativePath))',$(SqlDate $date.AddDays(15)),@Now,CONVERT(nvarchar(64),@Actor),0);"
            foreach ($step in $workflow.Steps) {
                $historyId = New-StableGuid "$($workflow.Code)-$i-$($step.Code)-history"
                $doiTuongSql = if ($step.Code -eq "THUC_HIEN_KHAO_SAT") { SqlGuid $doiTuongId } else { "NULL" }
                $phieuSql = if ($step.Code -eq "THUC_HIEN_KHAO_SAT") { SqlGuid $phieuId } else { "NULL" }
                Add-Line $sql "    INSERT INTO [kspl].[LichSuXuLyKhaoSats] ([Id],[CuocKhaoSatId],[DoiTuongKhaoSatId],[PhieuNopKhaoSatId],[HanhDong],[NoiDung],[ThoiGianXuLy],[NguoiXuLyId],[DonViXuLyId]) VALUES ($(SqlGuid $historyId),$(SqlGuid $cuocId),$doiTuongSql,$phieuSql,N'$($step.Code)',N'Hoàn thành bước $($step.Name)',$(SqlDate $date),@Actor,@DonViSoanThao);"
                $date = $date.AddDays(2)
            }
            Add-Line $sql "END"
            Add-Line $sql ""
            continue
        }
    }
}

Add-Line $sql "COMMIT TRANSACTION;"
Add-Line $sql ""
Add-Line $sql "SELECT N'DANG_KY_XAY_DUNG_VAN_BAN' AS [Nhom], COUNT(1) AS [SoHoSoMau] FROM [dbo].[DangKyXayDungVanBans] WHERE [MaHoSo] LIKE N'DK-MAU-%'"
Add-Line $sql "UNION ALL SELECT N'XD_QD_UBND_TINH', COUNT(1) FROM [dbo].[HoSoXayDungVanBans] WHERE [MaHoSo] LIKE N'XDQD-MAU-%'"
Add-Line $sql "UNION ALL SELECT N'XD_NQ_HDND_TINH', COUNT(1) FROM [dbo].[HoSoXayDungVanBans] WHERE [MaHoSo] LIKE N'XDNQ-MAU-%'"
Add-Line $sql "UNION ALL SELECT N'TC_THI_HANH_PHAP_LUAT', COUNT(1) FROM [thpl].[KeHoachThiHanhPhapLuats] WHERE [MaKeHoach] LIKE N'THPL-MAU-%'"
Add-Line $sql "UNION ALL SELECT N'KS_THI_HANH_PHAP_LUAT', COUNT(1) FROM [kspl].[CuocKhaoSats] WHERE [MaCuocKhaoSat] LIKE N'KSTHPL-MAU-%';"

$fullSqlPath = Join-Path (Get-Location) $OutputSqlPath
New-Item -ItemType Directory -Path (Split-Path -Parent $fullSqlPath) -Force | Out-Null
Set-Content -LiteralPath $fullSqlPath -Value $sql.ToString() -Encoding UTF8

Write-Output "Created sample attachments: $($sampleFiles.Count)"
Write-Output "Created SQL seed: $fullSqlPath"

if (-not $Execute) {
    Write-Output "Dry run only. Re-run with -Execute -Password <password> to apply the generated SQL to SQL Server."
    return
}

if ([string]::IsNullOrWhiteSpace($Password)) {
    throw "Password is required when -Execute is used."
}

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

