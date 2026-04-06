using System;
using FluentValidation;
using ICMS.Application.DTOs.Maternal;

namespace ICMS.Application.Validators.Maternal
{
    public class AddAncVisitDtoValidator : AbstractValidator<AddAncVisitDto>
    {
        public AddAncVisitDtoValidator()
        {
            RuleFor(x => x.VisitDate)
                .NotEmpty().WithMessage("Visit date is required.");

            RuleFor(x => x.PregnancyDurationInWeeks)
                .InclusiveBetween(1, 44).WithMessage("Pregnancy duration must be between 1 and 44 weeks.");

            RuleFor(x => x.WeightInKilo)
                .GreaterThan(0).WithMessage("Weight must be greater than 0.");

            RuleFor(x => x.BloodPressure)
                .NotEmpty().WithMessage("Blood pressure is required.")
                .Matches(@"^\d{2,3}/\d{2,3}$").WithMessage("Blood pressure must follow the 'number/number' format (e.g., 120/80).");

            RuleFor(x => x.FetalHeartbeat)
                .Must(value => value == "N/A" || int.TryParse(value, out _))
                .WithMessage("Fetal Heartbeat must be a number or 'N/A'.");

            RuleFor(x => x.DoctorSuggestedNextVisit)
                .Must((dto, nextVisit) => !nextVisit.HasValue || nextVisit.Value > dto.VisitDate)
                .WithMessage("Doctor Suggested Next Visit must be after the current visit date.");
        }
    }
}
