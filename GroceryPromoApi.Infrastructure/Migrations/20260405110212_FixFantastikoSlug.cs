using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GroceryPromoApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixFantastikoSlug : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Supermarkets",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-0002-0000-0000-000000000000"),
                column: "Slug",
                value: "fantastiko");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Supermarkets",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-0002-0000-0000-000000000000"),
                column: "Slug",
                value: "fantastico");
        }
    }
}
