using System;
using FluentValidation;
using ICMS.Application.DTOs.Maternal;

namespace ICMS.Application.Validators.Maternal
{
    public class StartPregnancyDtoValidator : AbstractValidator<StartPregnancyDto>
    {
        public StartPregnancyDtoValidator()
        {
            RuleFor(x => x.PersonId)
                .GreaterThan(0)
                .WithMessage("Person Id is required.");

            RuleFor(x => x.LMP)
                .NotEmpty()
                .WithMessage("Last Menstrual Period Date is required.");

            RuleFor(x => x.EDD)
                .NotEmpty()
                .WithMessage("Expected Delivery Date is required.")
                .Must((dto, edd) => edd > dto.LMP)
                .WithMessage("Expected Delivery Date must be after the Last Menstrual Period Date.");

            // Advanced check for standard gestation period (optional but recommended)
            RuleFor(x => x.EDD)
                .Must((dto, edd) =>
                {
                    var daysDiff = edd.ToDateTime(TimeOnly.MinValue) - dto.LMP.ToDateTime(TimeOnly.MinValue);
                    return daysDiff.TotalDays >= 200 && daysDiff.TotalDays <= 320;
                })
                .WithMessage("Expected Delivery Date should be around 280 days (approx. 9 months) after the Last Menstrual Period Date.");

            RuleFor(x => x.PreviousPregnancyComplications)
                .SetValidator(new PreviousPregnancyComplicationsDtoValidator()!)
                .When(x => x.PreviousPregnancyComplications != null);

            RuleFor(x => x.PreviousPregnancyDeliveryComplications)
                .SetValidator(new PreviousPregnancyDeliveryComplicationsDtoValidator()!)
                .When(x => x.PreviousPregnancyDeliveryComplications != null);

            RuleFor(x => x.PreviousPostpartumComplications)
                .SetValidator(new PreviousPostpartumComplicationsDtoValidator()!)
                .When(x => x.PreviousPostpartumComplications != null);
        }
    }
}
