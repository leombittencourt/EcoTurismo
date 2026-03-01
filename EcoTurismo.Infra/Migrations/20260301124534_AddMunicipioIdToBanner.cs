using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcoTurismo.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddMunicipioIdToBanner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BannersMunicipio");

            migrationBuilder.AddColumn<Guid>(
                name: "MunicipioId",
                table: "Banners",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Banners_MunicipioId",
                table: "Banners",
                column: "MunicipioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Banners_Municipios_MunicipioId",
                table: "Banners",
                column: "MunicipioId",
                principalTable: "Municipios",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Banners_Municipios_MunicipioId",
                table: "Banners");

            migrationBuilder.DropIndex(
                name: "IX_Banners_MunicipioId",
                table: "Banners");

            migrationBuilder.DropColumn(
                name: "MunicipioId",
                table: "Banners");

            migrationBuilder.CreateTable(
                name: "BannersMunicipio",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MunicipioId = table.Column<Guid>(type: "uuid", nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ImagemUrl = table.Column<string>(type: "text", nullable: false),
                    Link = table.Column<string>(type: "text", nullable: true),
                    Ordem = table.Column<int>(type: "integer", nullable: false),
                    Subtitulo = table.Column<string>(type: "text", nullable: true),
                    Titulo = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BannersMunicipio", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BannersMunicipio_Municipios_MunicipioId",
                        column: x => x.MunicipioId,
                        principalTable: "Municipios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BannersMunicipio_MunicipioId",
                table: "BannersMunicipio",
                column: "MunicipioId");
        }
    }
}
