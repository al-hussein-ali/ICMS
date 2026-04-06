using FluentValidation;
using ICMS.Application.DTOs.Maternal;

namespace ICMS.Application.Validators.Maternal
{
    public class NewbornDtoValidator : AbstractValidator<NewbornDto>
    {
        public NewbornDtoValidator()
        {
            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Invalid newborn status.");

            RuleFor(x => x.Weight)
                .GreaterThan(0).WithMessage("Weight must be greater than 0.")
                .LessThanOrEqualTo(10).WithMessage("Weight is unusually high.");

            RuleFor(x => x.Gender)
                .IsInEnum().WithMessage("Invalid gender.");
        }
    }
}
