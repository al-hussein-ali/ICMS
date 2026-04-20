using ICMS.Application.Interfaces.Services;
using FluentValidation;
using ICMS.Application.DTOs.Maternal;

namespace ICMS.Application.Validators.Maternal
{
    public class PreviousPostpartumComplicationsDtoValidator : AbstractValidator<PreviousPostpartumComplicationsDto>
    {
        public PreviousPostpartumComplicationsDtoValidator(ILocalizer localizer)
        {
            // All bools. No special rules.
        }
    }
}

