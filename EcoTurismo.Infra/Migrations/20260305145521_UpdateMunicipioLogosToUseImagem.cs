using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcoTurismo.Infra.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMunicipioLogosToUseImagem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Logo",
                table: "Municipios");

            migrationBuilder.DropColumn(
                name: "LogoAreaPublica",
                table: "Municipios");

            migrationBuilder.DropColumn(
                name: "LogoTelaLogin",
                table: "Municipios");

            migrationBuilder.AddColumn<Guid>(
                name: "LogoAreaPublicaId",
                table: "Municipios",
                type: "uuid",
                nullable: true,
                comment: "FK para imagem do logo da área pública");

            migrationBuilder.AddColumn<Guid>(
                name: "LogoId",
                table: "Municipios",
                type: "uuid",
                nullable: true,
                comment: "FK para imagem do logo geral do município");

            migrationBuilder.AddColumn<Guid>(
                name: "LogoTelaLoginId",
                table: "Municipios",
                type: "uuid",
                nullable: true,
                comment: "FK para imagem do logo da tela de login");

            migrationBuilder.CreateIndex(
                name: "IX_Municipios_LogoAreaPublicaId",
                table: "Municipios",
                column: "LogoAreaPublicaId");

            migrationBuilder.CreateIndex(
                name: "IX_Municipios_LogoId",
                table: "Municipios",
                column: "LogoId");

            migrationBuilder.CreateIndex(
                name: "IX_Municipios_LogoTelaLoginId",
                table: "Municipios",
                column: "LogoTelaLoginId");

            migrationBuilder.AddForeignKey(
                name: "FK_Municipios_Imagens_LogoAreaPublicaId",
                table: "Municipios",
                column: "LogoAreaPublicaId",
                principalTable: "Imagens",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Municipios_Imagens_LogoId",
                table: "Municipios",
                column: "LogoId",
                principalTable: "Imagens",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Municipios_Imagens_LogoTelaLoginId",
                table: "Municipios",
                column: "LogoTelaLoginId",
                principalTable: "Imagens",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Municipios_Imagens_LogoAreaPublicaId",
                table: "Municipios");

            migrationBuilder.DropForeignKey(
                name: "FK_Municipios_Imagens_LogoId",
                table: "Municipios");

            migrationBuilder.DropForeignKey(
                name: "FK_Municipios_Imagens_LogoTelaLoginId",
                table: "Municipios");

            migrationBuilder.DropIndex(
                name: "IX_Municipios_LogoAreaPublicaId",
                table: "Municipios");

            migrationBuilder.DropIndex(
                name: "IX_Municipios_LogoId",
                table: "Municipios");

            migrationBuilder.DropIndex(
                name: "IX_Municipios_LogoTelaLoginId",
                table: "Municipios");

            migrationBuilder.DropColumn(
                name: "LogoAreaPublicaId",
                table: "Municipios");

            migrationBuilder.DropColumn(
                name: "LogoId",
                table: "Municipios");

            migrationBuilder.DropColumn(
                name: "LogoTelaLoginId",
                table: "Municipios");

            migrationBuilder.AddColumn<string>(
                name: "Logo",
                table: "Municipios",
                type: "text",
                nullable: true,
                comment: "URL do logotipo do município");

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
    }
}
