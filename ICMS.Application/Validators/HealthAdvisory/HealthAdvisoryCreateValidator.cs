using FluentValidation;
using ICMS.Application.DTOs.HealthAdvisory;
using ICMS.Application.Interfaces.Services;
using System;

namespace ICMS.Application.Validators.HealthAdvisory
{
    public class HealthAdvisoryCreateValidator : AbstractValidator<HealthAdvisoryCreateDto>
    {
        public HealthAdvisoryCreateValidator(ILocalizer localizer)
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage(localizer["TitleRequired"])
                .MaximumLength(250).WithMessage(localizer["TitleMaxLength"]);

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage(localizer["ContentRequired"])
                .MaximumLength(2000).WithMessage(localizer["ContentMaxLength"]);

            RuleFor(x => x.Target)
                .IsInEnum().WithMessage(localizer["InvalidTarget"]);

            RuleFor(x => x.ScheduledDate)
                .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow.AddHours(3)))
                .When(x => x.ScheduledDate.HasValue)
                .WithMessage(localizer["ScheduledDateMustBeFuture"]);
        }

    }
}
