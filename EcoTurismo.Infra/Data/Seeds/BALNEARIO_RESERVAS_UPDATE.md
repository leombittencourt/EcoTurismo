# Atualização de Permissões - Role Balneario

## Mudança Realizada

A role **Balneario** agora pode **criar reservas**, além das permissões que já tinha.

## Justificativa

Reservas podem ser criadas in loco pelos funcionários do balneário, portanto faz sentido que eles tenham essa permissão.

## Alterações

### 1. AuthorizationSeed.cs
Adicionada a permissão `reservas:create` para a role Balneario:

**Antes:**
```csharp
var balnearioPermissions = permissions.Where(p =>
    p.Resource == "quiosques" ||
    (p.Resource == "reservas" && (p.Action == "read" || p.Action == "update" || p.Action == "validate")) ||
    (p.Resource == "atrativos" && p.Action == "read") ||
    p.Resource == "municipios"
);
```

**Depois:**
```csharp
var balnearioPermissions = permissions.Where(p =>
    p.Resource == "quiosques" ||
    (p.Resource == "reservas" && (p.Action == "create" || p.Action == "read" || p.Action == "update" || p.Action == "validate")) ||
    (p.Resource == "atrativos" && p.Action == "read") ||
    p.Resource == "municipios"
);
```

### 2. CreateReservaEndpoint.cs
Mudado de `AllowAnonymous()` para `Permissions()`:

**Antes:**
```csharp
public override void Configure()
{
    Post("/api/reservas");
    AllowAnonymous();
}
```

**Depois:**
```csharp
public override void Configure()
{
    Post("/api/reservas");
    Permissions(AuthDomain.Permissions.ReservasCreate);
}
```

## Permissões de Reservas Atualizadas

### 🔴 ADMIN (5/5)
- ✅ `reservas:create`
- ✅ `reservas:read`
- ✅ `reservas:update`
- ✅ `reservas:delete`
- ✅ `reservas:validate`

### 🟠 PREFEITURA (1/5)
- ❌ `reservas:create`
- ✅ `reservas:read`
- ❌ `reservas:update`
- ❌ `reservas:delete`
- ❌ `reservas:validate`

### 🟡 BALNEARIO (4/5) - ⭐ ATUALIZADO
- ✅ `reservas:create` ← **NOVA**
- ✅ `reservas:read`
- ✅ `reservas:update`
- ❌ `reservas:delete`
- ✅ `reservas:validate`

### 🟢 PUBLICO (2/5)
- ✅ `reservas:create`
- ✅ `reservas:read`
- ❌ `reservas:update`
- ❌ `reservas:delete`
- ❌ `reservas:validate`

## Impacto

1. **Endpoint agora requer autenticação**: Usuários não autenticados não poderão mais criar reservas diretamente via API
2. **Balneario pode criar reservas**: Funcionários do balneário podem criar reservas in loco
3. **Publico ainda pode criar**: Usuários públicos autenticados ainda podem criar suas próprias reservas

## ⚠️ Atenção

Se você quiser manter a funcionalidade de **criação de reservas sem autenticação** (usuários anônimos), considere:

### Opção 1: Dois Endpoints
- `/api/reservas` (anônimo) - Para público criar suas reservas
- `/api/reservas/in-loco` (autenticado) - Para balneário criar reservas in loco

### Opção 2: AllowAnonymous com Optional Auth
```csharp
public override void Configure()
{
    Post("/api/reservas");
    AllowAnonymous();
    // A autenticação é opcional, mas se autenticado valida permissões
}
```

### Opção 3: Manter como está
Exigir autenticação para criar reservas (decisão tomada atualmente).

## Próximos Passos

1. ✅ AuthorizationSeed atualizado
2. ✅ CreateReservaEndpoint atualizado
3. ✅ Compilação bem-sucedida
4. ⬜ Executar seed novamente ou atualizar via SQL:
   ```sql
   -- Adicionar permissão reservas:create para Balneario
   INSERT INTO "RolePermissions" ("RoleId", "PermissionId", "GrantedAt")
   SELECT 
       r."Id" as "RoleId",
       p."Id" as "PermissionId",
       NOW() as "GrantedAt"
   FROM "Roles" r
   CROSS JOIN "Permissions" p
   WHERE r."NormalizedName" = 'BALNEARIO'
     AND p."Name" = 'reservas:create'
     AND NOT EXISTS (
         SELECT 1 FROM "RolePermissions" rp 
         WHERE rp."RoleId" = r."Id" AND rp."PermissionId" = p."Id"
     );
   ```
5. ⬜ Testar criação de reservas com role Balneario
6. ⬜ Atualizar documentação da API

## Resumo

| Antes | Depois |
|-------|--------|
| Endpoint público (sem auth) | Endpoint protegido por permissão |
| Balneario: 3 permissões de reservas | Balneario: 4 permissões de reservas |
| Apenas Publico e Admin podiam criar | Publico, Admin e Balneario podem criar |
