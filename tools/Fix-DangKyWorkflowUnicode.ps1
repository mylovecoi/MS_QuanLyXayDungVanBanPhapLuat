$ErrorActionPreference = "Stop"

$configPath = Join-Path $PSScriptRoot "..\BE\services\DangKyXayDungVanBanService\appsettings.Development.json"
$config = Get-Content -LiteralPath $configPath -Raw | ConvertFrom-Json
$connectionString = $config.ConnectionStrings.DefaultConnection

$workflowId = [Guid]"33333333-3333-3333-3333-333333333301"
$stepProposalId = [Guid]"33333333-3333-3333-3333-333333333311"
$stepApprovalResultId = [Guid]"33333333-3333-3333-3333-333333333313"
$transitionForwardId = [Guid]"33333333-3333-3333-3333-333333333321"
$transitionReturnId = [Guid]"33333333-3333-3333-3333-333333333324"
$transitionCompleteId = [Guid]"33333333-3333-3333-3333-333333333326"

$proposalName = "Đề xuất danh mục/Đăng ký xây dựng văn bản QPPL"
$approvalResultName = "Phê duyệt và cập nhật kết quả"

$sql = @"
UPDATE dm.DanhMucQuyTrinhSoanThaos
SET TenQuyTrinh = @WorkflowName,
    MoTa = @WorkflowDescription,
    UpdatedDate = SYSDATETIME()
WHERE Id = @WorkflowId;

UPDATE dm.DanhMucBuocQuyTrinhs
SET TenBuoc = @ProposalName,
    MoTa = @ProposalDescription,
    UpdatedDate = SYSDATETIME()
WHERE Id = @StepProposalId;

UPDATE dm.DanhMucBuocQuyTrinhs
SET TenBuoc = @ApprovalResultName,
    MoTa = @ApprovalResultDescription,
    UpdatedDate = SYSDATETIME()
WHERE Id = @StepApprovalResultId;

UPDATE dm.DanhMucChuyenBuocQuyTrinhs
SET MoTa = @ForwardDescription,
    UpdatedDate = SYSDATETIME()
WHERE Id = @TransitionForwardId;

UPDATE dm.DanhMucChuyenBuocQuyTrinhs
SET MoTa = @ReturnDescription,
    UpdatedDate = SYSDATETIME()
WHERE Id = @TransitionReturnId;

UPDATE dm.DanhMucChuyenBuocQuyTrinhs
SET MoTa = @CompleteDescription,
    UpdatedDate = SYSDATETIME()
WHERE Id = @TransitionCompleteId;

UPDATE dm.DanhMucTrangThaiNghiepVuQuyTrinhs
SET MoTa = CASE MaTrangThai
        WHEN 'DANG_SOAN_THAO' THEN @StatusDraftDescription
        WHEN 'DA_TRINH_PHE_DUYET' THEN @StatusSubmittedDescription
        WHEN 'BI_TRA_LAI' THEN @StatusReturnedDescription
        WHEN 'DA_PHE_DUYET' THEN @StatusApprovedDescription
        WHEN 'HOAN_THANH' THEN @StatusCompletedDescription
        WHEN 'KHONG_PHE_DUYET' THEN @StatusRejectedDescription
        ELSE MoTa
    END,
    UpdatedDate = SYSDATETIME()
WHERE QuyTrinhSoanThaoId = @WorkflowId;

UPDATE dbo.DangKyXayDungVanBanLichSuXuLys
SET TenBuocTu = CASE
        WHEN TuBuocId = @StepProposalId THEN @ProposalName
        WHEN TuBuocId = @StepApprovalResultId THEN @ApprovalResultName
        ELSE TenBuocTu
    END,
    TenBuocDen = CASE
        WHEN DenBuocId = @StepProposalId THEN @ProposalName
        WHEN DenBuocId = @StepApprovalResultId THEN @ApprovalResultName
        ELSE TenBuocDen
    END,
    UpdatedAt = SYSDATETIME()
WHERE DangKyXayDungVanBanId IN (
    SELECT Id FROM dbo.DangKyXayDungVanBans WHERE QuyTrinhSoanThaoId = @WorkflowId
);

SELECT b.ThuTuSapXep, b.MaBuoc, b.TenBuoc, b.MoTa
FROM dm.DanhMucBuocQuyTrinhs b
WHERE b.QuyTrinhSoanThaoId = @WorkflowId
ORDER BY b.ThuTuSapXep;
"@

$connection = [System.Data.SqlClient.SqlConnection]::new($connectionString)
$command = $connection.CreateCommand()
$command.CommandText = $sql

function Add-Param([string]$Name, $Value) {
    $parameter = $command.Parameters.Add("@$Name", [System.Data.SqlDbType]::NVarChar, -1)
    if ($Value -is [Guid]) {
        $parameter.SqlDbType = [System.Data.SqlDbType]::UniqueIdentifier
    }
    $parameter.Value = $Value
}

Add-Param "WorkflowId" $workflowId
Add-Param "StepProposalId" $stepProposalId
Add-Param "StepApprovalResultId" $stepApprovalResultId
Add-Param "TransitionForwardId" $transitionForwardId
Add-Param "TransitionReturnId" $transitionReturnId
Add-Param "TransitionCompleteId" $transitionCompleteId
Add-Param "WorkflowName" "Đề xuất danh mục / Đăng ký xây dựng QPPL"
Add-Param "WorkflowDescription" "Quy trình đăng ký xây dựng văn bản QPPL theo tài liệu QT đăng ký"
Add-Param "ProposalName" $proposalName
Add-Param "ProposalDescription" "Cơ quan chủ trì lập hồ sơ, trình hồ sơ đến cơ quan có thẩm quyền và hệ thống lưu trữ hồ sơ đề xuất/đăng ký."
Add-Param "ApprovalResultName" $approvalResultName
Add-Param "ApprovalResultDescription" "Việc phê duyệt diễn ra ngoài hệ thống; cơ quan chủ trì cập nhật kết quả phê duyệt lên phần mềm."
Add-Param "ForwardDescription" "Trình hồ sơ đề xuất/đăng ký sang bước phê duyệt và cập nhật kết quả."
Add-Param "ReturnDescription" "Trả hồ sơ về bước đề xuất/đăng ký để chỉnh sửa."
Add-Param "CompleteDescription" "Kết thúc quy trình đăng ký sau khi cập nhật kết quả."
Add-Param "StatusDraftDescription" "Hồ sơ đang được lập/cập nhật."
Add-Param "StatusSubmittedDescription" "Hồ sơ đã trình cơ quan thẩm quyền."
Add-Param "StatusReturnedDescription" "Hồ sơ bị trả lại để bổ sung."
Add-Param "StatusApprovedDescription" "Hồ sơ đăng ký được phê duyệt."
Add-Param "StatusCompletedDescription" "Hoàn thành quy trình đăng ký."
Add-Param "StatusRejectedDescription" "Hồ sơ không được phê duyệt."

$connection.Open()
try {
    $reader = $command.ExecuteReader()
    while ($reader.Read()) {
        [PSCustomObject]@{
            ThuTuSapXep = $reader["ThuTuSapXep"]
            MaBuoc = $reader["MaBuoc"]
            TenBuoc = $reader["TenBuoc"]
            MoTa = $reader["MoTa"]
        }
    }
}
finally {
    $connection.Close()
}
