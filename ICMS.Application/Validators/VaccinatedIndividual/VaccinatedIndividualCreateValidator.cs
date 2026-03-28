using FluentValidation;
using ICMS.Application.DTOs.Person;
using ICMS.Application.DTOs.VaccinatedIndividual;

namespace ICMS.Application.Validators.VaccinatedIndividual
{
    internal class VaccinatedIndividualCreateValidator : AbstractValidator<VaccinatedIndividualCreateDto>
    {
        public  VaccinatedIndividualCreateValidator(IValidator<PersonCreateDto> personCreateValidator)
        {



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
                .MaximumLength(60)
                .WithMessage("The Directorate must be at most 100 characters.");
     


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
                    .SetValidator(personCreateValidator);
            });

        }
    }
}
