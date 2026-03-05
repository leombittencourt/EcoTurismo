using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcoTurismo.Infra.Migrations
{
    /// <inheritdoc />
    public partial class RemoveImagensColumnsFromAtrativos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Imagem",
                table: "Atrativos");

            migrationBuilder.DropColumn(
                name: "Imagens",
                table: "Atrativos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Imagem",
                table: "Atrativos",
                type: "text",
                nullable: true,
                comment: "URL da imagem do atrativo");

            migrationBuilder.AddColumn<string>(
                name: "Imagens",
                table: "Atrativos",
                type: "text",
                nullable: true,
                comment: "Array JSON de múltiplas imagens em base64");
        }
    }
}
