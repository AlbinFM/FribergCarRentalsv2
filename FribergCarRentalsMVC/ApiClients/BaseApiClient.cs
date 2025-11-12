using System.Net;

namespace FribergCarRentalsMVC.ApiClients
{
    public abstract class BaseApiClient
    {
        protected readonly HttpClient Http;

        protected BaseApiClient(HttpClient http) => Http = http;

        protected async Task<T?> GetAsync<T>(string url, CancellationToken ct = default)
        {
            using var resp = await Http.GetAsync(url, ct);
            if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new UnauthorizedAccessException();
            if (!resp.IsSuccessStatusCode) throw new HttpRequestException(await resp.Content.ReadAsStringAsync());
            return await resp.Content.ReadFromJsonAsync<T>(cancellationToken: ct);
        }

        protected async Task<T?> PostAsync<T>(string url, object? body = null, CancellationToken ct = default)
        {
            using var resp = body is null
                ? await Http.PostAsync(url, null, ct)
                : await Http.PostAsJsonAsync(url, body, ct);

            if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new UnauthorizedAccessException();
            if (!resp.IsSuccessStatusCode) throw new HttpRequestException(await resp.Content.ReadAsStringAsync());
            return await resp.Content.ReadFromJsonAsync<T>(cancellationToken: ct);
        }

        protected async Task PutAsync(string url, object? body = null, CancellationToken ct = default)
        {
            using var resp = body is null
                ? await Http.PutAsync(url, null, ct)
                : await Http.PutAsJsonAsync(url, body, ct);

            if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new UnauthorizedAccessException();
            if (!resp.IsSuccessStatusCode) throw new HttpRequestException(await resp.Content.ReadAsStringAsync());
        }

        protected async Task DeleteAsync(string url, CancellationToken ct = default)
        {
            using var resp = await Http.DeleteAsync(url, ct);
            if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new UnauthorizedAccessException();
            if (!resp.IsSuccessStatusCode) throw new HttpRequestException(await resp.Content.ReadAsStringAsync());
        }
    }
}