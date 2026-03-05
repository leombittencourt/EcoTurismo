using EcoTurismo.Api.Authorization;
using EcoTurismo.Application.DTOs;
using EcoTurismo.Infra.Data;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace EcoTurismo.Api.Endpoints.Uploads.Atrativos;

public class UploadImagensAtrativoEndpoint : Endpoint<UploadImagensAtrativoRequest>
{
    private readonly EcoTurismoDbContext _db;

    public UploadImagensAtrativoEndpoint(EcoTurismoDbContext db)
    {
        _db = db;
    }

    public override void Configure()
    {
        Post("/api/uploads/atrativos/{AtrativoId}/imagens");
        Policies(RolePolicies.AdminOrPrefeituraPolicy);
        AllowFileUploads();
        Description(d => d
            .WithTags("Uploads", "Atrativos")
            .WithSummary("Upload de múltiplas imagens para um atrativo")
            .WithDescription("Converte arquivos para base64 e salva no campo Imagens como JSON array")
            .Produces(200)
            .Produces(400)
            .Produces(404));
    }

    public override async Task HandleAsync(UploadImagensAtrativoRequest req, CancellationToken ct)
    {
        var atrativo = await _db.Atrativos
            .FirstOrDefaultAsync(a => a.Id == req.AtrativoId, ct);

        if (atrativo is null)
        {
            ThrowError("Atrativo não encontrado.");
            return;
        }

        // Parse imagens existentes
        var imagensExistentes = string.IsNullOrWhiteSpace(atrativo.Imagens)
            ? new List<ImagemAtrativoDto>()
            : JsonSerializer.Deserialize<List<ImagemAtrativoDto>>(atrativo.Imagens) ?? new List<ImagemAtrativoDto>();

        // Verificar limite total
        if (imagensExistentes.Count + req.Imagens.Length > 20)
        {
            ThrowError("Limite de 20 imagens por atrativo excedido.");
            return;
        }

        // Próxima ordem disponível
        var proximaOrdem = imagensExistentes.Count > 0
            ? imagensExistentes.Max(i => i.Ordem) + 1
            : 1;

        // Converter cada arquivo para base64
        var novasImagens = new List<ImagemAtrativoDto>();

        for (int i = 0; i < req.Imagens.Length; i++)
        {
            var file = req.Imagens[i];

            string base64;
            try
            {
                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream, ct);
                var bytes = memoryStream.ToArray();
                var base64String = Convert.ToBase64String(bytes);
                base64 = $"data:{file.ContentType};base64,{base64String}";
            }
            catch (Exception ex)
            {
                ThrowError($"Erro ao processar imagem {file.FileName}: {ex.Message}");
                return;
            }

            var ordem = req.Ordens != null && req.Ordens.Length > i
                ? req.Ordens[i]
                : proximaOrdem + i;

            var descricao = req.Descricoes != null && req.Descricoes.Length > i
                ? req.Descricoes[i]
                : null;

            var imagemId = Guid.NewGuid().ToString();
            var isPrincipal = req.PrincipalId == imagemId || (imagensExistentes.Count == 0 && i == 0);

            novasImagens.Add(new ImagemAtrativoDto(
                Id: imagemId,
                Url: base64,
                Ordem: ordem,
                Principal: isPrincipal,
                Descricao: descricao
            ));
        }

        // Se tem nova principal, desmarcar todas as antigas
        if (novasImagens.Any(i => i.Principal))
        {
            imagensExistentes = imagensExistentes.Select(i => i with { Principal = false }).ToList();
        }

        // Adicionar novas imagens
        imagensExistentes.AddRange(novasImagens);

        // Salvar JSON atualizado
        atrativo.Imagens = JsonSerializer.Serialize(imagensExistentes);
        atrativo.UpdatedAt = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync(ct);
    }
}
