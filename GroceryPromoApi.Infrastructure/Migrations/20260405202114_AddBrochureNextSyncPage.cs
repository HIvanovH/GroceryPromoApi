using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GroceryPromoApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBrochureNextSyncPage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NextSyncPage",
                table: "Brochures",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NextSyncPage",
                table: "Brochures");
        }
    }
}
