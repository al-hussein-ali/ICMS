using FluentValidation;
using ICMS.Application.DTOs.DoseReport;

namespace ICMS.Application.Validators.DoseReport
{
    internal class DoseReportCreateValidator : AbstractValidator<DoseReportCreateDto>
    {
        public DoseReportCreateValidator()
        {
            RuleFor(x => x.BatchId)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Batch Id is required.");

            RuleFor(x => x.UserId)
                .GreaterThanOrEqualTo(1)
                .WithMessage("User Id is required.");

            When(x => x.Description != null, () =>
            {
                RuleFor(x => x.Description)
                    .MaximumLength(1000)
                    .WithMessage("Description must be at most 1000 characters.");
            });
        }
    }
}
