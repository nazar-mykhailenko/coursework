using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgroPlaner.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    LocationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.LocationId);
                });

            migrationBuilder.CreateTable(
                name: "Plants",
                columns: table => new
                {
                    PlantId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MinSoilTempForSeeding = table.Column<double>(type: "float", nullable: false),
                    BaseTempForGDD = table.Column<double>(type: "float", nullable: false),
                    MaturityGDD = table.Column<double>(type: "float", nullable: false),
                    KcValues = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RootDepth = table.Column<double>(type: "float", nullable: false),
                    AllowableDepletionFraction = table.Column<double>(type: "float", nullable: false),
                    NitrogenContent = table.Column<double>(type: "float", nullable: false),
                    PhosphorusContent = table.Column<double>(type: "float", nullable: false),
                    PotassiumContent = table.Column<double>(type: "float", nullable: false),
                    NitrogenUptakeDistribution = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhosphorusUptakeDistribution = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PotassiumUptakeDistribution = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FertilizerType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FertilizerNitrogenPercentage = table.Column<double>(type: "float", nullable: false),
                    FertilizerPhosphorusPercentage = table.Column<double>(type: "float", nullable: false),
                    FertilizerPotassiumPercentage = table.Column<double>(type: "float", nullable: false),
                    WiltingPoint = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plants", x => x.PlantId);
                });

            migrationBuilder.CreateTable(
                name: "WeatherData",
                columns: table => new
                {
                    WeatherDataId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LocationId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaxTemp = table.Column<double>(type: "float", nullable: false),
                    MinTemp = table.Column<double>(type: "float", nullable: false),
                    Precipitation = table.Column<double>(type: "float", nullable: false),
                    WindSpeed = table.Column<double>(type: "float", nullable: false),
                    SolarRadiation = table.Column<double>(type: "float", nullable: false),
                    RelativeHumidity = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeatherData", x => x.WeatherDataId);
                    table.ForeignKey(
                        name: "FK_WeatherData_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "LocationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Crops",
                columns: table => new
                {
                    CropId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlantId = table.Column<int>(type: "int", nullable: false),
                    LocationId = table.Column<int>(type: "int", nullable: false),
                    SoilDataId = table.Column<int>(type: "int", nullable: true),
                    ExpectedYield = table.Column<double>(type: "float", nullable: false),
                    CumulativeGDDToday = table.Column<double>(type: "float", nullable: false),
                    FieldArea = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Crops", x => x.CropId);
                    table.ForeignKey(
                        name: "FK_Crops_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "LocationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Crops_Plants_PlantId",
                        column: x => x.PlantId,
                        principalTable: "Plants",
                        principalColumn: "PlantId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PlantGrowthStage",
                columns: table => new
                {
                    PlantGrowthStageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StageName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MinGDD = table.Column<double>(type: "float", nullable: false),
                    MaxGDD = table.Column<double>(type: "float", nullable: false),
                    OrderIndex = table.Column<int>(type: "int", nullable: false),
                    PlantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlantGrowthStage", x => x.PlantGrowthStageId);
                    table.ForeignKey(
                        name: "FK_PlantGrowthStage_Plants_PlantId",
                        column: x => x.PlantId,
                        principalTable: "Plants",
                        principalColumn: "PlantId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SoilData",
                columns: table => new
                {
                    SoilDataId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CropId = table.Column<int>(type: "int", nullable: false),
                    CurrentMoisture = table.Column<double>(type: "float", nullable: false),
                    FieldCapacity = table.Column<double>(type: "float", nullable: false),
                    Temperature = table.Column<double>(type: "float", nullable: false),
                    AvailableNitrogen = table.Column<double>(type: "float", nullable: false),
                    AvailablePhosphorus = table.Column<double>(type: "float", nullable: false),
                    AvailablePotassium = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoilData", x => x.SoilDataId);
                    table.ForeignKey(
                        name: "FK_SoilData_Crops_CropId",
                        column: x => x.CropId,
                        principalTable: "Crops",
                        principalColumn: "CropId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FertilizationEvents",
                columns: table => new
                {
                    FertilizationEventId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SoilDataId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NitrogenAmount = table.Column<double>(type: "float", nullable: false),
                    PhosphorusAmount = table.Column<double>(type: "float", nullable: false),
                    PotassiumAmount = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FertilizationEvents", x => x.FertilizationEventId);
                    table.ForeignKey(
                        name: "FK_FertilizationEvents_SoilData_SoilDataId",
                        column: x => x.SoilDataId,
                        principalTable: "SoilData",
                        principalColumn: "SoilDataId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IrrigationEvents",
                columns: table => new
                {
                    IrrigationEventId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SoilDataId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Amount = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IrrigationEvents", x => x.IrrigationEventId);
                    table.ForeignKey(
                        name: "FK_IrrigationEvents_SoilData_SoilDataId",
                        column: x => x.SoilDataId,
                        principalTable: "SoilData",
                        principalColumn: "SoilDataId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Crops_LocationId",
                table: "Crops",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Crops_PlantId",
                table: "Crops",
                column: "PlantId");

            migrationBuilder.CreateIndex(
                name: "IX_FertilizationEvents_SoilDataId",
                table: "FertilizationEvents",
                column: "SoilDataId");

            migrationBuilder.CreateIndex(
                name: "IX_IrrigationEvents_SoilDataId",
                table: "IrrigationEvents",
                column: "SoilDataId");

            migrationBuilder.CreateIndex(
                name: "IX_PlantGrowthStage_PlantId",
                table: "PlantGrowthStage",
                column: "PlantId");

            migrationBuilder.CreateIndex(
                name: "IX_SoilData_CropId",
                table: "SoilData",
                column: "CropId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WeatherData_LocationId",
                table: "WeatherData",
                column: "LocationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FertilizationEvents");

            migrationBuilder.DropTable(
                name: "IrrigationEvents");

            migrationBuilder.DropTable(
                name: "PlantGrowthStage");

            migrationBuilder.DropTable(
                name: "WeatherData");

            migrationBuilder.DropTable(
                name: "SoilData");

            migrationBuilder.DropTable(
                name: "Crops");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropTable(
                name: "Plants");
        }
    }
}
