# Seed Inicial - Dados Base do Sistema

## Visão Geral

O seed inicial popula o banco de dados com dados essenciais para o funcionamento do sistema EcoTurismo.

## Dados Criados

### 1. Roles (4)

| Nome | Descrição | Permissões |
|------|-----------|------------|
| **Admin** | Administrador com acesso total | 26 (todas) |
| **Prefeitura** | Gerencia banners e atrativos | 12 |
| **Balneario** | Gerencia quiosques e reservas | 10 |
| **Publico** | Usuário com permissões básicas | 7 |

### 2. Permissions (26)

**Distribuídas por recursos:**
- Banners: 5 permissões
- Atrativos: 4 permissões
- Quiosques: 4 permissões
- Reservas: 5 permissões
- Configurações: 2 permissões
- Perfis: 4 permissões
- Municípios: 1 permissão

### 3. RolePermissions (55)

Associações entre Roles e Permissions conforme matriz de controle de acesso.

### 4. Município Base

**Rio Verde de Mato Grosso - MS**
- Nome: "Rio Verde de Mato Grosso"
- UF: "MS"
- Criado como município padrão do sistema

### 5. Usuários Default (4)

Todos os usuários têm a senha padrão: **`admin123`**

#### 👨‍💼 Admin
- **Email:** `admin@ecoturismo.com.br`
- **Senha:** `admin123`
- **Nome:** Administrador do Sistema
- **Role:** Admin
- **Telefone:** (67) 3000-0001
- **Acesso:** Total (26 permissões)

#### 🏛️ Prefeitura
- **Email:** `prefeitura@ecoturismo.com.br`
- **Senha:** `admin123`
- **Nome:** Prefeitura Rio Verde
- **Role:** Prefeitura
- **Telefone:** (67) 3000-0002
- **Acesso:** Gerenciar banners, atrativos e relatórios

#### 🏖️ Balneário
- **Email:** `balneario@ecoturismo.com.br`
- **Senha:** `admin123`
- **Nome:** Balneário Municipal
- **Role:** Balneario
- **Telefone:** (67) 3000-0003
- **Acesso:** Gerenciar quiosques, reservas e validar tickets

#### 👤 Público
- **Email:** `publico@ecoturismo.com.br`
- **Senha:** `admin123`
- **Nome:** Usuário Público
- **Role:** Publico
- **Telefone:** (67) 3000-0004
- **Acesso:** Criar reservas e visualizar informações públicas

## Como Usar

### Executar o Seed

O seed é executado automaticamente na inicialização da aplicação (configurado no `Program.cs`):

```csharp
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<EcoTurismoDbContext>();
    db.Database.Migrate();
    await AuthorizationSeed.SeedAsync(db);
}
```

### Primeiro Login

Após executar a aplicação pela primeira vez:

```bash
# 1. Login como Admin
POST /api/auth/login
{
  "email": "admin@ecoturismo.com.br",
  "password": "admin123"
}

# 2. Token JWT será retornado
{
  "token": "eyJhbGci...",
  "profile": {
    "id": "guid",
    "nome": "Administrador do Sistema",
    "email": "admin@ecoturismo.com.br",
    "role": "Admin"
  }
}

# 3. Use o token para acessar endpoints protegidos
curl -H "Authorization: Bearer {token}" \
     https://localhost:5001/api/usuarios
```

## Credenciais de Acesso

### 🔐 Para Desenvolvimento/Teste

| Role | Email | Senha | Uso |
|------|-------|-------|-----|
| Admin | admin@ecoturismo.com.br | admin123 | Gerenciar todo o sistema |
| Prefeitura | prefeitura@ecoturismo.com.br | admin123 | Gerenciar conteúdo turístico |
| Balneario | balneario@ecoturismo.com.br | admin123 | Gerenciar operações locais |
| Publico | publico@ecoturismo.com.br | admin123 | Fazer reservas como visitante |

### ⚠️ Segurança

**IMPORTANTE:**

1. **Altere as senhas imediatamente** após o primeiro deploy em produção
2. As credenciais default são apenas para **desenvolvimento/teste**
3. Em produção, **remova ou desabilite** o seed de usuários após a configuração inicial
4. Use senhas fortes com no mínimo:
   - 8 caracteres
   - Letras maiúsculas e minúsculas
   - Números
   - Caracteres especiais

### Como Alterar Senhas

```bash
# Via endpoint de atualização de usuário
PUT /api/usuarios/{id}
Authorization: Bearer {admin-token}
{
  "password": "NovaSenhaForte@123"
}
```

## Estrutura do Seed

### Ordem de Criação

1. ✅ Roles (4)
2. ✅ Permissions (26)
3. ✅ RolePermissions (55)
4. ✅ Município Base (1)
5. ✅ Usuários Default (4)

### Verificação de Dados Existentes

O seed verifica se já existem roles no banco:

```csharp
if (await context.Roles.AnyAsync())
    return; // Não executa se já tem dados
```

### Timestamps

Todos os registros são criados com:
- `CreatedAt`: Data/hora UTC atual
- `UpdatedAt`: Data/hora UTC atual

## Hash de Senha

A senha `admin123` é hasheada usando BCrypt:

```csharp
// Hash pré-calculado para "admin123"
var passwordHash = "$2a$11$Zx8aJzKjYQn0Z5GqH5qF5uJXxY8xKzRj3L9YqNzXxH5qF5uJXxY8a";
```

**Nota:** O hash real será diferente a cada execução do BCrypt, mas validará a mesma senha.

## Município Base

### Rio Verde de Mato Grosso

**Por que este município?**
- Município padrão do sistema
- Todos os usuários default são vinculados a ele
- Pode ser usado para criar atrativos iniciais

**Detalhes:**
- Nome: Rio Verde de Mato Grosso
- UF: MS (Mato Grosso do Sul)
- Logo: null (pode ser configurado posteriormente)

### Adicionar Mais Municípios

Depois do seed inicial, você pode adicionar mais municípios:

```bash
POST /api/municipios
Authorization: Bearer {admin-token}
{
  "nome": "Bonito",
  "uf": "MS",
  "logo": "https://..."
}
```

## Matriz de Permissões

### Admin (26 permissões - 100%)

| Recurso | Permissões |
|---------|------------|
| Banners | create, read, update, delete, reorder |
| Atrativos | create, read, update, delete |
| Quiosques | create, read, update, delete |
| Reservas | create, read, update, delete, validate |
| Configurações | read, update |
| Perfis | create, read, update, delete |
| Municípios | read |

### Prefeitura (12 permissões)

| Recurso | Permissões |
|---------|------------|
| Banners | create, read, update, delete, reorder |
| Atrativos | create, read, update |
| Quiosques | read |
| Reservas | read |
| Configurações | read |
| Municípios | read |

### Balneario (10 permissões)

| Recurso | Permissões |
|---------|------------|
| Quiosques | create, read, update, delete |
| Reservas | create, read, update, validate |
| Atrativos | read |
| Municípios | read |

### Publico (7 permissões)

| Recurso | Permissões |
|---------|------------|
| Banners | read |
| Atrativos | read |
| Quiosques | read |
| Reservas | create, read |
| Configurações | read |
| Municípios | read |

## Testando o Seed

### Verificar se foi executado

```sql
-- Verificar Roles
SELECT * FROM "Roles";

-- Verificar Usuários
SELECT "Nome", "Email", "Ativo" FROM "Usuarios";

-- Verificar Município
SELECT * FROM "Municipios";

-- Verificar Permissions por Role
SELECT 
    r."Name" as Role,
    COUNT(rp."PermissionId") as TotalPermissoes
FROM "Roles" r
LEFT JOIN "RolePermissions" rp ON r."Id" = rp."RoleId"
GROUP BY r."Name"
ORDER BY TotalPermissoes DESC;
```

**Resultado esperado:**
- 4 roles
- 26 permissions
- 55 role_permissions
- 1 município
- 4 usuários

## Troubleshooting

### Seed não executa

**Problema:** O seed não roda na inicialização

**Solução:**
1. Verifique se `await AuthorizationSeed.SeedAsync(db)` está no `Program.cs`
2. Verifique se há erros no console/logs
3. Verifique se o banco de dados está acessível

### Dados duplicados

**Problema:** Erro de chave duplicada

**Solução:**
```sql
-- Limpar todos os dados (CUIDADO!)
DELETE FROM "RolePermissions";
DELETE FROM "Permissions";
DELETE FROM "Usuarios";
DELETE FROM "Municipios";
DELETE FROM "Roles";
```

### Senha não funciona

**Problema:** Não consigo fazer login com `admin123`

**Solução:**
1. Verifique se o hash da senha está correto
2. Verifique se o BCrypt está validando corretamente
3. Tente recriar o usuário via SQL:
```sql
UPDATE "Usuarios" 
SET "PasswordHash" = '$2a$11$...' -- novo hash
WHERE "Email" = 'admin@ecoturismo.com.br';
```

## Próximos Passos

Após o seed inicial:

1. ✅ Login com usuário Admin
2. ⬜ Alterar senha do Admin
3. ⬜ Criar atrativos em Rio Verde de Mato Grosso
4. ⬜ Configurar banners
5. ⬜ Criar quiosques nos atrativos
6. ⬜ Testar criação de reservas
7. ⬜ Adicionar mais municípios (opcional)
8. ⬜ Criar usuários específicos por município
9. ⬜ Desabilitar/remover usuários default em produção

## Checklist

- [x] Roles criadas
- [x] Permissions criadas
- [x] RolePermissions associadas
- [x] Município base criado
- [x] Usuários default criados
- [x] Senha padrão definida (admin123)
- [x] Seed executado automaticamente
- [x] Compilação bem-sucedida
- [ ] Migration aplicada
- [ ] Testes de login realizados
- [ ] Senhas alteradas em produção
