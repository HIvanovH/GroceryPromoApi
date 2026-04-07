using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GroceryPromoApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addCatalogueProductAndOffer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Favourites");

            migrationBuilder.DropTable(
                name: "ProductsPending");

            migrationBuilder.AddColumn<Guid>(
                name: "CatalogueProductId",
                table: "Products",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "CatalogueProducts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    NormalizedName = table.Column<string>(type: "text", nullable: false),
                    NormalizedQuantity = table.Column<string>(type: "text", nullable: true),
                    Category = table.Column<string>(type: "text", nullable: true),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    LastSeenAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogueProducts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CatalogueProductOffers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CatalogueProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    SupermarketId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentPriceEur = table.Column<decimal>(type: "numeric", nullable: false),
                    NormalPriceEur = table.Column<decimal>(type: "numeric", nullable: true),
                    PromoValidUntil = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogueProductOffers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CatalogueProductOffers_CatalogueProducts_CatalogueProductId",
                        column: x => x.CatalogueProductId,
                        principalTable: "CatalogueProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CatalogueProductOffers_Supermarkets_SupermarketId",
                        column: x => x.SupermarketId,
                        principalTable: "Supermarkets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserFavourites",
                columns: table => new
                {
                    FavouritesId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFavourites", x => new { x.FavouritesId, x.UserId });
                    table.ForeignKey(
                        name: "FK_UserFavourites_CatalogueProducts_FavouritesId",
                        column: x => x.FavouritesId,
                        principalTable: "CatalogueProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserFavourites_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_CatalogueProductId",
                table: "Products",
                column: "CatalogueProductId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogueProductOffers_CatalogueProductId",
                table: "CatalogueProductOffers",
                column: "CatalogueProductId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogueProductOffers_SupermarketId",
                table: "CatalogueProductOffers",
                column: "SupermarketId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFavourites_UserId",
                table: "UserFavourites",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_CatalogueProducts_CatalogueProductId",
                table: "Products",
                column: "CatalogueProductId",
                principalTable: "CatalogueProducts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_CatalogueProducts_CatalogueProductId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "CatalogueProductOffers");

            migrationBuilder.DropTable(
                name: "UserFavourites");

            migrationBuilder.DropTable(
                name: "CatalogueProducts");

            migrationBuilder.DropIndex(
                name: "IX_Products_CatalogueProductId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CatalogueProductId",
                table: "Products");

            migrationBuilder.CreateTable(
                name: "Favourites",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Category = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NormalizedName = table.Column<string>(type: "text", nullable: true),
                    NormalizedQuantity = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Favourites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Favourites_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductsPending",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExternalId = table.Column<int>(type: "integer", nullable: false),
                    Issue = table.Column<string>(type: "text", nullable: false),
                    NormalizedName = table.Column<string>(type: "text", nullable: true),
                    NormalizedQuantity = table.Column<string>(type: "text", nullable: true),
                    RawName = table.Column<string>(type: "text", nullable: false),
                    RawQuantity = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductsPending", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Favourites_UserId",
                table: "Favourites",
                column: "UserId");
        }
    }
}
