using FluentValidation;
using ICMS.Application.DTOs.PreviousPostpartumComplications;

namespace ICMS.Application.Validators.PreviousPostpartumComplications
{
    internal class PreviousPostpartumComplicationsCreateValidator : AbstractValidator<PreviousPostpartumComplicationsCreateDto>
    {
        public PreviousPostpartumComplicationsCreateValidator()
        {
            RuleFor(x => x.PregnancyDetailId)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Pregnancy detail id is required.");

            // Booleans are present in DTO; no further validation needed.
        }
    }
}
