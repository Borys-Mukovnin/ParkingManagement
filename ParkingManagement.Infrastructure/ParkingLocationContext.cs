using Microsoft.EntityFrameworkCore;
using ParkingManagement.Domain.Aggregates.ParkingLocationAggregate;

namespace ParkingManagement.Infrastructure
{
    public class ParkingLocationContext : DbContext
    {
        public DbSet<ParkingLocation> ParkingLocations { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }

        public ParkingLocationContext(DbContextOptions<ParkingLocationContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            if (!optionsBuilder.IsConfigured)
            {
                throw new InvalidOperationException("No database provider has been configured for 'ParkingLocationContext'. Please configure a database provider in the application startup.");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<ParkingLocation>(etb =>
            {
                etb.HasMany(e => e.Vehicles)
                   .WithOne(e => e.ParkingLocation)
                   .HasForeignKey(e => e.ParkingLocationId)
                   .OnDelete(DeleteBehavior.SetNull);
            }
            );
                
        }
    }
}
