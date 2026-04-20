using ICMS.Application.Interfaces.Services;
using FluentValidation;
using ICMS.Application.DTOs.PreviousPostpartumComplications;

namespace ICMS.Application.Validators.PreviousPostpartumComplications
{
    internal class PreviousPostpartumComplicationsCreateValidator : AbstractValidator<PreviousPostpartumComplicationsCreateDto>
    {
        public PreviousPostpartumComplicationsCreateValidator(ILocalizer localizer)
        {
            RuleFor(x => x.PregnancyDetailId)
                .GreaterThanOrEqualTo(1)
                .WithMessage(x => localizer["RequiredField", "This field"]);

            // Booleans are present in DTO; no further validation needed.
        }
    }
}

