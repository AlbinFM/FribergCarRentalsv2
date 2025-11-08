using System.ComponentModel.DataAnnotations;

namespace FribergCarRentalsAPI.Models
{
    public class Admin
    {
        public int Id { get; set; } // Id för admin

        [Required, EmailAddress]
        public required string Email { get; set; } // E-postadress för admin

        [Required]
        public required string Password { get; set; } // Lösenord för admin
    }
}