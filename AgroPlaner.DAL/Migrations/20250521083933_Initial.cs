using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AgroPlaner.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
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
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    LocationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.LocationId);
                    table.ForeignKey(
                        name: "FK_Locations_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
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
                name: "Crops",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlantId = table.Column<int>(type: "int", nullable: false),
                    LocationId = table.Column<int>(type: "int", nullable: false),
                    SoilDataId = table.Column<int>(type: "int", nullable: true),
                    ExpectedYield = table.Column<double>(type: "float", nullable: false),
                    CumulativeGDDToday = table.Column<double>(type: "float", nullable: false),
                    FieldArea = table.Column<double>(type: "float", nullable: false),
                    PlantingDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Crops", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Crops_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
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
                        principalColumn: "Id",
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

            migrationBuilder.InsertData(
                table: "Plants",
                columns: new[] { "PlantId", "AllowableDepletionFraction", "BaseTempForGDD", "FertilizerNitrogenPercentage", "FertilizerPhosphorusPercentage", "FertilizerPotassiumPercentage", "FertilizerType", "KcValues", "MaturityGDD", "MinSoilTempForSeeding", "Name", "NitrogenContent", "NitrogenUptakeDistribution", "PhosphorusContent", "PhosphorusUptakeDistribution", "PotassiumContent", "PotassiumUptakeDistribution", "RootDepth", "WiltingPoint" },
                values: new object[,]
                {
                    { 1, 0.40000000000000002, 4.5, 15.0, 15.0, 15.0, "15-15-15", "[0.5,0.825,1.15,0.75]", 1200.0, 10.0, "Potato", 4.2999999999999998, "[0.2,0.4,0.3,0.1]", 1.3999999999999999, "[0.2,0.4,0.3,0.1]", 5.7999999999999998, "[0.2,0.4,0.3,0.1]", 0.59999999999999998, 50.0 },
                    { 2, 0.40000000000000002, 10.0, 15.0, 15.0, 15.0, "15-15-15", "[0.6,0.875,1.15,0.8]", 1000.0, 10.0, "Tomato", 3.0, "[0.2,0.4,0.3,0.1]", 1.2, "[0.2,0.4,0.3,0.1]", 4.2000000000000002, "[0.2,0.4,0.3,0.1]", 1.0, 50.0 },
                    { 3, 0.55000000000000004, 0.0, 15.0, 15.0, 15.0, "15-15-15", "[0.3,0.75,1.15,0.65]", 2900.0, 4.0, "Wheat", 20.0, "[0.2,0.4,0.3,0.1]", 8.0, "[0.2,0.4,0.3,0.1]", 4.5, "[0.2,0.4,0.3,0.1]", 1.5, 50.0 },
                    { 4, 0.5, 10.0, 15.0, 15.0, 15.0, "15-15-15", "[0.3,0.75,1.2,0.5]", 1500.0, 10.0, "Corn", 21.0, "[0.2,0.4,0.3,0.1]", 7.0, "[0.2,0.4,0.3,0.1]", 5.0, "[0.2,0.4,0.3,0.1]", 1.2, 50.0 },
                    { 5, 0.45000000000000001, 5.0, 15.0, 15.0, 15.0, "15-15-15", "[0.35,0.8,1.2,0.7]", 1000.0, 4.0, "Beetroot", 3.5, "[0.2,0.4,0.3,0.1]", 1.5, "[0.2,0.4,0.3,0.1]", 5.0, "[0.2,0.4,0.3,0.1]", 0.80000000000000004, 50.0 }
                });

            migrationBuilder.InsertData(
                table: "PlantGrowthStage",
                columns: new[] { "PlantGrowthStageId", "MaxGDD", "MinGDD", "OrderIndex", "PlantId", "StageName" },
                values: new object[,]
                {
                    { 1, 240.0, 0.0, 0, 1, "Initial" },
                    { 2, 600.0, 240.0, 0, 1, "Development" },
                    { 3, 960.0, 600.0, 0, 1, "Mid-season" },
                    { 4, 1200.0, 960.0, 0, 1, "Late-season" },
                    { 5, 200.0, 0.0, 0, 2, "Initial" },
                    { 6, 500.0, 200.0, 0, 2, "Development" },
                    { 7, 800.0, 500.0, 0, 2, "Mid-season" },
                    { 8, 1000.0, 800.0, 0, 2, "Late-season" },
                    { 9, 580.0, 0.0, 0, 3, "Initial" },
                    { 10, 1450.0, 580.0, 0, 3, "Development" },
                    { 11, 2320.0, 1450.0, 0, 3, "Mid-season" },
                    { 12, 2900.0, 2320.0, 0, 3, "Late-season" },
                    { 13, 300.0, 0.0, 0, 4, "Initial" },
                    { 14, 750.0, 300.0, 0, 4, "Development" },
                    { 15, 1200.0, 750.0, 0, 4, "Mid-season" },
                    { 16, 1500.0, 1200.0, 0, 4, "Late-season" },
                    { 17, 200.0, 0.0, 0, 5, "Initial" },
                    { 18, 500.0, 200.0, 0, 5, "Development" },
                    { 19, 800.0, 500.0, 0, 5, "Mid-season" },
                    { 20, 1000.0, 800.0, 0, 5, "Late-season" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Crops_LocationId",
                table: "Crops",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Crops_PlantId",
                table: "Crops",
                column: "PlantId");

            migrationBuilder.CreateIndex(
                name: "IX_Crops_UserId",
                table: "Crops",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FertilizationEvents_SoilDataId",
                table: "FertilizationEvents",
                column: "SoilDataId");

            migrationBuilder.CreateIndex(
                name: "IX_IrrigationEvents_SoilDataId",
                table: "IrrigationEvents",
                column: "SoilDataId");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_UserId",
                table: "Locations",
                column: "UserId");

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
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "FertilizationEvents");

            migrationBuilder.DropTable(
                name: "IrrigationEvents");

            migrationBuilder.DropTable(
                name: "PlantGrowthStage");

            migrationBuilder.DropTable(
                name: "WeatherData");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "SoilData");

            migrationBuilder.DropTable(
                name: "Crops");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropTable(
                name: "Plants");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
