using ICMS.Application.Interfaces.Services;
using FluentValidation;
using ICMS.Application.DTOs.VisitDetails;

namespace ICMS.Application.Validators.VisitDetails
{
    internal class VisitDetailsCreateValidator : AbstractValidator<VisitDetailsCreateDto>
    {
        public VisitDetailsCreateValidator(ILocalizer localizer)
        {
            RuleFor(x => x.PregnancyDetailsId)
                .GreaterThanOrEqualTo(1)
                .WithMessage(localizer["RequiredField", "This field"]);

            RuleFor(x => x.VisitDate)
                .Must(d => d != default)
                .WithMessage(localizer["RequiredField", "This field"])
                .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
                .WithMessage(localizer["FutureDateNotAllowed"]);

            RuleFor(x => x.WeightInKilo)
                .InclusiveBetween(30m, 250m)
                .WithMessage(localizer["InvalidField", "Weight"]);

            RuleFor(x => x.BloodPressure)
                .NotEmpty()
                .WithMessage(localizer["RequiredField", "This field"])
                .MaximumLength(30)
                .WithMessage(localizer["InvalidField", "This field"]);

            RuleFor(x => x.PregnancyDurationInWeeks)
                .InclusiveBetween(1, 44)
                .WithMessage(localizer["InvalidField", "Pregnancy duration"]);

            RuleFor(x => x.NextVisitDate)
                .Must((dto, nextVisit) => !nextVisit.HasValue || nextVisit.Value > dto.VisitDate)
                .WithMessage(localizer["InvalidField", "Next visit date"]);

            RuleFor(x => x.AnaemiaOrHemoglobinType)
                .NotEmpty()
                .WithMessage(localizer["RequiredField", "This field"])
                .MaximumLength(100)
                .WithMessage(localizer["InvalidField", "This field"]);

            When(x => x.ClinicalExaminationAndObservation != null, () =>
            {
                RuleFor(x => x.ClinicalExaminationAndObservation)
                    .MaximumLength(1000)
                    .WithMessage(localizer["InvalidField", "This field"]);
            });

            RuleFor(x => x.APPInUrineTest)
                .NotEmpty()
                .WithMessage(localizer["RequiredField", "This field"])
                .MaximumLength(30)
                .WithMessage(localizer["InvalidField", "This field"]);

            RuleFor(x => x.OGTTInUrineTest)
                .NotEmpty()
                .WithMessage(localizer["RequiredField", "This field"])
                .MaximumLength(30)
                .WithMessage(localizer["InvalidField", "This field"]);

            RuleFor(x => x.FetalHeartbeat)
                .NotEmpty()
                .WithMessage(localizer["RequiredField", "This field"])
                .MaximumLength(30)
                .WithMessage(localizer["InvalidField", "This field"]);

            RuleFor(x => x.FetalMovement)
                .NotEmpty()
                .WithMessage(localizer["RequiredField", "This field"])
                .MaximumLength(50)
                .WithMessage(localizer["InvalidField", "This field"]);

            RuleFor(x => x.FetalPosition)
                .NotEmpty()
                .WithMessage(localizer["RequiredField", "This field"])
                .MaximumLength(40)
                .WithMessage(localizer["InvalidField", "This field"]);
        }
    }
}

