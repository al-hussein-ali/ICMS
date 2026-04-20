using ICMS.Application.Interfaces.Services;
using FluentValidation;
using ICMS.Application.DTOs.ImmunizationRecord;

namespace ICMS.Application.Validators.ImmunizationRecord
{
    internal class ImmunizationRecordCreateValidator : AbstractValidator<ImmunizationRecordCreateDto>
    {
        public ImmunizationRecordCreateValidator(ILocalizer localizer)
        {
            RuleFor(x => x.IndividualId)
                .GreaterThanOrEqualTo(1)
                .WithMessage(x => localizer["RequiredField", "This field"]);

            RuleFor(x => x.DoseId)
                .GreaterThanOrEqualTo(1)
                .WithMessage(x => localizer["RequiredField", "This field"]);

            When(x => x.FieldVisitId != null, () =>
            {
                RuleFor(x => x.FieldVisitId!.Value)
                    .GreaterThanOrEqualTo(1)
                    .WithMessage(x => localizer["InvalidField", "This field"]);
            });

            RuleFor(x => x.VaccinationDate)
                .Must(d => d != default)
                .NotEmpty()
                .WithMessage(x => localizer["RequiredField", "This field"]);

            RuleFor(x => x.TakenIn)
                .NotEmpty()
                .WithMessage(x => localizer["RequiredField", "This field"])
                .MaximumLength(200)
                .WithMessage(x => localizer["InvalidField", "This field"]);

            When(x => x.Notes != null, () =>
            {
                RuleFor(x => x.Notes)
                    .MaximumLength(500)
                    .WithMessage(x => localizer["InvalidField", "This field"]);
            });
        }
    }
}

