using FribergCarRentalsMVC.ApiClients.Interfaces;
using FribergCarRentalsMVC.DTOs;

namespace FribergCarRentalsMVC.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthApiClient _authApi;
        private readonly IHttpContextAccessor _ctx;

        public AuthService(IAuthApiClient authApi, IHttpContextAccessor ctx)
        {
            _authApi = authApi;
            _ctx = ctx;
        }

        public async Task<bool> Login(CustomersAllDto.LoginDto dto, bool fetchProfile = true, CancellationToken ct = default)
        {
            try
            {
                var auth = await _authApi.Login(dto, ct);
                SaveAuth(auth, dto.Email);

                if (fetchProfile)
                {
                    var me = await _authApi.GetMe(ct);
                    _ctx.HttpContext!.Session.SetInt32("CustomerId", me.Id);
                }
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }
        
        public async Task<bool> AdminLogin(AdminLoginDto dto, CancellationToken ct = default)
        {
            try
            {
                var auth = await _authApi.AdminLogin(dto, ct);
                SaveAuth(auth, dto.Email);
                return IsAdmin();
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }

        public async Task<bool> Register(CustomersAllDto.CustomerRegisterDto dto, CancellationToken ct = default)
        {
            var created = await _authApi.Register(dto, ct);
            if (created is null)
                throw new HttpRequestException("Registration returned no content.");
            
            return await Login(new CustomersAllDto.LoginDto
            {
                Email = dto.Email,
                Password = dto.Password
            }, fetchProfile: true, ct);
        }

        public Task Logout()
        {
            _ctx.HttpContext!.Session.Clear();
            return Task.CompletedTask;
        }

        public bool IsAdmin()
        {
            var rolesCsv = _ctx.HttpContext!.Session.GetString("Roles") ?? "";
            return rolesCsv
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Any(r => r.Equals("Admin", StringComparison.OrdinalIgnoreCase));
        }
        
        private void SaveAuth(AuthResponseDto auth, string email)
        {
            var http = _ctx.HttpContext!;
            http.Session.SetString("Jwt", auth.Token);
            http.Session.SetString("JwtExpiresAt", auth.ExpiresAt.ToString("o"));
            http.Session.SetString("UserEmail", email);

            var roles = auth.Roles ?? new List<string>();
            http.Session.SetString("Roles", string.Join(",", roles));
        }
    }
}
