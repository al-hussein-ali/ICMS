using ICMS.Application.Interfaces.Services;
using System;
using FluentValidation;
using ICMS.Application.DTOs.Maternal;

namespace ICMS.Application.Validators.Maternal
{
    public class AddAncVisitDtoValidator : AbstractValidator<AddAncVisitDto>
    {
        public AddAncVisitDtoValidator(ILocalizer localizer)
        {
            RuleFor(x => x.VisitDate)
                .NotEmpty().WithMessage(x => localizer["RequiredField", "This field"]);

            RuleFor(x => x.PregnancyDurationInWeeks)
                .InclusiveBetween(1, 44).WithMessage(x => localizer["InvalidField", "This field"]);

            RuleFor(x => x.WeightInKilo)
                .GreaterThan(0).WithMessage(x => localizer["InvalidField", "This field"]);

            RuleFor(x => x.BloodPressure)
                .NotEmpty().WithMessage(x => localizer["RequiredField", "This field"])
                .Matches(@"^\d{2,3}/\d{2,3}$").WithMessage(x => localizer["ValidationError", "Blood pressure must follow the 'number/number' format (e.g., 120/80)."]);

            RuleFor(x => x.FetalHeartbeat)
                .Must(value => value == "N/A" || int.TryParse(value, out _))
                .WithMessage(x => localizer["InvalidField", "This field"]);

            RuleFor(x => x.DoctorSuggestedNextVisit)
                .Must((dto, nextVisit) => !nextVisit.HasValue || nextVisit.Value > dto.VisitDate)
                .WithMessage(x => localizer["InvalidField", "This field"]);
        }
    }
}

