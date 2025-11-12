using System.ComponentModel.DataAnnotations;

namespace FribergCarRentalsMVC.DTOs
{
    public class AdminLoginDto
    {
        [Required, EmailAddress] public string Email { get; set; } = string.Empty;
       
        [Required, DataType(DataType.Password)] public string Password { get; set; } = string.Empty;
    }
}