using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DoubleToDecimal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "UnliquidProducts",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "REAL");

            migrationBuilder.AlterColumn<decimal>(
                name: "PriceFrom",
                table: "Services",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "REAL");

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 1,
                column: "PriceFrom",
                value: 12.50m);

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 2,
                column: "PriceFrom",
                value: 6.80m);

            migrationBuilder.UpdateData(
                table: "UnliquidProducts",
                keyColumn: "Id",
                keyValue: 1,
                column: "Price",
                value: 450.00m);

            migrationBuilder.UpdateData(
                table: "UnliquidProducts",
                keyColumn: "Id",
                keyValue: 2,
                column: "Price",
                value: 12.80m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Price",
                table: "UnliquidProducts",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<double>(
                name: "PriceFrom",
                table: "Services",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "TEXT");

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 1,
                column: "PriceFrom",
                value: 12.5);

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 2,
                column: "PriceFrom",
                value: 6.7999999999999998);

            migrationBuilder.UpdateData(
                table: "UnliquidProducts",
                keyColumn: "Id",
                keyValue: 1,
                column: "Price",
                value: 450.0);

            migrationBuilder.UpdateData(
                table: "UnliquidProducts",
                keyColumn: "Id",
                keyValue: 2,
                column: "Price",
                value: 12.800000000000001);
        }
    }
}
