using FluentValidation;
using ICMS.Application.DTOs.Newborn;

namespace ICMS.Application.Validators.Newborn
{
    internal class NewbornCreateValidator : AbstractValidator<NewbornCreateDto>
    {
        public NewbornCreateValidator()
        {
            RuleFor(x => x.PregnancyDetailsId)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Pregnancy details id is required.");

            RuleFor(x => x.NewbornWeightInGrams)
                .GreaterThan(0)
                .WithMessage("Newborn weight must be greater than 0.");

            RuleFor(x => x.NewbornStatus)
                .NotNull()
                .WithMessage("Newborn status is required.");

            RuleFor(x => x.NewbornGender)
                .NotNull()
                .WithMessage("Newborn gender is required.");
        }
    }
}
