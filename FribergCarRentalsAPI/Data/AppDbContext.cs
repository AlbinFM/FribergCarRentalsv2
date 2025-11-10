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
            
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = "2a5caf8a-5db7-4cfa-9089-72eb577a682e",
                    Name = ApiRoles.User,
                    NormalizedName = ApiRoles.User.ToUpper()
                },
                new IdentityRole
                {
                    Id = "f0a8206b-bccb-4ad8-a331-ee48429c2286",
                    Name = ApiRoles.Admin,
                    NormalizedName = ApiRoles.Admin.ToUpper()
                }
            );
            
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

            builder.Entity<Models.Booking>()
                .HasIndex(b => new { b.CarId, b.IsConfirmed, b.StartDate, b.EndDate });

            builder.Entity<Models.Booking>()
                .HasIndex(b => new { b.CustomerId, b.StartDate, b.EndDate });
        }
    }
}
