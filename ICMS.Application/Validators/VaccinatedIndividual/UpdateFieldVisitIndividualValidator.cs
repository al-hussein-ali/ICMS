using ICMS.Application.Interfaces.Services;
using FluentValidation;
using ICMS.Application.DTOs.VaccinatedIndividual;


namespace ICMS.Application.Validators.VaccinatedIndividual
{
    internal class UpdateFieldVisitIndividualValidator : AbstractValidator<UpdateFieldVisitIndividualDto>
    {
        public UpdateFieldVisitIndividualValidator(ILocalizer localizer)
        {

            RuleFor(x => x.IndividualId)
                .GreaterThanOrEqualTo(1)
                .WithMessage(localizer["RequiredField", "This field"]);


            RuleFor(x => x.DoseId)
                .GreaterThanOrEqualTo(1)
                .WithMessage(localizer["RequiredField", "This field"]);


            RuleFor(x => x.FieldVisitId)
                .GreaterThanOrEqualTo(1)
                .WithMessage(localizer["InvalidField", "This field"]);


            RuleFor(x => x.VaccinationDate)
                .Must(d => d != default)
                .NotEmpty()
                .WithMessage(localizer["RequiredField", "This field"])
                .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
                .WithMessage(localizer["FutureDateNotAllowed"]);

            RuleFor(x => x.TakenIn)
                .NotEmpty()
                .WithMessage(localizer["RequiredField", "This field"])
                .MaximumLength(200)
                .WithMessage(localizer["InvalidField", "This field"]);

            When(x => x.Note != null, () =>
            {
                RuleFor(x => x.Note)
                    .MaximumLength(500)
                    .WithMessage(localizer["InvalidField", "This field"]);
            });
        }
    }
}

