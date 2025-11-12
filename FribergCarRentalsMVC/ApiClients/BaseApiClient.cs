using System.Net;

namespace FribergCarRentalsMVC.ApiClients
{
    public abstract class BaseApiClient
    {
        private readonly HttpClient _http;

        protected BaseApiClient(HttpClient http) => _http = http;

        protected async Task<T?> GetAsync<T>(string url, CancellationToken ct = default)
        {
            using var resp = await _http.GetAsync(url, ct);
            if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new UnauthorizedAccessException();
            if (!resp.IsSuccessStatusCode) throw new HttpRequestException(await resp.Content.ReadAsStringAsync());
            return await resp.Content.ReadFromJsonAsync<T>(cancellationToken: ct);
        }

        protected async Task<T?> PostAsync<T>(string url, object? body = null, CancellationToken ct = default)
        {
            using var resp = body is null
                ? await _http.PostAsync(url, null, ct)
                : await _http.PostAsJsonAsync(url, body, ct);

            if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new UnauthorizedAccessException();
            if (!resp.IsSuccessStatusCode) throw new HttpRequestException(await resp.Content.ReadAsStringAsync());
            return await resp.Content.ReadFromJsonAsync<T>(cancellationToken: ct);
        }
        

        protected async Task DeleteAsync(string url, CancellationToken ct = default)
        {
            using var resp = await _http.DeleteAsync(url, ct);
            if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new UnauthorizedAccessException(); 
            if (!resp.IsSuccessStatusCode) throw new HttpRequestException(await resp.Content.ReadAsStringAsync());
        }
    }
}