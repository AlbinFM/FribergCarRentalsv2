using FribergCarRentalsMVC.DTOs;

namespace FribergCarRentalsMVC.ApiClients.Interfaces
{
    public interface IAuthApiClient
    {
        Task<AuthResponseDto> Login(CustomersAllDto.LoginDto dto, CancellationToken ct = default);
        Task<AuthResponseDto> AdminLogin(CustomersAllDto.LoginDto dto, CancellationToken ct = default);
        Task<CustomersAllDto.CustomerDto> GetMe(CancellationToken ct = default);
    }
}