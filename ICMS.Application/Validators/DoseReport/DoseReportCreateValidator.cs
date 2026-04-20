using ICMS.Application.Interfaces.Services;
using FluentValidation;
using ICMS.Application.DTOs.DoseReport;

namespace ICMS.Application.Validators.DoseReport
{
    internal class DoseReportCreateValidator : AbstractValidator<DoseReportCreateDto>
    {
        public DoseReportCreateValidator(ILocalizer localizer)
        {
            RuleFor(x => x.BatchId)
                .GreaterThanOrEqualTo(1)
                .WithMessage(x => localizer["RequiredField", "This field"]);

            When(x => x.Description != null, () =>
            {
                RuleFor(x => x.Description)
                    .MaximumLength(1000)
                    .WithMessage(x => localizer["InvalidField", "This field"]);
            });
        }
    }
}

