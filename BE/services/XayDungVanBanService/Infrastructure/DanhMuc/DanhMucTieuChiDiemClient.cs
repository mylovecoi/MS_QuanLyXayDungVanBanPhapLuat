using System.Net.Http.Json;
using System.Text.Json;

namespace XayDungVanBanService.Infrastructure.DanhMuc;

public sealed class DanhMucTieuChiDiemClient(HttpClient httpClient) : IDanhMucTieuChiDiemClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<IReadOnlyList<DanhMucTieuChiDiemItem>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await httpClient.GetFromJsonAsync<Envelope>("api/danh-muc/tieu-chi-diem/options", JsonOptions, cancellationToken);
            return result?.IsSuccess == true
                ? result.Data?.Where(x => x.TrangThai).ToList() ?? []
                : [];
        }
        catch (HttpRequestException) { return []; }
        catch (JsonException) { return []; }
    }

    private sealed class Envelope
    {
        public bool IsSuccess { get; init; }
        public List<DanhMucTieuChiDiemItem>? Data { get; init; }
    }
}
