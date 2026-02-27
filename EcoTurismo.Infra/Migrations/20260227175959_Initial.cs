using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcoTurismo.Infra.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Banners",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, comment: "Identificador único do banner"),
                    Titulo = table.Column<string>(type: "text", nullable: true, comment: "Título exibido no banner"),
                    Subtitulo = table.Column<string>(type: "text", nullable: true, comment: "Subtítulo exibido no banner"),
                    ImagemUrl = table.Column<string>(type: "text", nullable: false, comment: "URL da imagem do banner"),
                    Link = table.Column<string>(type: "text", nullable: true, comment: "Link de redirecionamento ao clicar no banner"),
                    Ordem = table.Column<int>(type: "integer", nullable: false, comment: "Ordem de exibição do banner"),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false, comment: "Indica se o banner está ativo para exibição"),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, comment: "Data de criação do registro"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, comment: "Data da última atualização do registro")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Banners", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConfiguracoesSistema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, comment: "Identificador único da configuração"),
                    Chave = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false, comment: "Chave única da configuração"),
                    Valor = table.Column<string>(type: "text", nullable: true, comment: "Valor da configuração"),
                    Descricao = table.Column<string>(type: "text", nullable: true, comment: "Descrição da finalidade da configuração"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, comment: "Data da última atualização do registro")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracoesSistema", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Municipios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, comment: "Identificador único do município"),
                    Nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false, comment: "Nome do município"),
                    Uf = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false, comment: "Unidade federativa (sigla do estado)"),
                    Logo = table.Column<string>(type: "text", nullable: true, comment: "URL do logotipo do município"),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, comment: "Data de criação do registro")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Municipios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, comment: "Identificador único da permissão"),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false, comment: "Nome completo da permissão (ex: banners:create)"),
                    Resource = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, comment: "Recurso da permissão (ex: banners, atrativos)"),
                    Action = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, comment: "Ação da permissão (ex: create, read, update, delete)"),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true, comment: "Descrição da finalidade da permissão"),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, comment: "Data de criação do registro")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, comment: "Identificador único da role"),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, comment: "Nome da role (ex: Admin, Prefeitura)"),
                    NormalizedName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, comment: "Nome normalizado em maiúsculas para busca"),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true, comment: "Descrição da finalidade da role"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true, comment: "Indica se a role está ativa"),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, comment: "Data de criação do registro"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, comment: "Data da última atualização do registro")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Atrativos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, comment: "Identificador único do atrativo"),
                    MunicipioId = table.Column<Guid>(type: "uuid", nullable: false, comment: "FK para o município ao qual o atrativo pertence"),
                    Nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false, comment: "Nome do atrativo turístico"),
                    Tipo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, comment: "Tipo do atrativo (balneario, parque, etc.)"),
                    Descricao = table.Column<string>(type: "text", nullable: true, comment: "Descrição detalhada do atrativo"),
                    Imagem = table.Column<string>(type: "text", nullable: true, comment: "URL da imagem do atrativo"),
                    CapacidadeMaxima = table.Column<int>(type: "integer", nullable: false, comment: "Capacidade máxima de visitantes"),
                    OcupacaoAtual = table.Column<int>(type: "integer", nullable: false, comment: "Quantidade atual de visitantes no local"),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, comment: "Status do atrativo (ativo, inativo)"),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, comment: "Data de criação do registro"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, comment: "Data da última atualização do registro")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Atrativos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Atrativos_Municipios_MunicipioId",
                        column: x => x.MunicipioId,
                        principalTable: "Municipios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false, comment: "FK para a role"),
                    PermissionId = table.Column<Guid>(type: "uuid", nullable: false, comment: "FK para a permissão"),
                    GrantedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, comment: "Data em que a permissão foi concedida à role")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => new { x.RoleId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Profiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, comment: "Identificador único do perfil (mesmo ID do auth)"),
                    Nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false, comment: "Nome completo do usuário"),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false, comment: "Endereço de e-mail do usuário"),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false, comment: "FK para a role do usuário"),
                    MunicipioId = table.Column<Guid>(type: "uuid", nullable: true, comment: "FK para o município vinculado"),
                    AtrativoId = table.Column<Guid>(type: "uuid", nullable: true, comment: "FK para o atrativo vinculado"),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, comment: "Data de criação do registro"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, comment: "Data da última atualização do registro"),
                    PasswordHash = table.Column<string>(type: "text", nullable: false, comment: "Hash da senha do usuário")
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

            migrationBuilder.CreateTable(
                name: "Quiosques",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, comment: "Identificador único do quiosque"),
                    AtrativoId = table.Column<Guid>(type: "uuid", nullable: true, comment: "FK para o atrativo ao qual o quiosque pertence"),
                    Numero = table.Column<int>(type: "integer", nullable: false, comment: "Número identificador do quiosque"),
                    TemChurrasqueira = table.Column<bool>(type: "boolean", nullable: false, comment: "Indica se o quiosque possui churrasqueira"),
                    Status = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false, comment: "Status do quiosque (disponivel, ocupado, manutencao)"),
                    PosicaoX = table.Column<int>(type: "integer", nullable: false, comment: "Posição X do quiosque no mapa"),
                    PosicaoY = table.Column<int>(type: "integer", nullable: false, comment: "Posição Y do quiosque no mapa"),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, comment: "Data de criação do registro"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, comment: "Data da última atualização do registro")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quiosques", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Quiosques_Atrativos_AtrativoId",
                        column: x => x.AtrativoId,
                        principalTable: "Atrativos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Reservas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, comment: "Identificador único da reserva"),
                    AtrativoId = table.Column<Guid>(type: "uuid", nullable: false, comment: "FK para o atrativo reservado"),
                    QuiosqueId = table.Column<Guid>(type: "uuid", nullable: true, comment: "FK para o quiosque reservado (opcional)"),
                    NomeVisitante = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false, comment: "Nome completo do visitante"),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false, comment: "E-mail do visitante"),
                    Cpf = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: false, comment: "CPF do visitante"),
                    CidadeOrigem = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false, comment: "Cidade de origem do visitante"),
                    UfOrigem = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false, comment: "UF de origem do visitante"),
                    Tipo = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false, comment: "Tipo da reserva (day_use, pernoite)"),
                    Data = table.Column<DateOnly>(type: "date", nullable: false, comment: "Data de início da reserva"),
                    DataFim = table.Column<DateOnly>(type: "date", nullable: true, comment: "Data de fim da reserva (para pernoite)"),
                    QuantidadePessoas = table.Column<int>(type: "integer", nullable: false, comment: "Quantidade de pessoas na reserva"),
                    Status = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false, comment: "Status da reserva (confirmada, cancelada, utilizada)"),
                    Token = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, comment: "Token único para validação da reserva"),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, comment: "Data de criação do registro")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reservas_Atrativos_AtrativoId",
                        column: x => x.AtrativoId,
                        principalTable: "Atrativos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reservas_Quiosques_QuiosqueId",
                        column: x => x.QuiosqueId,
                        principalTable: "Quiosques",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Validacoes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, comment: "Identificador único da validação"),
                    ReservaId = table.Column<Guid>(type: "uuid", nullable: true, comment: "FK para a reserva validada"),
                    AtrativoId = table.Column<Guid>(type: "uuid", nullable: true, comment: "FK para o atrativo onde ocorreu a validação"),
                    OperadorId = table.Column<Guid>(type: "uuid", nullable: true, comment: "FK para o operador que realizou a validação"),
                    Token = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, comment: "Token validado"),
                    Valido = table.Column<bool>(type: "boolean", nullable: false, comment: "Indica se o token era válido no momento da validação"),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, comment: "Data de criação do registro")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Validacoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Validacoes_Atrativos_AtrativoId",
                        column: x => x.AtrativoId,
                        principalTable: "Atrativos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Validacoes_Profiles_OperadorId",
                        column: x => x.OperadorId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Validacoes_Reservas_ReservaId",
                        column: x => x.ReservaId,
                        principalTable: "Reservas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Atrativos_MunicipioId",
                table: "Atrativos",
                column: "MunicipioId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguracoesSistema_Chave",
                table: "ConfiguracoesSistema",
                column: "Chave",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Name",
                table: "Permissions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Resource_Action",
                table: "Permissions",
                columns: new[] { "Resource", "Action" },
                unique: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_Quiosques_AtrativoId",
                table: "Quiosques",
                column: "AtrativoId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_AtrativoId",
                table: "Reservas",
                column: "AtrativoId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_QuiosqueId",
                table: "Reservas",
                column: "QuiosqueId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_Token",
                table: "Reservas",
                column: "Token");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_RoleId",
                table: "RolePermissions",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_NormalizedName",
                table: "Roles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Validacoes_AtrativoId",
                table: "Validacoes",
                column: "AtrativoId");

            migrationBuilder.CreateIndex(
                name: "IX_Validacoes_OperadorId",
                table: "Validacoes",
                column: "OperadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Validacoes_ReservaId",
                table: "Validacoes",
                column: "ReservaId");

            migrationBuilder.CreateIndex(
                name: "IX_Validacoes_Token",
                table: "Validacoes",
                column: "Token");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Banners");

            migrationBuilder.DropTable(
                name: "ConfiguracoesSistema");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "Validacoes");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "Profiles");

            migrationBuilder.DropTable(
                name: "Reservas");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Quiosques");

            migrationBuilder.DropTable(
                name: "Atrativos");

            migrationBuilder.DropTable(
                name: "Municipios");
        }
    }
}
