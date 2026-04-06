using FluentValidation;
using ICMS.Application.DTOs.Maternal;

namespace ICMS.Application.Validators.Maternal
{
    public class PreviousPregnancyComplicationsDtoValidator : AbstractValidator<PreviousPregnancyComplicationsDto>
    {
        public PreviousPregnancyComplicationsDtoValidator()
        {
            // All fields are bools, so no special validation needed unless we want to enforce at least one true?
            // For now, no specific rules.
        }
    }
}
