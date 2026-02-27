namespace EcoTurismo.Domain.Authorization;

public static class Permissions
{
    // Banners
    public const string BannersCreate = "banners:create";
    public const string BannersRead = "banners:read";
    public const string BannersUpdate = "banners:update";
    public const string BannersDelete = "banners:delete";
    public const string BannersReorder = "banners:reorder";

    // Atrativos
    public const string AtrativosCreate = "atrativos:create";
    public const string AtrativosRead = "atrativos:read";
    public const string AtrativosUpdate = "atrativos:update";
    public const string AtrativosDelete = "atrativos:delete";

    // Quiosques
    public const string QuiosquesCreate = "quiosques:create";
    public const string QuiosquesRead = "quiosques:read";
    public const string QuiosquesUpdate = "quiosques:update";
    public const string QuiosquesDelete = "quiosques:delete";

    // Reservas
    public const string ReservasCreate = "reservas:create";
    public const string ReservasRead = "reservas:read";
    public const string ReservasUpdate = "reservas:update";
    public const string ReservasDelete = "reservas:delete";
    public const string ReservasValidate = "reservas:validate";

    // Configurações
    public const string ConfiguracoesRead = "configuracoes:read";
    public const string ConfiguracoesUpdate = "configuracoes:update";

    // Perfis
    public const string ProfilesCreate = "profiles:create";
    public const string ProfilesRead = "profiles:read";
    public const string ProfilesUpdate = "profiles:update";
    public const string ProfilesDelete = "profiles:delete";

    // Municipios
    public const string MunicipiosRead = "municipios:read";
}
