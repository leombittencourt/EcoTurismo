using FastEndpoints;
using FluentValidation;

namespace EcoTurismo.Api.Endpoints.Banners;

public class UpdateBannerValidator : Validator<UpdateBannerRequest>
{
    public UpdateBannerValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id é obrigatório");
    }
}
