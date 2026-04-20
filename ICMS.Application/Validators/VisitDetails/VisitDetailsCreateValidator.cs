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
                .WithMessage(x => localizer["RequiredField", "This field"]);

            RuleFor(x => x.VisitDate)
                .Must(d => d != default)
                .WithMessage(x => localizer["RequiredField", "This field"]);

            RuleFor(x => x.WeightInKilo)
                .GreaterThan(0)
                .WithMessage(x => localizer["InvalidField", "This field"]);

            RuleFor(x => x.BloodPressure)
                .NotEmpty()
                .WithMessage(x => localizer["RequiredField", "This field"])
                .MaximumLength(30)
                .WithMessage(x => localizer["InvalidField", "This field"]);

            RuleFor(x => x.PregnancyDurationInWeeks)
                .GreaterThanOrEqualTo(0)
                .WithMessage(x => localizer["InvalidField", "This field"]);

            RuleFor(x => x.AnaemiaOrHemoglobinType)
                .NotEmpty()
                .WithMessage(x => localizer["RequiredField", "This field"])
                .MaximumLength(100)
                .WithMessage(x => localizer["InvalidField", "This field"]);

            When(x => x.ClinicalExaminationAndObservation != null, () =>
            {
                RuleFor(x => x.ClinicalExaminationAndObservation)
                    .MaximumLength(1000)
                    .WithMessage(x => localizer["InvalidField", "This field"]);
            });

            RuleFor(x => x.APPInUrineTest)
                .NotEmpty()
                .WithMessage(x => localizer["RequiredField", "This field"])
                .MaximumLength(30)
                .WithMessage(x => localizer["InvalidField", "This field"]);

            RuleFor(x => x.OGTTInUrineTest)
                .NotEmpty()
                .WithMessage(x => localizer["RequiredField", "This field"])
                .MaximumLength(30)
                .WithMessage(x => localizer["InvalidField", "This field"]);

            RuleFor(x => x.FetalHeartbeat)
                .NotEmpty()
                .WithMessage(x => localizer["RequiredField", "This field"])
                .MaximumLength(30)
                .WithMessage(x => localizer["InvalidField", "This field"]);

            RuleFor(x => x.FetalMovement)
                .NotEmpty()
                .WithMessage(x => localizer["RequiredField", "This field"])
                .MaximumLength(50)
                .WithMessage(x => localizer["InvalidField", "This field"]);

            RuleFor(x => x.FetalPosition)
                .NotEmpty()
                .WithMessage(x => localizer["RequiredField", "This field"])
                .MaximumLength(40)
                .WithMessage(x => localizer["InvalidField", "This field"]);
        }
    }
}

