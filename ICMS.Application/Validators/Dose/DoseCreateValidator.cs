using FluentValidation;
using ICMS.Application.DTOs.Dose;

namespace ICMS.Application.Validators.Dose
{
    internal class DoseCreateValidator : AbstractValidator<DoseCreateDto>
    {
        public DoseCreateValidator()
        {
            RuleFor(x => x.VaccineId)
                .GreaterThanOrEqualTo(1)
                .WithMessage("The Vaccine Id is not valid.")
                ;

            RuleFor(x => x.DoseName)
                .NotEmpty()
                .WithMessage("The Dose Name is required.")
                .Length(1, 150)
                .WithMessage("The Dose Name must be between 1 and 150 characters long.");

            RuleFor(x => x.DoseOrder)
                .NotNull()
                .WithMessage("The Dose Order is required.")
                .Must(_isValidDoseOrder)
                .WithMessage("The Dose Order is invalid.")
                ;

            RuleFor(x => x.RecommendedAgeInMonths)
                .NotNull()
                .WithMessage("The Recommended Age In Months is required.")
                .GreaterThanOrEqualTo(0)
                .WithMessage("The Recommended Age In Months is not valid.")
                ;

            RuleFor(x => x.RecommendedAgeGroup)
                .NotEmpty()
                .WithMessage("The Recommended Age Group is required.")
                .Length(1,100)
                .WithMessage("The Recommended Age Group must be between 1 and 100 characters long.");


            When(x => x.Notes != null, () =>
            {
                RuleFor(x => x.Notes)
                    .Length(1, 500)
                    .WithMessage("The Note must be between 1 and 500 characters long.");
            });
           
        }

        private bool _isValidDoseOrder(byte doseOrder)
        {
            return doseOrder >= 1;
        }
    }
}
