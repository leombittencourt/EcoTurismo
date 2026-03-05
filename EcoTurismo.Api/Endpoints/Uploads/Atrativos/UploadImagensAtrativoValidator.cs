using FastEndpoints;
using FluentValidation;

namespace EcoTurismo.Api.Endpoints.Uploads.Atrativos;

public class UploadImagensAtrativoValidator : Validator<UploadImagensAtrativoRequest>
{
    public UploadImagensAtrativoValidator()
    {
        RuleFor(x => x.AtrativoId)
            .NotEmpty()
            .WithMessage("O ID do atrativo é obrigatório.");

        RuleFor(x => x.Imagens)
            .NotEmpty()
            .WithMessage("É necessário enviar pelo menos uma imagem.");

        RuleFor(x => x.Imagens)
            .Must(imagens => imagens.Length <= 10)
            .When(x => x.Imagens != null && x.Imagens.Length > 0)
            .WithMessage("Máximo de 10 imagens por upload.");

        RuleForEach(x => x.Imagens)
            .Must(BeAValidImage)
            .WithMessage("Arquivo deve ser uma imagem válida (jpg, jpeg, png, gif, webp).");

        RuleForEach(x => x.Imagens)
            .Must(BeWithinSizeLimit)
            .WithMessage("Cada imagem não pode ter mais de 5MB.");

        RuleFor(x => x.Descricoes)
            .Must((req, descricoes) => descricoes == null || descricoes.Length == req.Imagens.Length)
            .When(x => x.Descricoes != null)
            .WithMessage("Quantidade de descrições deve ser igual à quantidade de imagens.");

        RuleFor(x => x.Ordens)
            .Must((req, ordens) => ordens == null || ordens.Length == req.Imagens.Length)
            .When(x => x.Ordens != null)
            .WithMessage("Quantidade de ordens deve ser igual à quantidade de imagens.");
    }

    private bool BeAValidImage(IFormFile? file)
    {
        if (file == null) return false;

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        return allowedExtensions.Contains(extension);
    }

    private bool BeWithinSizeLimit(IFormFile? file)
    {
        if (file == null) return false;

        const long maxSizeInBytes = 5 * 1024 * 1024; // 5MB
        return file.Length <= maxSizeInBytes;
    }
}
