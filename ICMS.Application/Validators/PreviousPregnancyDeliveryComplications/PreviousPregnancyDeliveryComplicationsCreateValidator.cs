using FluentValidation;
using ICMS.Application.DTOs.PreviousPregnancyDeliveryComplications;

namespace ICMS.Application.Validators.PreviousPregnancyDeliveryComplications
{
    internal class PreviousPregnancyDeliveryComplicationsCreateValidator : AbstractValidator<PreviousPregnancyDeliveryComplicationsCreateDto>
    {
        public PreviousPregnancyDeliveryComplicationsCreateValidator()
        {
            RuleFor(x => x.PregnancyDetailId)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Pregnancy detail id is required.");
        }
    }
}
