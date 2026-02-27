# CRUD de Usuários - EcoTurismo API

## Resumo

Sistema completo de CRUD (Create, Read, Update, Delete) para gerenciamento de usuários, seguindo o padrão do projeto com RBAC.

## Estrutura Criada

### 📁 Domain Layer
✅ **Usuario.cs** - Entidade de domínio
```csharp
public class Usuario
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public Guid RoleId { get; set; }
    public Guid? MunicipioId { get; set; }
    public Guid? AtrativoId { get; set; }
    public string? Telefone { get; set; }
    public string? Cpf { get; set; }
    public bool Ativo { get; set; }
    // ...
}
```

### 📁 Infrastructure Layer
✅ **UsuarioConfiguration.cs** - Configuração do EF Core
- Tabela: `Usuarios`
- Índices: Email (unique), CPF (unique), RoleId
- Relacionamentos: Role, Municipio, Atrativo

### 📁 Application Layer

✅ **UsuarioDtos.cs** - DTOs
- `UsuarioDto` - Resposta completa
- `UsuarioListItem` - Listagem resumida
- `UsuarioCreateRequest` - Criação
- `UsuarioUpdateRequest` - Atualização

✅ **IUsuarioService.cs** - Interface do serviço
```csharp
public interface IUsuarioService
{
    Task<List<UsuarioListItem>> ListarAsync();
    Task<UsuarioDto?> ObterPorIdAsync(Guid id);
    Task<UsuarioDto> CriarAsync(UsuarioCreateRequest request);
    Task<UsuarioDto?> AtualizarAsync(Guid id, UsuarioUpdateRequest request);
    Task<bool> ExcluirAsync(Guid id);
}
```

✅ **UsuarioService.cs** - Implementação do serviço
- Hash de senha com BCrypt
- Validação de email duplicado
- Validação de role ativa
- Atualização automática de timestamps

### 📁 API Layer

✅ **CreateUsuarioEndpoint.cs** + Validator
- POST `/api/usuarios`
- Permissão: `profiles:create`
- Validações: Nome, Email, Password obrigatórios

✅ **ListUsuariosEndpoint.cs**
- GET `/api/usuarios`
- Permissão: `profiles:read`
- Retorna apenas usuários ativos

✅ **GetUsuarioEndpoint.cs**
- GET `/api/usuarios/{id}`
- Permissão: `profiles:read`

✅ **UpdateUsuarioEndpoint.cs** + Validator
- PUT `/api/usuarios/{id}`
- Permissão: `profiles:update`
- Atualização parcial (campos opcionais)
- Validação de email duplicado

✅ **DeleteUsuarioEndpoint.cs**
- DELETE `/api/usuarios/{id}`
- Permissão: `profiles:delete`
- Exclusão física (não soft delete)

## Endpoints Disponíveis

### 1. Criar Usuário
```http
POST /api/usuarios
Authorization: Bearer {token}
Content-Type: application/json

{
  "nome": "João Silva",
  "email": "joao@example.com",
  "password": "senha123",
  "roleId": "guid-da-role",
  "municipioId": "guid-municipio",
  "atrativoId": "guid-atrativo",
  "telefone": "(11) 98765-4321",
  "cpf": "123.456.789-00"
}
```

**Resposta 201 Created:**
```json
{
  "id": "guid",
  "nome": "João Silva",
  "email": "joao@example.com",
  "roleName": "Balneario",
  "roleId": "guid",
  "municipioId": "guid",
  "atrativoId": "guid",
  "telefone": "(11) 98765-4321",
  "cpf": "123.456.789-00",
  "ativo": true
}
```

### 2. Listar Usuários
```http
GET /api/usuarios
Authorization: Bearer {token}
```

**Resposta 200 OK:**
```json
[
  {
    "id": "guid",
    "nome": "João Silva",
    "email": "joao@example.com",
    "roleName": "Balneario",
    "ativo": true
  }
]
```

### 3. Obter Usuário por ID
```http
GET /api/usuarios/{id}
Authorization: Bearer {token}
```

**Resposta 200 OK:** (mesma estrutura do Create)

### 4. Atualizar Usuário
```http
PUT /api/usuarios/{id}
Authorization: Bearer {token}
Content-Type: application/json

{
  "nome": "João Silva Atualizado",
  "email": "joao.novo@example.com",
  "password": "novaSenha123",
  "roleId": "novo-guid-role",
  "telefone": "(11) 99999-9999",
  "cpf": "111.222.333-44",
  "ativo": true
}
```

**Observações:**
- Todos os campos são opcionais
- Se `password` for fornecido, será rehashed
- Email é validado para unicidade

### 5. Excluir Usuário
```http
DELETE /api/usuarios/{id}
Authorization: Bearer {token}
```

**Resposta 204 No Content** (sucesso)
**Resposta 404 Not Found** (usuário não existe)

## Permissões Necessárias

| Operação | Permissão | Roles com Acesso |
|----------|-----------|------------------|
| Create | `profiles:create` | Admin |
| Read | `profiles:read` | Admin |
| Update | `profiles:update` | Admin |
| Delete | `profiles:delete` | Admin |

## Validações

### Create (Obrigatórios)
- ✅ Nome (max 200 chars)
- ✅ Email (válido, max 200 chars, único)
- ✅ Password (min 6 chars)
- ✅ RoleId (role deve existir e estar ativa)

### Create (Opcionais)
- MunicipioId
- AtrativoId
- Telefone (max 20 chars)
- CPF (max 14 chars, único)

### Update (Todos opcionais)
- Nome, Email, Password, RoleId, etc.
- Email validado para unicidade se alterado
- Role validada se alterada

## Features Implementadas

1. **Hash de Senha**: BCrypt para segurança
2. **Validação de Email**: Garantia de unicidade
3. **Validação de CPF**: Unicidade (se fornecido)
4. **Validação de Role**: Verifica se existe e está ativa
5. **Soft State**: Campo `Ativo` para desativar sem excluir
6. **Timestamps**: `CreatedAt` e `UpdatedAt` automáticos
7. **Relacionamentos**: Role, Municipio, Atrativo
8. **Validators**: FluentValidation nos endpoints

## Diferenças entre Usuario e Profile

| Característica | Usuario | Profile |
|----------------|---------|---------|
| **Propósito** | Usuários do sistema | Perfis herdados (legacy) |
| **Campos extras** | Telefone, CPF, Ativo | - |
| **Uso** | CRUD completo | Apenas /me endpoint |
| **Recomendação** | Usar para novos usuários | Migrar para Usuario |

## Testes

### Testar Create
```bash
# 1. Login como Admin
POST /api/auth/login
{
  "email": "admin@example.com",
  "password": "senha"
}

# 2. Usar token recebido
TOKEN="seu_token_aqui"

# 3. Buscar ID de uma role (ex: Publico)
GET /api/roles

# 4. Criar usuário
POST /api/usuarios
Authorization: Bearer $TOKEN
{
  "nome": "Teste Usuario",
  "email": "teste@example.com",
  "password": "senha123",
  "roleId": "guid-role-publico"
}
```

### Testar Update
```bash
PUT /api/usuarios/{id}
Authorization: Bearer $TOKEN
{
  "nome": "Nome Atualizado",
  "ativo": false
}
```

### Testar Delete
```bash
DELETE /api/usuarios/{id}
Authorization: Bearer $TOKEN
```

## Próximos Passos

1. ✅ Entidade criada
2. ✅ Configuração EF Core
3. ✅ DTOs criados
4. ✅ Service implementado
5. ✅ Endpoints criados (CRUD completo)
6. ✅ Validators implementados
7. ✅ Serviço registrado no DI
8. ✅ Compilação bem-sucedida
9. ⬜ Criar migration:
   ```bash
   dotnet ef migrations add AdicionarEntidadeUsuario --project EcoTurismo.Infra --startup-project EcoTurismo.Api
   ```
10. ⬜ Aplicar migration:
    ```bash
    dotnet ef database update --project EcoTurismo.Infra --startup-project EcoTurismo.Api
    ```
11. ⬜ Testar endpoints com Swagger
12. ⬜ (Opcional) Criar endpoint de busca/filtros
13. ⬜ (Opcional) Migrar dados de Profile para Usuario

## Arquivos Criados

**Domain:**
- `Domain/Entities/Usuario.cs`

**Infrastructure:**
- `Infra/Configurations/UsuarioConfiguration.cs`

**Application:**
- `Application/DTOs/UsuarioDtos.cs`
- `Application/Interfaces/IUsuarioService.cs`
- `Application/Services/UsuarioService.cs`

**API:**
- `Api/Endpoints/Usuarios/Create/CreateUsuarioEndpoint.cs`
- `Api/Endpoints/Usuarios/Create/CreateUsuarioValidator.cs`
- `Api/Endpoints/Usuarios/List/ListUsuariosEndpoint.cs`
- `Api/Endpoints/Usuarios/Get/GetUsuarioEndpoint.cs`
- `Api/Endpoints/Usuarios/Update/UpdateUsuarioEndpoint.cs`
- `Api/Endpoints/Usuarios/Update/UpdateUsuarioValidator.cs`
- `Api/Endpoints/Usuarios/Delete/DeleteUsuarioEndpoint.cs`

**Total: 11 arquivos criados + 2 atualizados**

## Checklist

- [x] Entidade Usuario criada
- [x] Configuração EF Core
- [x] DTOs definidos
- [x] Interface IUsuarioService
- [x] Service UsuarioService
- [x] Endpoint Create + Validator
- [x] Endpoint List
- [x] Endpoint Get
- [x] Endpoint Update + Validator
- [x] Endpoint Delete
- [x] Serviço registrado no DI
- [x] DbContext atualizado
- [x] Compilação bem-sucedida
- [ ] Migration criada
- [ ] Migration aplicada
- [ ] Testes realizados
