using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcoTurismo.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddLocalizacaoToAtrativos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Endereco",
                table: "Atrativos",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                comment: "Endereço do atrativo");

            migrationBuilder.AddColumn<decimal>(
                name: "Latitude",
                table: "Atrativos",
                type: "numeric(10,7)",
                precision: 10,
                scale: 7,
                nullable: true,
                comment: "Latitude do atrativo");

            migrationBuilder.AddColumn<decimal>(
                name: "Longitude",
                table: "Atrativos",
                type: "numeric(10,7)",
                precision: 10,
                scale: 7,
                nullable: true,
                comment: "Longitude do atrativo");

            migrationBuilder.AddColumn<string>(
                name: "MapUrl",
                table: "Atrativos",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                comment: "URL do mapa (Google Maps, etc)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Endereco",
                table: "Atrativos");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Atrativos");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Atrativos");

            migrationBuilder.DropColumn(
                name: "MapUrl",
                table: "Atrativos");
        }
    }
}
