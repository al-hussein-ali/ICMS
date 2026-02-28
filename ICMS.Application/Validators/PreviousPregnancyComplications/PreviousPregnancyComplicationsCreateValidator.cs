using FluentValidation;
using ICMS.Application.DTOs.PreviousPregnancyComplications;

namespace ICMS.Application.Validators.PreviousPregnancyComplications
{
    internal class PreviousPregnancyComplicationsCreateValidator : AbstractValidator<PreviousPregnancyComplicationsCreateDto>
    {
        public PreviousPregnancyComplicationsCreateValidator()
        {
            RuleFor(x => x.PregnancyDetailId)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Pregnancy detail id is required.");
        }
    }
}
