using EcoTurismo.Domain.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace EcoTurismo.Api.Authorization;

public static class Policies
{
    // Banners
    public const string BannersManage = "BannersManage";
    public const string BannersView = "BannersView";

    // Atrativos
    public const string AtrativosManage = "AtrativosManage";
    public const string AtrativosView = "AtrativosView";

    // Quiosques
    public const string QuiosquesManage = "QuiosquesManage";
    public const string QuiosquesView = "QuiosquesView";

    // Reservas
    public const string ReservasManage = "ReservasManage";
    public const string ReservasView = "ReservasView";
    public const string ReservasCreate = "ReservasCreate";
    public const string ReservasValidate = "ReservasValidate";

    // Configurações
    public const string ConfiguracoesManage = "ConfiguracoesManage";
    public const string ConfiguracoesView = "ConfiguracoesView";

    // Perfis
    public const string ProfilesManage = "ProfilesManage";

    public static void AddPolicies(this AuthorizationOptions options)
    {
        // Criar policies para cada permissão individual
        AddPermissionPolicies(options);

        // Policies compostas (mantidas para compatibilidade)
        // Banners
        options.AddPolicy(BannersManage, policy =>
            policy.Requirements.Add(new PermissionRequirement(
                Permissions.BannersCreate,
                Permissions.BannersUpdate,
                Permissions.BannersDelete,
                Permissions.BannersReorder
            )));

        options.AddPolicy(BannersView, policy =>
            policy.Requirements.Add(new PermissionRequirement(Permissions.BannersRead)));

        // Atrativos
        options.AddPolicy(AtrativosManage, policy =>
            policy.Requirements.Add(new PermissionRequirement(
                Permissions.AtrativosCreate,
                Permissions.AtrativosUpdate,
                Permissions.AtrativosDelete
            )));

        options.AddPolicy(AtrativosView, policy =>
            policy.Requirements.Add(new PermissionRequirement(Permissions.AtrativosRead)));

        // Quiosques
        options.AddPolicy(QuiosquesManage, policy =>
            policy.Requirements.Add(new PermissionRequirement(
                Permissions.QuiosquesCreate,
                Permissions.QuiosquesUpdate,
                Permissions.QuiosquesDelete
            )));

        options.AddPolicy(QuiosquesView, policy =>
            policy.Requirements.Add(new PermissionRequirement(Permissions.QuiosquesRead)));

        // Reservas
        options.AddPolicy(ReservasManage, policy =>
            policy.Requirements.Add(new PermissionRequirement(
                Permissions.ReservasUpdate,
                Permissions.ReservasDelete
            )));

        options.AddPolicy(ReservasView, policy =>
            policy.Requirements.Add(new PermissionRequirement(Permissions.ReservasRead)));

        options.AddPolicy(ReservasCreate, policy =>
            policy.Requirements.Add(new PermissionRequirement(Permissions.ReservasCreate)));

        options.AddPolicy(ReservasValidate, policy =>
            policy.Requirements.Add(new PermissionRequirement(Permissions.ReservasValidate)));

        // Configurações
        options.AddPolicy(ConfiguracoesManage, policy =>
            policy.Requirements.Add(new PermissionRequirement(Permissions.ConfiguracoesUpdate)));

        options.AddPolicy(ConfiguracoesView, policy =>
            policy.Requirements.Add(new PermissionRequirement(Permissions.ConfiguracoesRead)));

        // Perfis
        options.AddPolicy(ProfilesManage, policy =>
            policy.Requirements.Add(new PermissionRequirement(
                Permissions.ProfilesCreate,
                Permissions.ProfilesUpdate,
                Permissions.ProfilesDelete
            )));
    }

    private static void AddPermissionPolicies(AuthorizationOptions options)
    {
        // Usar reflection para criar uma policy para cada constante de permissão
        var permissionType = typeof(Permissions);
        var permissionFields = permissionType.GetFields(
            System.Reflection.BindingFlags.Public | 
            System.Reflection.BindingFlags.Static | 
            System.Reflection.BindingFlags.FlattenHierarchy)
            .Where(f => f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(string));

        foreach (var field in permissionFields)
        {
            var permissionValue = field.GetValue(null)?.ToString();
            if (!string.IsNullOrEmpty(permissionValue))
            {
                options.AddPolicy(permissionValue, policy =>
                    policy.Requirements.Add(new PermissionRequirement(permissionValue)));
            }
        }
    }
}
