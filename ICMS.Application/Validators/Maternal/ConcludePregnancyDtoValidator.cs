using ICMS.Application.Interfaces.Services;
using FluentValidation;
using ICMS.Application.DTOs.Maternal;

namespace ICMS.Application.Validators.Maternal
{
    public class ConcludePregnancyDtoValidator : AbstractValidator<ConcludePregnancyDto>
    {
        public ConcludePregnancyDtoValidator(ILocalizer localizer)
        {
            RuleFor(x => x.DeliveryDate)
                .NotEmpty().WithMessage(x => localizer["RequiredField", "This field"])
                .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
                .WithMessage(x => localizer["FutureDateNotAllowed"]);

            RuleFor(x => x.BirthNature)
                .IsInEnum().WithMessage(x => localizer["InvalidField", "This field"]);

            RuleFor(x => x.BirthLocationType)
                .IsInEnum().WithMessage(x => localizer["InvalidField", "This field"]);

            RuleFor(x => x.BirthLocationDetails)
                .NotEmpty().WithMessage(x => localizer["RequiredField", "This field"]);

            RuleFor(x => x.Newborns)
                .NotEmpty().WithMessage(x => localizer["RequiredField", "This field"])
                .Must(x => x != null && x.Count > 0).WithMessage(x => localizer["RequiredField", "This field"]);

            RuleForEach(x => x.Newborns)
                .SetValidator(new NewbornDtoValidator(localizer));
        }
    }
}

