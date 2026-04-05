using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GroceryPromoApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEurPriceFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "OldPriceEur",
                table: "Products",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceEur",
                table: "Products",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OldPriceEur",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "PriceEur",
                table: "Products");
        }
    }
}
