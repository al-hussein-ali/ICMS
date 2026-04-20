using ICMS.Application.Interfaces.Services;
using FluentValidation;
using ICMS.Application.DTOs.Person;
using ICMS.Application.DTOs.VaccinatedIndividual;

namespace ICMS.Application.Validators.VaccinatedIndividual
{
    internal class VaccinatedIndividualCreateValidator : AbstractValidator<VaccinatedIndividualCreateDto>
    {
        public  VaccinatedIndividualCreateValidator(IValidator<PersonCreateDto> personCreateValidator, ILocalizer localizer)
        {
            RuleFor(x => x.DirectorateId)
                .NotEmpty()
                .WithMessage(x => localizer["RequiredField", "This field"])
                .GreaterThan(0)
                .WithMessage(x => localizer["InvalidField", "This field"]);

            RuleFor(x => x.NeighborhoodId)
                .NotEmpty()
                .WithMessage(x => localizer["RequiredField", "This field"])
                .GreaterThan(0)
                .WithMessage(x => localizer["InvalidField", "This field"]);

            RuleFor(x => x)
                .Must(x => x.PersonId != null || x.PersonCreateDto != null)
                .WithMessage(x => localizer["InvalidField", "This field"]);

            When(x => x.PersonId != null, () =>
            {
                RuleFor(x => x.PersonId)
                    .GreaterThanOrEqualTo(1)
                    .WithMessage(x => localizer["InvalidField", "This field"]);
            });


            When(x => x.PersonCreateDto != null, () =>
            {
                RuleFor(x => x.PersonCreateDto)
                    .SetValidator(personCreateValidator!);
            });

        }
    }
}

