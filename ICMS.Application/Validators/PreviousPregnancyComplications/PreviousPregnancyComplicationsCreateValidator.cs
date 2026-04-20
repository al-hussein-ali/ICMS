using ICMS.Application.Interfaces.Services;
using FluentValidation;
using ICMS.Application.DTOs.PreviousPregnancyComplications;

namespace ICMS.Application.Validators.PreviousPregnancyComplications
{
    internal class PreviousPregnancyComplicationsCreateValidator : AbstractValidator<PreviousPregnancyComplicationsCreateDto>
    {
        public PreviousPregnancyComplicationsCreateValidator(ILocalizer localizer)
        {
            RuleFor(x => x.PregnancyDetailId)
                .GreaterThanOrEqualTo(1)
                .WithMessage(x => localizer["RequiredField", "This field"]);
        }
    }
}

