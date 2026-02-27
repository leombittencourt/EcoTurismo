# Script para Resetar Banco e Executar Seed com Hash Correto

## 🔧 Problema Resolvido

O hash da senha no seed estava incorreto (era um placeholder fake). 
Agora o seed gera um hash BCrypt **real** para a senha "admin123".

## ✅ Solução Aplicada

### AuthorizationSeed.cs
```csharp
// Antes (❌ Hash fake)
var passwordHash = "$2a$11$Zx8aJzKjYQn0Z5GqH5qF5uJXxY8xKzRj3L9YqNzXxH5qF5uJXxY8a";

// Depois (✅ Hash real gerado dinamicamente)
var passwordHash = BCrypt.Net.BCrypt.HashPassword("admin123");
```

## 🚀 Como Aplicar a Correção

### Opção 1: Resetar Banco Completo (Recomendado)

```bash
# Parar aplicação se estiver rodando

# Deletar banco de dados
dotnet ef database drop --project EcoTurismo.Infra --startup-project EcoTurismo.Api --force

# Criar banco novamente
dotnet ef database update --project EcoTurismo.Infra --startup-project EcoTurismo.Api

# OU simplesmente executar a aplicação (migrations e seed automáticos)
dotnet run --project EcoTurismo.Api
```

### Opção 2: Atualizar Apenas Senhas (Mais Rápido)

```sql
-- Conectar no PostgreSQL e executar:

-- Gerar novo hash para admin123
-- Use este script PowerShell para gerar o hash:
```

```powershell
# gerar-hash.ps1
Add-Type -Path "caminho\para\BCrypt.Net.dll"
$hash = [BCrypt.Net.BCrypt]::HashPassword("admin123")
Write-Host "Hash gerado: $hash"
```

Depois atualize no banco:

```sql
-- Atualizar senhas de todos os usuários
UPDATE "Usuarios" 
SET "PasswordHash" = '{HASH_GERADO_ACIMA}'
WHERE "Email" IN (
    'admin@ecoturismo.com.br',
    'prefeitura@ecoturismo.com.br',
    'balneario@ecoturismo.com.br',
    'publico@ecoturismo.com.br'
);
```

### Opção 3: Apenas Limpar Dados de Autorização

```sql
-- Conectar no PostgreSQL

-- Limpar dados (mas manter estrutura)
DELETE FROM "RolePermissions";
DELETE FROM "Permissions";
DELETE FROM "Usuarios";
DELETE FROM "Municipios";
DELETE FROM "Roles";

-- Executar aplicação (seed rodará automaticamente)
```

## 🧪 Como Testar

### 1. Verificar se seed rodou
```
Console Output:
🌱 Iniciando seed de dados iniciais...
   🔐 Gerando hash de senha para usuários default...
   ✅ Hash gerado: $2a$11$abcd1234...
   ✅ 4 Usuários default criados (senha: admin123)
```

### 2. Testar login
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
  "token": "eyJhbGci...",
  "profile": {
    "id": "guid",
    "nome": "Administrador do Sistema",
    "email": "admin@ecoturismo.com.br",
    "role": "Admin"
  }
}
```

### 3. Verificar hash no banco
```sql
SELECT 
    "Nome",
    "Email",
    LEFT("PasswordHash", 20) || '...' as "HashPreview"
FROM "Usuarios";
```

**Resultado esperado:**
```
Nome                        Email                           HashPreview
------------------------    -----------------------------   --------------------
Administrador do Sistema    admin@ecoturismo.com.br        $2a$11$abcd1234...
Prefeitura Rio Verde        prefeitura@ecoturismo.com.br   $2a$11$abcd1234...
Balneário Municipal         balneario@ecoturismo.com.br    $2a$11$abcd1234...
Usuário Público             publico@ecoturismo.com.br      $2a$11$abcd1234...
```

## 📋 Credenciais (Após Correção)

| Email | Senha | Role |
|-------|-------|------|
| admin@ecoturismo.com.br | admin123 | Admin |
| prefeitura@ecoturismo.com.br | admin123 | Prefeitura |
| balneario@ecoturismo.com.br | admin123 | Balneário |
| publico@ecoturismo.com.br | admin123 | Público |

## ⚠️ IMPORTANTE

### Por que o hash anterior não funcionava?

O hash no seed era:
```
$2a$11$Zx8aJzKjYQn0Z5GqH5qF5uJXxY8xKzRj3L9YqNzXxH5qF5uJXxY8a
```

Este **NÃO** é um hash válido do BCrypt para "admin123". Era apenas um placeholder de exemplo.

### Como funciona o BCrypt?

BCrypt gera hashes **diferentes** a cada execução, mesmo para a mesma senha:
```csharp
var hash1 = BCrypt.HashPassword("admin123"); // $2a$11$abc...
var hash2 = BCrypt.HashPassword("admin123"); // $2a$11$xyz... (diferente!)

// Mas ambos validam corretamente:
BCrypt.Verify("admin123", hash1); // true
BCrypt.Verify("admin123", hash2); // true
```

Por isso agora o seed gera o hash dinamicamente a cada execução.

## 🔍 Verificar se Hash está Correto

### PowerShell
```powershell
# Testar se hash valida a senha
dotnet run --project EcoTurismo.Tests -- TesteBCrypt
```

### C# Console
```csharp
using BCrypt.Net;

var senha = "admin123";
var hash = BCrypt.HashPassword(senha);
Console.WriteLine($"Hash: {hash}");
Console.WriteLine($"Válido: {BCrypt.Verify(senha, hash)}");
```

## ✅ Checklist de Verificação

- [x] BCrypt.Net-Next adicionado ao projeto Infra
- [x] Seed atualizado para gerar hash dinamicamente
- [x] Compilação bem-sucedida
- [ ] Banco resetado
- [ ] Aplicação executada (seed rodou)
- [ ] Login testado com sucesso
- [ ] Hash verificado no banco

## 🎯 Próximos Passos

1. **Pare a aplicação** (se estiver rodando em debug)
2. **Delete o banco**: `dotnet ef database drop --force`
3. **Execute a aplicação**: `dotnet run --project EcoTurismo.Api`
4. **Aguarde seed**: Veja no console "✅ Hash gerado"
5. **Teste login**: POST /api/auth/login

## 📚 Arquivos Modificados

- ✅ `AuthorizationSeed.cs` - Gera hash dinamicamente
- ✅ `EcoTurismo.Infra.csproj` - Adicionado BCrypt.Net-Next

## 🎉 Resultado Final

**Senha "admin123" agora funcionará corretamente!**

O seed gera um hash BCrypt válido toda vez que roda, garantindo que a senha sempre será validada corretamente pelo AuthService.
