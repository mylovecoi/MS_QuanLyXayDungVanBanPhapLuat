param(
    [string]$Server = "192.168.100.250",
    [string]$Database = "CSDLXayDungVanBan_MS",
    [string]$Username = "sa",
    [string]$Password,
    [string]$SqlPath = "BE/services/DanhMucService/Infrastructure/Persistence/Data/seed_all_service_trang_thai.sql"
)

if ([string]::IsNullOrWhiteSpace($Password)) {
    throw "Password is required."
}

$ErrorActionPreference = "Stop"

$fullSqlPath = Join-Path (Get-Location) $SqlPath
if (-not (Test-Path -LiteralPath $fullSqlPath)) {
    throw "SQL file not found: $fullSqlPath"
}

$connectionString = "Server=$Server;Database=$Database;User ID=$Username;Password=$Password;MultipleActiveResultSets=true;TrustServerCertificate=True;"
$sql = [System.IO.File]::ReadAllText($fullSqlPath, [System.Text.Encoding]::UTF8)

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

$connection = [System.Data.SqlClient.SqlConnection]::new($connectionString)
$connection.Open()

try {
    $beforeCommand = $connection.CreateCommand()
    $beforeCommand.CommandText = "SELECT COUNT(1) FROM [dm].[DanhMucTrangThais];"
    $before = [int]$beforeCommand.ExecuteScalar()

    $batches = Split-SqlBatches $sql
    foreach ($batch in $batches) {
        $command = $connection.CreateCommand()
        $command.CommandTimeout = 120
        $command.CommandText = $batch
        $reader = $command.ExecuteReader()

        try {
            while ($reader.Read()) {
                if ($reader.FieldCount -ge 2 -and $reader.GetName(0) -eq "NhomTrangThai") {
                    Write-Output ("{0}: {1}" -f $reader["NhomTrangThai"], $reader["SoLuong"])
                }
            }
        }
        finally {
            $reader.Close()
        }
    }

    $afterCommand = $connection.CreateCommand()
    $afterCommand.CommandText = "SELECT COUNT(1) FROM [dm].[DanhMucTrangThais];"
    $after = [int]$afterCommand.ExecuteScalar()

    Write-Output "Database: [$Database]"
    Write-Output "Rows before: $before"
    Write-Output "Rows after: $after"
    Write-Output "Rows inserted: $($after - $before)"
}
finally {
    $connection.Close()
}
