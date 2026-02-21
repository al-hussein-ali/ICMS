using FluentValidation;
using ICMS.Application.DTOs.Dose;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMS.Application.Validators.Dose
{
    internal class DoseCreateValidator : AbstractValidator<DoseCreateDto>
    {
        public DoseCreateValidator()
        {
            RuleFor(x => x.VaccineId)
                .GreaterThanOrEqualTo(1)
                .WithMessage("The Vaccine Id is not valid.");

            RuleFor(x => x.DoseName)
                .NotEmpty()
                .WithMessage("The Dose Name is required.");

            RuleFor(x => x.DoseOrder)
                .NotNull()
                .WithMessage("The Dose Order is required.")
                .Must(_isValidDoseOrder)
                .WithMessage("The Dose Order is invalid.");

            RuleFor(x => x.RecommendedAgeInMonths)
                .NotNull()
                .WithMessage("The Recommended Age In Months is required.")
                .GreaterThanOrEqualTo(0)
                .WithMessage("The Recommended Age In Months is not valid.");

            RuleFor(x => x.RecommendedAgeGroup)
                .NotEmpty()
                .WithMessage("The Recommended Age Group is required.");


            RuleFor(x => x.Notes);

        }

        private bool _isValidDoseOrder(byte doseOrder)
        {
            return doseOrder >= 1;
        }
    }
}
