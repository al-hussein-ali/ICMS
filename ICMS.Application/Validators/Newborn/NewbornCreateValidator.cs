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
                .WithMessage(x => localizer["RequiredField", "This field"]);

            RuleFor(x => x.NewbornWeightInGrams)
                .GreaterThan(0)
                .WithMessage(x => localizer["InvalidField", "This field"]);

            RuleFor(x => x.NewbornStatus)
                .NotNull()
                .WithMessage(x => localizer["RequiredField", "This field"]);

            RuleFor(x => x.NewbornGender)
                .NotNull()
                .WithMessage(x => localizer["RequiredField", "This field"]);
        }
    }
}

