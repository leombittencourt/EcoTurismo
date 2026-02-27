using Microsoft.AspNetCore.Authorization;

namespace EcoTurismo.Api.Authorization;

public static class RolePolicies
{
    // Nomes das roles
    public const string Admin = "Admin";
    public const string Prefeitura = "Prefeitura";
    public const string Balneario = "Balneario";
    public const string Publico = "Publico";

    // Policies para cada role
    public const string AdminPolicy = "RequireAdminRole";
    public const string PrefeituraPolicy = "RequirePrefeituraRole";
    public const string BalnearioPolicy = "RequireBalnearioRole";
    public const string PublicoPolicy = "RequirePublicoRole";

    // Policies compostas
    public const string AdminOrPrefeituraPolicy = "RequireAdminOrPrefeitura";
    public const string AdminOrBalnearioPolicy = "RequireAdminOrBalneario";
    public const string AnyAuthenticatedPolicy = "RequireAnyAuthenticated";

    public static void AddRolePolicies(this AuthorizationOptions options)
    {
        // Policies individuais por role
        options.AddPolicy(AdminPolicy, policy =>
            policy.Requirements.Add(new RoleRequirement(Admin)));

        options.AddPolicy(PrefeituraPolicy, policy =>
            policy.Requirements.Add(new RoleRequirement(Prefeitura)));

        options.AddPolicy(BalnearioPolicy, policy =>
            policy.Requirements.Add(new RoleRequirement(Balneario)));

        options.AddPolicy(PublicoPolicy, policy =>
            policy.Requirements.Add(new RoleRequirement(Publico)));

        // Policies compostas (múltiplas roles)
        options.AddPolicy(AdminOrPrefeituraPolicy, policy =>
            policy.Requirements.Add(new RoleRequirement(Admin, Prefeitura)));

        options.AddPolicy(AdminOrBalnearioPolicy, policy =>
            policy.Requirements.Add(new RoleRequirement(Admin, Balneario)));

        // Qualquer usuário autenticado (qualquer role)
        options.AddPolicy(AnyAuthenticatedPolicy, policy =>
            policy.Requirements.Add(new RoleRequirement(Admin, Prefeitura, Balneario, Publico)));
    }
}
