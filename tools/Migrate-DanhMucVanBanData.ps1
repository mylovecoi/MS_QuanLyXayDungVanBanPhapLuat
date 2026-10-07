param(
    [string]$SourceServer = "192.168.100.250",
    [string]$SourceDatabase = "CSDLXayDungVanBan",
    [string]$TargetServer = "192.168.100.250",
    [string]$TargetDatabase = "CSDLXayDungVanBan_MS",
    [string]$Username = "sa",
    [string]$Password,
    [switch]$Execute
)

if ([string]::IsNullOrWhiteSpace($Password)) {
    throw "Password is required."
}

$ErrorActionPreference = "Stop"

$sourceConnectionString = "Server=$SourceServer;Database=$SourceDatabase;User ID=$Username;Password=$Password;MultipleActiveResultSets=true;TrustServerCertificate=True;"
$targetConnectionString = "Server=$TargetServer;Database=$TargetDatabase;User ID=$Username;Password=$Password;MultipleActiveResultSets=true;TrustServerCertificate=True;"

function New-Command {
    param(
        [System.Data.SqlClient.SqlConnection]$Connection,
        [string]$CommandText
    )

    $command = $Connection.CreateCommand()
    $command.CommandTimeout = 120
    $command.CommandText = $CommandText
    return $command
}

function Get-TableName {
    param(
        [System.Data.SqlClient.SqlConnection]$Connection,
        [string[]]$Schemas,
        [string]$Table
    )

    foreach ($schema in $Schemas) {
        $command = New-Command $Connection "SELECT CASE WHEN OBJECT_ID(N'[$schema].[$Table]', N'U') IS NULL THEN 0 ELSE 1 END;"
        if ([int]$command.ExecuteScalar() -eq 1) {
            return "[$schema].[$Table]"
        }
    }

    throw "Khong tim thay bang $Table trong cac schema: $($Schemas -join ', ')."
}

function Get-Count {
    param(
        [System.Data.SqlClient.SqlConnection]$Connection,
        [string]$TableName
    )

    $command = New-Command $Connection "SELECT COUNT(1) FROM $TableName;"
    return [int]$command.ExecuteScalar()
}

$sourceConnection = [System.Data.SqlClient.SqlConnection]::new($sourceConnectionString)
$targetConnection = [System.Data.SqlClient.SqlConnection]::new($targetConnectionString)

$sourceConnection.Open()
$targetConnection.Open()

try {
    $sourceTable = Get-TableName $sourceConnection @("dbo", "dm") "DanhMucVanBans"
    $targetTable = Get-TableName $targetConnection @("dm", "dbo") "DanhMucVanBans"

    $sourceCountBefore = Get-Count $sourceConnection $sourceTable
    $targetCountBefore = Get-Count $targetConnection $targetTable

    Write-Output "Source table: [$SourceDatabase].$sourceTable"
    Write-Output "Target table: [$TargetDatabase].$targetTable"
    Write-Output "Source rows before: $sourceCountBefore"
    Write-Output "Target rows before: $targetCountBefore"

    if (-not $Execute) {
        Write-Output "Dry run only. Add -Execute to migrate missing rows."
        return
    }

    $transaction = $targetConnection.BeginTransaction()

    try {
        $insertCommand = $targetConnection.CreateCommand()
        $insertCommand.Transaction = $transaction
        $insertCommand.CommandTimeout = 120
        $insertCommand.CommandText = @"
INSERT INTO $targetTable
(
    [Id],
    [TenLoaiVanBan],
    [CapChinhQuyen],
    [ChuTheBanHanh],
    [KyHieuMau],
    [ThuTuSapXep],
    [TrangThai],
    [MoTa],
    [GhiChu],
    [CreatedBy],
    [CreatedDate],
    [UpdatedBy],
    [UpdatedDate]
)
VALUES
(
    @Id,
    @TenLoaiVanBan,
    @CapChinhQuyen,
    @ChuTheBanHanh,
    @KyHieuMau,
    @ThuTuSapXep,
    @TrangThai,
    @MoTa,
    @GhiChu,
    @CreatedBy,
    @CreatedDate,
    @UpdatedBy,
    @UpdatedDate
);
"@

        $parameterTypes = [ordered]@{
            Id = [System.Data.SqlDbType]::UniqueIdentifier
            TenLoaiVanBan = [System.Data.SqlDbType]::NVarChar
            CapChinhQuyen = [System.Data.SqlDbType]::NVarChar
            ChuTheBanHanh = [System.Data.SqlDbType]::NVarChar
            KyHieuMau = [System.Data.SqlDbType]::NVarChar
            ThuTuSapXep = [System.Data.SqlDbType]::Int
            TrangThai = [System.Data.SqlDbType]::Bit
            MoTa = [System.Data.SqlDbType]::NVarChar
            GhiChu = [System.Data.SqlDbType]::NVarChar
            CreatedBy = [System.Data.SqlDbType]::UniqueIdentifier
            CreatedDate = [System.Data.SqlDbType]::DateTime2
            UpdatedBy = [System.Data.SqlDbType]::UniqueIdentifier
            UpdatedDate = [System.Data.SqlDbType]::DateTime2
        }

        $columns = @($parameterTypes.Keys)

        foreach ($entry in $parameterTypes.GetEnumerator()) {
            $parameter = $insertCommand.Parameters.Add("@$($entry.Key)", $entry.Value)
            if ($entry.Value -eq [System.Data.SqlDbType]::NVarChar) {
                $parameter.Size = -1
            }
        }

        $existingIds = [System.Collections.Generic.HashSet[Guid]]::new()
        $existingCommand = $targetConnection.CreateCommand()
        $existingCommand.Transaction = $transaction
        $existingCommand.CommandTimeout = 120
        $existingCommand.CommandText = "SELECT [Id] FROM $targetTable;"
        $existingReader = $existingCommand.ExecuteReader()

        try {
            while ($existingReader.Read()) {
                [void]$existingIds.Add([Guid]$existingReader["Id"])
            }
        }
        finally {
            $existingReader.Close()
        }

        $sourceCommand = New-Command $sourceConnection @"
SELECT
    [Id],
    ISNULL([TenLoaiVanBan], N'') AS [TenLoaiVanBan],
    ISNULL([CapChinhQuyen], N'') AS [CapChinhQuyen],
    ISNULL([ChuTheBanHanh], N'') AS [ChuTheBanHanh],
    [KyHieuMau],
    ISNULL([ThuTuSapXep], 1) AS [ThuTuSapXep],
    ISNULL([TrangThai], CONVERT(bit, 1)) AS [TrangThai],
    [MoTa],
    [GhiChu],
    ISNULL([CreatedBy], '00000000-0000-0000-0000-000000000000') AS [CreatedBy],
    ISNULL([CreatedDate], SYSUTCDATETIME()) AS [CreatedDate],
    ISNULL([UpdatedBy], '00000000-0000-0000-0000-000000000000') AS [UpdatedBy],
    ISNULL([UpdatedDate], SYSUTCDATETIME()) AS [UpdatedDate]
FROM $sourceTable
ORDER BY [ThuTuSapXep], [TenLoaiVanBan], [Id];
"@

        $sourceReader = $sourceCommand.ExecuteReader()
        $inserted = 0
        $skipped = 0

        try {
            while ($sourceReader.Read()) {
                $id = [Guid]$sourceReader["Id"]

                if ($existingIds.Contains($id)) {
                    $skipped++
                    continue
                }

                foreach ($column in $columns) {
                    $insertCommand.Parameters["@$column"].Value = $sourceReader[$column]
                }

                [void]$insertCommand.ExecuteNonQuery()
                [void]$existingIds.Add($id)
                $inserted++
            }
        }
        finally {
            $sourceReader.Close()
        }

        $transaction.Commit()

        $targetCountAfter = Get-Count $targetConnection $targetTable
        Write-Output "Inserted rows: $inserted"
        Write-Output "Skipped existing rows: $skipped"
        Write-Output "Target rows after: $targetCountAfter"
    }
    catch {
        $transaction.Rollback()
        throw
    }
}
finally {
    $sourceConnection.Close()
    $targetConnection.Close()
}
