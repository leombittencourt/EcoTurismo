using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcoTurismo.Infra.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAtrativoTipoToEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Tipo",
                table: "Atrativos",
                type: "character varying(25)",
                maxLength: 25,
                nullable: false,
                comment: "Tipo do atrativo (balneario, cachoeira, trilha, parque, fazenda_ecoturismo)",
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldComment: "Tipo do atrativo (balneario, parque, etc.)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Tipo",
                table: "Atrativos",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                comment: "Tipo do atrativo (balneario, parque, etc.)",
                oldClrType: typeof(string),
                oldType: "character varying(25)",
                oldMaxLength: 25,
                oldComment: "Tipo do atrativo (balneario, cachoeira, trilha, parque, fazenda_ecoturismo)");
        }
    }
}
