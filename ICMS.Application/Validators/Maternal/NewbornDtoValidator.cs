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
                .GreaterThan(0).WithMessage(x => localizer["InvalidField", "This field"])
                .LessThanOrEqualTo(10).WithMessage(x => localizer["ValidationError", "Weight is unusually high."]);

            RuleFor(x => x.Gender)
                .IsInEnum().WithMessage(x => localizer["InvalidField", "This field"]);
        }
    }
}

