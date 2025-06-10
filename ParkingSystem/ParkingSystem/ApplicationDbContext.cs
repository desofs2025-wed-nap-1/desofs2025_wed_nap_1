using Microsoft.EntityFrameworkCore;
using ParkingSystem.Core.Aggregates;

namespace ParkingSystem.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Park> Parks { get; set; }
        public DbSet<InOutRecord> InOutRecords { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().Property(u => u.role).HasConversion<string>();

            // modelBuilder.Entity<User>().HasKey(u => u.Id);
            // modelBuilder.Entity<Vehicle>().HasKey(v => v.Id);
            // modelBuilder.Entity<Park>().HasKey(p => p.Id);
             modelBuilder.Entity<InOutRecord>().HasKey(p => p.Id);

        }
    }
}
