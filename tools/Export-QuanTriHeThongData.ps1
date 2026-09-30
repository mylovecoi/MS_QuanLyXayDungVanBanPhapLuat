param(
    [string]$Server = "14.248.85.58",
    [string]$SourceDatabase = "CSDLXayDungVanBan",
    [string]$TargetDatabase = "CSDLXayDungVanBan_MS",
    [string]$Username = "sa",
    [string]$Password,
    [string]$OutputPath = "BE/services/QuanTriHeThongService/Infrastructure/Persistence/Data/qtht_data_from_CSDLXayDungVanBan.sql"
)

if ([string]::IsNullOrWhiteSpace($Password)) {
    throw "Password is required."
}

$ErrorActionPreference = "Stop"

$tables = @(
    "GroupsPermision",
    "Logs",
    "OptionDatas",
    "Permission",
    "QuestionAnswers",
    "RoleActions",
    "SystemInfo",
    "Users"
)

function Format-SqlLiteral {
    param([object]$Value)

    if ($null -eq $Value -or $Value -is [DBNull]) {
        return "NULL"
    }

    if ($Value -is [bool]) {
        if ($Value) { return "1" }
        return "0"
    }

    if ($Value -is [Guid]) {
        return "'" + $Value.ToString() + "'"
    }

    if ($Value -is [DateTime]) {
        return "'" + $Value.ToString("yyyy-MM-ddTHH:mm:ss.fffffff", [System.Globalization.CultureInfo]::InvariantCulture) + "'"
    }

    if ($Value -is [byte[]]) {
        if ($Value.Length -eq 0) {
            return "0x"
        }

        return "0x" + (($Value | ForEach-Object { $_.ToString("X2") }) -join "")
    }

    if ($Value -is [sbyte] -or $Value -is [byte] -or
        $Value -is [int16] -or $Value -is [uint16] -or
        $Value -is [int32] -or $Value -is [uint32] -or
        $Value -is [int64] -or $Value -is [uint64] -or
        $Value -is [single] -or $Value -is [double] -or
        $Value -is [decimal]) {
        return [System.Convert]::ToString($Value, [System.Globalization.CultureInfo]::InvariantCulture)
    }

    $text = [string]$Value
    return "N'" + $text.Replace("'", "''") + "'"
}

function Quote-Name {
    param([string]$Name)
    return "[" + $Name.Replace("]", "]]") + "]"
}

$connectionString = "Server=$Server;Database=$SourceDatabase;User ID=$Username;Password=$Password;MultipleActiveResultSets=true;TrustServerCertificate=True;"
$connection = [System.Data.SqlClient.SqlConnection]::new($connectionString)
$connection.Open()

try {
    $outputFullPath = Join-Path (Get-Location) $OutputPath
    $outputDirectory = Split-Path -Parent $outputFullPath
    New-Item -ItemType Directory -Force -Path $outputDirectory | Out-Null

    $lines = [System.Collections.Generic.List[string]]::new()
    $lines.Add("-- Data export for QuanTriHeThongService")
    $lines.Add("-- Source: [$SourceDatabase].[dbo]")
    $lines.Add("-- Target: [$TargetDatabase].[qtht]")
    $lines.Add("-- Generated: " + (Get-Date).ToString("yyyy-MM-dd HH:mm:ss"))
    $lines.Add("")
    $lines.Add("USE " + (Quote-Name $TargetDatabase) + ";")
    $lines.Add("GO")
    $lines.Add("SET XACT_ABORT ON;")
    $lines.Add("BEGIN TRANSACTION;")
    $lines.Add("")

    foreach ($table in $tables) {
        $command = $connection.CreateCommand()
        $command.CommandText = "SELECT * FROM [dbo]." + (Quote-Name $table) + " ORDER BY [Id];"
        $reader = $command.ExecuteReader()

        try {
            $schema = $reader.GetSchemaTable()
            $columns = @()
            foreach ($row in $schema.Rows) {
                $columns += [string]$row["ColumnName"]
            }

            $quotedColumns = ($columns | ForEach-Object { Quote-Name $_ }) -join ", "
            $rowCount = 0

            $lines.Add("-- " + $table)
            while ($reader.Read()) {
                $values = @()
                foreach ($column in $columns) {
                    $values += Format-SqlLiteral $reader[$column]
                }

                $idLiteral = Format-SqlLiteral $reader["Id"]
                $lines.Add("IF NOT EXISTS (SELECT 1 FROM [qtht]." + (Quote-Name $table) + " WHERE [Id] = $idLiteral)")
                $lines.Add("BEGIN")
                $lines.Add("    INSERT INTO [qtht]." + (Quote-Name $table) + " ($quotedColumns)")
                $lines.Add("    VALUES (" + ($values -join ", ") + ");")
                $lines.Add("END")
                $rowCount++
            }

            $lines.Add("-- Rows exported from dbo.${table}: $rowCount")
            $lines.Add("")
        }
        finally {
            $reader.Close()
        }
    }

    $lines.Add("COMMIT TRANSACTION;")
    $lines.Add("GO")

    [System.IO.File]::WriteAllLines($outputFullPath, $lines, [System.Text.UTF8Encoding]::new($false))
    Write-Output $outputFullPath
}
finally {
    $connection.Close()
}
