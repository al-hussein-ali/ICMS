using ICMS.Application.Interfaces.Services;
using FluentValidation;
using ICMS.Application.DTOs.Newborn;

namespace ICMS.Application.Validators.Newborn
{
    internal class NewbornCreateValidator : AbstractValidator<NewbornCreateDto>
    {
        public NewbornCreateValidator(ILocalizer localizer)
        {
            RuleFor(x => x.PregnancyDetailsId)
                .GreaterThanOrEqualTo(1)
                .WithMessage(localizer["RequiredField", "This field"]);

            RuleFor(x => x.NewbornWeightInGrams)
                .InclusiveBetween(500, 8000)
                .WithMessage(localizer["InvalidField", "Newborn weight in grams"]);

            RuleFor(x => x.NewbornStatus)
                .NotNull()
                .WithMessage(localizer["RequiredField", "This field"]);

            RuleFor(x => x.NewbornGender)
                .NotNull()
                .WithMessage(localizer["RequiredField", "This field"]);
        }
    }
}

