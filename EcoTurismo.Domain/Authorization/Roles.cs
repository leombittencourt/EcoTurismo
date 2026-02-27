namespace EcoTurismo.Domain.Authorization;

public static class Roles
{
    public const string Admin = "admin";
    public const string Prefeitura = "prefeitura";
    public const string Balneario = "balneario";
    public const string Publico = "publico";

    public static readonly string[] All = { Admin, Prefeitura, Balneario, Publico };
    public static readonly string[] Staff = { Admin, Prefeitura, Balneario };
    public static readonly string[] Management = { Admin, Prefeitura };
}
