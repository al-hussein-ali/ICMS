using ICMS.Application.Interfaces.Services;
using FluentValidation;
using ICMS.Application.DTOs.Maternal;

namespace ICMS.Application.Validators.Maternal
{
    public class NewbornDtoValidator : AbstractValidator<NewbornDto>
    {
        public NewbornDtoValidator(ILocalizer localizer)
        {
            RuleFor(x => x.Status)
                .IsInEnum().WithMessage(x => localizer["InvalidField", "This field"]);

            RuleFor(x => x.Weight)
                .InclusiveBetween(0.5m, 8.0m).WithMessage(x => localizer["InvalidField", "Newborn weight"]);

            RuleFor(x => x.Gender)
                .IsInEnum().WithMessage(x => localizer["InvalidField", "This field"]);
        }
    }
}

