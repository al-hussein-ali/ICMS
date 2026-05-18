using ICMS.Application.Interfaces.Services;
using FluentValidation;
using ICMS.Application.DTOs.Dose;

namespace ICMS.Application.Validators.Dose
{
    internal class DoseCreateValidator : AbstractValidator<DoseCreateDto>
    {
        public DoseCreateValidator(ILocalizer localizer)
        {
            RuleFor(x => x.VaccineId)
                .GreaterThanOrEqualTo(1)
                .WithMessage(x => localizer["ValidationError", "The Vaccine Id is not valid."])
                ;

            RuleFor(x => x.DoseName)
                .NotEmpty()
                .WithMessage(x => localizer["RequiredField", "This field"])
                .Length(1, 150)
                .WithMessage(x => localizer["InvalidField", "This field"]);

            RuleFor(x => x.DoseOrder)
                .NotNull()
                .WithMessage(x => localizer["RequiredField", "This field"])
                .Must(_isValidDoseOrder)
                .WithMessage(x => localizer["InvalidField", "This field"])
                ;

            RuleFor(x => x.RecommendedAgeInWeeks)
                .NotNull()
                .WithMessage(x => localizer["RequiredField", "This field"])
                .GreaterThanOrEqualTo(0)
                .WithMessage(x => localizer["ValidationError", "The Recommended Age In Weeks is not valid."])
                ;

            RuleFor(x => x.RecommendedAgeGroup)
                .NotEmpty()
                .WithMessage(x => localizer["RequiredField", "This field"])
                .Length(1,100)
                .WithMessage(x => localizer["InvalidField", "This field"]);


            When(x => !string.IsNullOrEmpty(x.Notes), () =>
            {
                RuleFor(x => x.Notes)
                    .Length(1, 500)
                    .WithMessage(x => localizer["InvalidField", "This field"]);
            });
           
        }

        private bool _isValidDoseOrder(byte doseOrder)
        {
            return doseOrder >= 1;
        }
    }
}

