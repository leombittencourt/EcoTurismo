# Troubleshooting: 403 Forbidden com Token Válido

## 🔴 Problema

Usuário Admin recebe **403 Forbidden** mesmo com token JWT válido e todas as permissões.

## ✅ Soluções Aplicadas

### 1. Endpoint de Debug Criado
`GET /api/auth/debug-token`

Use este endpoint para ver **exatamente** o que está no seu token:
```bash
GET https://localhost:5001/api/auth/debug-token
Authorization: Bearer {seu_token}
```

**Resposta mostra:**
- Claims do token
- role_id
- role_name  
- Todas as permissões

### 2. Logs Adicionados ao Handler

Agora o `PermissionAuthorizationHandler` loga:
- ✅ Claims do usuário
- ✅ Permissões necessárias
- ✅ Permissões do usuário
- ✅ Resultado da autorização

### 3. Endpoint Corrigido

`ListUsuariosEndpoint.cs` agora usa:
```csharp
Policies("profiles:read");  // ✅ Correto
```

Ao invés de:
```csharp
Permissions(AuthDomain.Permissions.ProfilesRead);  // ❌ Não funciona no FastEndpoints
```

## 🧪 Como Diagnosticar

### Passo 1: Verificar o Token
```bash
# 1. Fazer login
POST https://localhost:5001/api/auth/login
{
  "email": "admin@ecoturismo.com.br",
  "password": "admin123"
}

# 2. Copiar o token da resposta

# 3. Verificar conteúdo do token
GET https://localhost:5001/api/auth/debug-token
Authorization: Bearer {token}
```

**O que verificar na resposta:**
```json
{
  "isAuthenticated": true,  // ✅ Deve ser true
  "name": "Administrador do Sistema",
  "claims": [
    { "type": "role_id", "value": "guid-da-role" },  // ✅ Deve existir
    { "type": "permission", "value": "banners:create" },
    { "type": "permission", "value": "profiles:read" }  // ✅ Deve ter esta
  ],
  "roleId": "guid",  // ✅ Deve existir
  "roleName": "Admin",  // ✅ Deve ser Admin
  "permissions": [  // ✅ Admin deve ter 26 permissões
    "banners:create",
    "banners:read",
    "profiles:read",  // ✅ Esta é necessária
    ...
  ]
}
```

### Passo 2: Verificar Logs

Ao tentar acessar `/api/usuarios`, você verá nos logs:

**Se funcionar (✅):**
```
🔐 Verificando permissões. Usuário: Administrador do Sistema
📋 Permissões necessárias: profiles:read
✅ Autorizado! Usuário tem permissão
```

**Se falhar (❌):**
```
❌ Claim 'role_id' não encontrada no token
Claims disponíveis: nameid=..., email=...
```

ou

```
📋 Permissões necessárias: profiles:read
❌ Negado! Usuário não tem nenhuma das permissões necessárias
Permissões do usuário: banners:create, banners:read, ...
```

### Passo 3: Verificar Banco de Dados

```sql
-- Ver permissões da role Admin
SELECT 
    r."Name" as Role,
    p."Name" as Permission
FROM "Roles" r
INNER JOIN "RolePermissions" rp ON r."Id" = rp."RoleId"
INNER JOIN "Permissions" p ON rp."PermissionId" = p."Id"
WHERE r."Name" = 'Admin'
ORDER BY p."Name";
```

**Admin deve ter 26 permissões**, incluindo:
- profiles:create
- profiles:read ✅ **Esta é necessária**
- profiles:update
- profiles:delete

## 🔧 Possíveis Causas e Soluções

### Causa 1: Seed não rodou corretamente

**Sintoma:** Token não tem permissões

**Solução:**
```bash
# Resetar banco e rodar seed novamente
dotnet ef database drop --project EcoTurismo.Infra --startup-project EcoTurismo.Api --force
dotnet run --project EcoTurismo.Api
```

### Causa 2: Token antigo (antes do seed correto)

**Sintoma:** Token não tem `role_id` ou permissões

**Solução:**
```bash
# Fazer login novamente para gerar novo token
POST /api/auth/login
{
  "email": "admin@ecoturismo.com.br",
  "password": "admin123"
}
```

### Causa 3: Endpoint usando método errado

**Sintoma:** 403 mesmo com permissões

**Antes (❌):**
```csharp
Permissions(AuthDomain.Permissions.ProfilesRead);
```

**Depois (✅):**
```csharp
Policies("profiles:read");
```

### Causa 4: Handler não registrado

Verificar no `Program.cs`:
```csharp
builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
```

### Causa 5: Políticas não configuradas

Verificar no `Program.cs`:
```csharp
builder.Services.AddAuthorization(options =>
{
    options.AddPolicies();  // ✅ Deve estar presente
});
```

## 📋 Checklist de Diagnóstico

- [ ] Fazer login e obter token novo
- [ ] Verificar token com `/api/auth/debug-token`
- [ ] Confirmar que `role_id` está no token
- [ ] Confirmar que permissões estão no token (26 para Admin)
- [ ] Verificar que `profiles:read` está nas permissões
- [ ] Ver logs do `PermissionAuthorizationHandler`
- [ ] Verificar banco: Admin tem 26 permissões
- [ ] Confirmar que endpoint usa `Policies()` corretamente
- [ ] Testar novamente

## 🎯 Teste Rápido

```bash
# 1. Login
curl -X POST https://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@ecoturismo.com.br","password":"admin123"}'

# Copiar o token da resposta

# 2. Debug token
curl https://localhost:5001/api/auth/debug-token \
  -H "Authorization: Bearer {token}"

# Verificar se tem role_id e permissões

# 3. Testar endpoint
curl https://localhost:5001/api/usuarios \
  -H "Authorization: Bearer {token}"

# Deve retornar 200 OK com lista de usuários
```

## 🎉 Resultado Esperado

Após aplicar as correções:

**Logs:**
```
🔐 Verificando permissões. Usuário: Administrador do Sistema
📋 Permissões necessárias: profiles:read
✅ Autorizado! Usuário tem permissão
```

**Response:**
```json
HTTP/1.1 200 OK
[
  {
    "id": "guid",
    "nome": "Administrador do Sistema",
    "email": "admin@ecoturismo.com.br",
    "role": "Admin",
    "ativo": true
  }
]
```

## 📚 Arquivos Modificados

1. ✅ `DebugTokenEndpoint.cs` - Endpoint de diagnóstico
2. ✅ `PermissionAuthorizationHandler.cs` - Logs detalhados
3. ✅ `ListUsuariosEndpoint.cs` - Corrigido para usar `Policies()`

## ⚠️ IMPORTANTE

Se ainda não funcionar após tudo isso:

1. **Parar a aplicação completamente**
2. **Deletar o banco**: `dotnet ef database drop --force`
3. **Executar aplicação**: `dotnet run`
4. **Fazer novo login**: Gerar token fresco
5. **Testar debug-token**: Verificar se tudo está lá
6. **Testar endpoint**: Deve funcionar!

Se mesmo assim não funcionar, execute o endpoint de debug e me mostre a saída!
