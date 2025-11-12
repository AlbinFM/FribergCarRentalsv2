using FribergCarRentalsMVC.DTOs;

namespace FribergCarRentalsMVC.Services
{
    public interface IAuthService
    {
        Task<bool> Login(CustomersAllDto.LoginDto dto, bool fetchProfile = true, CancellationToken ct = default);
        Task<bool> Register(CustomersAllDto.CustomerRegisterDto dto, CancellationToken ct = default);
        Task<bool> AdminLogin(AdminLoginDto dto, CancellationToken ct = default);
        Task Logout();
        bool IsAdmin();

    }
}