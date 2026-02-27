using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcoTurismo.Infra.Migrations
{
    /// <inheritdoc />
    public partial class Initial2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, comment: "Identificador único do usuário"),
                    Nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false, comment: "Nome completo do usuário"),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false, comment: "Endereço de e-mail do usuário"),
                    PasswordHash = table.Column<string>(type: "text", nullable: false, comment: "Hash da senha do usuário"),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false, comment: "FK para a role do usuário"),
                    MunicipioId = table.Column<Guid>(type: "uuid", nullable: true, comment: "FK para o município vinculado"),
                    AtrativoId = table.Column<Guid>(type: "uuid", nullable: true, comment: "FK para o atrativo vinculado"),
                    Telefone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true, comment: "Telefone do usuário"),
                    Cpf = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: true, comment: "CPF do usuário"),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true, comment: "Indica se o usuário está ativo no sistema"),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, comment: "Data de criação do registro"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, comment: "Data da última atualização do registro")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Usuarios_Atrativos_AtrativoId",
                        column: x => x.AtrativoId,
                        principalTable: "Atrativos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Usuarios_Municipios_MunicipioId",
                        column: x => x.MunicipioId,
                        principalTable: "Municipios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Usuarios_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_AtrativoId",
                table: "Usuarios",
                column: "AtrativoId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Cpf",
                table: "Usuarios",
                column: "Cpf",
                unique: true,
                filter: "\"Cpf\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_MunicipioId",
                table: "Usuarios",
                column: "MunicipioId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_RoleId",
                table: "Usuarios",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
