using FastEndpoints;
using FluentValidation;

namespace EcoTurismo.Api.Endpoints.Uploads;

public class UploadBannerValidator : Validator<UploadBannerRequest>
{
    public UploadBannerValidator()
    {
        RuleFor(x => x.Imagem)
            .NotNull()
            .WithMessage("A imagem é obrigatória.");

        RuleFor(x => x.Imagem)
            .Must(BeAValidImage)
            .When(x => x.Imagem != null)
            .WithMessage("O arquivo deve ser uma imagem válida (jpg, jpeg, png, gif, webp).");

        RuleFor(x => x.Imagem)
            .Must(BeWithinSizeLimit)
            .When(x => x.Imagem != null)
            .WithMessage("A imagem não pode ter mais de 5MB.");

        RuleFor(x => x.Titulo)
            .NotEmpty()
            .WithMessage("O título é obrigatório.")
            .MaximumLength(200)
            .WithMessage("O título não pode ter mais de 200 caracteres.");

        RuleFor(x => x.Subtitulo)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.Subtitulo))
            .WithMessage("O subtítulo não pode ter mais de 500 caracteres.");

        RuleFor(x => x.Link)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrEmpty(x.Link))
            .WithMessage("O link não pode ter mais de 1000 caracteres.");

        RuleFor(x => x.Ordem)
            .GreaterThanOrEqualTo(0)
            .When(x => x.Ordem.HasValue)
            .WithMessage("A ordem deve ser um número positivo.");
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
