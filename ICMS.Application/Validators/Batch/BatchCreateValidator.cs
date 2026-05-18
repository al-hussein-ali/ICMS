using FluentValidation;
using ICMS.Application.DTOs.Batch;
using ICMS.Application.Interfaces.Services;

namespace ICMS.Application.Validators.Batch
{
    internal class BatchCreateValidator : AbstractValidator<BatchCreateDto>
    {
        public BatchCreateValidator(ILocalizer localizer)
        {
            RuleFor(x => x.DoseId)
                .GreaterThanOrEqualTo(1)
                .WithMessage(x => localizer["GreaterThanZero", "Dose Id"]);

            RuleFor(x => x.BatchName)
                .NotEmpty()
                .WithMessage(x => localizer["RequiredField", "Batch Name"])
                .MaximumLength(250)
                .WithMessage(x => localizer["MaxLength", "Batch Name", 250]);

            RuleFor(x => x.CreationDate)
                .Must(d => d != default)
                .WithMessage(x => localizer["RequiredField", "Creation Date"]);

            RuleFor(x => x.CountryOfOrigin)
                .NotEmpty()
                .WithMessage(x => localizer["RequiredField", "Country of origin"])
                .MaximumLength(150)
                .WithMessage(x => localizer["MaxLength", "Country of origin", 150]);

            RuleFor(x => x.CookNumber)
                .NotEmpty()
                .WithMessage(x => localizer["RequiredField", "Cook number"])
                .MaximumLength(200)
                .WithMessage(x => localizer["MaxLength", "Cook number", 200]);

            RuleFor(x => x.ExpiryDate)
                .Must(d => d != default)
                .WithMessage(x => localizer["RequiredField", "Expiry Date"]);

            RuleFor(x => x.TotalQuantity)
                .GreaterThanOrEqualTo(0)
                .WithMessage(x => localizer["PositiveNumber", "Total quantity"]);

            When(x => !string.IsNullOrEmpty(x.Notes), () =>
            {
                RuleFor(x => x.Notes)
                    .MaximumLength(500)
                    .WithMessage(x => localizer["MaxLength", "Notes", 500]);
            });
        }
    }
}
