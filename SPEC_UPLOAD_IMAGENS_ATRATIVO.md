# Especificação: Upload de Imagens para Atrativos

## 📋 Objetivo

Implementar sistema de upload de imagens para a entidade `Atrativo`, permitindo que cada atrativo tenha múltiplas imagens armazenadas em base64.

---

## 🗄️ Alterações no Banco de Dados

### Entidade Atrativo (Atualizar)

**Arquivo:** `EcoTurismo.Domain/Entities/Atrativo.cs`

Adicionar propriedade:
```csharp
public string? Imagens { get; set; }
```

**Formato esperado:** JSON array de objetos com base64
```json
[
  {
    "id": "uuid",
    "url": "data:image/jpeg;base64,/9j/4AAQ...",
    "ordem": 1,
    "principal": true,
    "descricao": "Foto principal"
  },
  {
    "id": "uuid",
    "url": "data:image/png;base64,iVBORw0KGg...",
    "ordem": 2,
    "principal": false,
    "descricao": "Vista lateral"
  }
]
```

### Configuration (Atualizar)

**Arquivo:** `EcoTurismo.Infra/Configurations/AtrativoConfiguration.cs`

Adicionar:
```csharp
builder.Property(a => a.Imagens)
    .HasColumnName("Imagens")
    .HasComment("Array JSON de imagens em base64")
    .HasColumnType("text");
```

### Migration

Criar migration `AddImagensToAtrativo`:
- ALTER TABLE "Atrativos" ADD "Imagens" text;

---

## 📡 Endpoints a Implementar

### 1. POST /api/uploads/atrativos/{atrativoId}/imagens

**Descrição:** Upload de uma ou múltiplas imagens para um atrativo

**Autorização:** Admin ou Prefeitura

**Request (multipart/form-data):**
```
AtrativoId: guid (na rota)
Imagens: IFormFile[] (array de arquivos, obrigatório)
Descricoes: string[] (array de descrições, opcional)
Ordem: int[] (array de ordens, opcional)
Principal: string (guid da imagem principal, opcional)
```

**Validações:**
- Cada imagem:
  - Formatos aceitos: `.jpg`, `.jpeg`, `.png`, `.gif`, `.webp`
  - Tamanho máximo: 5MB por imagem
  - Mínimo: 1 imagem
  - Máximo: 10 imagens por upload
- Atrativo deve existir
- Total de imagens no atrativo não pode ultrapassar 20

**Lógica:**
1. Validar atrativo existe
2. Converter cada IFormFile para base64
3. Gerar UUID para cada imagem
4. Se array Imagens já existe, fazer parse JSON
5. Adicionar novas imagens ao array
6. Se Principal informado, marcar apenas essa como principal
7. Salvar JSON atualizado no campo Imagens
8. Retornar lista de imagens adicionadas

**Response 200 OK:**
```json
{
  "success": true,
  "message": "3 imagens adicionadas com sucesso",
  "data": {
    "atrativoId": "guid",
    "atrativoNome": "Cachoeira do Sol",
    "totalImagens": 5,
    "imagensAdicionadas": [
      {
        "id": "uuid",
        "url": "data:image/jpeg;base64,/9j/4AAQ...",
        "ordem": 3,
        "principal": false,
        "descricao": "Vista frontal"
      },
      {
        "id": "uuid",
        "url": "data:image/png;base64,iVBORw0KGg...",
        "ordem": 4,
        "principal": true,
        "descricao": "Foto principal"
      },
      {
        "id": "uuid",
        "url": "data:image/jpeg;base64,/9j/4BBB...",
        "ordem": 5,
        "principal": false,
        "descricao": null
      }
    ]
  }
}
```

**Response 400 Bad Request:**
```json
{
  "success": false,
  "errorMessage": "Limite de 20 imagens por atrativo excedido",
  "data": null
}
```

**Response 404 Not Found:**
```json
{
  "success": false,
  "errorMessage": "Atrativo não encontrado",
  "data": null
}
```

---

### 2. DELETE /api/uploads/atrativos/{atrativoId}/imagens/{imagemId}

**Descrição:** Remove uma imagem específica do atrativo

**Autorização:** Admin ou Prefeitura

**Request:**
```
AtrativoId: guid (na rota)
ImagemId: guid (na rota)
```

**Lógica:**
1. Validar atrativo existe
2. Parse JSON do campo Imagens
3. Remover imagem com id correspondente
4. Se era a principal e há outras, marcar primeira como principal
5. Salvar JSON atualizado

**Response 200 OK:**
```json
{
  "success": true,
  "message": "Imagem removida com sucesso",
  "data": {
    "atrativoId": "guid",
    "imagemRemovidaId": "uuid",
    "totalImagensRestantes": 4
  }
}
```

**Response 404:**
```json
{
  "success": false,
  "errorMessage": "Imagem não encontrada",
  "data": null
}
```

---

### 3. PUT /api/uploads/atrativos/{atrativoId}/imagens/{imagemId}/principal

**Descrição:** Define uma imagem como principal

**Autorização:** Admin ou Prefeitura

**Request:**
```
AtrativoId: guid (na rota)
ImagemId: guid (na rota)
```

**Lógica:**
1. Parse JSON do campo Imagens
2. Marcar todas como principal: false
3. Marcar imagemId como principal: true
4. Salvar JSON atualizado

**Response 200 OK:**
```json
{
  "success": true,
  "message": "Imagem definida como principal",
  "data": {
    "imagemId": "uuid",
    "atrativoId": "guid"
  }
}
```

---

### 4. PUT /api/uploads/atrativos/{atrativoId}/imagens/reordenar

**Descrição:** Reordena as imagens do atrativo

**Autorização:** Admin ou Prefeitura

**Request Body (JSON):**
```json
{
  "imagens": [
    { "id": "uuid1", "ordem": 1 },
    { "id": "uuid2", "ordem": 2 },
    { "id": "uuid3", "ordem": 3 }
  ]
}
```

**Lógica:**
1. Parse JSON do campo Imagens
2. Atualizar ordem de cada imagem
3. Ordenar array por ordem
4. Salvar JSON atualizado

**Response 200 OK:**
```json
{
  "success": true,
  "message": "Imagens reordenadas com sucesso",
  "data": {
    "atrativoId": "guid",
    "totalImagens": 5
  }
}
```

---

### 5. GET /api/atrativos/{id}/imagens

**Descrição:** Lista todas as imagens de um atrativo (já deve existir no GetAtrativoEndpoint)

**Autorização:** Público

**Response 200 OK:**
```json
{
  "id": "guid",
  "nome": "Cachoeira do Sol",
  "imagens": [
    {
      "id": "uuid",
      "url": "data:image/jpeg;base64,/9j/4AAQ...",
      "ordem": 1,
      "principal": true,
      "descricao": "Foto principal"
    },
    {
      "id": "uuid",
      "url": "data:image/png;base64,iVBORw0KGg...",
      "ordem": 2,
      "principal": false,
      "descricao": "Vista lateral"
    }
  ]
}
```

---

## 📁 Estrutura de Arquivos a Criar

```
EcoTurismo.Api/Endpoints/Uploads/Atrativos/
├── UploadImagensAtrativoRequest.cs
├── UploadImagensAtrativoValidator.cs
├── UploadImagensAtrativoEndpoint.cs
├── DeleteImagemAtrativoRequest.cs
├── DeleteImagemAtrativoEndpoint.cs
├── SetImagemPrincipalRequest.cs
├── SetImagemPrincipalEndpoint.cs
├── ReordenarImagensRequest.cs
├── ReordenarImagensValidator.cs
└── ReordenarImagensEndpoint.cs
```

---

## 🔧 DTOs/Models Necessários

### ImagemAtrativoDto
```csharp
public record ImagemAtrativoDto(
    string Id,           // UUID da imagem
    string Url,          // Base64 data URI
    int Ordem,           // Ordem de exibição
    bool Principal,      // É a imagem principal?
    string? Descricao    // Descrição opcional
);
```

### UploadImagensAtrativoRequest
```csharp
public class UploadImagensAtrativoRequest
{
    public Guid AtrativoId { get; set; }
    public IFormFile[] Imagens { get; set; } = Array.Empty<IFormFile>();
    public string[]? Descricoes { get; set; }
    public int[]? Ordens { get; set; }
    public string? PrincipalId { get; set; }
}
```

### DeleteImagemAtrativoRequest
```csharp
public class DeleteImagemAtrativoRequest
{
    public Guid AtrativoId { get; set; }
    public string ImagemId { get; set; } = string.Empty;
}
```

### SetImagemPrincipalRequest
```csharp
public class SetImagemPrincipalRequest
{
    public Guid AtrativoId { get; set; }
    public string ImagemId { get; set; } = string.Empty;
}
```

### ReordenarImagensRequest
```csharp
public class ReordenarImagensRequest
{
    public Guid AtrativoId { get; set; }
    public List<ImagemOrdemDto> Imagens { get; set; } = new();
}

public record ImagemOrdemDto(string Id, int Ordem);
```

---

## ✅ Validações Detalhadas

### Upload de Imagens
- ✅ Array Imagens não pode ser vazio
- ✅ Cada imagem deve ter formato válido (jpg, jpeg, png, gif, webp)
- ✅ Cada imagem não pode exceder 5MB
- ✅ Máximo 10 imagens por upload
- ✅ Total de imagens no atrativo não pode exceder 20
- ✅ Se Descricoes informado, tamanho deve ser igual a Imagens
- ✅ Se Ordens informado, tamanho deve ser igual a Imagens
- ✅ Atrativo deve existir
- ✅ PrincipalId deve ser UUID válido se informado

### Delete de Imagem
- ✅ ImagemId deve ser UUID válido
- ✅ Atrativo deve existir
- ✅ Imagem deve existir no atrativo

### Definir Principal
- ✅ ImagemId deve ser UUID válido
- ✅ Atrativo deve existir
- ✅ Imagem deve existir no atrativo

### Reordenar
- ✅ Array Imagens não pode ser vazio
- ✅ Todos IDs devem existir no atrativo
- ✅ Ordens devem ser números positivos

---

## 🔄 Lógica de Conversão Base64

```csharp
// Para cada IFormFile:
1. Ler arquivo para MemoryStream
2. Converter para byte[]
3. Converter para Base64 string
4. Adicionar prefixo data URI: $"data:{contentType};base64,{base64}"
```

---

## 📊 Formato JSON do Campo Imagens

**Estrutura:**
```json
[
  {
    "id": "uuid",              // Identificador único da imagem
    "url": "data:image/...",   // Base64 completo com data URI
    "ordem": 1,                // Ordem de exibição (int)
    "principal": true,         // Booleano
    "descricao": "string"      // Opcional (pode ser null)
  }
]
```

**Regras:**
- Apenas uma imagem pode ter `principal: true`
- `ordem` deve ser sequencial e único
- Array vazio `[]` é válido (atrativo sem imagens)
- `null` no campo Imagens é tratado como array vazio

---

## 🎨 Exemplo de Uso Frontend

### Upload de Imagens
```javascript
const formData = new FormData();
formData.append('Imagens', file1);
formData.append('Imagens', file2);
formData.append('Imagens', file3);
formData.append('Descricoes', 'Foto principal');
formData.append('Descricoes', 'Vista lateral');
formData.append('Descricoes', 'Detalhe');

const response = await fetch(`/api/uploads/atrativos/${atrativoId}/imagens`, {
  method: 'POST',
  headers: { 'Authorization': `Bearer ${token}` },
  body: formData
});
```

### Deletar Imagem
```javascript
await fetch(`/api/uploads/atrativos/${atrativoId}/imagens/${imagemId}`, {
  method: 'DELETE',
  headers: { 'Authorization': `Bearer ${token}` }
});
```

### Definir Principal
```javascript
await fetch(`/api/uploads/atrativos/${atrativoId}/imagens/${imagemId}/principal`, {
  method: 'PUT',
  headers: { 'Authorization': `Bearer ${token}` }
});
```

### Reordenar
```javascript
await fetch(`/api/uploads/atrativos/${atrativoId}/imagens/reordenar`, {
  method: 'PUT',
  headers: {
    'Authorization': `Bearer ${token}`,
    'Content-Type': 'application/json'
  },
  body: JSON.stringify({
    imagens: [
      { id: 'uuid1', ordem: 1 },
      { id: 'uuid2', ordem: 2 },
      { id: 'uuid3', ordem: 3 }
    ]
  })
});
```

---

## 🔐 Segurança e Autorização

**Todos os endpoints de upload/modificação:**
- Requerem autenticação (JWT)
- Roles permitidas: `Admin` ou `Prefeitura`
- Policy: `RolePolicies.AdminOrPrefeituraPolicy`

**Endpoint de leitura:**
- Público (AllowAnonymous)

---

## 📋 Tags do Swagger

Todos os endpoints devem usar as tags:
- `"Uploads"`
- `"Atrativos"`

---

## ⚠️ Considerações Importantes

1. **Limite de imagens:** Máximo 20 imagens por atrativo
2. **Tamanho total:** Base64 aumenta ~33%, considere tamanho total no banco
3. **Performance:** Para muitas imagens, considere paginação no GET
4. **Consistência:** Sempre manter pelo menos uma imagem como principal se houver imagens
5. **Transações:** Use transações para garantir consistência ao modificar JSON
6. **Validação JSON:** Sempre validar JSON ao fazer parse do campo Imagens
7. **Ordem automática:** Se ordem não informada, usar próximo número disponível
8. **Principal automático:** Se não há principal e imagens são adicionadas, primeira é principal

---

## 🎯 Regras de Negócio

1. Se atrativo não tem imagens e adiciona pela primeira vez, primeira é principal
2. Se remove a imagem principal, próxima (menor ordem) vira principal
3. Se remove todas as imagens, campo Imagens fica `[]` (não null)
4. Ordens são reindexadas após reordenação (1, 2, 3, 4...)
5. Ao adicionar novas imagens, ordem começa após última ordem existente
6. Descrição é opcional, pode ser null
7. Upload permite múltiplas imagens de uma vez (batch)
8. Cada operação retorna o estado atualizado completo

---

## 📝 Checklist de Implementação

- [ ] Adicionar campo `Imagens` na entidade `Atrativo`
- [ ] Atualizar `AtrativoConfiguration`
- [ ] Criar migration `AddImagensToAtrativo`
- [ ] Criar DTOs (ImagemAtrativoDto, etc)
- [ ] Criar Request classes
- [ ] Criar Validators
- [ ] Implementar UploadImagensAtrativoEndpoint
- [ ] Implementar DeleteImagemAtrativoEndpoint
- [ ] Implementar SetImagemPrincipalEndpoint
- [ ] Implementar ReordenarImagensEndpoint
- [ ] Atualizar GetAtrativoEndpoint para retornar imagens parseadas
- [ ] Testar todos os endpoints
- [ ] Documentar no Swagger
- [ ] Criar testes unitários (opcional)

---

**Este documento deve ser usado como especificação completa para implementar o sistema de upload de imagens para atrativos.**
