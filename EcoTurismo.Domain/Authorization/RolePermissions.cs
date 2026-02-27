namespace EcoTurismo.Domain.Authorization;

public static class RolePermissions
{
    private static readonly Dictionary<string, HashSet<string>> _rolePermissions = new()
    {
        [Roles.Admin] = new HashSet<string>
        {
            // Admin tem todas as permissões
            Permissions.BannersCreate, Permissions.BannersRead, Permissions.BannersUpdate, Permissions.BannersDelete, Permissions.BannersReorder,
            Permissions.AtrativosCreate, Permissions.AtrativosRead, Permissions.AtrativosUpdate, Permissions.AtrativosDelete,
            Permissions.QuiosquesCreate, Permissions.QuiosquesRead, Permissions.QuiosquesUpdate, Permissions.QuiosquesDelete,
            Permissions.ReservasCreate, Permissions.ReservasRead, Permissions.ReservasUpdate, Permissions.ReservasDelete, Permissions.ReservasValidate,
            Permissions.ConfiguracoesRead, Permissions.ConfiguracoesUpdate,
            Permissions.ProfilesCreate, Permissions.ProfilesRead, Permissions.ProfilesUpdate, Permissions.ProfilesDelete,
            Permissions.MunicipiosRead
        },

        [Roles.Prefeitura] = new HashSet<string>
        {
            // Prefeitura pode gerenciar banners, atrativos e ver relatórios
            Permissions.BannersCreate, Permissions.BannersRead, Permissions.BannersUpdate, Permissions.BannersDelete, Permissions.BannersReorder,
            Permissions.AtrativosCreate, Permissions.AtrativosRead, Permissions.AtrativosUpdate,
            Permissions.QuiosquesRead,
            Permissions.ReservasRead,
            Permissions.ConfiguracoesRead,
            Permissions.MunicipiosRead
        },

        [Roles.Balneario] = new HashSet<string>
        {
            // Balneário pode gerenciar quiosques e reservas
            Permissions.QuiosquesCreate, Permissions.QuiosquesRead, Permissions.QuiosquesUpdate, Permissions.QuiosquesDelete,
            Permissions.ReservasRead, Permissions.ReservasUpdate, Permissions.ReservasValidate,
            Permissions.AtrativosRead,
            Permissions.MunicipiosRead
        },

        [Roles.Publico] = new HashSet<string>
        {
            // Público pode apenas visualizar e criar reservas
            Permissions.BannersRead,
            Permissions.AtrativosRead,
            Permissions.QuiosquesRead,
            Permissions.ReservasCreate, Permissions.ReservasRead,
            Permissions.ConfiguracoesRead,
            Permissions.MunicipiosRead
        }
    };

    public static bool HasPermission(string role, string permission)
    {
        return _rolePermissions.TryGetValue(role, out var permissions) && 
               permissions.Contains(permission);
    }

    public static IEnumerable<string> GetPermissions(string role)
    {
        return _rolePermissions.TryGetValue(role, out var permissions) 
            ? permissions 
            : Enumerable.Empty<string>();
    }

    public static bool HasAnyPermission(string role, params string[] permissions)
    {
        if (!_rolePermissions.TryGetValue(role, out var rolePermissions))
            return false;

        return permissions.Any(p => rolePermissions.Contains(p));
    }

    public static bool HasAllPermissions(string role, params string[] permissions)
    {
        if (!_rolePermissions.TryGetValue(role, out var rolePermissions))
            return false;

        return permissions.All(p => rolePermissions.Contains(p));
    }
}
