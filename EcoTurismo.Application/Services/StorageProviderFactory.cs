using EcoTurismo.Application.Interfaces;
using EcoTurismo.Application.Services.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EcoTurismo.Application.Services;

/// <summary>
/// Factory para criar o Storage Provider apropriado baseado na configuração
/// </summary>
public class StorageProviderFactory
{
    private readonly IConfiguration _configuration;
    private readonly ILoggerFactory _loggerFactory;

    public StorageProviderFactory(IConfiguration configuration, ILoggerFactory loggerFactory)
    {
        _configuration = configuration;
        _loggerFactory = loggerFactory;
    }

    public IStorageProvider Create()
    {
        // Ler configuração do appsettings.json
        var providerName = _configuration["Storage:Provider"]?.ToLowerInvariant() ?? "base64";

        return providerName switch
        {
            "base64" => new Base64StorageProvider(),
            
            "oci" => new OCIStorageProvider(
                _configuration, 
                _loggerFactory.CreateLogger<OCIStorageProvider>()
            ),
            
            // Adicionar outros providers aqui quando implementar:
            // "s3" => new S3StorageProvider(...),
            // "azure" => new AzureBlobStorageProvider(...),
            // "local" => new LocalFileStorageProvider(...),
            
            _ => throw new NotSupportedException(
                $"Storage provider '{providerName}' não é suportado. " +
                $"Valores válidos: base64, oci")
        };
    }
}
