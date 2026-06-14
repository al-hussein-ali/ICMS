using ICMS.Application.Interfaces.Services;
using FluentValidation;
using ICMS.Application.DTOs.VaccinatedIndividual;

namespace ICMS.Application.Validators.VaccinatedIndividual
{
    internal class NewFieldVaccinatedIndividualCreateValidator : AbstractValidator<NewFieldVaccinatedIndividualDto>
    {
        public NewFieldVaccinatedIndividualCreateValidator(IValidator<ICMS.Application.DTOs.Person.PersonCreateDto> personValidator, ILocalizer localizer)
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


            RuleFor(x => x.Person)
                .NotNull()
                .WithMessage(x => localizer["RequiredField", "This field"])
                .SetValidator(personValidator);

            RuleFor(x => x.DoseId)
                .GreaterThan(0)
                .WithMessage(x => localizer["RequiredField", "This field"]);

            RuleFor(x => x.VaccinationDate)
                .NotEmpty()
                .WithMessage(x => localizer["RequiredField", "This field"])
                .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
                .WithMessage(x => localizer["FutureDateNotAllowed"]);

            RuleFor(x => x.TakenIn)
                .NotEmpty()
                .WithMessage(x => localizer["RequiredField", "This field"]);
        }
    }
}

