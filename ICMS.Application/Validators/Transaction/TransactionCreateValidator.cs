using FluentValidation;
using ICMS.Application.DTOs.Transaction;

namespace ICMS.Application.Validators.Transaction
{
    internal class TransactionCreateValidator : AbstractValidator<TransactionCreateDto>
    {
        public TransactionCreateValidator()
        {
            RuleFor(x => x.BatchId)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Batch Id is required.");

            RuleFor(x => x.TransactionType)
                .NotNull()
                .WithMessage("Transaction type is required.");

            RuleFor(x => x.TransactionDate)
                .NotNull()
                .WithMessage("Transaction date is required.");

            RuleFor(x => x.Quantity)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Quantity must be zero or positive.");

            RuleFor(x => x.PermissionNumber)
                .NotEmpty()
                .WithMessage("Permission number is required.")
                .MaximumLength(200)
                .WithMessage("Permission number must be at most 200 characters.");

            RuleFor(x => x.SourceorDestination)
                .NotEmpty()
                .WithMessage("Source or destination is required.")
                .MaximumLength(250)
                .WithMessage("Source or destination must be at most 250 characters.");
        }
    }
}
