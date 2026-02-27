# Script de Teste do Login - EcoTurismo API

## 🧪 Passo a Passo para Testar

### 1. Verificar se há usuários no banco
```bash
GET https://localhost:5001/api/auth/test-seed
```

**Resposta esperada:**
```json
{
  "total": 4,
  "usuarios": [
    {
      "id": "...",
      "nome": "Administrador do Sistema",
      "email": "admin@ecoturismo.com.br",
      "role": "Admin",
      "ativo": true,
      "passwordHashPreview": "$2a$11$Zx8aJzKjYQn0..."
    }
  ],
  "mensagem": "✅ Usuários encontrados!"
}
```

Se retornar 0 usuários, execute o seed:
```bash
# Parar aplicação
# Limpar tabelas
# Executar aplicação novamente (seed roda automaticamente)
```

### 2. Testar Login - Admin
```bash
POST https://localhost:5001/api/auth/login
Content-Type: application/json

{
  "email": "admin@ecoturismo.com.br",
  "password": "admin123"
}
```

**Resposta esperada (200 OK):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "profile": {
    "id": "guid",
    "nome": "Administrador do Sistema",
    "email": "admin@ecoturismo.com.br",
    "role": "Admin",
    "municipioId": "guid",
    "atrativoId": null
  }
}
```

### 3. Testar Login - Prefeitura
```bash
POST https://localhost:5001/api/auth/login
Content-Type: application/json

{
  "email": "prefeitura@ecoturismo.com.br",
  "password": "admin123"
}
```

### 4. Testar Login - Balneário
```bash
POST https://localhost:5001/api/auth/login
Content-Type: application/json

{
  "email": "balneario@ecoturismo.com.br",
  "password": "admin123"
}
```

### 5. Testar Login - Público
```bash
POST https://localhost:5001/api/auth/login
Content-Type: application/json

{
  "email": "publico@ecoturismo.com.br",
  "password": "admin123"
}
```

### 6. Testar Login Inválido
```bash
POST https://localhost:5001/api/auth/login
Content-Type: application/json

{
  "email": "naoexiste@example.com",
  "password": "senhaerrada"
}
```

**Resposta esperada (401 Unauthorized):**
```
Unauthorized
```

## 🔧 Troubleshooting

### Problema: "Unauthorized" mesmo com credenciais corretas

**Verificar logs:**
Os logs agora mostram detalhes do login:
```
❌ Login falhou: Usuário não encontrado com email 'admin@ecoturismo.com.br'
❌ Login falhou: Senha incorreta para usuário 'admin@ecoturismo.com.br'
❌ Login falhou: Usuário 'admin@ecoturismo.com.br' está inativo
✅ Login bem-sucedido: admin@ecoturismo.com.br (Admin)
```

**Soluções:**

1. **Usuário não existe**
   ```sql
   SELECT * FROM "Usuarios" WHERE "Email" = 'admin@ecoturismo.com.br';
   ```
   Se vazio, execute o seed.

2. **Senha incorreta**
   ```sql
   -- Resetar senha do admin
   UPDATE "Usuarios" 
   SET "PasswordHash" = '$2a$11$Zx8aJzKjYQn0Z5GqH5qF5uJXxY8xKzRj3L9YqNzXxH5qF5uJXxY8a'
   WHERE "Email" = 'admin@ecoturismo.com.br';
   ```

3. **Usuário inativo**
   ```sql
   UPDATE "Usuarios" 
   SET "Ativo" = true
   WHERE "Email" = 'admin@ecoturismo.com.br';
   ```

### Problema: "StackOverflowException"

**Solução:** Já corrigido no `Program.cs` com:
```csharp
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});
```

### Problema: Token gerado mas não funciona nos endpoints

**Verificar:**
1. JWT configurado corretamente no `appsettings.json`
2. Mesma chave JWT em todos os ambientes
3. Endpoint protegido tem `[Authorize]` ou `Permissions()`

## 🧪 Teste com cURL

### Linux/Mac/Git Bash
```bash
# Testar seed
curl https://localhost:5001/api/auth/test-seed

# Login admin
curl -X POST https://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@ecoturismo.com.br","password":"admin123"}'

# Usar token
TOKEN="seu_token_aqui"
curl -H "Authorization: Bearer $TOKEN" \
  https://localhost:5001/api/usuarios
```

### PowerShell
```powershell
# Testar seed
Invoke-RestMethod -Uri https://localhost:5001/api/auth/test-seed

# Login admin
$body = @{
    email = "admin@ecoturismo.com.br"
    password = "admin123"
} | ConvertTo-Json

$response = Invoke-RestMethod -Uri https://localhost:5001/api/auth/login `
  -Method Post `
  -ContentType "application/json" `
  -Body $body

$token = $response.token

# Usar token
Invoke-RestMethod -Uri https://localhost:5001/api/usuarios `
  -Headers @{ Authorization = "Bearer $token" }
```

## ✅ Checklist de Teste

- [ ] Aplicação rodando sem erros
- [ ] `/api/auth/test-seed` retorna 4 usuários
- [ ] Login admin funciona (200 OK)
- [ ] Login prefeitura funciona
- [ ] Login balneário funciona
- [ ] Login público funciona
- [ ] Login inválido retorna 401
- [ ] Token pode ser usado em endpoints protegidos
- [ ] Logs aparecem no console

## 📋 Credenciais Default

| Role | Email | Senha |
|------|-------|-------|
| Admin | admin@ecoturismo.com.br | admin123 |
| Prefeitura | prefeitura@ecoturismo.com.br | admin123 |
| Balneário | balneario@ecoturismo.com.br | admin123 |
| Público | publico@ecoturismo.com.br | admin123 |

## 🔍 Verificar Token JWT

Use https://jwt.io para decodificar o token e verificar os claims:

```json
{
  "nameid": "user-guid",
  "email": "admin@ecoturismo.com.br",
  "name": "Administrador do Sistema",
  "role": "Admin",
  "role_id": "role-guid",
  "role_name": "Admin",
  "permission": [
    "banners:create",
    "banners:read",
    "..."
  ],
  "municipio_id": "municipio-guid",
  "exp": 1234567890,
  "iss": "EcoTurismoApi",
  "aud": "EcoTurismoApp"
}
```
