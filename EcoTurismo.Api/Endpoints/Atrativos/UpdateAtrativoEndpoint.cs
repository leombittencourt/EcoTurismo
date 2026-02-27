using EcoTurismo.Application.DTOs;
using EcoTurismo.Infra.Data;
using FastEndpoints;
using FluentValidation;

namespace EcoTurismo.Api.Endpoints.Atrativos;

public class UpdateAtrativoRequest
{
    public Guid Id { get; set; }
    public string? Nome { get; set; }
    public string? Tipo { get; set; }
    public string? Descricao { get; set; }
    public string? Imagem { get; set; }
    public int? CapacidadeMaxima { get; set; }
    public int? OcupacaoAtual { get; set; }
    public string? Status { get; set; }
}

public class UpdateAtrativoValidator : Validator<UpdateAtrativoRequest>
{
    public UpdateAtrativoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id é obrigatório");

        RuleFor(x => x.Nome)
            .MaximumLength(200).WithMessage("Nome deve ter no máximo 200 caracteres")
            .When(x => x.Nome is not null);

        RuleFor(x => x.Tipo)
            .MaximumLength(20).WithMessage("Tipo deve ter no máximo 20 caracteres")
            .When(x => x.Tipo is not null);

        RuleFor(x => x.Status)
            .MaximumLength(20).WithMessage("Status deve ter no máximo 20 caracteres")
            .When(x => x.Status is not null);
    }
}

public class UpdateAtrativoEndpoint : Endpoint<UpdateAtrativoRequest, AtrativoDto>
{
    private readonly EcoTurismoDbContext _db;

    public UpdateAtrativoEndpoint(EcoTurismoDbContext db) => _db = db;

    public override void Configure()
    {
        Put("/api/atrativos/{Id}");
        Roles("admin", "prefeitura");
    }

    public override async Task HandleAsync(UpdateAtrativoRequest req, CancellationToken ct)
    {
        var a = await _db.Atrativos.FindAsync([req.Id], ct);

        if (a is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        if (req.Nome is not null) a.Nome = req.Nome;
        if (req.Tipo is not null) a.Tipo = req.Tipo;
        if (req.Descricao is not null) a.Descricao = req.Descricao;
        if (req.Imagem is not null) a.Imagem = req.Imagem;
        if (req.CapacidadeMaxima.HasValue) a.CapacidadeMaxima = req.CapacidadeMaxima.Value;
        if (req.OcupacaoAtual.HasValue) a.OcupacaoAtual = req.OcupacaoAtual.Value;
        if (req.Status is not null) a.Status = req.Status;

        await _db.SaveChangesAsync(ct);

        await Send.OkAsync(new AtrativoDto(
            a.Id, a.Nome, a.Tipo, a.MunicipioId,
            a.CapacidadeMaxima, a.OcupacaoAtual, a.Status,
            a.Descricao, a.Imagem
        ), ct);
    }
}
