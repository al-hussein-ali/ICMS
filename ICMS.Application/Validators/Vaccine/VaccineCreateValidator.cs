using ICMS.Application.Interfaces.Services;
using FluentValidation;
using ICMS.Application.DTOs.Vaccine;

namespace ICMS.Application.Validators.Vaccine
{
    internal class VaccineCreateValidator : AbstractValidator<VaccineCreateDto>
    {
        public VaccineCreateValidator(ILocalizer localizer)
        {
            RuleFor(x => x.VaccineName)
                .NotEmpty()
                .WithMessage(x => localizer["RequiredField", "This field"])
                .MaximumLength(100)
                .WithMessage(x => localizer["InvalidField", "This field"]);

            RuleFor(x => x.VaccineCode)
                .NotEmpty()
                .WithMessage(x => localizer["RequiredField", "This field"])
                .MaximumLength(100)
                .WithMessage(x => localizer["InvalidField", "This field"]);

            When(x => x.Description != null, () =>
            {
                RuleFor(x => x.Description)
                    .MaximumLength(600)
                    .WithMessage(x => localizer["InvalidField", "This field"]);
            });

            RuleFor(x => x.TotalDosages)
                .GreaterThanOrEqualTo((byte)0)
                .WithMessage(x => localizer["InvalidField", "This field"]);

            RuleFor(x => x.MinEligibleAgeInMonths)
                .GreaterThanOrEqualTo(0)
                .WithMessage(x => localizer["InvalidField", "This field"]);

            RuleFor(x => x.MaxEligibleAgeInMonths)
                .GreaterThanOrEqualTo(x => x.MinEligibleAgeInMonths)
                .WithMessage(x => localizer["InvalidField", "This field"]);
        }
    }
}

