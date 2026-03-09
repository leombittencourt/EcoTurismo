using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcoTurismo.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditoriaAcoesQuiosques : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditoriasAcoesQuiosques",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, comment: "Identificador unico da auditoria de acao administrativa em quiosque"),
                    QuiosqueId = table.Column<Guid>(type: "uuid", nullable: true, comment: "Quiosque alvo da acao administrativa"),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: true, comment: "Usuario autenticado que executou a acao"),
                    UsuarioNome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false, comment: "Nome do usuario no momento da acao"),
                    UsuarioRole = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, comment: "Role do usuario no momento da acao"),
                    Acao = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, comment: "Acao administrativa executada"),
                    Motivo = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false, comment: "Motivo informado para auditoria"),
                    Payload = table.Column<string>(type: "text", nullable: false, comment: "Snapshot resumido antes/depois da operacao"),
                    ReservasAfetadas = table.Column<int>(type: "integer", nullable: false, comment: "Quantidade de reservas afetadas pela acao"),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, comment: "Data/hora da execucao")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditoriasAcoesQuiosques", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditoriasAcoesQuiosques_Quiosques_QuiosqueId",
                        column: x => x.QuiosqueId,
                        principalTable: "Quiosques",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_AuditoriasAcoesQuiosques_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditoriasAcoesQuiosques_CreatedAt",
                table: "AuditoriasAcoesQuiosques",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AuditoriasAcoesQuiosques_QuiosqueId",
                table: "AuditoriasAcoesQuiosques",
                column: "QuiosqueId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditoriasAcoesQuiosques_UsuarioId",
                table: "AuditoriasAcoesQuiosques",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditoriasAcoesQuiosques");
        }
    }
}
