using FluentValidation;
using ICMS.Application.DTOs.Vaccine;

namespace ICMS.Application.Validators.Vaccine
{
    internal class VaccineCreateValidator : AbstractValidator<VaccineCreateDto>
    {
        public VaccineCreateValidator()
        {
            RuleFor(x => x.VaccineName)
                .NotEmpty()
                .WithMessage("The Vaccine Name is required")
                .MaximumLength(100)
                .WithMessage("The Vaccine Name must be at most 100 characters.");

            RuleFor(x => x.VaccineCode)
                .NotEmpty()
                .WithMessage("The Vaccine Code is required")
                .MaximumLength(100)
                .WithMessage("The Vaccine Code must be at most 100 characters.");

            When(x => x.Description != null, () =>
            {
                RuleFor(x => x.Description)
                    .MaximumLength(600)
                    .WithMessage("Description must be at most 600 characters.");
            });

            RuleFor(x => x.TotalDosages)
                .GreaterThanOrEqualTo((byte)0)
                .WithMessage("Total dosages must be zero or positive.");
        }
    }
}
