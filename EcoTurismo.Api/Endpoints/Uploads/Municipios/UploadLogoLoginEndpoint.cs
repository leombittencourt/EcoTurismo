using EcoTurismo.Api.Authorization;
using EcoTurismo.Infra.Data;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace EcoTurismo.Api.Endpoints.Uploads.Municipios;

public class UploadLogoLoginEndpoint : Endpoint<UploadLogoLoginRequest>
{
    private readonly EcoTurismoDbContext _db;

    public UploadLogoLoginEndpoint(EcoTurismoDbContext db)
    {
        _db = db;
    }

    public override void Configure()
    {
        Post("/api/uploads/municipios/{MunicipioId}/logo-login");
        Policies(RolePolicies.AdminOrPrefeituraPolicy);
        AllowFileUploads();
        Description(d => d
            .WithTags("Uploads", "Municípios")
            .WithSummary("Upload do logo da tela de login do município")
            .WithDescription("Converte o arquivo para base64 e salva no campo LogoTelaLogin")
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

        string logoBase64;
        try
        {
            using var memoryStream = new MemoryStream();
            await req.Logo.CopyToAsync(memoryStream, ct);
            var bytes = memoryStream.ToArray();
            var base64String = Convert.ToBase64String(bytes);
            var contentType = req.Logo.ContentType;
            logoBase64 = $"data:{contentType};base64,{base64String}";
        }
        catch (Exception ex)
        {
            ThrowError($"Erro ao processar a imagem: {ex.Message}");
            return;
        }

        municipio.LogoTelaLogin = logoBase64;
        municipio.CreatedAt = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(ct);

        await Send.OkAsync(new
        {
            success = true,
            message = "Logo da tela de login atualizado com sucesso",
            municipioId = municipio.Id,
            municipioNome = municipio.Nome,
            logoTelaLogin = logoBase64
        }, ct);
    }
}
