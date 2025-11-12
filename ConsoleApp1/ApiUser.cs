namespace consoleapp1
{
    using Microsoft.AspNetCore.Identity;

    public class ApiUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}