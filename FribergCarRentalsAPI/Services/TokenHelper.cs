using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FribergCarRentalsAPI.Data;
using Microsoft.IdentityModel.Tokens;

namespace FribergCarRentalsAPI.Services;

public static class TokenHelper
{
    public static (string token, DateTime expiresAt) Generate(ApiUser user, IEnumerable<string> roles, IConfiguration cfg)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(cfg["JwtSettings:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
        };
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var expires = DateTime.UtcNow.AddHours(8);

        var jwt = new JwtSecurityToken(
            issuer: cfg["JwtSettings:Issuer"],
            audience: cfg["JwtSettings:Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds);

        return (new JwtSecurityTokenHandler().WriteToken(jwt), expires);
    }
}