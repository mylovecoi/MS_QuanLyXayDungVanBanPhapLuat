using System.Net.Http.Json;
using System.Text.Json;

namespace XayDungVanBanService.Infrastructure.DanhMuc;

public sealed class DanhMucTrangThaiClient(HttpClient httpClient) : IDanhMucTrangThaiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<DanhMucTrangThaiItem?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await httpClient.GetFromJsonAsync<Envelope>($"api/danh-muc/trang-thai/{id}", JsonOptions, cancellationToken);
            return result?.IsSuccess == true ? result.Data : null;
        }
        catch (HttpRequestException)
        {
            return null;
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private sealed class Envelope
    {
        public bool IsSuccess { get; init; }
        public DanhMucTrangThaiItem? Data { get; init; }
    }
}
