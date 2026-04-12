using FluentValidation;
using ICMS.Application.DTOs.Batch;

namespace ICMS.Application.Validators.Batch
{
    internal class BatchCreateValidator : AbstractValidator<BatchCreateDto>
    {
        public BatchCreateValidator()
        {
            RuleFor(x => x.DoseId)
                .GreaterThanOrEqualTo(1)
                .WithMessage("The Dose Id is required and must be greater than 0.");

            RuleFor(x => x.CountryOfOrigin)
                .NotEmpty()
                .WithMessage("Country of origin is required.")
                .MaximumLength(150)
                .WithMessage("Country of origin must be at most 150 characters.");

            RuleFor(x => x.CookNumber)
                .NotEmpty()
                .WithMessage("Cook number is required.")
                .MaximumLength(200)
                .WithMessage("Cook number must be at most 200 characters.");

            RuleFor(x => x.ExpiryDate)
                .Must(d => d != default)
                .WithMessage("Expiry Date is required.");

            RuleFor(x => x.TotalQuantity)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Total quantity must be zero or a positive number.");

            When(x => x.Notes != null, () =>
            {
                RuleFor(x => x.Notes)
                    .MaximumLength(500)
                    .WithMessage("Notes must be at most 500 characters.");
            });
        }
    }
}
