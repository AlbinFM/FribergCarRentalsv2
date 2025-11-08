using FribergCarRentalsAPI.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FribergCarRentalsAPI.Data
{
    public class IdentityDbContext : IdentityDbContext<ApiUser>
    {
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
            : base(options)
        {
        }

        public DbSet<Models.Customer> Customers { get; set; }
        public DbSet<Models.Car> Cars { get; set; }
        public DbSet<Models.Booking> Bookings { get; set; }
        public DbSet<Models.Admin> Admins { get; set; }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Name = ApiRoles.Customer,
                    NormalizedName = ApiRoles.Customer,
                    Id = "2a5caf8a-5db7-4cfa-9089-72eb577a682e"
                },
                new IdentityRole
                {
                    Name = ApiRoles.Admin,
                    NormalizedName = ApiRoles.Admin,
                    Id = "f0a8206b-bccb-4ad8-a331-ee48429c2286"
                }
            );
            var hasher = new PasswordHasher<ApiUser>();
            builder.Entity<ApiUser>().HasData(
                new ApiUser
                {
                    Id = "d0ccb9bb-4baa-40e0-bdd3-346d7f84e9e0",
                    Email = "admin@friberg.com",
                    NormalizedEmail = "ADMIN@FRIBERG.COM",
                    UserName = "admin@friberg.com",
                    NormalizedUserName = "ADMIN@FRIBERG.COM",
                    FirstName = "System",
                    LastName = "Administrator",
                    PasswordHash = hasher.HashPassword(null, "Test1234")
                },
                new ApiUser
                {
                    Id = "82a28a9f-fee6-4200-a9d0-6ff23813222c",
                    Email = "customer@friberg.com",
                    NormalizedEmail = "CUSTOMER@FRIBERG.COM",
                    UserName = "customer@friberg.com",
                    NormalizedUserName = "CUSTOMER@FRIBERG.COM",
                    FirstName = "system",
                    LastName = "customer",
                    PasswordHash = hasher.HashPassword(null, "Test1234")
                }
            );

            builder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>
                {
                    RoleId = "2a5caf8a-5db7-4cfa-9089-72eb577a682e",
                    UserId = "82a28a9f-fee6-4200-a9d0-6ff23813222c"
                },
                new IdentityUserRole<string>
                {
                    RoleId = "f0a8206b-bccb-4ad8-a331-ee48429c2286",
                    UserId = "d0ccb9bb-4baa-40e0-bdd3-346d7f84e9e0"
                }
            );
        }
    }
}