using FluentValidation;
using ICMS.Application.DTOs.VaccinatedIndividual;

namespace ICMS.Application.Validators.VaccinatedIndividual
{
    internal class NewFieldVaccinatedIndividualCreateValidator : AbstractValidator<NewFieldVaccinatedIndividualDto>
    {
        public NewFieldVaccinatedIndividualCreateValidator(IValidator<ICMS.Application.DTOs.Person.PersonCreateDto> personValidator)
        {
            RuleFor(x => x.DirectorateId)
                .NotEmpty()
                .WithMessage("The Directorate is required")
                .GreaterThan(0)
                .WithMessage("DirectorateId must be greater than 0.");

            RuleFor(x => x.NeighborhoodId)
                .NotEmpty()
                .WithMessage("The Neighborhood is required")
                .GreaterThan(0)
                .WithMessage("NeighborhoodId must be greater than 0.");


            RuleFor(x => x.Person)
                .NotNull()
                .WithMessage("Person details are required.")
                .SetValidator(personValidator);

            RuleFor(x => x.DoseId)
                .GreaterThan(0)
                .WithMessage("Valid DoseId is required.");

            RuleFor(x => x.VaccinationDate)
                .NotEmpty()
                .WithMessage("Vaccination date is required.");

            RuleFor(x => x.TakenIn)
                .NotEmpty()
                .WithMessage("TakenIn (location) is required.");
        }
    }
}
