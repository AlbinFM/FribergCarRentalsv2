using FribergCarRentalsMVC.ApiClients.Interfaces;
using FribergCarRentalsMVC.DTOs;

namespace FribergCarRentalsMVC.ApiClients
{
    public class AuthApiClient : BaseApiClient, IAuthApiClient
    {
        public AuthApiClient(HttpClient http) : base(http) { }

        public Task<AuthResponseDto> Login(CustomersAllDto.LoginDto dto, CancellationToken ct = default)
            => PostAsync<AuthResponseDto>("api/auth/login", dto, ct)!;

        public Task<AuthResponseDto> AdminLogin(CustomersAllDto.LoginDto dto, CancellationToken ct = default)
            => PostAsync<AuthResponseDto>("api/auth/admin/login", dto, ct)!;

        public Task<CustomersAllDto.CustomerDto> GetMe(CancellationToken ct = default)
            => GetAsync<CustomersAllDto.CustomerDto>("api/customer/me", ct)!;
    }
}