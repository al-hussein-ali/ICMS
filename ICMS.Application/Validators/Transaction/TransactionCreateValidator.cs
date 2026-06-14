using ICMS.Application.Interfaces.Services;
using FluentValidation;
using ICMS.Application.DTOs.Transaction;

namespace ICMS.Application.Validators.Transaction
{
    internal class TransactionCreateValidator : AbstractValidator<TransactionCreateDto>
    {
        public TransactionCreateValidator(ILocalizer localizer)
        {
            RuleFor(x => x.BatchId)
                .GreaterThanOrEqualTo(1)
                .WithMessage(x => localizer["RequiredField", "This field"]);

            RuleFor(x => x.TransactionType)
                .NotNull()
                .WithMessage(x => localizer["RequiredField", "This field"]);

            RuleFor(x => x.TransactionDate)
                .NotNull()
                .WithMessage(x => localizer["RequiredField", "This field"])
                .LessThanOrEqualTo(DateTime.Now)
                .WithMessage(x => localizer["FutureDateNotAllowed"]);

            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage(x => localizer["InvalidField", "Quantity"]);

            RuleFor(x => x.PermissionNumber)
                .NotEmpty()
                .WithMessage(x => localizer["RequiredField", "This field"])
                .MaximumLength(200)
                .WithMessage(x => localizer["InvalidField", "This field"]);

            RuleFor(x => x.SourceOrDestination)
                .NotEmpty()
                .WithMessage(x => localizer["RequiredField", "This field"])
                .MaximumLength(250)
                .WithMessage(x => localizer["InvalidField", "This field"]);
        }
    }
}

