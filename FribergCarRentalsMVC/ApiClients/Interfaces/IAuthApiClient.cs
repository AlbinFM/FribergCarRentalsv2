using FribergCarRentalsMVC.DTOs;

namespace FribergCarRentalsMVC.ApiClients.Interfaces
{
    public interface IAuthApiClient
    {
        Task<AuthResponseDto> Login(CustomersAllDto.LoginDto dto, CancellationToken ct = default);
        Task<AuthResponseDto> AdminLogin(AdminLoginDto dto, CancellationToken ct = default);
        Task<CustomersAllDto.CustomerDto> GetMe(CancellationToken ct = default);
        Task<CustomersAllDto.CustomerDto> Register(CustomersAllDto.CustomerRegisterDto dto, CancellationToken ct = default);
    }
}