using FluentValidation;
using ICMS.Application.DTOs.VaccinatedIndividual;

namespace ICMS.Application.Validators.VaccinatedIndividual
{
    internal class VaccinatedIndividualCreateValidator : AbstractValidator<VaccinatedIndividualCreateDto>
    {
        public VaccinatedIndividualCreateValidator()
        {

            RuleFor(x => x.CardNumber)
                .NotEmpty()
                .WithMessage("The Card Number is required")
                .Length(1, 100).WithMessage("The Card Number should be at least 1 and at most 100 length");

            RuleFor(x => x.Area)
                .NotEmpty()
                .WithMessage("The Area is required")
                .MaximumLength(100)
                .WithMessage("The Area must be at most 100 characters.");

            RuleFor(x => x.Neighborhood)
                .NotEmpty()
                .WithMessage("The Neighborhood is required")
                .MaximumLength(100)
                .WithMessage("The Neighborhood must be at most 100 characters.");

            RuleFor(x => x.Directorate)
                .NotEmpty()
                .WithMessage("The Directorate is required")
                .MaximumLength(100)
                .WithMessage("The Directorate must be at most 100 characters.");

            RuleFor(x => x.PersonId)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Person Id is required and must be greater than 0.");
        }
    }
}
