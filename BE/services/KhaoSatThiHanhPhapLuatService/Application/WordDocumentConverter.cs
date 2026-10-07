using System.Diagnostics;

namespace KhaoSatThiHanhPhapLuatService.Application;

public interface IWordDocumentConverter { Task<string> ConvertDocToDocxAsync(string sourcePath, string outputDirectory, CancellationToken ct); }
public sealed class WordDocumentConverter : IWordDocumentConverter
{
    public async Task<string> ConvertDocToDocxAsync(string sourcePath, string outputDirectory, CancellationToken ct)
    {
        var executable = OperatingSystem.IsWindows() ? "soffice.exe" : "soffice";
        var start = new ProcessStartInfo(executable, $"--headless --convert-to docx --outdir \"{outputDirectory}\" \"{sourcePath}\"") { RedirectStandardError = true, RedirectStandardOutput = true, UseShellExecute = false, CreateNoWindow = true };
        try { using var process = Process.Start(start) ?? throw new InvalidOperationException("Không thể khởi chạy LibreOffice."); await process.WaitForExitAsync(ct); if (process.ExitCode != 0) throw new InvalidOperationException((await process.StandardError.ReadToEndAsync(ct)).Trim()); }
        catch (System.ComponentModel.Win32Exception) { throw new InvalidOperationException("Máy chủ chưa cài LibreOffice headless để chuyển file .doc."); }
        var target = Path.Combine(outputDirectory, Path.GetFileNameWithoutExtension(sourcePath) + ".docx");
        if (!File.Exists(target)) throw new InvalidOperationException("LibreOffice không tạo được file DOCX."); return target;
    }
}
