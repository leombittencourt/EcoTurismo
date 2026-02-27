# Guia: Autorização Baseada em Roles (Simplificada)

## ✅ Mudança Implementada

Sistema agora valida apenas a **ROLE** do token JWT, sem consultar permissões no banco.

## 🎯 Como Funciona

### Antes (Complexo)
```
Request → Token → role_id → Banco de Dados → Permissões → Autorizado/Negado
```

### Depois (Simples)
```
Request → Token → role claim → Autorizado/Negado
```

## 📋 Roles Disponíveis

| Role | Descrição |
|------|-----------|
| Admin | Administrador do sistema |
| Prefeitura | Gestor da prefeitura |
| Balneario | Gestor do balneário |
| Publico | Usuário público |

## 🔧 Políticas Criadas

### Individuais
```csharp
RolePolicies.AdminPolicy              // Apenas Admin
RolePolicies.PrefeituraPolicy         // Apenas Prefeitura
RolePolicies.BalnearioPolicy          // Apenas Balneário
RolePolicies.PublicoPolicy            // Apenas Público
```

### Compostas
```csharp
RolePolicies.AdminOrPrefeituraPolicy  // Admin OU Prefeitura
RolePolicies.AdminOrBalnearioPolicy   // Admin OU Balneário
RolePolicies.AnyAuthenticatedPolicy   // Qualquer usuário autenticado
```

## 💻 Como Usar nos Endpoints

### Exemplo 1: Apenas Admin
```csharp
public class DeleteUsuarioEndpoint : Endpoint<DeleteUsuarioRequest>
{
    public override void Configure()
    {
        Delete("/api/usuarios/{id}");
        Policies(RolePolicies.AdminPolicy);  // ✅ Apenas Admin
    }
}
```

### Exemplo 2: Admin ou Prefeitura
```csharp
public class ListUsuariosEndpoint : EndpointWithoutRequest<List<UsuarioListItem>>
{
    public override void Configure()
    {
        Get("/api/usuarios");
        Policies(RolePolicies.AdminOrPrefeituraPolicy);  // ✅ Admin OU Prefeitura
    }
}
```

### Exemplo 3: Qualquer usuário autenticado
```csharp
public class MeEndpoint : EndpointWithoutRequest<MeResponse>
{
    public override void Configure()
    {
        Get("/api/profiles/me");
        Policies(RolePolicies.AnyAuthenticatedPolicy);  // ✅ Qualquer role
    }
}
```

### Exemplo 4: Sem autorização (público)
```csharp
public class LoginEndpoint : Endpoint<LoginRequest, LoginResponse>
{
    public override void Configure()
    {
        Post("/api/auth/login");
        AllowAnonymous();  // ✅ Sem autenticação necessária
    }
}
```

## 🧪 Testar

### 1. Fazer Login
```bash
POST /api/auth/login
{
  "email": "admin@ecoturismo.com.br",
  "password": "admin123"
}
```

**Resposta:**
```json
{
  "token": "eyJhbGci...",
  "profile": {
    "role": "Admin"  // ✅ Role está aqui
  }
}
```

### 2. Verificar Token
```bash
GET /api/auth/debug-token
Authorization: Bearer {token}
```

**Resposta deve incluir:**
```json
{
  "roleName": "Admin",
  "claims": [
    { "type": "http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "value": "Admin" }
  ]
}
```

### 3. Testar Endpoint
```bash
GET /api/usuarios
Authorization: Bearer {token}
```

**Logs esperados:**
```
🔐 Verificando role. Usuário: Administrador do Sistema
👤 Role do usuário: Admin
🎯 Roles permitidas: Admin, Prefeitura
✅ Autorizado! Role 'Admin' é permitida
```

**Resposta: 200 OK** ✅

## 📊 Matriz de Autorização

| Endpoint | Admin | Prefeitura | Balneário | Público |
|----------|-------|------------|-----------|---------|
| GET /api/usuarios | ✅ | ✅ | ❌ | ❌ |
| POST /api/usuarios | ✅ | ❌ | ❌ | ❌ |
| GET /api/profiles/me | ✅ | ✅ | ✅ | ✅ |
| POST /api/auth/login | ✅ | ✅ | ✅ | ✅ |

## 🎨 Exemplos de Configuração

### Admin exclusivo
```csharp
Delete("/api/usuarios/{id}");
Policies(RolePolicies.AdminPolicy);
```

### Gestores (Admin ou Prefeitura)
```csharp
Get("/api/relatorios");
Policies(RolePolicies.AdminOrPrefeituraPolicy);
```

### Operadores (Admin ou Balneário)
```csharp
Post("/api/validacoes");
Policies(RolePolicies.AdminOrBalnearioPolicy);
```

### Todos autenticados
```csharp
Get("/api/banners");
Policies(RolePolicies.AnyAuthenticatedPolicy);
```

### Público (sem auth)
```csharp
Get("/api/banners/public");
AllowAnonymous();
```

## ⚠️ IMPORTANTE

### O token DEVE conter a role
O `AuthService` já adiciona a role no token:
```csharp
new Claim(ClaimTypes.Role, roleName)
```

Se o token não tiver a role, o endpoint retornará **403 Forbidden**.

### Verificar com debug-token
Sempre use o endpoint de debug para verificar o token:
```bash
GET /api/auth/debug-token
```

## 🔍 Troubleshooting

### 403 mesmo com token válido

**1. Verificar se role está no token:**
```bash
GET /api/auth/debug-token
```

**2. Ver logs do RoleAuthorizationHandler:**
```
🔐 Verificando role. Usuário: ...
👤 Role do usuário: Admin
🎯 Roles permitidas: Admin, Prefeitura
```

**3. Verificar se a role do token está correta:**
- Token diz: "Admin"
- Role permitida: "Admin"
- ✅ Deve funcionar!

### Token não tem role

**Solução:**
```bash
# Fazer novo login para gerar token com role
POST /api/auth/login
```

## 📚 Arquivos Criados

1. ✅ `RoleAuthorizationHandler.cs` - Handler que valida roles
2. ✅ `RolePolicies.cs` - Políticas baseadas em roles
3. ✅ `Program.cs` - Registrado novo handler
4. ✅ Endpoints atualizados com novas políticas

## 🎉 Vantagens

1. **Simples**: Não consulta banco de dados
2. **Rápido**: Valida apenas o claim do token
3. **Fácil**: Basta verificar a role
4. **Logs**: Debug completo de cada autorização

## 🚀 Próximos Passos

1. Reiniciar aplicação (para carregar novo handler)
2. Fazer novo login
3. Testar endpoint `/api/usuarios`
4. Ver logs no console
5. Deve funcionar! ✅

## ✅ Checklist

- [x] RoleAuthorizationHandler criado
- [x] RolePolicies configuradas
- [x] Handler registrado no Program.cs
- [x] Endpoints atualizados
- [x] Compilação bem-sucedida
- [ ] Aplicação reiniciada
- [ ] Novo login realizado
- [ ] Endpoint testado
- [ ] Logs verificados

**Agora os endpoints validam APENAS a role do token, sem consultar o banco!** 🎉
