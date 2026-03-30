using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GroceryPromoApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SupermarketSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Supermarkets",
                columns: new[] { "Id", "Name", "Slug" },
                values: new object[,]
                {
                    { new Guid("a1b2c3d4-0001-0000-0000-000000000000"), "Billa", "billa" },
                    { new Guid("a1b2c3d4-0002-0000-0000-000000000000"), "Fantastico", "fantastico" },
                    { new Guid("a1b2c3d4-0003-0000-0000-000000000000"), "Kaufland", "kaufland" },
                    { new Guid("a1b2c3d4-0004-0000-0000-000000000000"), "Lidl", "lidl" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Supermarkets",
                keyColumn: "Id",
                keyValues: new object[]
                {
                    new Guid("a1b2c3d4-0001-0000-0000-000000000000"),
                    new Guid("a1b2c3d4-0002-0000-0000-000000000000"),
                    new Guid("a1b2c3d4-0003-0000-0000-000000000000"),
                    new Guid("a1b2c3d4-0004-0000-0000-000000000000")
                });
        }
    }
}
