using EcoTurismo.Api.Authorization;
using EcoTurismo.Application.DTOs;
using EcoTurismo.Application.Interfaces;
using EcoTurismo.Infra.Data;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace EcoTurismo.Api.Endpoints.Uploads.Municipios;

public class UploadLogoLoginEndpoint : Endpoint<UploadLogoLoginRequest>
{
    private readonly EcoTurismoDbContext _db;
    private readonly IImageService _imageService;

    public UploadLogoLoginEndpoint(EcoTurismoDbContext db, IImageService imageService)
    {
        _db = db;
        _imageService = imageService;
    }

    public override void Configure()
    {
        Post("/api/uploads/municipios/{MunicipioId}/logo-login");
        Policies(RolePolicies.AdminOrPrefeituraPolicy);
        AllowFileUploads();
        Description(d => d
            .WithTags("Uploads", "Municípios")
            .WithSummary("Upload do logo da tela de login do município")
            .Produces(200)
            .Produces(400)
            .Produces(404));
    }

    public override async Task HandleAsync(UploadLogoLoginRequest req, CancellationToken ct)
    {
        var municipio = await _db.Municipios
            .FirstOrDefaultAsync(m => m.Id == req.MunicipioId, ct);

        if (municipio is null)
        {
            ThrowError("Município não encontrado.");
            return;
        }

        // Fazer upload da imagem usando IImageService
        try
        {
            using var memoryStream = new MemoryStream();
            await req.Logo.CopyToAsync(memoryStream, ct);
            var bytes = memoryStream.ToArray();

            var uploadRequest = new ImagemUploadRequest(
                EntidadeTipo: "Municipio",
                EntidadeId: municipio.Id,
                Categoria: "logo_login",
                ImagemBytes: bytes,
                NomeArquivo: req.Logo.FileName,
                TipoMime: req.Logo.ContentType,
                Ordem: 0
            );

            var result = await _imageService.SalvarImagemAsync(uploadRequest);

            if (!result.Success)
            {
                ThrowError($"Erro ao processar a imagem: {result.ErrorMessage}");
                return;
            }

            // Remover imagem antiga se existir
            if (municipio.LogoTelaLoginId.HasValue)
            {
                await _imageService.RemoverImagemAsync(municipio.LogoTelaLoginId.Value);
            }

            // Atualizar município com a nova imagem
            municipio.LogoTelaLoginId = result.Data!.Id;
            await _db.SaveChangesAsync(ct);

            await Send.OkAsync(new
            {
                success = true,
                message = "Logo da tela de login atualizado com sucesso",
                municipioId = municipio.Id,
                municipioNome = municipio.Nome,
                imagem = result.Data
            }, ct);
        }
        catch (Exception ex)
        {
            ThrowError($"Erro ao processar a imagem: {ex.Message}");
        }
    }
}
