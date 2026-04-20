using ICMS.Application.Interfaces.Services;
using FluentValidation;
using ICMS.Application.DTOs.Maternal;

namespace ICMS.Application.Validators.Maternal
{
    public class PreviousPregnancyDeliveryComplicationsDtoValidator : AbstractValidator<PreviousPregnancyDeliveryComplicationsDto>
    {
        public PreviousPregnancyDeliveryComplicationsDtoValidator(ILocalizer localizer)
        {
            // All bools. No special rules.
        }
    }
}

