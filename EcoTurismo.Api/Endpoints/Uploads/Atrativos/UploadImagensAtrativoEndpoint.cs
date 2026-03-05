using EcoTurismo.Api.Authorization;
using EcoTurismo.Domain.Entities;
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
            .WithDescription("Salva arquivos na tabela Imagens como base64")
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

        // Buscar imagens existentes da tabela Imagens
        var imagensExistentes = await _db.Imagens
            .Where(i => i.EntidadeTipo == "Atrativo" && i.EntidadeId == req.AtrativoId)
            .ToListAsync(ct);

        // Verificar limite total
        if (imagensExistentes.Count + req.Imagens.Count > 20)
        {
            ThrowError("Limite de 20 imagens por atrativo excedido.");
            return;
        }

        // Próxima ordem disponível
        var proximaOrdem = imagensExistentes.Count > 0
            ? imagensExistentes.Max(i => i.Ordem) + 1
            : 1;

        // Converter cada arquivo para base64
        var novasImagens = new List<Imagem>();

        for (int i = 0; i < req.Imagens.Count; i++)
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

            var categoria = imagensExistentes.Count == 0 && i == 0 ? "principal" : "galeria";

            var metadados = JsonSerializer.Serialize(new
            {
                fileName = file.FileName,
                contentType = file.ContentType,
                size = file.Length,
                descricao
            });

            var novaImagem = new Imagem
            {
                Id = Guid.NewGuid(),
                EntidadeTipo = "Atrativo",
                EntidadeId = req.AtrativoId,
                Categoria = categoria,
                ImagemUrl = base64,
                StorageProvider = "base64",
                Ordem = ordem,
                MetadadosJson = metadados,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };

            novasImagens.Add(novaImagem);
        }

        // Se tem nova principal, desmarcar todas as antigas
        if (novasImagens.Any(i => i.Categoria == "principal"))
        {
            foreach (var img in imagensExistentes)
            {
                if (img.Categoria == "principal")
                {
                    img.Categoria = "galeria";
                    img.UpdatedAt = DateTimeOffset.UtcNow;
                }
            }
        }

        // Adicionar novas imagens
        await _db.Imagens.AddRangeAsync(novasImagens, ct);

        atrativo.UpdatedAt = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync(ct);
    }
}
