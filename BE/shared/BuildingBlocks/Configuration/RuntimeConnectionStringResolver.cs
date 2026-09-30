using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace BuildingBlocks.Configuration;

public static class RuntimeConnectionStringResolver
{
    public static string GetRequiredConnectionString(IConfiguration configuration, string consumerName)
    {
        var connectionString = ResolveConnectionString(configuration);
        if (!string.IsNullOrWhiteSpace(connectionString))
        {
            return connectionString;
        }

        throw new InvalidOperationException(
            $"Khong tim thay ConnectionStrings:DefaultConnection cho {consumerName}. " +
            "Hay cau hinh appsettings.json/appsettings.Development.json hoac bien moi truong DEFAULT_CONNECTION / ConnectionStrings__DefaultConnection.");
    }

    public static string? ResolveConnectionString(IConfiguration? configuration = null)
    {
        var fromConfiguration = configuration?.GetSection("ConnectionStrings")["DefaultConnection"];
        if (!string.IsNullOrWhiteSpace(fromConfiguration))
        {
            return fromConfiguration;
        }

        var fromNestedEnvironment = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
        if (!string.IsNullOrWhiteSpace(fromNestedEnvironment))
        {
            return fromNestedEnvironment;
        }

        var fromEnvironment = Environment.GetEnvironmentVariable("DEFAULT_CONNECTION");
        if (!string.IsNullOrWhiteSpace(fromEnvironment))
        {
            return fromEnvironment;
        }

        foreach (var candidate in BuildCandidatePaths())
        {
            var value = TryReadConnectionString(candidate);
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
        }

        return null;
    }

    private static IEnumerable<string> BuildCandidatePaths()
    {
        var baseDirectory = AppContext.BaseDirectory;
        var currentDirectory = Directory.GetCurrentDirectory();
        var projectNames = new[]
        {
            "Backend.Api",
            "DanhMucService",
            "QuanTriHeThongService",
            "DangKyVanBanService",
            "XayDungVanBanService"
        };

        foreach (var projectName in projectNames)
        {
            yield return Path.Combine(currentDirectory, projectName, "appsettings.json");
            yield return Path.Combine(currentDirectory, projectName, "appsettings.Development.json");
            yield return Path.Combine(currentDirectory, projectName, "bin", "Debug", "net10.0", "appsettings.json");
            yield return Path.Combine(currentDirectory, projectName, "bin", "Debug", "net10.0", "appsettings.Development.json");

            yield return Path.Combine(currentDirectory, "appsettings.json");
            yield return Path.Combine(currentDirectory, "appsettings.Development.json");

            yield return Path.GetFullPath(Path.Combine(baseDirectory, "..", "..", "..", "appsettings.json"));
            yield return Path.GetFullPath(Path.Combine(baseDirectory, "..", "..", "..", "appsettings.Development.json"));
            yield return Path.GetFullPath(Path.Combine(baseDirectory, "..", "..", "..", projectName, "appsettings.json"));
            yield return Path.GetFullPath(Path.Combine(baseDirectory, "..", "..", "..", projectName, "appsettings.Development.json"));
            yield return Path.GetFullPath(Path.Combine(baseDirectory, "..", "..", "..", "..", projectName, "appsettings.json"));
            yield return Path.GetFullPath(Path.Combine(baseDirectory, "..", "..", "..", "..", projectName, "appsettings.Development.json"));
        }
    }

    private static string? TryReadConnectionString(string candidate)
    {
        if (!File.Exists(candidate))
        {
            return null;
        }

        try
        {
            using var stream = File.OpenRead(candidate);
            using var document = JsonDocument.Parse(stream);
            if (document.RootElement.TryGetProperty("ConnectionStrings", out var connectionStrings)
                && connectionStrings.TryGetProperty("DefaultConnection", out var defaultConnection))
            {
                return defaultConnection.GetString();
            }
        }
        catch
        {
            return null;
        }

        return null;
    }
}
