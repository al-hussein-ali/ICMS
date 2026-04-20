using ICMS.Application.Interfaces.Services;
using FluentValidation;
using ICMS.Application.DTOs.Schedules;
using System;

namespace ICMS.Application.Validators.Schedules
{
    internal class MissedScheduleQueryValidator : AbstractValidator<MissedScheduleQueryDto>
    {
        public MissedScheduleQueryValidator(ILocalizer localizer)
        {
            RuleFor(x => x.FromDate)
                .NotEmpty()
                .WithMessage(x => localizer["RequiredField", "This field"]);

            RuleFor(x => x.ToDate)
                .GreaterThanOrEqualTo(x => x.FromDate)
                .When(x => x.ToDate.HasValue)
                .WithMessage(x => localizer["InvalidField", "This field"]);
        }
    }
}

