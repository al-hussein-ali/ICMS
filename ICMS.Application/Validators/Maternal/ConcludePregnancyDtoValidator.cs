using FluentValidation;
using ICMS.Application.DTOs.Maternal;

namespace ICMS.Application.Validators.Maternal
{
    public class ConcludePregnancyDtoValidator : AbstractValidator<ConcludePregnancyDto>
    {
        public ConcludePregnancyDtoValidator()
        {
            RuleFor(x => x.DeliveryDate)
                .NotEmpty().WithMessage("Delivery date is required.");

            RuleFor(x => x.BirthNature)
                .IsInEnum().WithMessage("Invalid birth nature.");

            RuleFor(x => x.BirthLocationType)
                .IsInEnum().WithMessage("Invalid birth location type.");

            RuleFor(x => x.BirthLocationDetails)
                .NotEmpty().WithMessage("Birth location details are required.");

            RuleFor(x => x.Newborns)
                .NotEmpty().WithMessage("At least one newborn record is required.")
                .Must(x => x != null && x.Count > 0).WithMessage("At least one newborn record is required.");

            RuleForEach(x => x.Newborns)
                .SetValidator(new NewbornDtoValidator());
        }
    }
}
