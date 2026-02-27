using FastEndpoints;
using FluentValidation;

namespace EcoTurismo.Api.Endpoints.Banners;

public class CreateBannerValidator : Validator<CreateBannerRequest>
{
    public CreateBannerValidator()
    {
        RuleFor(x => x.ImagemUrl)
            .NotEmpty().WithMessage("ImagemUrl é obrigatório");
    }
}
