param(
    [string]$Server = "14.248.85.58",
    [string]$Database = "CSDLXayDungVanBan_MS",
    [string]$Username = "sa",
    [string]$Password,
    [switch]$Execute,
    [string]$CqqlTemplatePath = "C:\Users\vuhuo\Downloads\PHIẾU KHÁO SÁT CQQL THUC PHAM CHỨC NĂNG.docx",
    [string]$NguoiDanTemplatePath = "C:\Users\vuhuo\Downloads\PHIẾU KHẢO SÁT ND THỰC PHẨM CHỨC NĂNG.docx",
    [string]$OutputSqlPath = "tools/seed_khao_sat_thuc_pham_chuc_nang.generated.sql"
)

$ErrorActionPreference = "Stop"

Add-Type -AssemblyName System.IO.Compression

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
    if ($null -eq $Value) { return "NULL" }
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

function SqlDecimal {
    param([decimal]$Value)
    return $Value.ToString([Globalization.CultureInfo]::InvariantCulture)
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
            if (-not [string]::IsNullOrWhiteSpace($batch)) { $batches.Add($batch) }
            $current.Clear() | Out-Null
            continue
        }

        $current.AppendLine($line) | Out-Null
    }

    $lastBatch = $current.ToString().Trim()
    if (-not [string]::IsNullOrWhiteSpace($lastBatch)) { $batches.Add($lastBatch) }
    return $batches
}

function Normalize-Text {
    param([AllowNull()][string]$Value)

    if ($null -eq $Value) { return "" }
    $decomposed = $Value.Trim().ToLowerInvariant().Normalize([Text.NormalizationForm]::FormD)
    $builder = [Text.StringBuilder]::new()
    foreach ($ch in $decomposed.ToCharArray()) {
        $category = [Globalization.CharUnicodeInfo]::GetUnicodeCategory($ch)
        if ($category -ne [Globalization.UnicodeCategory]::NonSpacingMark -and [char]::IsLetterOrDigit($ch)) {
            [void]$builder.Append($ch)
        }
    }

    return $builder.ToString()
}

function Open-ZipShared {
    param([string]$Path)

    $stream = [IO.FileStream]::new($Path, [IO.FileMode]::Open, [IO.FileAccess]::Read, [IO.FileShare]::ReadWrite)
    return [IO.Compression.ZipArchive]::new($stream, [IO.Compression.ZipArchiveMode]::Read, $false)
}

function Get-DocxParagraphs {
    param([string]$Path)

    $archive = Open-ZipShared $Path
    try {
        $entry = $archive.GetEntry("word/document.xml")
        if ($null -eq $entry) { throw "File Word không có word/document.xml: $Path" }

        $stream = $entry.Open()
        try {
            [xml]$xml = [IO.StreamReader]::new($stream).ReadToEnd()
        }
        finally {
            $stream.Dispose()
        }

        $namespaceManager = [Xml.XmlNamespaceManager]::new($xml.NameTable)
        $namespaceManager.AddNamespace("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main")

        return @($xml.SelectNodes("//w:p", $namespaceManager) | ForEach-Object {
            (($_.SelectNodes(".//w:t", $namespaceManager) | ForEach-Object { $_."#text" }) -join "").Trim()
        } | Where-Object { -not [string]::IsNullOrWhiteSpace($_) })
    }
    finally {
        $archive.Dispose()
    }
}

function Add-QuestionIfMissing {
    param(
        [System.Collections.Generic.List[object]]$Rows,
        [AllowNull()][string]$Code,
        [AllowNull()][string]$Content,
        [int]$Order
    )

    if ([string]::IsNullOrWhiteSpace($Code)) { return }
    if ($Rows | Where-Object { $_.MaCauHoi -eq $Code } | Select-Object -First 1) { return }

    $normalized = Normalize-Text $Content
    $freeText = $normalized.Contains("kiennghi") -or $normalized.Contains("ykien") -or $normalized.Contains("dexuat")
    $Rows.Add([pscustomobject]@{
        RowNumber = $Order
        MaCauHoi = $Code
        NoiDung = $Content.Trim()
        LoaiCauHoi = if ($freeText) { "VAN_BAN" } else { "LUA_CHON" }
        MaDapAn = ""
        DapAn = ""
        BatBuoc = $false
        ChoPhepNhieuLuaChon = $normalized.Contains("cothechonmothoacnhieu")
        MauSoTyLe = "PHIEU_HOP_LE"
        CoYKienTuDo = $freeText
    })
}

function Parse-SurveyDocx {
    param(
        [string]$Path,
        [string]$GroupCode
    )

    $rows = [System.Collections.Generic.List[object]]::new()
    $questionPattern = [Text.RegularExpressions.Regex]::new("^\s*(?:câu\s*hỏi|câu)\s*(\d+)\s*[:.\-]?\s*(.+)$", [Text.RegularExpressions.RegexOptions]::IgnoreCase)
    $optionPattern = [Text.RegularExpressions.Regex]::new("^\s*(?:[☐□\-]\s*)?([a-zđ])\s*[\).:\-]\s*(.+)$", [Text.RegularExpressions.RegexOptions]::IgnoreCase)

    $code = $null
    $content = ""
    $order = 0
    foreach ($paragraph in (Get-DocxParagraphs $Path)) {
        $question = $questionPattern.Match($paragraph)
        if ($question.Success) {
            Add-QuestionIfMissing -Rows $rows -Code $code -Content $content -Order $order
            $code = "Q$($question.Groups[1].Value)"
            $content = $question.Groups[2].Value.Trim()
            $order++
            continue
        }

        $option = $optionPattern.Match($paragraph)
        if ($code -and $option.Success) {
            $answerCode = $option.Groups[1].Value.ToUpperInvariant()
            if ($answerCode -eq "Đ") { $answerCode = "E" }
            $answerText = ($option.Groups[2].Value -replace "☐", "").Trim()
            if ([string]::IsNullOrWhiteSpace($answerText)) { continue }

            $normalizedContent = Normalize-Text $content
            $rows.Add([pscustomobject]@{
                RowNumber = $order
                MaCauHoi = $code
                NoiDung = $content
                LoaiCauHoi = "LUA_CHON"
                MaDapAn = $answerCode
                DapAn = $answerText
                BatBuoc = $false
                ChoPhepNhieuLuaChon = $normalizedContent.Contains("cothechonmothoacnhieu")
                MauSoTyLe = "PHIEU_HOP_LE"
                CoYKienTuDo = $normalizedContent.Contains("kiennghi") -or $normalizedContent.Contains("ykien") -or $normalizedContent.Contains("dexuat")
            })
        }
    }

    Add-QuestionIfMissing -Rows $rows -Code $code -Content $content -Order $order

    if ($rows.Count -eq 0) {
        throw "Không nhận diện được câu hỏi trong file $Path."
    }

    return [pscustomobject]@{
        GroupCode = $GroupCode
        SourcePath = $Path
        Rows = $rows
    }
}

function Copy-SharedFile {
    param([string]$Source, [string]$Destination)

    New-Item -ItemType Directory -Path (Split-Path -Parent $Destination) -Force | Out-Null
    $inputStream = [IO.FileStream]::new($Source, [IO.FileMode]::Open, [IO.FileAccess]::Read, [IO.FileShare]::ReadWrite)
    try {
        $outputStream = [IO.FileStream]::new($Destination, [IO.FileMode]::Create, [IO.FileAccess]::Write, [IO.FileShare]::None)
        try {
            $inputStream.CopyTo($outputStream)
        }
        finally {
            $outputStream.Dispose()
        }
    }
    finally {
        $inputStream.Dispose()
    }
}

function New-SampleTextFile {
    param(
        [string]$RelativeDirectory,
        [string]$FileName,
        [string[]]$Lines
    )

    $serviceRoot = Join-Path (Get-Location) "BE/services/KhaoSatThiHanhPhapLuatService"
    $directory = Join-Path $serviceRoot $RelativeDirectory
    New-Item -ItemType Directory -Path $directory -Force | Out-Null
    $fullPath = Join-Path $directory $FileName
    Set-Content -LiteralPath $fullPath -Value $Lines -Encoding UTF8
    return [pscustomobject]@{
        RelativePath = (Join-Path $RelativeDirectory $FileName).Replace("\", "/")
        FullPath = $fullPath
        Size = (Get-Item -LiteralPath $fullPath).Length
    }
}

function New-SampleDocxCopy {
    param(
        [string]$SourcePath,
        [string]$RelativeDirectory,
        [string]$FileName
    )

    $serviceRoot = Join-Path (Get-Location) "BE/services/KhaoSatThiHanhPhapLuatService"
    $directory = Join-Path $serviceRoot $RelativeDirectory
    $fullPath = Join-Path $directory $FileName
    Copy-SharedFile -Source $SourcePath -Destination $fullPath
    return [pscustomobject]@{
        RelativePath = (Join-Path $RelativeDirectory $FileName).Replace("\", "/")
        FullPath = $fullPath
        Size = (Get-Item -LiteralPath $fullPath).Length
    }
}

function Get-AnswerCounts {
    param(
        [int]$Total,
        [int]$OptionCount,
        [bool]$MultiSelect,
        [int]$QuestionIndex
    )

    $counts = @()
    if ($OptionCount -le 0) { return $counts }

    if ($MultiSelect) {
        for ($i = 0; $i -lt $OptionCount; $i++) {
            $raw = [Math]::Round($Total * (0.28 + (($QuestionIndex + $i) % 4) * 0.12))
            $counts += [Math]::Max(1, [int]$raw)
        }

        return $counts
    }

    $weights = @(5, 4, 2, 1, 1)
    $sumWeights = 0
    for ($i = 0; $i -lt $OptionCount; $i++) {
        $sumWeights += $weights[$i % $weights.Count]
    }

    $remaining = $Total
    for ($i = 0; $i -lt $OptionCount; $i++) {
        if ($i -eq ($OptionCount - 1)) {
            $counts += $remaining
            break
        }

        $count = [Math]::Floor($Total * $weights[$i % $weights.Count] / $sumWeights)
        $count = [Math]::Max(0, [int]$count)
        $counts += $count
        $remaining -= $count
    }

    return $counts
}

if (-not (Test-Path -LiteralPath $CqqlTemplatePath)) { throw "Không tìm thấy file mẫu CQQL: $CqqlTemplatePath" }
if (-not (Test-Path -LiteralPath $NguoiDanTemplatePath)) { throw "Không tìm thấy file mẫu người dân: $NguoiDanTemplatePath" }

$forms = @(
    @{
        GroupCode = "COQUAN"
        GroupName = "Cơ quan quản lý nhà nước"
        SourcePath = $CqqlTemplatePath
        TemplateFileName = "mau-phieu-cqql-thuc-pham-chuc-nang.docx"
        SubmissionFileName = "phieu-nop-cqql-thuc-pham-chuc-nang.txt"
        Total = 18
        UnitVariable = "@DonViCqql"
    },
    @{
        GroupCode = "NGUOIDAN"
        GroupName = "Người dân"
        SourcePath = $NguoiDanTemplatePath
        TemplateFileName = "mau-phieu-nguoi-dan-thuc-pham-chuc-nang.docx"
        SubmissionFileName = "phieu-nop-nguoi-dan-thuc-pham-chuc-nang.txt"
        Total = 25
        UnitVariable = "@DonViNguoiDan"
    }
)

$relativeBase = "uploads/sample-data/khao-sat-thuc-pham-chuc-nang"
foreach ($form in $forms) {
    $parsed = Parse-SurveyDocx -Path $form.SourcePath -GroupCode $form.GroupCode
    $form.ParsedRows = $parsed.Rows
    $form.TemplateFile = New-SampleDocxCopy -SourcePath $form.SourcePath -RelativeDirectory "$relativeBase/mau-phieu" -FileName $form.TemplateFileName

    $submissionLines = @(
        "Phiếu nộp khảo sát đã hoàn thành",
        "Nhóm đối tượng: $($form.GroupName)",
        "Tổng số phiếu hợp lệ: $($form.Total)",
        "Nguồn: dữ liệu seed tự tạo từ mẫu Word",
        ""
    )

    foreach ($questionGroup in ($form.ParsedRows | Group-Object MaCauHoi | Sort-Object { [int]($_.Name -replace "\D", "") })) {
        $question = $questionGroup.Group[0]
        $submissionLines += "$($question.MaCauHoi). $($question.NoiDung)"
        if ([string]::IsNullOrWhiteSpace($question.MaDapAn)) {
            $submissionLines += "  Ý kiến: Đề nghị tiếp tục rà soát quy định, tăng cường tuyên truyền và công khai thông tin quản lý thực phẩm chức năng."
        }
        else {
            $counts = Get-AnswerCounts -Total $form.Total -OptionCount $questionGroup.Group.Count -MultiSelect $question.ChoPhepNhieuLuaChon -QuestionIndex ([int]($question.MaCauHoi -replace "\D", ""))
            for ($i = 0; $i -lt $questionGroup.Group.Count; $i++) {
                $option = $questionGroup.Group[$i]
                $submissionLines += "  $($option.MaDapAn)) $($option.DapAn): $($counts[$i])/$($form.Total)"
            }
        }
    }

    $form.SubmissionFile = New-SampleTextFile -RelativeDirectory "$relativeBase/phieu-nop" -FileName $form.SubmissionFileName -Lines $submissionLines
}

$reportFile = New-SampleTextFile -RelativeDirectory "$relativeBase/bao-cao" -FileName "bao-cao-khao-sat-thuc-pham-chuc-nang.txt" -Lines @(
    "Báo cáo tổng hợp khảo sát thực phẩm chức năng",
    "Trạng thái: Đã chốt",
    "Dữ liệu gồm 2 nhóm đối tượng: cơ quan quản lý nhà nước và người dân.",
    "Câu trả lời được tự tạo theo từng phương án trong 2 mẫu phiếu Word."
)

$actorId = [Guid]"00000000-0000-0000-0000-000000000001"
$surveyId = New-StableGuid "kstpcn-2026-01-cuoc-khao-sat"
$reportId = New-StableGuid "kstpcn-2026-01-bao-cao"
$maCuocKhaoSat = "KSTPCN-2026-01"
$tenCuocKhaoSat = "Khảo sát tình hình thi hành pháp luật an toàn thực phẩm đối với thực phẩm chức năng"

$sql = [System.Text.StringBuilder]::new()
Add-Line $sql "/*"
Add-Line $sql "    Bộ dữ liệu khảo sát đã hoàn thành theo 2 mẫu phiếu thực phẩm chức năng."
Add-Line $sql "    Generated by tools/Seed-KhaoSatThucPhamChucNang.ps1"
Add-Line $sql "    Safe to re-run: inserts are guarded by MaCuocKhaoSat = $maCuocKhaoSat."
Add-Line $sql "*/"
Add-Line $sql "SET NOCOUNT ON;"
Add-Line $sql "SET XACT_ABORT ON;"
Add-Line $sql "SET QUOTED_IDENTIFIER ON;"
Add-Line $sql ""
Add-Line $sql "BEGIN TRANSACTION;"
Add-Line $sql "DECLARE @Now datetime2 = SYSUTCDATETIME();"
Add-Line $sql "DECLARE @Actor uniqueidentifier = '$actorId';"
Add-Line $sql "DECLARE @DonViChuTri uniqueidentifier;"
Add-Line $sql "DECLARE @DonViCqql uniqueidentifier;"
Add-Line $sql "DECLARE @DonViNguoiDan uniqueidentifier;"
Add-Line $sql "DECLARE @CanBo uniqueidentifier;"
Add-Line $sql "DECLARE @LinhVuc uniqueidentifier;"
Add-Line $sql "DECLARE @TrangThaiCuocKetThuc uniqueidentifier;"
Add-Line $sql "DECLARE @TrangThaiDoiTuongXacNhan uniqueidentifier;"
Add-Line $sql "DECLARE @TrangThaiPhieuHopLe uniqueidentifier;"
Add-Line $sql "SELECT TOP (1) @DonViChuTri = [Id] FROM [dm].[DanhMucDonVis] ORDER BY [CreatedDate], [Id];"
Add-Line $sql "SELECT TOP (1) @DonViCqql = [Id] FROM [dm].[DanhMucDonVis] ORDER BY [CreatedDate], [Id];"
Add-Line $sql "SELECT TOP (1) @DonViNguoiDan = [Id] FROM [dm].[DanhMucDonVis] WHERE [Id] <> @DonViCqql ORDER BY [CreatedDate], [Id];"
Add-Line $sql "SELECT TOP (1) @CanBo = [Id] FROM [dm].[DanhMucCanBos] ORDER BY [CreatedDate], [Id];"
Add-Line $sql "SELECT TOP (1) @LinhVuc = [Id] FROM [dm].[DanhMucLinhVucs] WHERE [TrangThai] = 1 ORDER BY [CreatedDate], [Id];"
Add-Line $sql "SELECT TOP (1) @TrangThaiCuocKetThuc = [Id] FROM [dm].[DanhMucTrangThais] WHERE [NhomTrangThai] = N'CUOC_KHAO_SAT_THI_HANH_PHAP_LUAT' AND [MaTrangThai] = N'DA_KET_THUC';"
Add-Line $sql "SELECT TOP (1) @TrangThaiDoiTuongXacNhan = [Id] FROM [dm].[DanhMucTrangThais] WHERE [NhomTrangThai] = N'DOI_TUONG_KHAO_SAT' AND [MaTrangThai] = N'DA_XAC_NHAN';"
Add-Line $sql "SELECT TOP (1) @TrangThaiPhieuHopLe = [Id] FROM [dm].[DanhMucTrangThais] WHERE [NhomTrangThai] = N'PHIEU_NOP_KHAO_SAT' AND [MaTrangThai] = N'PHIEU_HOP_LE';"
Add-Line $sql "IF @DonViChuTri IS NULL OR @DonViCqql IS NULL OR @DonViNguoiDan IS NULL THROW 51000, N'Cần có ít nhất 2 đơn vị trong dm.DanhMucDonVis để seed 2 nhóm đối tượng khảo sát.', 1;"
Add-Line $sql "IF @TrangThaiCuocKetThuc IS NULL OR @TrangThaiDoiTuongXacNhan IS NULL OR @TrangThaiPhieuHopLe IS NULL THROW 51000, N'Chưa seed đủ danh mục trạng thái khảo sát thi hành pháp luật.', 1;"
Add-Line $sql ""
Add-Line $sql "IF NOT EXISTS (SELECT 1 FROM [kspl].[CuocKhaoSats] WHERE [MaCuocKhaoSat] = N'$maCuocKhaoSat')"
Add-Line $sql "BEGIN"
Add-Line $sql "    INSERT INTO [kspl].[CuocKhaoSats] ([Id],[MaCuocKhaoSat],[TenCuocKhaoSat],[MucDich],[PhamVi],[LinhVucId],[DonViChuTriId],[TuNgay],[DenNgay],[TrangThaiId],[CreatedAt],[CreatedBy],[IsDeleted])"
Add-Line $sql "    VALUES ($(SqlGuid $surveyId),N'$maCuocKhaoSat',$(SqlString $tenCuocKhaoSat),N'Đánh giá thực trạng thi hành pháp luật và nhu cầu hoàn thiện pháp luật về an toàn thực phẩm đối với thực phẩm chức năng.',N'Cơ quan quản lý nhà nước và người dân trên địa bàn khảo sát.',@LinhVuc,@DonViChuTri,'2026-09-01','2026-10-31',@TrangThaiCuocKetThuc,@Now,CONVERT(nvarchar(64),@Actor),0);"

foreach ($form in $forms) {
    $groupId = New-StableGuid "kstpcn-2026-01-$($form.GroupCode)-nhom"
    $templateId = New-StableGuid "kstpcn-2026-01-$($form.GroupCode)-mau"
    $subjectId = New-StableGuid "kstpcn-2026-01-$($form.GroupCode)-doi-tuong"
    $submissionId = New-StableGuid "kstpcn-2026-01-$($form.GroupCode)-phieu-nop"
    $questionGroups = @($form.ParsedRows | Group-Object MaCauHoi | Sort-Object { [int]($_.Name -replace "\D", "") })
    $groupCode = $form.GroupCode

    Add-Line $sql ""
    Add-Line $sql "    -- Nhóm $($form.GroupName)"
    Add-Line $sql "    INSERT INTO [kspl].[NhomDoiTuongKhaoSats] ([Id],[CuocKhaoSatId],[MaNhom],[TenNhom],[ThuTu],[CreatedAt],[CreatedBy],[IsDeleted])"
    Add-Line $sql "    VALUES ($(SqlGuid $groupId),$(SqlGuid $surveyId),N'$groupCode',$(SqlString $form.GroupName),$($forms.IndexOf($form) + 1),@Now,CONVERT(nvarchar(64),@Actor),0);"
    Add-Line $sql "    INSERT INTO [kspl].[MauPhieuKhaoSats] ([Id],[CuocKhaoSatId],[NhomDoiTuongKhaoSatId],[PhienBan],[TenFile],[DuongDanFile],[MaHash],[DaPhatHanh],[TrangThaiMauPhieu],[NgayHieuLuc],[NgayHetHieuLuc],[CreatedAt],[CreatedBy],[IsDeleted])"
    Add-Line $sql "    VALUES ($(SqlGuid $templateId),$(SqlGuid $surveyId),$(SqlGuid $groupId),1,N'$($form.TemplateFileName)',N'$($form.TemplateFile.RelativePath)',N'$((New-StableGuid "kstpcn-2026-01-$groupCode-template-hash").ToString("N"))',1,N'DANG_SU_DUNG','2026-09-01T00:00:00','2026-12-31T00:00:00',@Now,CONVERT(nvarchar(64),@Actor),0);"

    foreach ($questionGroup in $questionGroups) {
        $question = $questionGroup.Group[0]
        $questionId = New-StableGuid "kstpcn-2026-01-$groupCode-$($question.MaCauHoi)-stat"
        $templateQuestionId = New-StableGuid "kstpcn-2026-01-$groupCode-$($question.MaCauHoi)-template-question"
        $statCode = "$groupCode::$($question.MaCauHoi)"
        $answerIndex = 0

        Add-Line $sql "    INSERT INTO [kspl].[CauHoiThongKes] ([Id],[CuocKhaoSatId],[MaCauHoiThongKe],[NoiDung],[LoaiCauHoi],[ChoPhepNhieuLuaChon],[MauSoTyLe],[CoYKienTuDo],[CreatedAt],[CreatedBy],[IsDeleted])"
        Add-Line $sql "    VALUES ($(SqlGuid $questionId),$(SqlGuid $surveyId),N'$statCode',$(SqlString $question.NoiDung),N'$($question.LoaiCauHoi)',$(if ($question.ChoPhepNhieuLuaChon) { 1 } else { 0 }),N'PHIEU_HOP_LE',$(if ($question.CoYKienTuDo) { 1 } else { 0 }),@Now,CONVERT(nvarchar(64),@Actor),0);"
        Add-Line $sql "    INSERT INTO [kspl].[CauHoiMauPhieus] ([Id],[MauPhieuKhaoSatId],[CauHoiThongKeId],[MaCauHoi],[NoiDung],[LoaiCauHoi],[BatBuoc],[ChoPhepNhieuLuaChon],[MauSoTyLe],[CoYKienTuDo],[ThuTu],[CreatedAt],[CreatedBy],[IsDeleted])"
        Add-Line $sql "    VALUES ($(SqlGuid $templateQuestionId),$(SqlGuid $templateId),$(SqlGuid $questionId),N'$($question.MaCauHoi)',$(SqlString $question.NoiDung),N'$($question.LoaiCauHoi)',0,$(if ($question.ChoPhepNhieuLuaChon) { 1 } else { 0 }),N'PHIEU_HOP_LE',$(if ($question.CoYKienTuDo) { 1 } else { 0 }),$($question.RowNumber),@Now,CONVERT(nvarchar(64),@Actor),0);"

        foreach ($option in $questionGroup.Group | Where-Object { -not [string]::IsNullOrWhiteSpace($_.MaDapAn) }) {
            $answerIndex++
            $optionId = New-StableGuid "kstpcn-2026-01-$groupCode-$($question.MaCauHoi)-$($option.MaDapAn)-option"
            Add-Line $sql "    INSERT INTO [kspl].[LuaChonTraLois] ([Id],[CauHoiMauPhieuId],[MaLuaChon],[NoiDung],[ThuTu],[CreatedAt],[CreatedBy],[IsDeleted])"
            Add-Line $sql "    VALUES ($(SqlGuid $optionId),$(SqlGuid $templateQuestionId),N'$($option.MaDapAn)',$(SqlString $option.DapAn),$answerIndex,@Now,CONVERT(nvarchar(64),@Actor),0);"
        }
    }

    Add-Line $sql "    INSERT INTO [kspl].[DoiTuongKhaoSats] ([Id],[CuocKhaoSatId],[NhomDoiTuongKhaoSatId],[MauPhieuKhaoSatId],[DonViId],[CanBoId],[HanNop],[TrangThaiId],[CreatedAt],[CreatedBy],[IsDeleted])"
    Add-Line $sql "    VALUES ($(SqlGuid $subjectId),$(SqlGuid $surveyId),$(SqlGuid $groupId),$(SqlGuid $templateId),$($form.UnitVariable),@CanBo,'2026-10-15',@TrangThaiDoiTuongXacNhan,@Now,CONVERT(nvarchar(64),@Actor),0);"
    Add-Line $sql "    INSERT INTO [kspl].[PhieuNopKhaoSats] ([Id],[CuocKhaoSatId],[NhomDoiTuongKhaoSatId],[DoiTuongKhaoSatId],[MauPhieuKhaoSatId],[TenFile],[DuongDanFile],[MaHash],[TrangThaiId],[NgayImport],[TenPhanMem],[PhienBanPhanMem],[GhiChuNguonDuLieu],[CreatedAt],[CreatedBy],[IsDeleted])"
    Add-Line $sql "    VALUES ($(SqlGuid $submissionId),$(SqlGuid $surveyId),$(SqlGuid $groupId),$(SqlGuid $subjectId),$(SqlGuid $templateId),N'$($form.SubmissionFileName)',N'$($form.SubmissionFile.RelativePath)',N'$((New-StableGuid "kstpcn-2026-01-$groupCode-submission-hash").ToString("N"))',@TrangThaiPhieuHopLe,'2026-10-10T09:00:00',N'Seed khảo sát thực phẩm chức năng',N'1.0',N'Dữ liệu câu trả lời tự tạo theo mẫu phiếu Word.',@Now,CONVERT(nvarchar(64),@Actor),0);"

    foreach ($questionGroup in $questionGroups) {
        $question = $questionGroup.Group[0]
        $questionId = New-StableGuid "kstpcn-2026-01-$groupCode-$($question.MaCauHoi)-stat"
        $templateQuestionId = New-StableGuid "kstpcn-2026-01-$groupCode-$($question.MaCauHoi)-template-question"
        $options = @($questionGroup.Group | Where-Object { -not [string]::IsNullOrWhiteSpace($_.MaDapAn) })

        if ($options.Count -eq 0) {
            $answerId = New-StableGuid "kstpcn-2026-01-$groupCode-$($question.MaCauHoi)-text-answer"
            Add-Line $sql "    INSERT INTO [kspl].[CauTraLoiKhaoSats] ([Id],[PhieuNopKhaoSatId],[CauHoiThongKeId],[CauHoiMauPhieuId],[LuaChonTraLoiId],[GiaTriText],[GiaTriSo],[SoLuong],[TongSoTraLoi],[CreatedAt],[CreatedBy],[IsDeleted])"
            Add-Line $sql "    VALUES ($(SqlGuid $answerId),$(SqlGuid $submissionId),$(SqlGuid $questionId),$(SqlGuid $templateQuestionId),NULL,N'Đề nghị tiếp tục rà soát quy định, tăng cường tuyên truyền, kiểm soát quảng cáo và công khai cơ sở vi phạm.',NULL,NULL,NULL,@Now,CONVERT(nvarchar(64),@Actor),0);"
            continue
        }

        $counts = Get-AnswerCounts -Total $form.Total -OptionCount $options.Count -MultiSelect $question.ChoPhepNhieuLuaChon -QuestionIndex ([int]($question.MaCauHoi -replace "\D", ""))
        for ($i = 0; $i -lt $options.Count; $i++) {
            $option = $options[$i]
            $optionId = New-StableGuid "kstpcn-2026-01-$groupCode-$($question.MaCauHoi)-$($option.MaDapAn)-option"
            $answerId = New-StableGuid "kstpcn-2026-01-$groupCode-$($question.MaCauHoi)-$($option.MaDapAn)-answer"
            Add-Line $sql "    INSERT INTO [kspl].[CauTraLoiKhaoSats] ([Id],[PhieuNopKhaoSatId],[CauHoiThongKeId],[CauHoiMauPhieuId],[LuaChonTraLoiId],[GiaTriText],[GiaTriSo],[SoLuong],[TongSoTraLoi],[CreatedAt],[CreatedBy],[IsDeleted])"
            Add-Line $sql "    VALUES ($(SqlGuid $answerId),$(SqlGuid $submissionId),$(SqlGuid $questionId),$(SqlGuid $templateQuestionId),$(SqlGuid $optionId),NULL,NULL,$($counts[$i]),$($form.Total),@Now,CONVERT(nvarchar(64),@Actor),0);"
        }

        if ($question.CoYKienTuDo) {
            $answerId = New-StableGuid "kstpcn-2026-01-$groupCode-$($question.MaCauHoi)-free-text-answer"
            Add-Line $sql "    INSERT INTO [kspl].[CauTraLoiKhaoSats] ([Id],[PhieuNopKhaoSatId],[CauHoiThongKeId],[CauHoiMauPhieuId],[LuaChonTraLoiId],[GiaTriText],[GiaTriSo],[SoLuong],[TongSoTraLoi],[CreatedAt],[CreatedBy],[IsDeleted])"
            Add-Line $sql "    VALUES ($(SqlGuid $answerId),$(SqlGuid $submissionId),$(SqlGuid $questionId),$(SqlGuid $templateQuestionId),NULL,N'Cần bổ sung hướng dẫn rõ hơn về quản lý quảng cáo, truy xuất nguồn gốc và cơ chế tiếp nhận phản ánh.',NULL,NULL,$($form.Total),@Now,CONVERT(nvarchar(64),@Actor),0);"
        }
    }
}

Add-Line $sql ""
Add-Line $sql "    INSERT INTO [kspl].[BaoCaoKhaoSats] ([Id],[CuocKhaoSatId],[TenBaoCao],[SoKyHieu],[NgayBaoCao],[TrangThai],[NgayChot],[UuDiem],[HanChe],[KienNghi],[DuongDanFileXuat],[TenFileXuat],[NgayXuat],[CreatedAt],[CreatedBy],[IsDeleted])"
Add-Line $sql "    VALUES ($(SqlGuid $reportId),$(SqlGuid $surveyId),N'Báo cáo tổng hợp khảo sát thực phẩm chức năng',N'KSTPCN-2026-01/BC','2026-10-12',N'DA_CHOT','2026-10-12T09:00:00',N'Dữ liệu phản ánh có nhận thức và mức độ phối hợp nhất định trong quản lý thực phẩm chức năng.',N'Một số nội dung còn khó kiểm soát như quảng cáo trên môi trường mạng, nguồn gốc sản phẩm và cơ sở dữ liệu quản lý.',N'Tăng cường hậu kiểm, hoàn thiện pháp luật, đẩy mạnh tuyên truyền và ứng dụng công nghệ thông tin.',N'$($reportFile.RelativePath)',N'bao-cao-khao-sat-thuc-pham-chuc-nang.txt','2026-10-12T09:00:00',@Now,CONVERT(nvarchar(64),@Actor),0);"

foreach ($form in $forms) {
    $groupCode = $form.GroupCode
    foreach ($questionGroup in ($form.ParsedRows | Group-Object MaCauHoi | Sort-Object { [int]($_.Name -replace "\D", "") })) {
        $question = $questionGroup.Group[0]
        $questionId = New-StableGuid "kstpcn-2026-01-$groupCode-$($question.MaCauHoi)-stat"
        $options = @($questionGroup.Group | Where-Object { -not [string]::IsNullOrWhiteSpace($_.MaDapAn) })

        if ($options.Count -eq 0) {
            $detailId = New-StableGuid "kstpcn-2026-01-$groupCode-$($question.MaCauHoi)-report-text"
            Add-Line $sql "    INSERT INTO [kspl].[ChiTietBaoCaoKhaoSats] ([Id],[BaoCaoKhaoSatId],[CauHoiThongKeId],[MaCauHoi],[NoiDungCauHoi],[MaLuaChon],[NoiDungLuaChon],[SoLuong],[MauSoTyLe],[TyLe],[YKienTuDo],[CreatedAt],[CreatedBy],[IsDeleted])"
            Add-Line $sql "    VALUES ($(SqlGuid $detailId),$(SqlGuid $reportId),$(SqlGuid $questionId),N'$groupCode::$($question.MaCauHoi)',$(SqlString $question.NoiDung),NULL,NULL,0,$($form.Total),0,N'Đề nghị tiếp tục rà soát quy định, tăng cường tuyên truyền, kiểm soát quảng cáo và công khai cơ sở vi phạm.',@Now,CONVERT(nvarchar(64),@Actor),0);"
            continue
        }

        $counts = Get-AnswerCounts -Total $form.Total -OptionCount $options.Count -MultiSelect $question.ChoPhepNhieuLuaChon -QuestionIndex ([int]($question.MaCauHoi -replace "\D", ""))
        for ($i = 0; $i -lt $options.Count; $i++) {
            $option = $options[$i]
            $detailId = New-StableGuid "kstpcn-2026-01-$groupCode-$($question.MaCauHoi)-$($option.MaDapAn)-report"
            $rate = if ($form.Total -eq 0) { 0 } else { [Math]::Round(([decimal]$counts[$i]) * 100 / ([decimal]$form.Total), 2) }
            Add-Line $sql "    INSERT INTO [kspl].[ChiTietBaoCaoKhaoSats] ([Id],[BaoCaoKhaoSatId],[CauHoiThongKeId],[MaCauHoi],[NoiDungCauHoi],[MaLuaChon],[NoiDungLuaChon],[SoLuong],[MauSoTyLe],[TyLe],[YKienTuDo],[CreatedAt],[CreatedBy],[IsDeleted])"
            Add-Line $sql "    VALUES ($(SqlGuid $detailId),$(SqlGuid $reportId),$(SqlGuid $questionId),N'$groupCode::$($question.MaCauHoi)',$(SqlString $question.NoiDung),N'$($option.MaDapAn)',$(SqlString $option.DapAn),$($counts[$i]),$($form.Total),$(SqlDecimal $rate),NULL,@Now,CONVERT(nvarchar(64),@Actor),0);"
        }
    }
}

$historySteps = @(
    @{ Code = "TAO_CUOC_KHAO_SAT"; Text = "Tạo cuộc khảo sát và nạp 2 mẫu phiếu Word." },
    @{ Code = "THUC_HIEN_KHAO_SAT"; Text = "Phân công 2 đơn vị khảo sát và nộp phiếu hoàn thành." },
    @{ Code = "DANH_GIA_KET_QUA"; Text = "Dữ liệu câu trả lời đã được rà soát hợp lệ." },
    @{ Code = "TONG_HOP_BAO_CAO"; Text = "Chốt báo cáo tổng hợp khảo sát." }
)
$stepDate = [datetime]"2026-10-09T09:00:00"
foreach ($step in $historySteps) {
    $historyId = New-StableGuid "kstpcn-2026-01-history-$($step.Code)"
    Add-Line $sql "    INSERT INTO [kspl].[LichSuXuLyKhaoSats] ([Id],[CuocKhaoSatId],[DoiTuongKhaoSatId],[PhieuNopKhaoSatId],[HanhDong],[NoiDung],[ThoiGianXuLy],[NguoiXuLyId],[DonViXuLyId])"
    Add-Line $sql "    VALUES ($(SqlGuid $historyId),$(SqlGuid $surveyId),NULL,NULL,N'$($step.Code)',$(SqlString $step.Text),$(SqlDate $stepDate),@Actor,@DonViChuTri);"
    $stepDate = $stepDate.AddDays(1)
}

Add-Line $sql "END"
Add-Line $sql ""
Add-Line $sql "COMMIT TRANSACTION;"
Add-Line $sql ""
Add-Line $sql "SELECT N'Cuộc khảo sát thực phẩm chức năng' AS [NoiDung], COUNT(1) AS [SoLuong] FROM [kspl].[CuocKhaoSats] WHERE [MaCuocKhaoSat] = N'$maCuocKhaoSat'"
Add-Line $sql "UNION ALL SELECT N'Nhóm đối tượng', COUNT(1) FROM [kspl].[NhomDoiTuongKhaoSats] WHERE [CuocKhaoSatId] = $(SqlGuid $surveyId)"
Add-Line $sql "UNION ALL SELECT N'Mẫu phiếu', COUNT(1) FROM [kspl].[MauPhieuKhaoSats] WHERE [CuocKhaoSatId] = $(SqlGuid $surveyId)"
Add-Line $sql "UNION ALL SELECT N'Câu hỏi thống kê', COUNT(1) FROM [kspl].[CauHoiThongKes] WHERE [CuocKhaoSatId] = $(SqlGuid $surveyId)"
Add-Line $sql "UNION ALL SELECT N'Phiếu nộp', COUNT(1) FROM [kspl].[PhieuNopKhaoSats] WHERE [CuocKhaoSatId] = $(SqlGuid $surveyId)"
Add-Line $sql "UNION ALL SELECT N'Báo cáo đã chốt', COUNT(1) FROM [kspl].[BaoCaoKhaoSats] WHERE [CuocKhaoSatId] = $(SqlGuid $surveyId) AND [TrangThai] = N'DA_CHOT';"

$fullSqlPath = Join-Path (Get-Location) $OutputSqlPath
New-Item -ItemType Directory -Path (Split-Path -Parent $fullSqlPath) -Force | Out-Null
Set-Content -LiteralPath $fullSqlPath -Value $sql.ToString() -Encoding UTF8

$questionCount = ($forms | ForEach-Object { $_.ParsedRows | Group-Object MaCauHoi } | Measure-Object).Count
$optionCount = ($forms | ForEach-Object { $_.ParsedRows | Where-Object { -not [string]::IsNullOrWhiteSpace($_.MaDapAn) } } | Measure-Object).Count
Write-Output "Parsed survey templates: $($forms.Count)"
Write-Output "Parsed questions: $questionCount"
Write-Output "Parsed answer options: $optionCount"
Write-Output "Created sample attachments under: BE/services/KhaoSatThiHanhPhapLuatService/$relativeBase"
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

