namespace FribergCarRentalsAPI.Data
{
    using Microsoft.AspNetCore.Identity;

    public class ApiUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}