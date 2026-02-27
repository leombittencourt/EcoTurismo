# Configuração JWT - EcoTurismo API

## Visão Geral

O sistema utiliza JWT (JSON Web Token) para autenticação e autorização de usuários.

## Configurações no appsettings.json

### Estrutura

```json
{
  "Jwt": {
    "Key": "ChaveSecretaComPeloMenos32Caracteres",
    "Issuer": "EcoTurismoApi",
    "Audience": "EcoTurismoApp"
  }
}
```

### Propriedades

#### Key (Obrigatório)
- **Descrição**: Chave secreta usada para assinar e validar os tokens JWT
- **Requisitos**: 
  - Mínimo de 32 caracteres
  - Deve ser única e aleatória
  - Nunca deve ser commitada no Git em produção
- **Exemplo**: `"SuaChaveSecretaSuperSeguraComPeloMenos32Caracteres!"`

#### Issuer (Obrigatório)
- **Descrição**: Identifica quem emitiu o token (emissor)
- **Valor padrão**: `"EcoTurismoApi"`
- **Uso**: Validação de origem do token

#### Audience (Obrigatório)
- **Descrição**: Identifica para quem o token foi criado (audiência)
- **Valor padrão**: `"EcoTurismoApp"`
- **Uso**: Validação de destino do token

## Configuração por Ambiente

### Development (appsettings.Development.json)

```json
{
  "Jwt": {
    "Key": "ChaveDeDesenvolvimentoComPeloMenos32CaracteresParaSerSegura!",
    "Issuer": "EcoTurismoApi",
    "Audience": "EcoTurismoApp"
  }
}
```

### Production

⚠️ **IMPORTANTE**: Em produção, **NUNCA** coloque a chave diretamente no `appsettings.json`!

Use uma das seguintes opções:

#### Opção 1: Variáveis de Ambiente
```bash
export Jwt__Key="SuaChaveProdução"
export Jwt__Issuer="EcoTurismoApi"
export Jwt__Audience="EcoTurismoApp"
```

#### Opção 2: User Secrets (Desenvolvimento)
```bash
dotnet user-secrets set "Jwt:Key" "SuaChaveAqui"
dotnet user-secrets set "Jwt:Issuer" "EcoTurismoApi"
dotnet user-secrets set "Jwt:Audience" "EcoTurismoApp"
```

#### Opção 3: Azure Key Vault (Produção)
```csharp
builder.Configuration.AddAzureKeyVault(
    new Uri($"https://{keyVaultName}.vault.azure.net/"),
    new DefaultAzureCredential());
```

## Como Gerar uma Chave Segura

### PowerShell
```powershell
# Gera uma chave aleatória de 64 caracteres
-join ((65..90) + (97..122) + (48..57) + 33,35,36,37,38,42,43,45,61,63,64 | Get-Random -Count 64 | % {[char]$_})
```

### Linux/Mac (bash)
```bash
# Gera uma chave aleatória base64
openssl rand -base64 32
```

### C# (código)
```csharp
using System.Security.Cryptography;

var key = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
Console.WriteLine(key);
```

## Token JWT Gerado

Exemplo de claims no token:

```json
{
  "nameid": "guid-do-usuario",
  "email": "usuario@example.com",
  "name": "Nome do Usuário",
  "role": "Admin",
  "role_id": "guid-da-role",
  "role_name": "Admin",
  "permission": [
    "banners:create",
    "banners:read",
    "..."
  ],
  "municipio_id": "guid-municipio",
  "atrativo_id": "guid-atrativo",
  "exp": 1234567890,
  "iss": "EcoTurismoApi",
  "aud": "EcoTurismoApp"
}
```

## Duração do Token

- **Padrão**: 8 horas
- **Configurado em**: `AuthService.cs`

```csharp
expires: DateTime.UtcNow.AddHours(8)
```

Para alterar, modifique o valor em `EcoTurismo.Application/Services/AuthService.cs`.

## Uso nos Endpoints

### Com FastEndpoints

```csharp
public override void Configure()
{
    Post("/api/banners");
    Permissions(AuthDomain.Permissions.BannersCreate);
}
```

### Obter Informações do Token

```csharp
// No endpoint
var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
var email = User.FindFirst(ClaimTypes.Email)?.Value;
var role = User.FindFirst(ClaimTypes.Role)?.Value;
var roleId = User.FindFirst("role_id")?.Value;
```

## Troubleshooting

### Erro: "IDX10503: Signature validation failed"
- **Causa**: A chave JWT está incorreta ou foi alterada
- **Solução**: Verifique se a chave em `appsettings.json` está correta

### Erro: "IDX10214: Audience validation failed"
- **Causa**: O Audience do token não corresponde ao configurado
- **Solução**: Verifique se `Jwt:Audience` está configurado corretamente

### Erro: "IDX10205: Issuer validation failed"
- **Causa**: O Issuer do token não corresponde ao configurado
- **Solução**: Verifique se `Jwt:Issuer` está configurado corretamente

### Token expira muito rápido
- **Solução**: Aumente o valor de `AddHours()` em `AuthService.cs`

## Segurança

### ✅ Boas Práticas

1. **Chave forte**: Mínimo 32 caracteres, aleatória
2. **Variáveis de ambiente**: Use em produção
3. **HTTPS**: Sempre use HTTPS em produção
4. **Rotação de chaves**: Considere rotacionar a chave periodicamente
5. **Tempo de expiração**: Não use tempos muito longos

### ❌ O que NÃO fazer

1. Commitar chaves no Git
2. Usar chaves fracas ("123456", "password", etc.)
3. Reutilizar chaves entre ambientes
4. Expor a chave em logs
5. Tokens sem expiração

## Exemplo de Requisição

```bash
# Login
curl -X POST https://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@example.com","password":"senha123"}'

# Resposta
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "profile": {
    "id": "...",
    "nome": "Admin",
    "email": "admin@example.com",
    "role": "Admin"
  }
}

# Usar o token
curl -X POST https://localhost:5001/api/banners \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..." \
  -H "Content-Type: application/json" \
  -d '{"titulo":"Novo Banner","imagemUrl":"http://..."}'
```

## Referências

- [JWT.io](https://jwt.io/) - Decodificador de tokens
- [Microsoft Docs - JWT Bearer Authentication](https://docs.microsoft.com/aspnet/core/security/authentication/jwt-authn)
- [OWASP - JWT Security](https://cheatsheetseries.owasp.org/cheatsheets/JSON_Web_Token_for_Java_Cheat_Sheet.html)
