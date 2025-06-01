using Microsoft.EntityFrameworkCore;
using ParkingSystem.Core.Aggregates;

namespace ParkingSystem.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> users { get; set; }
        public DbSet<Vehicle> vehicle { get; set; }
        public DbSet<Park> park { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // modelBuilder.Entity<User>().HasKey(u => u.Id);
            // modelBuilder.Entity<Vehicle>().HasKey(v => v.Id);
            // modelBuilder.Entity<Park>().HasKey(p => p.Id);

        }
    }
}
