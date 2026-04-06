using FluentValidation;
using ICMS.Application.DTOs.Person;
using ICMS.Application.DTOs.VaccinatedIndividual;

namespace ICMS.Application.Validators.VaccinatedIndividual
{
    internal class VaccinatedIndividualCreateValidator : AbstractValidator<VaccinatedIndividualCreateDto>
    {
        public  VaccinatedIndividualCreateValidator(IValidator<PersonCreateDto> personCreateValidator)
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

            RuleFor(x => x)
                .Must(x => x.PersonId != null || x.PersonCreateDto != null)
                .WithMessage("Either PersonId or PersonCreateDto must be provided.");

            When(x => x.PersonId != null, () =>
            {
                RuleFor(x => x.PersonId)
                    .GreaterThanOrEqualTo(1)
                    .WithMessage("PersonId must be greater than 0.");
            });


            When(x => x.PersonCreateDto != null, () =>
            {
                RuleFor(x => x.PersonCreateDto)
                    .SetValidator(personCreateValidator!);
            });

        }
    }
}
