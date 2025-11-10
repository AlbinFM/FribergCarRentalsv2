using System.Net.Http.Json;
using FribergCarRentalsMVC.ApiClients.Interfaces;
using FribergCarRentalsMVC.DTOs;

namespace FribergCarRentalsMVC.ApiClients
{
    public class CustomerApiClient : ICustomerApiClient
    {
        private readonly HttpClient _http;
        public CustomerApiClient(HttpClient http) => _http = http;

        public async Task<CustomersAllDto.CustomerDto?> Register(
            CustomersAllDto.CustomerRegisterDto dto,
            CancellationToken ct = default)
        {
            using var resp = await _http.PostAsJsonAsync("api/customer/register", dto, ct);
            if (resp.StatusCode == System.Net.HttpStatusCode.Conflict)
                throw new InvalidOperationException("Email already in use.");
            if (!resp.IsSuccessStatusCode)
                throw new HttpRequestException($"Register failed: {(int)resp.StatusCode} {resp.ReasonPhrase}");

            return await resp.Content.ReadFromJsonAsync<CustomersAllDto.CustomerDto>(cancellationToken: ct);
        }
    }
}