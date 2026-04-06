using FluentValidation;
using ICMS.Application.DTOs.Maternal;

namespace ICMS.Application.Validators.Maternal
{
    public class PreviousPostpartumComplicationsDtoValidator : AbstractValidator<PreviousPostpartumComplicationsDto>
    {
        public PreviousPostpartumComplicationsDtoValidator()
        {
            // All bools. No special rules.
        }
    }
}
