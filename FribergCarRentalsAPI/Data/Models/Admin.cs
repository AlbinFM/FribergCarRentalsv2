using System.ComponentModel.DataAnnotations;

namespace FribergCarRentalsAPI.Data.Models
{
    public class Admin
    {
        public int Id { get; set; } // Id för admin

        [Required, EmailAddress]
        public required string Email { get; set; } // E-postadress för admin

        public string ApiUserId { get; set; }
       
        public ApiUser ApiUser { get; set; }
    }
}