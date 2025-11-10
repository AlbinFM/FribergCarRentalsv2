// IAuthService.cs
using FribergCarRentalsMVC.DTOs;

namespace FribergCarRentalsMVC.Services
{
    public interface IAuthService
    {
        Task<bool> Login(CustomersAllDto.LoginDto dto, bool fetchProfile = true, CancellationToken ct = default); 
        Task<bool> LoginAdmin(CustomersAllDto.LoginDto dto, CancellationToken ct = default); 
        Task<bool> RegisterAndLogin(CustomersAllDto.CustomerRegisterDto dto, CancellationToken ct = default);
        Task Logout();

        bool IsAuthenticated();
        bool IsAdmin();
        int? GetCustomerId();
        string? GetEmail();
    }
}