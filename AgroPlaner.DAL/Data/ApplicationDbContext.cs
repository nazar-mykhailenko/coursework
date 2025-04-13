using AgroPlaner.DAL.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AgroPlaner.DAL.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Plant> Plants { get; set; }
        public DbSet<Crop> Crops { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<SoilData> SoilData { get; set; }
        public DbSet<WeatherData> WeatherData { get; set; }
        public DbSet<IrrigationEvent> IrrigationEvents { get; set; }
        public DbSet<FertilizationEvent> FertilizationEvents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure one-to-many relationship between Plant and Crop
            modelBuilder
                .Entity<Plant>()
                .HasMany(p => p.Crops)
                .WithOne(c => c.Plant)
                .HasForeignKey(c => c.PlantId)
                .OnDelete(DeleteBehavior.Restrict); // Don't delete Plant when Crop is deleted

            // Configure one-to-many relationship between Location and Crop
            modelBuilder
                .Entity<Location>()
                .HasMany(l => l.Crops)
                .WithOne(c => c.Location)
                .HasForeignKey(c => c.LocationId)
                .OnDelete(DeleteBehavior.Restrict); // Don't delete Location when Crop is deleted

            // Configure one-to-many relationship between Location and WeatherData
            modelBuilder
                .Entity<Location>()
                .HasMany(l => l.WeatherHistory)
                .WithOne(w => w.Location)
                .HasForeignKey(w => w.LocationId);

            // Configure one-to-one relationship between Crop and SoilData with cascade delete
            modelBuilder
                .Entity<Crop>()
                .HasOne(c => c.Soil)
                .WithOne(s => s.Crop)
                .HasForeignKey<SoilData>(s => s.CropId)
                .OnDelete(DeleteBehavior.Cascade); // Delete SoilData when Crop is deleted

            // Configure one-to-many relationship between SoilData and IrrigationEvent
            modelBuilder
                .Entity<SoilData>()
                .HasMany(s => s.IrrigationHistory)
                .WithOne(i => i.SoilData)
                .HasForeignKey(i => i.SoilDataId);

            // Configure one-to-many relationship between SoilData and FertilizationEvent
            modelBuilder
                .Entity<SoilData>()
                .HasMany(s => s.FertilizationHistory)
                .WithOne(f => f.SoilData)
                .HasForeignKey(f => f.SoilDataId);

            // Configure one-to-many relationship between ApplicationUser and Location
            modelBuilder
                .Entity<Location>()
                .HasOne(l => l.User)
                .WithMany(u => u.Locations)
                .HasForeignKey(l => l.UserId);

            modelBuilder
                .Entity<Crop>()
                .HasOne(l => l.User)
                .WithMany(u => u.Crops)
                .HasForeignKey(l => l.UserId);
        }
    }
}
