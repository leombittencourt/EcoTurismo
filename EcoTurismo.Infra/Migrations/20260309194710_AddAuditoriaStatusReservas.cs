using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcoTurismo.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditoriaStatusReservas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditoriasStatusReservas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, comment: "Identificador unico da auditoria de status da reserva"),
                    ReservaId = table.Column<Guid>(type: "uuid", nullable: false, comment: "Reserva alterada"),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: true, comment: "Usuario autenticado que executou a alteracao"),
                    UsuarioNome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false, comment: "Nome do usuario no momento da alteracao"),
                    UsuarioRole = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, comment: "Role do usuario no momento da alteracao"),
                    StatusAnterior = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false, comment: "Status anterior da reserva"),
                    StatusNovo = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false, comment: "Novo status da reserva"),
                    Motivo = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false, comment: "Motivo informado para alteracao de status"),
                    Payload = table.Column<string>(type: "text", nullable: false, comment: "Snapshot da reserva na alteracao"),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, comment: "Data/hora da alteracao")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditoriasStatusReservas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditoriasStatusReservas_Reservas_ReservaId",
                        column: x => x.ReservaId,
                        principalTable: "Reservas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AuditoriasStatusReservas_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditoriasStatusReservas_CreatedAt",
                table: "AuditoriasStatusReservas",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AuditoriasStatusReservas_ReservaId",
                table: "AuditoriasStatusReservas",
                column: "ReservaId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditoriasStatusReservas_UsuarioId",
                table: "AuditoriasStatusReservas",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditoriasStatusReservas");
        }
    }
}
