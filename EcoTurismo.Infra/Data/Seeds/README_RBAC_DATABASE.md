# Sistema RBAC com Banco de Dados - EcoTurismo

## Visão Geral

Sistema completo de **RBAC (Role-Based Access Control)** implementado com entidades do banco de dados, substituindo as classes estáticas por um modelo dinâmico e gerenciável.

## Estrutura do Sistema

### 1. Entidades Criadas

#### Role (Roles)
```csharp
public class Role
{
    public Guid Id { get; set; }
    public string Name { get; set; }              // "Admin", "Prefeitura", etc.
    public string NormalizedName { get; set; }     // "ADMIN", "PREFEITURA", etc.
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}
```

#### Permission (Permissions)
```csharp
public class Permission
{
    public Guid Id { get; set; }
    public string Name { get; set; }        // "banners:create"
    public string Resource { get; set; }    // "banners"
    public string Action { get; set; }      // "create"
    public string? Description { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
```

#### RolePermission (RolePermissions)
```csharp
public class RolePermission
{
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }
    public DateTimeOffset GrantedAt { get; set; }
}
```

### 2. Mudança no Profile

**Antes:**
```csharp
public string Role { get; set; } = "publico";
```

**Depois:**
```csharp
public Guid RoleId { get; set; }
public Role Role { get; set; } = null!;  // Navigation property
```

### 3. Arquivos Criados

**Domain Layer:**
- `Domain/Entities/Role.cs`
- `Domain/Entities/Permission.cs`
- `Domain/Entities/RolePermission.cs`
- `Domain/Entities/Profile.cs` (atualizado)

**Infrastructure Layer:**
- `Infra/Configurations/RoleConfiguration.cs`
- `Infra/Configurations/PermissionConfiguration.cs`
- `Infra/Configurations/RolePermissionConfiguration.cs`
- `Infra/Configurations/ProfileConfiguration.cs` (atualizado)
- `Infra/Data/Seeds/AuthorizationSeed.cs`

**Application Layer:**
- `Application/Interfaces/IPermissionService.cs`
- `Application/Services/PermissionService.cs`
- `Application/Services/AuthService.cs` (atualizado)

**API Layer:**
- `Api/Authorization/PermissionAuthorizationHandler.cs` (atualizado)
- `Api/Program.cs` (atualizado)
- `Api/Endpoints/Profiles/Me/MeEndpoint.cs` (atualizado)

### 4. Tabelas do Banco de Dados

```sql
-- Roles
CREATE TABLE Roles (
    Id UUID PRIMARY KEY,
    Name VARCHAR(50) NOT NULL,
    NormalizedName VARCHAR(50) NOT NULL UNIQUE,
    Description VARCHAR(500),
    IsActive BOOLEAN DEFAULT TRUE,
    CreatedAt TIMESTAMPTZ NOT NULL,
    UpdatedAt TIMESTAMPTZ NOT NULL
);

-- Permissions
CREATE TABLE Permissions (
    Id UUID PRIMARY KEY,
    Name VARCHAR(100) NOT NULL UNIQUE,
    Resource VARCHAR(50) NOT NULL,
    Action VARCHAR(50) NOT NULL,
    Description VARCHAR(500),
    CreatedAt TIMESTAMPTZ NOT NULL,
    UNIQUE(Resource, Action)
);

-- RolePermissions (N:N)
CREATE TABLE RolePermissions (
    RoleId UUID NOT NULL,
    PermissionId UUID NOT NULL,
    GrantedAt TIMESTAMPTZ NOT NULL,
    PRIMARY KEY (RoleId, PermissionId),
    FOREIGN KEY (RoleId) REFERENCES Roles(Id) ON DELETE CASCADE,
    FOREIGN KEY (PermissionId) REFERENCES Permissions(Id) ON DELETE CASCADE
);

-- Profiles (atualizado)
ALTER TABLE Profiles
DROP COLUMN Role,
ADD COLUMN RoleId UUID NOT NULL,
ADD CONSTRAINT FK_Profiles_Roles 
    FOREIGN KEY (RoleId) REFERENCES Roles(Id) ON DELETE RESTRICT;
```

### 5. Índices Criados

```sql
CREATE INDEX IX_Roles_NormalizedName ON Roles(NormalizedName);
CREATE INDEX IX_Permissions_Name ON Permissions(Name);
CREATE INDEX IX_Permissions_Resource_Action ON Permissions(Resource, Action);
CREATE INDEX IX_RolePermissions_RoleId ON RolePermissions(RoleId);
CREATE INDEX IX_RolePermissions_PermissionId ON RolePermissions(PermissionId);
CREATE INDEX IX_Profiles_RoleId ON Profiles(RoleId);
```

## Como Usar

### 1. Criar Migration

```bash
dotnet ef migrations add AdicionarSistemaRBAC --project EcoTurismo.Infra --startup-project EcoTurismo.Api
```

### 2. Aplicar Migration

```bash
dotnet ef database update --project EcoTurismo.Infra --startup-project EcoTurismo.Api
```

### 3. Popular Dados Iniciais (Seed)

Adicione no `Program.cs` antes de `app.Run()`:

```csharp
// Seed de autorização
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<EcoTurismoDbContext>();
    await EcoTurismo.Infra.Data.Seeds.AuthorizationSeed.SeedAsync(db);
}

app.Run();
```

## Roles e Permissões Padrão

### Admin
- **Todas** as permissões do sistema

### Prefeitura
- Banners: create, read, update, delete, reorder
- Atrativos: create, read, update
- Quiosques: read
- Reservas: read
- Configurações: read
- Municípios: read

### Balneario
- Quiosques: create, read, update, delete
- Reservas: read, update, validate
- Atrativos: read
- Municípios: read

### Publico
- Banners: read
- Atrativos: read
- Quiosques: read
- Reservas: create, read
- Configurações: read
- Municípios: read

## Serviços

### IPermissionService

```csharp
// Buscar permissões por Role ID
var permissions = await _permissionService.GetPermissionsByRoleIdAsync(roleId);

// Buscar permissões por nome da role
var permissions = await _permissionService.GetPermissionsByRoleNameAsync("Admin");

// Verificar se tem uma permissão
var hasPermission = await _permissionService.HasPermissionAsync(roleId, "banners:create");

// Verificar se tem qualquer permissão
var hasAny = await _permissionService.HasAnyPermissionAsync(roleId, 
    "banners:create", "banners:update");

// Buscar role por nome
var role = await _permissionService.GetRoleByNameAsync("Admin");

// Buscar role por ID
var role = await _permissionService.GetRoleByIdAsync(roleId);
```

## Token JWT

O token agora inclui:

```json
{
  "nameid": "user-guid",
  "email": "user@example.com",
  "name": "Nome do Usuário",
  "role": "Admin",
  "role_id": "role-guid",
  "role_name": "Admin",
  "permission": [
    "banners:create",
    "banners:read",
    "banners:update",
    "..."
  ],
  "municipio_id": "municipio-guid",
  "atrativo_id": "atrativo-guid"
}
```

## Cache

O `PermissionService` utiliza `IMemoryCache` para otimizar as consultas:
- Cache de permissões por role: 60 minutos
- Cache de roles: 60 minutos
- Chaves de cache: `Permissions_RoleId_{guid}`, `Permissions_RoleName_{name}`

## Gerenciamento Dinâmico

### Adicionar Nova Role

```csharp
var role = new Role
{
    Id = Guid.NewGuid(),
    Name = "NovaRole",
    NormalizedName = "NOVAROLE",
    Description = "Descrição da nova role",
    IsActive = true,
    CreatedAt = DateTimeOffset.UtcNow,
    UpdatedAt = DateTimeOffset.UtcNow
};

await _db.Roles.AddAsync(role);
await _db.SaveChangesAsync();
```

### Adicionar Nova Permission

```csharp
var permission = new Permission
{
    Id = Guid.NewGuid(),
    Name = "recurso:acao",
    Resource = "recurso",
    Action = "acao",
    Description = "Descrição da permissão",
    CreatedAt = DateTimeOffset.UtcNow
};

await _db.Permissions.AddAsync(permission);
await _db.SaveChangesAsync();
```

### Atribuir Permission a Role

```csharp
var rolePermission = new RolePermission
{
    RoleId = roleId,
    PermissionId = permissionId,
    GrantedAt = DateTimeOffset.UtcNow
};

await _db.RolePermissions.AddAsync(rolePermission);
await _db.SaveChangesAsync();

// Limpar cache
_cache.Remove($"Permissions_RoleId_{roleId}");
```

## Vantagens do Sistema

1. **Dinâmico**: Roles e permissões gerenciadas via banco de dados
2. **Escalável**: Fácil adicionar novas roles e permissões
3. **Auditável**: Histórico de quando permissões foram concedidas
4. **Performance**: Cache de permissões para otimizar consultas
5. **Flexível**: Não depende de código para alterar permissões
6. **Administrável**: Pode criar interface para gerenciar roles/permissions
7. **Seguro**: Validação em tempo real via banco de dados

## Migrando Código Existente

### Seeds de Perfis

Se você tem seeds de perfis, atualize de:

```csharp
var profile = new Profile
{
    Role = "admin"  // ❌ Antigo
};
```

Para:

```csharp
var adminRole = await _db.Roles
    .FirstOrDefaultAsync(r => r.NormalizedName == "ADMIN");

var profile = new Profile
{
    RoleId = adminRole.Id  // ✅ Novo
};
```

## Próximos Passos

1. ✅ Criar migration
2. ✅ Aplicar migration
3. ✅ Executar seed de autorização
4. ⬜ Atualizar seeds de perfis existentes
5. ⬜ Criar endpoints de gerenciamento de roles/permissions
6. ⬜ Criar interface de administração
7. ⬜ Adicionar auditoria de mudanças
8. ⬜ Implementar rate limiting por role

## Troubleshooting

### Cache não está atualizando

Limpe o cache após alterações:
```csharp
_cache.Remove($"Permissions_RoleId_{roleId}");
_cache.Remove($"Permissions_RoleName_{normalizedName}");
_cache.Remove($"Permissions_Role_{normalizedName}");
_cache.Remove($"Permissions_RoleById_{roleId}");
```

### Profile não tem role após migration

Execute o seed de autorização e atualize os profiles existentes:
```sql
-- Buscar role "Publico"
SELECT Id FROM Roles WHERE NormalizedName = 'PUBLICO';

-- Atualizar profiles sem role
UPDATE Profiles 
SET RoleId = (SELECT Id FROM Roles WHERE NormalizedName = 'PUBLICO')
WHERE RoleId IS NULL;
```

## Checklist

- [x] Entidades criadas
- [x] Configurações do EF Core
- [x] DbContext atualizado
- [x] Seed de autorização
- [x] PermissionService implementado
- [x] AuthService atualizado
- [x] PermissionAuthorizationHandler atualizado
- [x] Program.cs configurado
- [x] Compilação bem-sucedida
- [ ] Migration criada
- [ ] Migration aplicada
- [ ] Seed executado
- [ ] Testes realizados
