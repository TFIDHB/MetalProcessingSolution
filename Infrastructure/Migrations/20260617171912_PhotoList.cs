using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PhotoList : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "UnliquidProducts");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Services");

            migrationBuilder.CreateTable(
                name: "MetalServiceImage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ImageUrl = table.Column<string>(type: "TEXT", nullable: false),
                    MetalServiceId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetalServiceImage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MetalServiceImage_Services_MetalServiceId",
                        column: x => x.MetalServiceId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UnliquidProductImage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ImageUrl = table.Column<string>(type: "TEXT", nullable: false),
                    UnliquidProductId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnliquidProductImage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnliquidProductImage_UnliquidProducts_UnliquidProductId",
                        column: x => x.UnliquidProductId,
                        principalTable: "UnliquidProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MetalServiceImage_MetalServiceId",
                table: "MetalServiceImage",
                column: "MetalServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_UnliquidProductImage_UnliquidProductId",
                table: "UnliquidProductImage",
                column: "UnliquidProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MetalServiceImage");

            migrationBuilder.DropTable(
                name: "UnliquidProductImage");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "UnliquidProducts",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Services",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 1,
                column: "ImageUrl",
                value: "/images/no-image.png");

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 2,
                column: "ImageUrl",
                value: "/images/no-image.png");

            migrationBuilder.UpdateData(
                table: "UnliquidProducts",
                keyColumn: "Id",
                keyValue: 1,
                column: "ImageUrl",
                value: "/images/no-image.png");

            migrationBuilder.UpdateData(
                table: "UnliquidProducts",
                keyColumn: "Id",
                keyValue: 2,
                column: "ImageUrl",
                value: "/images/no-image.png");
        }
    }
}
