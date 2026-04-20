using ICMS.Application.Interfaces.Services;
using FluentValidation;
using ICMS.Application.DTOs.PreviousPregnancyDeliveryComplications;

namespace ICMS.Application.Validators.PreviousPregnancyDeliveryComplications
{
    internal class PreviousPregnancyDeliveryComplicationsCreateValidator : AbstractValidator<PreviousPregnancyDeliveryComplicationsCreateDto>
    {
        public PreviousPregnancyDeliveryComplicationsCreateValidator(ILocalizer localizer)
        {
            RuleFor(x => x.PregnancyDetailId)
                .GreaterThanOrEqualTo(1)
                .WithMessage(x => localizer["RequiredField", "This field"]);
        }
    }
}

