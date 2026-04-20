using FluentValidation;
using ICMS.Application.DTOs.Maternal;
using ICMS.Domain.Enums;
using ICMS.Application.Interfaces.Services;

namespace ICMS.Application.Validators.Maternal
{
    public class PregnantWomanCreateDtoValidator : AbstractValidator<PregnantWomanCreateDto>
    {
        public PregnantWomanCreateDtoValidator(ILocalizer localizer)
        {
            RuleFor(x => x.AgeRange)
                .NotEmpty().WithMessage(x => localizer["RequiredField", "Age range"]);

            RuleFor(x => x.BloodGroup)
                .IsInEnum().WithMessage(x => localizer["InvalidField", "Blood group"]);

            RuleFor(x => x.RhFactor)
                .IsInEnum().WithMessage(x => localizer["InvalidField", "Rh factor"]);

            // Either PersonId must be provided OR PersonCreateDto must be provided.
            RuleFor(x => x)
                .Must(x => (x.PersonId.HasValue && x.PersonId.Value > 0) || x.PersonCreateDto != null)
                .WithMessage(x => localizer["MissingPersonDetails"]);

            When(x => x.PersonCreateDto != null && (!x.PersonId.HasValue || x.PersonId.Value <= 0), () =>
            {
                RuleFor(x => x.PersonCreateDto!.FirstName)
                    .NotEmpty().WithMessage(x => localizer["RequiredField", "First name"]);
                RuleFor(x => x.PersonCreateDto!.LastName)
                    .NotEmpty().WithMessage(x => localizer["RequiredField", "Last name"]);
                RuleFor(x => x.PersonCreateDto!.Gender)
                    .Must(g => g.Equals("Female", System.StringComparison.OrdinalIgnoreCase))
                    .WithMessage(x => localizer["FemaleGenderRequired"]);
            });
        }
    }
}
