using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcoTurismo.Infra.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCoordinatesPrecision : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Longitude",
                table: "Atrativos",
                type: "numeric(12,9)",
                precision: 12,
                scale: 9,
                nullable: true,
                comment: "Longitude do atrativo",
                oldClrType: typeof(decimal),
                oldType: "numeric(10,7)",
                oldPrecision: 10,
                oldScale: 7,
                oldNullable: true,
                oldComment: "Longitude do atrativo");

            migrationBuilder.AlterColumn<decimal>(
                name: "Latitude",
                table: "Atrativos",
                type: "numeric(12,9)",
                precision: 12,
                scale: 9,
                nullable: true,
                comment: "Latitude do atrativo",
                oldClrType: typeof(decimal),
                oldType: "numeric(10,7)",
                oldPrecision: 10,
                oldScale: 7,
                oldNullable: true,
                oldComment: "Latitude do atrativo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Longitude",
                table: "Atrativos",
                type: "numeric(10,7)",
                precision: 10,
                scale: 7,
                nullable: true,
                comment: "Longitude do atrativo",
                oldClrType: typeof(decimal),
                oldType: "numeric(12,9)",
                oldPrecision: 12,
                oldScale: 9,
                oldNullable: true,
                oldComment: "Longitude do atrativo");

            migrationBuilder.AlterColumn<decimal>(
                name: "Latitude",
                table: "Atrativos",
                type: "numeric(10,7)",
                precision: 10,
                scale: 7,
                nullable: true,
                comment: "Latitude do atrativo",
                oldClrType: typeof(decimal),
                oldType: "numeric(12,9)",
                oldPrecision: 12,
                oldScale: 9,
                oldNullable: true,
                oldComment: "Latitude do atrativo");
        }
    }
}
