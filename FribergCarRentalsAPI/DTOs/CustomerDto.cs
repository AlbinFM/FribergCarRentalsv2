using System.ComponentModel.DataAnnotations;

namespace FribergCarRentalsAPI.DTOs
{
    public class CustomerDto
    {
        public int Id { get; set; }
        
        [Required]
        public string FullName { get; set; }
        
        [Required, EmailAddress]
        public string Email { get; set; }
    }
}