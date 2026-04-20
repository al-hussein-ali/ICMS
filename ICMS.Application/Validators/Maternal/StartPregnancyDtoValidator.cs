using ICMS.Application.Interfaces.Services;
using System;
using FluentValidation;
using ICMS.Application.DTOs.Maternal;

namespace ICMS.Application.Validators.Maternal
{
    public class StartPregnancyDtoValidator : AbstractValidator<StartPregnancyDto>
    {
        public StartPregnancyDtoValidator(ILocalizer localizer)
        {
            RuleFor(x => x.PersonId)
                .GreaterThan(0)
                .WithMessage(x => localizer["RequiredField", "This field"]);

            RuleFor(x => x.LMP)
                .NotEmpty()
                .WithMessage(x => localizer["RequiredField", "This field"]);

            RuleFor(x => x.EDD)
                .NotEmpty()
                .WithMessage(x => localizer["RequiredField", "This field"])
                .Must((dto, edd) => edd > dto.LMP)
                .WithMessage(x => localizer["InvalidField", "This field"]);

            // Advanced check for standard gestation period (optional but recommended)
            RuleFor(x => x.EDD)
                .Must((dto, edd) =>
                {
                    var daysDiff = edd.ToDateTime(TimeOnly.MinValue) - dto.LMP.ToDateTime(TimeOnly.MinValue);
                    return daysDiff.TotalDays >= 200 && daysDiff.TotalDays <= 320;
                })
                .WithMessage(x => localizer["ValidationError", "Expected Delivery Date should be around 280 days (approx. 9 months) after the Last Menstrual Period Date."]);

            RuleFor(x => x.PreviousPregnancyComplications)
                .SetValidator(new PreviousPregnancyComplicationsDtoValidator(localizer)!)
                .When(x => x.PreviousPregnancyComplications != null);

            RuleFor(x => x.PreviousPregnancyDeliveryComplications)
                .SetValidator(new PreviousPregnancyDeliveryComplicationsDtoValidator(localizer)!)
                .When(x => x.PreviousPregnancyDeliveryComplications != null);

            RuleFor(x => x.PreviousPostpartumComplications)
                .SetValidator(new PreviousPostpartumComplicationsDtoValidator(localizer)!)
                .When(x => x.PreviousPostpartumComplications != null);
        }
    }
}

