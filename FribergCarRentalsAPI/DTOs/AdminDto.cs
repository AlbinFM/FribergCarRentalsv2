using System.ComponentModel.DataAnnotations;

namespace FribergCarRentalsAPI.DTOs
{
    public class AdminDto
    {
        public int  Id { get; set; }
        
        [Required, EmailAddress]
        public string Email { get; set; }
    }
}