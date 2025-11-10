using System.ComponentModel.DataAnnotations;

namespace FribergCarRentalsAPI.DTOs
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        
        public List<string>? Roles { get; set; } = new();
    }
}