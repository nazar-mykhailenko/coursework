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
                .OnDelete(DeleteBehavior.Restrict); // Don't delete Plant when Crop is deleted            // Configure one-to-many relationship between Location and Crop
            modelBuilder
                .Entity<Location>()
                .HasMany(l => l.Crops)
                .WithOne(c => c.Location)
                .HasForeignKey(c => c.LocationId)
                .OnDelete(DeleteBehavior.Cascade); // Delete Crops when Location is deleted

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

            modelBuilder.Entity<Plant>().HasData(
                new Plant
                {
                    PlantId = 1,
                    Name = "Potato",
                    MinSoilTempForSeeding = 10.0,
                    BaseTempForGDD = 4.5,
                    MaturityGDD = 1200.0,
                    KcValues = new double[] { 0.50, 0.825, 1.15, 0.75 },
                    RootDepth = 0.6,
                    AllowableDepletionFraction = 0.4,
                    NitrogenContent = 4.3,
                    PhosphorusContent = 1.4,
                    PotassiumContent = 5.8,
                    NitrogenUptakeDistribution = new double[] { 0.2, 0.4, 0.3, 0.1 },
                    PhosphorusUptakeDistribution = new double[] { 0.2, 0.4, 0.3, 0.1 },
                    PotassiumUptakeDistribution = new double[] { 0.2, 0.4, 0.3, 0.1 },
                    FertilizerType = "15-15-15",
                    FertilizerNitrogenPercentage = 15.0,
                    FertilizerPhosphorusPercentage = 15.0,
                    FertilizerPotassiumPercentage = 15.0,
                    WiltingPoint = 50.0
                },
                new Plant
                {
                    PlantId = 2,
                    Name = "Tomato",
                    MinSoilTempForSeeding = 10.0,
                    BaseTempForGDD = 10.0,
                    MaturityGDD = 1000.0,
                    KcValues = new double[] { 0.6, 0.875, 1.15, 0.8 },
                    RootDepth = 1.0,
                    AllowableDepletionFraction = 0.4,
                    NitrogenContent = 3.0,
                    PhosphorusContent = 1.2,
                    PotassiumContent = 4.2,
                    NitrogenUptakeDistribution = new double[] { 0.2, 0.4, 0.3, 0.1 },
                    PhosphorusUptakeDistribution = new double[] { 0.2, 0.4, 0.3, 0.1 },
                    PotassiumUptakeDistribution = new double[] { 0.2, 0.4, 0.3, 0.1 },
                    FertilizerType = "15-15-15",
                    FertilizerNitrogenPercentage = 15.0,
                    FertilizerPhosphorusPercentage = 15.0,
                    FertilizerPotassiumPercentage = 15.0,
                    WiltingPoint = 50.0
                },
                new Plant
                {
                    PlantId = 3,
                    Name = "Wheat",
                    MinSoilTempForSeeding = 4.0,
                    BaseTempForGDD = 0.0,
                    MaturityGDD = 2900.0,
                    KcValues = new double[] { 0.3, 0.75, 1.15, 0.65 },
                    RootDepth = 1.5,
                    AllowableDepletionFraction = 0.55,
                    NitrogenContent = 20.0,
                    PhosphorusContent = 8.0,
                    PotassiumContent = 4.5,
                    NitrogenUptakeDistribution = new double[] { 0.2, 0.4, 0.3, 0.1 },
                    PhosphorusUptakeDistribution = new double[] { 0.2, 0.4, 0.3, 0.1 },
                    PotassiumUptakeDistribution = new double[] { 0.2, 0.4, 0.3, 0.1 },
                    FertilizerType = "15-15-15",
                    FertilizerNitrogenPercentage = 15.0,
                    FertilizerPhosphorusPercentage = 15.0,
                    FertilizerPotassiumPercentage = 15.0,
                    WiltingPoint = 50.0
                },
                new Plant
                {
                    PlantId = 4,
                    Name = "Corn",
                    MinSoilTempForSeeding = 10.0,
                    BaseTempForGDD = 10.0,
                    MaturityGDD = 1500.0,
                    KcValues = new double[] { 0.3, 0.75, 1.2, 0.5 },
                    RootDepth = 1.2,
                    AllowableDepletionFraction = 0.5,
                    NitrogenContent = 21.0,
                    PhosphorusContent = 7.0,
                    PotassiumContent = 5.0,
                    NitrogenUptakeDistribution = new double[] { 0.2, 0.4, 0.3, 0.1 },
                    PhosphorusUptakeDistribution = new double[] { 0.2, 0.4, 0.3, 0.1 },
                    PotassiumUptakeDistribution = new double[] { 0.2, 0.4, 0.3, 0.1 },
                    FertilizerType = "15-15-15",
                    FertilizerNitrogenPercentage = 15.0,
                    FertilizerPhosphorusPercentage = 15.0,
                    FertilizerPotassiumPercentage = 15.0,
                    WiltingPoint = 50.0
                },
                new Plant
                {
                    PlantId = 5,
                    Name = "Beetroot",
                    MinSoilTempForSeeding = 4.0,
                    BaseTempForGDD = 5.0,
                    MaturityGDD = 1000.0,
                    KcValues = new double[] { 0.35, 0.8, 1.2, 0.7 },
                    RootDepth = 0.8,
                    AllowableDepletionFraction = 0.45,
                    NitrogenContent = 3.5,
                    PhosphorusContent = 1.5,
                    PotassiumContent = 5.0,
                    NitrogenUptakeDistribution = new double[] { 0.2, 0.4, 0.3, 0.1 },
                    PhosphorusUptakeDistribution = new double[] { 0.2, 0.4, 0.3, 0.1 },
                    PotassiumUptakeDistribution = new double[] { 0.2, 0.4, 0.3, 0.1 },
                    FertilizerType = "15-15-15",
                    FertilizerNitrogenPercentage = 15.0,
                    FertilizerPhosphorusPercentage = 15.0,
                    FertilizerPotassiumPercentage = 15.0,
                    WiltingPoint = 50.0
                }
            );

            modelBuilder.Entity<PlantGrowthStage>().HasData(
                // Potato Growth Stages (1200 GDD: 20% Initial, 30% Development, 30% Mid-season, 20% Late-season)
                new PlantGrowthStage { PlantGrowthStageId = 1, PlantId = 1, StageName = "Initial", MinGDD = 0, MaxGDD = 240 },
                new PlantGrowthStage { PlantGrowthStageId = 2, PlantId = 1, StageName = "Development", MinGDD = 240, MaxGDD = 600 },
                new PlantGrowthStage { PlantGrowthStageId = 3, PlantId = 1, StageName = "Mid-season", MinGDD = 600, MaxGDD = 960 },
                new PlantGrowthStage { PlantGrowthStageId = 4, PlantId = 1, StageName = "Late-season", MinGDD = 960, MaxGDD = 1200 },
                // Tomato Growth Stages (1000 GDD: 20% Initial, 30% Development, 30% Mid-season, 20% Late-season)
                new PlantGrowthStage { PlantGrowthStageId = 5, PlantId = 2, StageName = "Initial", MinGDD = 0, MaxGDD = 200 },
                new PlantGrowthStage { PlantGrowthStageId = 6, PlantId = 2, StageName = "Development", MinGDD = 200, MaxGDD = 500 },
                new PlantGrowthStage { PlantGrowthStageId = 7, PlantId = 2, StageName = "Mid-season", MinGDD = 500, MaxGDD = 800 },
                new PlantGrowthStage { PlantGrowthStageId = 8, PlantId = 2, StageName = "Late-season", MinGDD = 800, MaxGDD = 1000 },
                // Wheat Growth Stages (2900 GDD: 20% Initial, 30% Development, 30% Mid-season, 20% Late-season)
                new PlantGrowthStage { PlantGrowthStageId = 9, PlantId = 3, StageName = "Initial", MinGDD = 0, MaxGDD = 580 },
                new PlantGrowthStage { PlantGrowthStageId = 10, PlantId = 3, StageName = "Development", MinGDD = 580, MaxGDD = 1450 },
                new PlantGrowthStage { PlantGrowthStageId = 11, PlantId = 3, StageName = "Mid-season", MinGDD = 1450, MaxGDD = 2320 },
                new PlantGrowthStage { PlantGrowthStageId = 12, PlantId = 3, StageName = "Late-season", MinGDD = 2320, MaxGDD = 2900 },
                // Corn Growth Stages (1500 GDD: 20% Initial, 30% Development, 30% Mid-season, 20% Late-season)
                new PlantGrowthStage { PlantGrowthStageId = 13, PlantId = 4, StageName = "Initial", MinGDD = 0, MaxGDD = 300 },
                new PlantGrowthStage { PlantGrowthStageId = 14, PlantId = 4, StageName = "Development", MinGDD = 300, MaxGDD = 750 },
                new PlantGrowthStage { PlantGrowthStageId = 15, PlantId = 4, StageName = "Mid-season", MinGDD = 750, MaxGDD = 1200 },
                new PlantGrowthStage { PlantGrowthStageId = 16, PlantId = 4, StageName = "Late-season", MinGDD = 1200, MaxGDD = 1500 },
                // Beetroot Growth Stages (1000 GDD: 20% Initial, 30% Development, 30% Mid-season, 20% Late-season)
                new PlantGrowthStage { PlantGrowthStageId = 17, PlantId = 5, StageName = "Initial", MinGDD = 0, MaxGDD = 200 },
                new PlantGrowthStage { PlantGrowthStageId = 18, PlantId = 5, StageName = "Development", MinGDD = 200, MaxGDD = 500 },
                new PlantGrowthStage { PlantGrowthStageId = 19, PlantId = 5, StageName = "Mid-season", MinGDD = 500, MaxGDD = 800 },
                new PlantGrowthStage { PlantGrowthStageId = 20, PlantId = 5, StageName = "Late-season", MinGDD = 800, MaxGDD = 1000 }
            );
        }
    }
}
