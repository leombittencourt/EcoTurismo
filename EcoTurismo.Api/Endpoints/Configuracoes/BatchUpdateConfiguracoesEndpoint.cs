using EcoTurismo.Infra.Data;
using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace EcoTurismo.Api.Endpoints.Configuracoes;

public class BatchUpdateConfiguracoesRequest
{
    public List<ConfigItem> Configs { get; set; } = [];
}

public class ConfigItem
{
    public string Chave { get; set; } = string.Empty;
    public string? Valor { get; set; }
}

public class BatchUpdateConfiguracoesValidator : Validator<BatchUpdateConfiguracoesRequest>
{
    public BatchUpdateConfiguracoesValidator()
    {
        RuleFor(x => x.Configs)
            .NotEmpty().WithMessage("Configs não pode estar vazio");

        RuleForEach(x => x.Configs)
            .ChildRules(c =>
            {
                c.RuleFor(x => x.Chave)
                    .NotEmpty().WithMessage("Chave é obrigatória");
            });
    }
}

public class BatchUpdateConfiguracoesEndpoint : Endpoint<BatchUpdateConfiguracoesRequest>
{
    private readonly EcoTurismoDbContext _db;

    public BatchUpdateConfiguracoesEndpoint(EcoTurismoDbContext db) => _db = db;

    public override void Configure()
    {
        Put("/api/configuracoes");
        Roles("admin");
    }

    public override async Task HandleAsync(BatchUpdateConfiguracoesRequest req, CancellationToken ct)
    {
        foreach (var item in req.Configs)
        {
            var config = await _db.Configuracoes
                .FirstOrDefaultAsync(c => c.Chave == item.Chave, ct);

            if (config is not null)
                config.Valor = item.Valor;
        }

        await _db.SaveChangesAsync(ct);
        await Send.NoContentAsync(ct);
    }
}
