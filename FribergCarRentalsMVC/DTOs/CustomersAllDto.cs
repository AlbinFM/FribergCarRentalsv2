using System.ComponentModel.DataAnnotations;

namespace FribergCarRentalsMVC.DTOs
{
    public class CustomersAllDto
    {

        public class CustomerRegisterDto
        {
            [Required] public string FullName { get; set; } = string.Empty;
            [Required, EmailAddress] public string Email { get; set; } = string.Empty;
            
            [Required, DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;
        }
        
        public class CustomerDto
        {
            public int Id { get; set; }
            public string FullName { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
        }
        
        public class LoginDto
        {
            [Required, EmailAddress] public string Email { get; set; } = string.Empty;
 
            [Required, DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;
        }
    }
}