using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgroPlaner.DAL.Migrations
{
    /// <inheritdoc />
    public partial class CascadeDeleteCropsWhenLocationDeleted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Crops_Locations_LocationId",
                table: "Crops");

            migrationBuilder.AddForeignKey(
                name: "FK_Crops_Locations_LocationId",
                table: "Crops",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "LocationId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Crops_Locations_LocationId",
                table: "Crops");

            migrationBuilder.AddForeignKey(
                name: "FK_Crops_Locations_LocationId",
                table: "Crops",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "LocationId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
