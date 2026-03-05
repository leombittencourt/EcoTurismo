using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcoTurismo.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddImagemTableAndUpdateBanner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Banners_Municipios_MunicipioId",
                table: "Banners");

            migrationBuilder.DropColumn(
                name: "ImagemUrl",
                table: "Banners");

            migrationBuilder.AlterColumn<Guid>(
                name: "MunicipioId",
                table: "Banners",
                type: "uuid",
                nullable: true,
                comment: "FK para o município (opcional)",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ImagemId",
                table: "Banners",
                type: "uuid",
                nullable: true,
                comment: "FK para a imagem do banner");

            migrationBuilder.CreateTable(
                name: "Imagens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, comment: "Identificador único da imagem"),
                    EntidadeTipo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, comment: "Tipo da entidade que possui esta imagem (ex: Banner, Atrativo, Municipio)"),
                    EntidadeId = table.Column<Guid>(type: "uuid", nullable: false, comment: "ID da entidade que possui esta imagem"),
                    Categoria = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, comment: "Categoria da imagem (principal, thumbnail, galeria, logo_login, logo_publico)"),
                    ImagemUrl = table.Column<string>(type: "text", nullable: false, comment: "URI ou caminho da imagem (base64, URL de OCI, S3, etc)"),
                    ThumbnailUrl = table.Column<string>(type: "text", nullable: true, comment: "URI ou caminho da versão redimensionada (thumbnail/preview)"),
                    StorageProvider = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true, comment: "Provedor de armazenamento (base64, oci, s3, azure) - Opcional para compatibilidade"),
                    Ordem = table.Column<int>(type: "integer", nullable: false, defaultValue: 0, comment: "Ordem de exibição (para galerias)"),
                    MetadadosJson = table.Column<string>(type: "text", nullable: false, defaultValue: "{}", comment: "Metadados da imagem em JSON (nome, tamanho, dimensões, etc)"),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, comment: "Data de criação do registro"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, comment: "Data da última atualização do registro")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Imagens", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Banners_ImagemId",
                table: "Banners",
                column: "ImagemId");

            migrationBuilder.CreateIndex(
                name: "IX_Banners_Ordem",
                table: "Banners",
                column: "Ordem");

            migrationBuilder.CreateIndex(
                name: "IX_Imagens_CreatedAt",
                table: "Imagens",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Imagens_Entidade",
                table: "Imagens",
                columns: new[] { "EntidadeTipo", "EntidadeId" });

            migrationBuilder.CreateIndex(
                name: "IX_Imagens_Entidade_Categoria",
                table: "Imagens",
                columns: new[] { "EntidadeTipo", "EntidadeId", "Categoria" });

            migrationBuilder.AddForeignKey(
                name: "FK_Banners_Imagens_ImagemId",
                table: "Banners",
                column: "ImagemId",
                principalTable: "Imagens",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Banners_Municipios_MunicipioId",
                table: "Banners",
                column: "MunicipioId",
                principalTable: "Municipios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Banners_Imagens_ImagemId",
                table: "Banners");

            migrationBuilder.DropForeignKey(
                name: "FK_Banners_Municipios_MunicipioId",
                table: "Banners");

            migrationBuilder.DropTable(
                name: "Imagens");

            migrationBuilder.DropIndex(
                name: "IX_Banners_ImagemId",
                table: "Banners");

            migrationBuilder.DropIndex(
                name: "IX_Banners_Ordem",
                table: "Banners");

            migrationBuilder.DropColumn(
                name: "ImagemId",
                table: "Banners");

            migrationBuilder.AlterColumn<Guid>(
                name: "MunicipioId",
                table: "Banners",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true,
                oldComment: "FK para o município (opcional)");

            migrationBuilder.AddColumn<string>(
                name: "ImagemUrl",
                table: "Banners",
                type: "text",
                nullable: false,
                defaultValue: "",
                comment: "URL da imagem do banner");

            migrationBuilder.AddForeignKey(
                name: "FK_Banners_Municipios_MunicipioId",
                table: "Banners",
                column: "MunicipioId",
                principalTable: "Municipios",
                principalColumn: "Id");
        }
    }
}
