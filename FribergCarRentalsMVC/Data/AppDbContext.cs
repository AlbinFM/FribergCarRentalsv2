using Microsoft.EntityFrameworkCore;

namespace FribergCarRentalsMVC.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){ }

        public DbSet<Models.Customer> Customers { get; set; }
        public DbSet<Models.Car> Cars { get; set; } 
        public DbSet<Models.Booking> Bookings { get; set; } 
        public DbSet<Models.Admin> Admins { get; set; } 
    }
}