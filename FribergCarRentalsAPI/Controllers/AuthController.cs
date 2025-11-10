using System.Security.Claims;
using FribergCarRentalsAPI.Constants;
using FribergCarRentalsAPI.Data;
using FribergCarRentalsAPI.DTOs;
using FribergCarRentalsAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FribergCarRentalsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApiUser> _userManager;
        private readonly IConfiguration _config;

        public AuthController(UserManager<ApiUser> userManager, IConfiguration config)
        {
            _userManager = userManager;
            _config = config;
        }

// POST: api/auth/login
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] CustomerLoginDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user is null) return Unauthorized();

            var valid = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!valid) return Unauthorized();

            var roles = await _userManager.GetRolesAsync(user);
            var (token, expiresAt) = TokenHelper.Generate(user, roles, _config);

            return Ok(new AuthResponseDto
            {
                Token = token,
                ExpiresAt = expiresAt,
                Roles = roles.ToList() 
            });
        }

        // POST: api/auth/admin/login
        [HttpPost("admin/login")]
        [AllowAnonymous]
        public async Task<IActionResult> AdminLogin([FromBody] AdminLoginDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user is null) return Unauthorized();

            var valid = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!valid) return Unauthorized();

            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Contains(ApiRoles.Admin))
                return Forbid();

            var (token, expiresAt) = TokenHelper.Generate(user, roles, _config);

            return Ok(new AuthResponseDto
            {
                Token = token,
                ExpiresAt = expiresAt,
                Roles = roles.ToList()
            });
        }

    }
}