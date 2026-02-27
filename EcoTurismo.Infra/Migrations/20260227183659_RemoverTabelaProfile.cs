using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcoTurismo.Infra.Migrations
{
    /// <inheritdoc />
    public partial class RemoverTabelaProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Validacoes_Profiles_OperadorId",
                table: "Validacoes");

            migrationBuilder.DropTable(
                name: "Profiles");

            migrationBuilder.AddColumn<Guid>(
                name: "MunicipioId1",
                table: "Usuarios",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RoleId1",
                table: "Usuarios",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_MunicipioId1",
                table: "Usuarios",
                column: "MunicipioId1");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_RoleId1",
                table: "Usuarios",
                column: "RoleId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Municipios_MunicipioId1",
                table: "Usuarios",
                column: "MunicipioId1",
                principalTable: "Municipios",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Roles_RoleId1",
                table: "Usuarios",
                column: "RoleId1",
                principalTable: "Roles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Validacoes_Usuarios_OperadorId",
                table: "Validacoes",
                column: "OperadorId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Municipios_MunicipioId1",
                table: "Usuarios");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Roles_RoleId1",
                table: "Usuarios");

            migrationBuilder.DropForeignKey(
                name: "FK_Validacoes_Usuarios_OperadorId",
                table: "Validacoes");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_MunicipioId1",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_RoleId1",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "MunicipioId1",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "RoleId1",
                table: "Usuarios");

            migrationBuilder.CreateTable(
                name: "Profiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, comment: "Identificador único do perfil (mesmo ID do auth)"),
                    AtrativoId = table.Column<Guid>(type: "uuid", nullable: true, comment: "FK para o atrativo vinculado"),
                    MunicipioId = table.Column<Guid>(type: "uuid", nullable: true, comment: "FK para o município vinculado"),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false, comment: "FK para a role do usuário"),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, comment: "Data de criação do registro"),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false, comment: "Endereço de e-mail do usuário"),
                    Nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false, comment: "Nome completo do usuário"),
                    PasswordHash = table.Column<string>(type: "text", nullable: false, comment: "Hash da senha do usuário"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, comment: "Data da última atualização do registro")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Profiles_Atrativos_AtrativoId",
                        column: x => x.AtrativoId,
                        principalTable: "Atrativos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Profiles_Municipios_MunicipioId",
                        column: x => x.MunicipioId,
                        principalTable: "Municipios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Profiles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_AtrativoId",
                table: "Profiles",
                column: "AtrativoId");

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_Email",
                table: "Profiles",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_MunicipioId",
                table: "Profiles",
                column: "MunicipioId");

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_RoleId",
                table: "Profiles",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Validacoes_Profiles_OperadorId",
                table: "Validacoes",
                column: "OperadorId",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
