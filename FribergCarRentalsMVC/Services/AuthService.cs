// AuthService.cs
using FribergCarRentalsMVC.ApiClients.Interfaces;
using FribergCarRentalsMVC.DTOs;

namespace FribergCarRentalsMVC.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthApiClient _authApi;
        private readonly ICustomerApiClient _customerApi;
        private readonly IHttpContextAccessor _ctx;

        public AuthService(IAuthApiClient authApi, ICustomerApiClient customerApi, IHttpContextAccessor ctx)
        {
            _authApi = authApi;
            _customerApi = customerApi;
            _ctx = ctx;
        }

        // Kund-login
        public async Task<bool> Login(CustomersAllDto.LoginDto dto, bool fetchProfile = true, CancellationToken ct = default)
        {
            var auth = await _authApi.Login(dto, ct);
            SaveAuth(auth, dto.Email);

            if (fetchProfile)
            {
                try
                {
                    var me = await _authApi.GetMe(ct);
                    _ctx.HttpContext!.Session.SetInt32("CustomerId", me.Id);
                }
                catch
                {
                    // Ignorera 401/404 – t.ex. om profilen saknas
                }
            }
            return true;
        }

        // Admin-login (ingen GetMe – admins har ofta ingen kundprofil)
        public async Task<bool> LoginAdmin(CustomersAllDto.LoginDto dto, CancellationToken ct = default)
        {
            var auth = await _authApi.AdminLogin(dto, ct);
            SaveAuth(auth, dto.Email);
            return true;
        }

        public async Task<bool> RegisterAndLogin(CustomersAllDto.CustomerRegisterDto dto, CancellationToken ct = default)
        {
            try
            {
                var created = await _customerApi.Register(dto, ct);
                if (created is null) throw new HttpRequestException("Registration returned no content.");
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Email already in use", StringComparison.OrdinalIgnoreCase))
            {
                throw;
            }

            return await Login(new CustomersAllDto.LoginDto { Email = dto.Email, Password = dto.Password }, fetchProfile: true, ct);
        }

        public Task Logout()
        {
            _ctx.HttpContext!.Session.Clear();
            return Task.CompletedTask;
        }

        public bool IsAuthenticated()
            => !string.IsNullOrEmpty(_ctx.HttpContext!.Session.GetString("Jwt"));

        public bool IsAdmin()
            => (_ctx.HttpContext!.Session.GetString("Roles") ?? "")
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Any(r => r.Equals("Admin", StringComparison.OrdinalIgnoreCase));

        public int? GetCustomerId() => _ctx.HttpContext!.Session.GetInt32("CustomerId");
        public string? GetEmail() => _ctx.HttpContext!.Session.GetString("UserEmail");

        private void SaveAuth(AuthResponseDto auth, string email)
        {
            var http = _ctx.HttpContext!;
            http.Session.SetString("Jwt", auth.Token);
            http.Session.SetString("JwtExpiresAt", auth.ExpiresAt.ToString("o"));
            http.Session.SetString("UserEmail", email);
            http.Session.SetString("Roles", string.Join(",", auth.Roles ?? new List<string>()));
        }
    }
}
