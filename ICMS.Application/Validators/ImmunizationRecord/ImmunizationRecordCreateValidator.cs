using FluentValidation;
using ICMS.Application.DTOs.ImmunizationRecord;

namespace ICMS.Application.Validators.ImmunizationRecord
{
    internal class ImmunizationRecordCreateValidator : AbstractValidator<ImmunizationRecordCreateDto>
    {
        public ImmunizationRecordCreateValidator()
        {
            RuleFor(x => x.IndividualId)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Individual Id is required.");

            RuleFor(x => x.DoseId)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Dose Id is required.");

            When(x => x.FieldVisitId != null, () =>
            {
                RuleFor(x => x.FieldVisitId!.Value)
                    .GreaterThanOrEqualTo(1)
                    .WithMessage("FieldVisitId must be greater than 0 when provided.");
            });

            RuleFor(x => x.VaccinationDate)
                .Must(d => d != default)
                .NotEmpty()
                .WithMessage("Vaccination date is required.");

            RuleFor(x => x.TakenIn)
                .NotEmpty()
                .WithMessage("Taken In is required.")
                .MaximumLength(200)
                .WithMessage("TakenIn must be at most 200 characters.");

            When(x => x.Notes != null, () =>
            {
                RuleFor(x => x.Notes)
                    .MaximumLength(500)
                    .WithMessage("Notes must be at most 500 characters.");
            });
        }
    }
}
