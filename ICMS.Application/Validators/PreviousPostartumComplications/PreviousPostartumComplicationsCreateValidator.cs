using FluentValidation;
using ICMS.Application.DTOs.PreviousPostartumComplications;

namespace ICMS.Application.Validators.PreviousPostartumComplications
{
    internal class PreviousPostartumComplicationsCreateValidator : AbstractValidator<PreviousPostartumComplicationsCreateDto>
    {
        public PreviousPostartumComplicationsCreateValidator()
        {
            RuleFor(x => x.PregnancyDetailId)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Pregnancy detail id is required.");

            // Booleans are present in DTO; no further validation needed.
        }
    }
}
