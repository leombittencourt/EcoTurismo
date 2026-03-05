# 🖼️ Sistema de Gerenciamento de Imagens - EcoTurismo

## 📋 Visão Geral

Sistema genérico e extensível para gerenciamento de imagens, suportando múltiplos provedores de armazenamento (Storage Providers).

## 🎯 Características

- ✅ **Multi-provider**: Base64, OCI, S3, Azure Blob (extensível)
- ✅ **Redimensionamento automático**: Gera thumbnails otimizados
- ✅ **Metadados estruturados**: Tamanho, dimensões, hash MD5, etc
- ✅ **Validação robusta**: Formatos e tamanhos de arquivo
- ✅ **Desacoplamento**: Troca de provider sem alterar código de negócio
- ✅ **Performance**: Índices otimizados para consultas
- ✅ **Auditoria**: CreatedAt e UpdatedAt em todas as imagens

## 🏗️ Arquitetura

```
┌─────────────────────────────────────────────────────────────┐
│                    API Layer (Endpoints)                     │
├─────────────────────────────────────────────────────────────┤
│                   Application Layer                          │
│  ┌─────────────────┐         ┌────────────────────────┐     │
│  │  IImageService  │────────▶│  IStorageProvider      │     │
│  └─────────────────┘         └────────────────────────┘     │
│         │                              │                     │
│         │                              │                     │
│  ┌──────▼──────────┐         ┌─────────▼────────────┐       │
│  │  ImageService   │         │ Base64StorageProvider│       │
│  │  (Orquestrador) │         │ OCIStorageProvider   │       │
│  └─────────────────┘         │ S3StorageProvider    │       │
│                              │ AzureStorageProvider │       │
│                              └──────────────────────┘       │
├─────────────────────────────────────────────────────────────┤
│                     Domain Layer                             │
│  ┌────────────┐  ┌──────────────┐  ┌──────────────────┐    │
│  │   Imagem   │  │ImageMetadata │  │ ImagemDtos       │    │
│  └────────────┘  └──────────────┘  └──────────────────┘    │
├─────────────────────────────────────────────────────────────┤
│                Infrastructure Layer (EF Core)                │
│                   PostgreSQL Database                        │
└─────────────────────────────────────────────────────────────┘
```

## 📁 Estrutura de Arquivos

```
EcoTurismo.Domain/
├── Entities/
│   └── Imagem.cs                    # Entidade principal
└── ValueObjects/
    └── ImageMetadata.cs             # Metadados estruturados

EcoTurismo.Application/
├── DTOs/
│   └── ImagemDtos.cs                # DTOs de request/response
├── Interfaces/
│   ├── IImageService.cs             # Interface principal
│   └── IStorageProvider.cs          # Interface para providers
└── Services/
    ├── ImageService.cs              # Orquestrador principal
    ├── StorageProviderFactory.cs    # Factory de providers
    └── Storage/
        ├── Base64StorageProvider.cs # Implementação Base64
        └── OCIStorageProvider.cs    # Implementação OCI (template)

EcoTurismo.Infra/
└── Configurations/
    └── ImagemConfiguration.cs       # Configuração EF Core
```

## 🔧 Configuração

### appsettings.json

```json
{
  "Storage": {
    "Provider": "base64",  // ou "oci", "s3", "azure"
    "OCI": {
      "Region": "us-ashburn-1",
      "Namespace": "seu-namespace",
      "BucketName": "ecoturismo-images",
      "TenancyOCID": "ocid1.tenancy.oc1...",
      "UserOCID": "ocid1.user.oc1...",
      "Fingerprint": "...",
      "PrivateKeyPath": "/path/to/key.pem"
    }
  }
}
```

### Registro de Serviços (Program.cs)

```csharp
// Storage Provider Factory
builder.Services.AddSingleton<StorageProviderFactory>();
builder.Services.AddScoped<IStorageProvider>(sp =>
{
    var factory = sp.GetRequiredService<StorageProviderFactory>();
    return factory.Create();
});

// Image Service
builder.Services.AddScoped<IImageService, ImageService>();
```

## 💻 Uso Básico

### 1. Upload de Imagem

```csharp
public class UploadImagemEndpoint : Endpoint<UploadImagemRequest>
{
    private readonly IImageService _imageService;

    public override async Task HandleAsync(UploadImagemRequest req, CancellationToken ct)
    {
        // Converter IFormFile para bytes
        using var ms = new MemoryStream();
        await req.Arquivo.CopyToAsync(ms, ct);
        var bytes = ms.ToArray();

        // Fazer upload
        var uploadRequest = new ImagemUploadRequest(
            EntidadeTipo: "Atrativo",
            EntidadeId: req.AtrativoId,
            Categoria: "galeria",
            ImagemBytes: bytes,
            NomeArquivo: req.Arquivo.FileName,
            TipoMime: req.Arquivo.ContentType,
            Ordem: 0
        );

        var result = await _imageService.SalvarImagemAsync(uploadRequest);
        
        if (!result.Success)
            ThrowError(result.ErrorMessage!);

        await SendOkAsync(result.Data!, ct);
    }
}
```

### 2. Listar Imagens de uma Entidade

```csharp
var imagens = await _imageService.ListarImagensAsync("Atrativo", atrativoId);

// Filtrar por categoria
var imagensPrincipais = await _imageService.ListarImagensPorCategoriaAsync(
    "Atrativo", 
    atrativoId, 
    "principal"
);
```

### 3. Remover Imagem

```csharp
var result = await _imageService.RemoverImagemAsync(imagemId);
```

## 🔄 Migração de Provider

### De Base64 para OCI (exemplo)

1. **Instalar pacote OCI**:
```bash
dotnet add package OCI.DotNetSDK.Objectstorage
```

2. **Implementar OCIStorageProvider** (ver template em `OCIStorageProvider.cs`)

3. **Configurar appsettings.json**:
```json
{
  "Storage": {
    "Provider": "oci",
    "OCI": { ... }
  }
}
```

4. **Migrar dados existentes** (script de migração):
```csharp
// Buscar todas as imagens base64
var imagensBase64 = await _db.Imagens
    .Where(i => i.StorageProvider == "base64")
    .ToListAsync();

var ociProvider = new OCIStorageProvider(...);

foreach (var imagem in imagensBase64)
{
    // Extrair bytes do base64
    var bytes = await _base64Provider.GetImageBytesAsync(imagem.ImagemUrl);
    
    // Salvar no OCI
    var novaUrl = await ociProvider.SaveImageAsync(
        bytes, 
        metadados.NomeArquivo, 
        metadados.TipoMime
    );
    
    // Atualizar registro
    imagem.ImagemUrl = novaUrl;
    imagem.StorageProvider = "oci";
}

await _db.SaveChangesAsync();
```

## 📊 Modelo de Dados

### Tabela Imagens

| Coluna | Tipo | Descrição |
|--------|------|-----------|
| Id | UUID | Identificador único |
| EntidadeTipo | VARCHAR(50) | Tipo da entidade (Banner, Atrativo, etc) |
| EntidadeId | UUID | ID da entidade proprietária |
| Categoria | VARCHAR(50) | Categoria (principal, galeria, logo_login) |
| ImagemUrl | TEXT | URL ou data URI da imagem |
| ThumbnailUrl | TEXT | URL ou data URI do thumbnail |
| StorageProvider | VARCHAR(20) | Provider usado (base64, oci, s3) - **Opcional** |
| Ordem | INT | Ordem de exibição |
| MetadadosJson | JSON | Metadados estruturados |
| CreatedAt | TIMESTAMP | Data de criação |
| UpdatedAt | TIMESTAMP | Data de atualização |

### Índices

- `IX_Imagens_Entidade` (EntidadeTipo, EntidadeId)
- `IX_Imagens_Entidade_Categoria` (EntidadeTipo, EntidadeId, Categoria)
- `IX_Imagens_CreatedAt` (CreatedAt)

## 🎨 Categorias de Imagens

| Entidade | Categorias Suportadas |
|----------|----------------------|
| Banner | `principal` |
| Atrativo | `principal`, `galeria` |
| Municipio | `logo_login`, `logo_publico`, `logo_geral` |
| Usuario | `avatar` (futuro) |
| Quiosque | `planta`, `foto` (futuro) |

## 🔒 Validações

- **Formatos permitidos**: JPG, JPEG, PNG, GIF, WEBP
- **Tamanho máximo**: 5MB
- **Thumbnail**: 400x400px (mantém aspect ratio)
- **Qualidade JPEG**: 85%

## 🚀 Próximos Passos

- [ ] Implementar S3StorageProvider
- [ ] Implementar AzureBlobStorageProvider
- [ ] Implementar LocalFileStorageProvider
- [ ] Adicionar suporte a WebP otimizado
- [ ] Implementar conversão automática de formatos
- [ ] Adicionar watermark automático
- [ ] CDN integration
- [ ] Lazy loading de imagens
- [ ] Progressive image loading

## 📝 Notas de Desenvolvimento

### Por que separar Storage Provider?

1. **Flexibilidade**: Trocar de provider sem reescrever lógica de negócio
2. **Testabilidade**: Mock de providers em testes
3. **Escalabilidade**: Base64 para dev, OCI/S3 para produção
4. **Custo**: Otimizar custos escolhendo o provider mais econômico

### Performance

- Base64: ~33% maior que binário, mas sem latência de rede
- OCI/S3: Menor uso de banco, mas com latência de rede
- Thumbnails: Reduzem tráfego em listagens

### Boas Práticas

1. Use thumbnails para listagens
2. Lazy load imagens full size
3. Considere CDN para imagens públicas
4. Implemente cache de imagens no frontend
5. Use hash MD5 para detectar duplicatas
6. **StorageProvider é opcional** - imagens legadas sem provider são tratadas como base64

## ⚠️ Notas de Compatibilidade

### StorageProvider Opcional

O campo `StorageProvider` é **nullable** para manter compatibilidade com:
- Imagens existentes antes da implementação do sistema
- Migração gradual de dados legados
- Flexibilidade em ambientes de desenvolvimento

**Comportamento quando NULL:**
- Sistema assume que é **base64** por padrão
- Migração de dados pode preencher o campo posteriormente
- Queries devem usar `?? "base64"` para segurança

## 📞 Suporte

Para dúvidas sobre implementação de novos providers, consulte:
- `IStorageProvider.cs` - Interface base
- `Base64StorageProvider.cs` - Exemplo de implementação simples
- `OCIStorageProvider.cs` - Template para implementação cloud
