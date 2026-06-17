using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddImageUrlsToEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "UnliquidProducts");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Services");
        }
    }
}
