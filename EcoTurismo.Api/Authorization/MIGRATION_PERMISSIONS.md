# Migração de Endpoints para Permissões do Domain.Authorization

## Resumo

Todos os endpoints foram atualizados para usar as permissões definidas em `EcoTurismo.Domain.Authorization.Permissions` ao invés das policies estáticas da `EcoTurismo.Api.Authorization`.

## Mudanças Realizadas

### Antes
Os endpoints usavam:
- `Roles("admin", "prefeitura", ...)` - Validação direta de roles
- `Policies(Authorization.Policies.BannersManage)` - Policies da camada API

### Depois
Os endpoints agora usam:
- `Permissions(AuthDomain.Permissions.BannersCreate)` - Permissões da camada Domain

## Arquivos Atualizados

### Banners
- ✅ `CreateBannerEndpoint.cs` - `Permissions.BannersCreate`
- ✅ `UpdateBannerEndpoint.cs` - `Permissions.BannersUpdate`
- ✅ `DeleteBannerEndpoint.cs` - `Permissions.BannersDelete`
- ✅ `ReorderBannersEndpoint.cs` - `Permissions.BannersReorder`

### Atrativos
- ✅ `UpdateAtrativoEndpoint.cs` - `Permissions.AtrativosUpdate`

### Quiosques
- ✅ `CreateQuiosqueEndpoint.cs` - `Permissions.QuiosquesCreate`
- ✅ `UpdateQuiosqueEndpoint.cs` - `Permissions.QuiosquesUpdate`
- ✅ `DeleteQuiosqueEndpoint.cs` - `Permissions.QuiosquesDelete`

### Reservas
- ✅ `UpdateReservaStatusEndpoint.cs` - `Permissions.ReservasUpdate`

### Configurações
- ✅ `BatchUpdateConfiguracoesEndpoint.cs` - `Permissions.ConfiguracoesUpdate`

## Estrutura Usada

### Alias de Namespace
Para evitar conflitos com o método `Permissions()` do FastEndpoints e o namespace `EcoTurismo.Api.Endpoints.Auth`, usamos um alias:

```csharp
using AuthDomain = EcoTurismo.Domain.Authorization;
```

### Uso nos Endpoints
```csharp
public override void Configure()
{
    Post("/api/banners");
    Permissions(AuthDomain.Permissions.BannersCreate);
}
```

## Como o Sistema Funciona

### 1. Permissões no Domain
```csharp
// EcoTurismo.Domain.Authorization.Permissions
public static class Permissions
{
    public const string BannersCreate = "banners:create";
    public const string BannersUpdate = "banners:update";
    // ...
}
```

### 2. Policies Dinâmicas
O `Policies.cs` agora cria automaticamente uma policy para cada permissão usando reflection:

```csharp
private static void AddPermissionPolicies(AuthorizationOptions options)
{
    // Cria uma policy para cada constante em Permissions
    var permissionFields = typeof(Permissions).GetFields(...);
    
    foreach (var field in permissionFields)
    {
        var permissionValue = field.GetValue(null)?.ToString();
        options.AddPolicy(permissionValue, policy =>
            policy.Requirements.Add(new PermissionRequirement(permissionValue)));
    }
}
```

### 3. PermissionAuthorizationHandler
Valida se o usuário tem a permissão necessária:

```csharp
protected override async Task HandleRequirementAsync(
    AuthorizationHandlerContext context,
    PermissionRequirement requirement)
{
    var roleIdClaim = context.User.FindFirst("role_id");
    var roleId = Guid.Parse(roleIdClaim.Value);
    
    // Busca permissões do banco de dados
    if (await _permissionService.HasAnyPermissionAsync(roleId, requirement.Permissions))
    {
        context.Succeed(requirement);
    }
}
```

### 4. PermissionService
Busca as permissões do banco de dados com cache:

```csharp
public async Task<IEnumerable<string>> GetPermissionsByRoleIdAsync(Guid roleId)
{
    // Cache de 60 minutos
    var permissions = await _db.RolePermissions
        .Where(rp => rp.RoleId == roleId)
        .Include(rp => rp.Permission)
        .Select(rp => rp.Permission.Name)
        .ToListAsync();
        
    return permissions;
}
```

## Mapeamento Completo

| Endpoint | Antes | Depois |
|----------|-------|--------|
| POST /api/banners | `Roles("admin", "prefeitura")` | `Permissions(Permissions.BannersCreate)` |
| PUT /api/banners/{Id} | `Roles("admin", "prefeitura")` | `Permissions(Permissions.BannersUpdate)` |
| DELETE /api/banners/{Id} | `Roles("admin", "prefeitura")` | `Permissions(Permissions.BannersDelete)` |
| PUT /api/banners/reorder | `Roles("admin", "prefeitura")` | `Permissions(Permissions.BannersReorder)` |
| PUT /api/atrativos/{Id} | `Roles("admin", "prefeitura")` | `Permissions(Permissions.AtrativosUpdate)` |
| POST /api/quiosques | `Roles("admin", "prefeitura", "balneario")` | `Permissions(Permissions.QuiosquesCreate)` |
| PUT /api/quiosques/{Id} | `Roles("admin", "prefeitura", "balneario")` | `Permissions(Permissions.QuiosquesUpdate)` |
| DELETE /api/quiosques/{Id} | `Roles("admin", "prefeitura", "balneario")` | `Permissions(Permissions.QuiosquesDelete)` |
| PUT /api/reservas/{Id}/status | `Roles("admin", "prefeitura", "balneario")` | `Permissions(Permissions.ReservasUpdate)` |
| PUT /api/configuracoes | `Roles("admin")` | `Permissions(Permissions.ConfiguracoesUpdate)` |

## Vantagens

1. **Centralização**: Todas as permissões definidas em um único lugar (`Domain.Authorization`)
2. **Dinâmico**: Permissões vindas do banco de dados via `PermissionService`
3. **Flexível**: Fácil adicionar novas permissões sem alterar código
4. **Desacoplado**: Domain não depende da camada API
5. **Testável**: Fácil mockar `PermissionService` em testes
6. **Cache**: Performance otimizada com cache de 60 minutos
7. **Auditável**: Histórico de permissões no banco de dados

## Próximos Passos

Para endpoints que ainda não foram migrados:

```csharp
// 1. Adicione o alias
using AuthDomain = EcoTurismo.Domain.Authorization;

// 2. Substitua Roles() ou Policies() antigo
public override void Configure()
{
    // ❌ Antigo
    // Roles("admin", "prefeitura");
    
    // ✅ Novo
    Permissions(AuthDomain.Permissions.NomePermissao);
}
```

## Compilação

✅ **Bem-sucedida** - Todos os 10 endpoints atualizados compilam corretamente.

## Checklist

- [x] Banners (4 endpoints)
- [x] Atrativos (1 endpoint)
- [x] Quiosques (3 endpoints)
- [x] Reservas (1 endpoint)
- [x] Configurações (1 endpoint)
- [ ] Endpoints de leitura (GET) - Podem usar `AllowAnonymous()` ou permissions de leitura
- [ ] Documentação atualizada
- [ ] Testes de integração
