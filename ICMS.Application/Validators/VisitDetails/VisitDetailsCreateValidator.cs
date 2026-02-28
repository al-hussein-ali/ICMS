using FluentValidation;
using ICMS.Application.DTOs.VisitDetails;

namespace ICMS.Application.Validators.VisitDetails
{
    internal class VisitDetailsCreateValidator : AbstractValidator<VisitDetailsCreateDto>
    {
        public VisitDetailsCreateValidator()
        {
            RuleFor(x => x.PregnancyDetailsId)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Pregnancy details id is required.");

            RuleFor(x => x.VisitDate)
                .Must(d => d != default)
                .WithMessage("Visit date is required.");

            RuleFor(x => x.WeightInKilo)
                .GreaterThan(0)
                .WithMessage("Weight must be greater than 0.");

            RuleFor(x => x.BloodPressure)
                .NotEmpty()
                .WithMessage("Blood pressure is required.")
                .MaximumLength(30)
                .WithMessage("Blood pressure must be at most 30 characters.");

            RuleFor(x => x.PregnancyDurationInWeeks)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Pregnancy duration must be zero or positive.");

            RuleFor(x => x.AnaemiaOrHemoglobinType)
                .NotEmpty()
                .WithMessage("Anaemia or Hemoglobin Type is required.")
                .MaximumLength(100)
                .WithMessage("Anaemia or Hemoglobin Type must be at most 100 characters.");

            When(x => x.ClinicalExaminationAndObservation != null, () =>
            {
                RuleFor(x => x.ClinicalExaminationAndObservation)
                    .MaximumLength(1000)
                    .WithMessage("Clinical examination must be at most 1000 characters.");
            });

            RuleFor(x => x.APPInUrineTest)
                .NotEmpty()
                .WithMessage("APP In Urine Test is required.")
                .MaximumLength(30)
                .WithMessage("APP In Urine Test must be at most 30 characters.");

            RuleFor(x => x.OGTTInUrineTest)
                .NotEmpty()
                .WithMessage("OGTT In Urine Test is required.")
                .MaximumLength(30)
                .WithMessage("OGTT In Urine Test must be at most 30 characters.");

            RuleFor(x => x.FetalHeartbeat)
                .NotEmpty()
                .WithMessage("Fetal heartbeat is required.")
                .MaximumLength(30)
                .WithMessage("Fetal heartbeat must be at most 30 characters.");

            RuleFor(x => x.FetalMovement)
                .NotEmpty()
                .WithMessage("Fetal movement is required.")
                .MaximumLength(50)
                .WithMessage("Fetal movement must be at most 50 characters.");

            RuleFor(x => x.FetalPosition)
                .NotEmpty()
                .WithMessage("Fetal position is required.")
                .MaximumLength(40)
                .WithMessage("Fetal position must be at most 40 characters.");
        }
    }
}
