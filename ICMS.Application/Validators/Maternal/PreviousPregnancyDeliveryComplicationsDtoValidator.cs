using FluentValidation;
using ICMS.Application.DTOs.Maternal;

namespace ICMS.Application.Validators.Maternal
{
    public class PreviousPregnancyDeliveryComplicationsDtoValidator : AbstractValidator<PreviousPregnancyDeliveryComplicationsDto>
    {
        public PreviousPregnancyDeliveryComplicationsDtoValidator()
        {
            // All bools. No special rules.
        }
    }
}
