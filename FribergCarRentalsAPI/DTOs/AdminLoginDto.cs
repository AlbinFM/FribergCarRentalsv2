using System.ComponentModel.DataAnnotations;

namespace FribergCarRentalsAPI.DTOs
{
    public class AdminLoginDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}