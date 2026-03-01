using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcoTurismo.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddLogosToMunicipio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LogoAreaPublica",
                table: "Municipios",
                type: "text",
                nullable: true,
                comment: "Logo em base64 para exibição na área pública/portal");

            migrationBuilder.AddColumn<string>(
                name: "LogoTelaLogin",
                table: "Municipios",
                type: "text",
                nullable: true,
                comment: "Logo em base64 para exibição na tela de login");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogoAreaPublica",
                table: "Municipios");

            migrationBuilder.DropColumn(
                name: "LogoTelaLogin",
                table: "Municipios");
        }
    }
}
