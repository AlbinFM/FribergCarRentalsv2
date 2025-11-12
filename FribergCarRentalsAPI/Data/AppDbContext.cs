using FribergCarRentalsAPI.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FribergCarRentalsAPI.Data
{
    public class AppDbContext : IdentityDbContext<ApiUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Models.Customer> Customers { get; set; }
        public DbSet<Models.Car> Cars { get; set; }
        public DbSet<Models.Booking> Bookings { get; set; }
       
        public DbSet<Models.Admin> Admins { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // --- Roller ---
            var roleAdminId = "f0a8206b-bccb-4ad8-a331-ee48429c2286";
            var roleUserId  = "2a5caf8a-5db7-4cfa-9089-72eb577a682e";

            builder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = roleUserId,  Name = ApiRoles.User,  NormalizedName = "USER",  ConcurrencyStamp = roleUserId },
                new IdentityRole { Id = roleAdminId, Name = ApiRoles.Admin, NormalizedName = "ADMIN", ConcurrencyStamp = roleAdminId }
            );

            // --- Admin user ---
            var adminUserId = "a1111111-2222-3333-4444-555555555555";
            const string adminEmail = "admin@friberg.com";
            const string adminUserName = adminEmail;
            const string passwordHash = "AQAAAAIAAYagAAAAEFjS3xI7GhAbTdQeT9sqXzx/22Prm5fw0yamO/uGJ2u56Pw00SomEjdLz/rwaPvN7A==";

            builder.Entity<ApiUser>().HasData(new ApiUser
            {
                Id = adminUserId,
                UserName = adminUserName,
                NormalizedUserName = adminUserName.ToUpper(),
                Email = adminEmail,
                NormalizedEmail = adminEmail.ToUpper(),
                EmailConfirmed = true,
                PasswordHash = passwordHash,
                SecurityStamp = "SECURITY-STAMP-ADMIN",
                ConcurrencyStamp = "CONCURRENCY-STAMP-ADMIN",
                FirstName = "System",
                LastName = "Administrator"
            });
            
            builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                UserId = adminUserId,
                RoleId = roleAdminId
            });
            
            builder.Entity<Models.Admin>().HasData(new Models.Admin
            {
                Id = 1,
                ApiUserId = adminUserId,
                Email = adminEmail
            });
            
            builder.Entity<Models.Customer>()
                .HasOne(c => c.ApiUser)
                .WithMany()
                .HasForeignKey(c => c.ApiUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Models.Admin>()
                .HasOne(a => a.ApiUser)
                .WithMany()
                .HasForeignKey(a => a.ApiUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Models.Booking>()
                .HasOne(b => b.Car)
                .WithMany(c => c.Bookings)
                .HasForeignKey(b => b.CarId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Models.Booking>()
                .HasOne(b => b.Customer)
                .WithMany(c => c.Bookings)
                .HasForeignKey(b => b.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.Entity<Models.Customer>()
                .HasIndex(c => c.ApiUserId)
                .IsUnique();
     
            builder.Entity<Models.Admin>()
                .HasIndex(a => a.ApiUserId)
                .IsUnique();

            builder.Entity<Models.Booking>()
                .HasIndex(b => new { b.CarId, b.IsConfirmed, b.StartDate, b.EndDate });

            builder.Entity<Models.Booking>()
                .HasIndex(b => new { b.CustomerId, b.StartDate, b.EndDate });
        }
    }
}
