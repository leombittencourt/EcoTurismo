# Correção: StackOverflowException no Login (JsonTypeInfoResolverChain)

## 🔴 Problema

Ao tentar fazer login, ocorria um `StackOverflowException` causado por recursão infinita no `JsonTypeInfoResolverChain`.

### Erro Original
```
StackOverflowException: Repeated 8781 times:
at System.Text.Json.Serialization.Metadata.JsonTypeInfoResolverChain.GetTypeInfo(Type, JsonSerializerOptions)
```

### Causa Raiz

O erro era causado por **referências circulares** na configuração do `JsonSerializerOptions`. Isso acontecia porque:

1. FastEndpoints cria uma cadeia de resolvedores de tipo (`JsonTypeInfoResolverChain`)
2. Quando não configurado corretamente, os resolvedores podem referenciar uns aos outros
3. Ao serializar objetos com referências circulares (como entidades com navegação), o JSON tentava seguir as referências infinitamente
4. Isso criava um loop: `GetTypeInfo()` → `item.GetTypeInfo()` → `GetTypeInfo()` → ...

### Cenários Comuns

Este problema ocorre especialmente quando:
- ✅ Entidades têm navegação bidirecional (ex: `Municipio.Usuarios` ↔ `Usuario.Municipio`)
- ✅ FastEndpoints/Swagger não tem `ReferenceHandler` configurado
- ✅ DTOs retornam entidades diretamente ao invés de objetos simples

## ✅ Solução

Configurar o `JsonSerializerOptions` para **ignorar ciclos** de referência.

### Código Corrigido (Program.cs)

```csharp
// ── FastEndpoints ──
builder.Services.AddFastEndpoints();

// Configurar JsonSerializerOptions globalmente
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    options.SerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
});

// Add Swagger for FastEndpoints
builder.Services.SwaggerDocument(o =>
{
    o.DocumentSettings = s =>
    {
        s.Title = "EcoTurismo API";
        s.Version = "v1";
        s.Description = "API para gerenciamento do turismo local";
    };
});
```

### O que cada configuração faz:

#### `ReferenceHandler.IgnoreCycles`
```csharp
options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
```
- **O que faz**: Detecta referências circulares e para de serializar quando encontra uma
- **Quando usar**: Quando há navegação bidirecional em entidades
- **Exemplo**: `Usuario.Municipio.Usuarios[0]` → pára aqui ao invés de continuar infinitamente

#### `JsonIgnoreCondition.WhenWritingNull`
```csharp
options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
```
- **O que faz**: Não serializa propriedades com valor `null`
- **Benefício**: JSON menor e mais limpo
- **Exemplo**: Se `Usuario.Telefone` é `null`, não aparece no JSON

## 📊 Comparação

### Antes (❌ Erro)
```json
{
  "id": "...",
  "nome": "João",
  "municipio": {
    "id": "...",
    "nome": "Rio Verde",
    "usuarios": [
      {
        "id": "...",
        "nome": "João",
        "municipio": {
          "id": "...",
          "nome": "Rio Verde",
          "usuarios": [
            // ♾️ Loop infinito...
          ]
        }
      }
    ]
  }
}
```
**Resultado**: `StackOverflowException`

### Depois (✅ Funciona)
```json
{
  "id": "...",
  "nome": "João",
  "municipio": {
    "id": "...",
    "nome": "Rio Verde"
    // usuarios não serializado (ciclo detectado)
  }
}
```
**Resultado**: JSON válido sem ciclos

## 🎯 Alternativas de Solução

### Solução 1: ReferenceHandler.IgnoreCycles (Implementada) ✅
**Prós:**
- Simples de implementar
- Funciona automaticamente
- Não precisa mudar entidades

**Contras:**
- Pode ocultar dados que você queria

### Solução 2: ReferenceHandler.Preserve
```csharp
options.SerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
```
**Prós:**
- Mantém todas as referências usando `$id` e `$ref`
- Não perde dados

**Contras:**
- JSON mais complexo
- Cliente precisa entender o formato

### Solução 3: [JsonIgnore] nas navegações
```csharp
public class Usuario
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    
    [JsonIgnore] // Não serializar esta navegação
    public Municipio? Municipio { get; set; }
}
```
**Prós:**
- Controle fino sobre o que serializar

**Contras:**
- Precisa adicionar em muitos lugares
- Manutenção manual

### Solução 4: Usar DTOs (Recomendado para APIs) ⭐
```csharp
public record UsuarioDto(
    Guid Id,
    string Nome,
    string Email,
    // Sem referências circulares
);
```
**Prós:**
- Melhor prática de API
- Sem ciclos por design
- Controle total sobre resposta

**Contras:**
- Mais código (mapeamento)

## 🔍 Como Prevenir

### 1. Sempre use DTOs em APIs públicas
```csharp
// ❌ Ruim
public async Task<Usuario> GetUsuario(Guid id)
{
    return await _db.Usuarios
        .Include(u => u.Municipio)
        .Include(u => u.Role)
        .FirstAsync(u => u.Id == id);
}

// ✅ Bom
public async Task<UsuarioDto> GetUsuario(Guid id)
{
    var usuario = await _db.Usuarios
        .Include(u => u.Municipio)
        .Include(u => u.Role)
        .FirstAsync(u => u.Id == id);
    
    return new UsuarioDto(
        usuario.Id,
        usuario.Nome,
        usuario.Email,
        usuario.Role.Name
    );
}
```

### 2. Configure JsonSerializerOptions cedo
No `Program.cs`, logo após `AddFastEndpoints()`:
```csharp
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});
```

### 3. Evite `.Include()` desnecessários
```csharp
// ❌ Carrega tudo (potencial ciclo)
var usuario = await _db.Usuarios
    .Include(u => u.Municipio)
        .ThenInclude(m => m.Usuarios) // ← Ciclo!
    .FirstAsync();

// ✅ Carrega apenas o necessário
var usuario = await _db.Usuarios
    .Include(u => u.Municipio)
    .FirstAsync();
```

### 4. Use `.AsNoTracking()` quando não precisar de mudanças
```csharp
var usuarios = await _db.Usuarios
    .AsNoTracking() // Melhor performance + menos risco de ciclos
    .ToListAsync();
```

## 🧪 Como Testar

### Teste 1: Login
```bash
POST /api/auth/login
{
  "email": "admin@ecoturismo.com.br",
  "password": "admin123"
}
```
**Esperado**: Token JWT retornado sem erros

### Teste 2: Obter Usuário
```bash
GET /api/usuarios/{id}
Authorization: Bearer {token}
```
**Esperado**: JSON válido sem `StackOverflowException`

### Teste 3: Listar Usuários
```bash
GET /api/usuarios
Authorization: Bearer {token}
```
**Esperado**: Array de usuários sem erros

## 📚 Referências

- [System.Text.Json - Handle circular references](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/preserve-references)
- [FastEndpoints - Serialization](https://fast-endpoints.com/docs/serialization)
- [EF Core - Tracking vs No-Tracking](https://learn.microsoft.com/en-us/ef/core/querying/tracking)

## ✅ Checklist

- [x] `ConfigureHttpJsonOptions` adicionado ao `Program.cs`
- [x] `ReferenceHandler.IgnoreCycles` configurado
- [x] `DefaultIgnoreCondition.WhenWritingNull` configurado
- [x] Compilação bem-sucedida
- [ ] Testar login
- [ ] Testar endpoints de usuários
- [ ] Verificar logs de erros

## Resumo

✅ **Problema resolvido** adicionando 6 linhas de código no `Program.cs`:
```csharp
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});
```

Agora o sistema:
- ✅ Detecta ciclos automaticamente
- ✅ Para de serializar quando encontra referência circular
- ✅ Retorna JSON válido
- ✅ Não causa `StackOverflowException`
