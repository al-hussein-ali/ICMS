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
                .MaximumLength(500)
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
                .InclusiveBetween((byte)0, (byte)20)
                .WithMessage(x => localizer["InvalidField", "This field"]);

            RuleFor(x => x.MinEligibleAgeInMonths)
                .InclusiveBetween(0, 1200)
                .WithMessage(x => localizer["InvalidField", "This field"]);

            RuleFor(x => x.MaxEligibleAgeInMonths)
                .InclusiveBetween(0, 1200)
                .WithMessage(x => localizer["InvalidField", "This field"])
                .GreaterThanOrEqualTo(x => x.MinEligibleAgeInMonths)
                .WithMessage(x => localizer["InvalidField", "This field"]);
        }
    }
}

