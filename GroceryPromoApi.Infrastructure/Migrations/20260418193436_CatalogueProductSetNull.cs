using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GroceryPromoApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CatalogueProductSetNull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_CatalogueProducts_CatalogueProductId",
                table: "Products");

            migrationBuilder.AlterColumn<Guid>(
                name: "CatalogueProductId",
                table: "Products",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_CatalogueProducts_CatalogueProductId",
                table: "Products",
                column: "CatalogueProductId",
                principalTable: "CatalogueProducts",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_CatalogueProducts_CatalogueProductId",
                table: "Products");

            migrationBuilder.AlterColumn<Guid>(
                name: "CatalogueProductId",
                table: "Products",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_CatalogueProducts_CatalogueProductId",
                table: "Products",
                column: "CatalogueProductId",
                principalTable: "CatalogueProducts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
