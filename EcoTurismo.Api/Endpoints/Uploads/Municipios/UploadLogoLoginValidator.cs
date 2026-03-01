using FastEndpoints;
using FluentValidation;

namespace EcoTurismo.Api.Endpoints.Uploads.Municipios;

public class UploadLogoLoginValidator : Validator<UploadLogoLoginRequest>
{
    public UploadLogoLoginValidator()
    {
        RuleFor(x => x.MunicipioId)
            .NotEmpty()
            .WithMessage("O ID do município é obrigatório.");

        RuleFor(x => x.Logo)
            .NotNull()
            .WithMessage("O logo é obrigatório.");

        RuleFor(x => x.Logo)
            .Must(BeAValidImage)
            .When(x => x.Logo != null)
            .WithMessage("O arquivo deve ser uma imagem válida (jpg, jpeg, png, gif, webp, svg).");

        RuleFor(x => x.Logo)
            .Must(BeWithinSizeLimit)
            .When(x => x.Logo != null)
            .WithMessage("O logo não pode ter mais de 2MB.");
    }

    private bool BeAValidImage(IFormFile? file)
    {
        if (file == null) return false;

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        
        return allowedExtensions.Contains(extension);
    }

    private bool BeWithinSizeLimit(IFormFile? file)
    {
        if (file == null) return false;
        
        const long maxSizeInBytes = 2 * 1024 * 1024; // 2MB
        return file.Length <= maxSizeInBytes;
    }
}
