using FluentValidation;
using ICMS.Application.DTOs.Schedules;
using System;

namespace ICMS.Application.Validators.Schedules
{
    internal class MissedScheduleQueryValidator : AbstractValidator<MissedScheduleQueryDto>
    {
        public MissedScheduleQueryValidator()
        {
            RuleFor(x => x.FromDate)
                .NotEmpty()
                .WithMessage("The start date is required.");

            RuleFor(x => x.ToDate)
                .GreaterThanOrEqualTo(x => x.FromDate)
                .When(x => x.ToDate.HasValue)
                .WithMessage("The end date must be greater than or equal to the start date.");
        }
    }
}
