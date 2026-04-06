using FluentValidation;
using ICMS.Application.DTOs.Maternal;
using ICMS.Domain.Enums;

namespace ICMS.Application.Validators.Maternal
{
    public class PregnantWomanCreateDtoValidator : AbstractValidator<PregnantWomanCreateDto>
    {
        public PregnantWomanCreateDtoValidator()
        {
            RuleFor(x => x.AgeRange)
                .NotEmpty().WithMessage("Age range is required.");

            RuleFor(x => x.BloodGroup)
                .IsInEnum().WithMessage("Invalid blood group.");

            RuleFor(x => x.RhFactor)
                .IsInEnum().WithMessage("Invalid Rh factor.");

            // Either PersonId must be provided OR PersonCreateDto must be provided.
            RuleFor(x => x)
                .Must(x => (x.PersonId.HasValue && x.PersonId.Value > 0) || x.PersonCreateDto != null)
                .WithMessage("Either an existing PersonId must be provided, or full Person details must be supplied to create a new profile.");

            When(x => x.PersonCreateDto != null && (!x.PersonId.HasValue || x.PersonId.Value <= 0), () =>
            {
                RuleFor(x => x.PersonCreateDto!.FirstName)
                    .NotEmpty().WithMessage("First name is required.");
                RuleFor(x => x.PersonCreateDto!.LastName)
                    .NotEmpty().WithMessage("Last name is required.");
                RuleFor(x => x.PersonCreateDto!.Gender)
                    .Must(g => g.Equals("Female", System.StringComparison.OrdinalIgnoreCase))
                    .WithMessage("Pregnant woman must be Female.");
            });
        }
    }
}
