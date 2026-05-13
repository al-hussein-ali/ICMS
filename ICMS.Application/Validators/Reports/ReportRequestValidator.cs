using FluentValidation;
using ICMS.Application.DTOs.Reports;
using ICMS.Application.Interfaces.Services;

namespace ICMS.Application.Validators.Reports
{
    public class ReportRequestValidator : AbstractValidator<ReportRequestDto>
    {
        public ReportRequestValidator(ILocalizer localizer)
        {
            RuleFor(x => x.ReportType)
                .IsInEnum().WithMessage(localizer["InvalidReportType"]);

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage(localizer["StartDateRequired"]);

            RuleFor(x => x.EndDate)
                .NotEmpty().WithMessage(localizer["EndDateRequired"])
                .GreaterThanOrEqualTo(x => x.StartDate)
                .WithMessage(localizer["EndDateMustBeAfterStart"]);

            RuleFor(x => x.Format)
                .IsInEnum().WithMessage(localizer["InvalidFormat"]);
        }
    }
}
