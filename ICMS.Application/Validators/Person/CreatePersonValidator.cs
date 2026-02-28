using FluentValidation;
using ICMS.Application.DTOs.Person;
using ICMS.Domain.Enums;

namespace ICMS.Application.Validators.Person
{
    public sealed class CreatePersonValidator : AbstractValidator<PersonCreateDto>
    {
        public CreatePersonValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("The First Name is required")
                .MaximumLength(20)
                .WithMessage("First Name must be at most 20 characters.");

            RuleFor(x => x.SecondName)
                .NotEmpty()
                .WithMessage("The Second Name is required")
                .MaximumLength(20)
                .WithMessage("Second Name must be at most 20 characters.");

            When(x => x.ThirdName != null, () =>
            {
                RuleFor(x => x.ThirdName)
                    .MaximumLength(20)
                    .WithMessage("Third Name must be at most 20 characters.");
            });

            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage("The Last Name is required")
                .MaximumLength(20)
                .WithMessage("Last Name must be at most 20 characters.");

            RuleFor(x => x.DateOfBirth)
                .NotNull()
                .WithMessage("The Date of Birth is required");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .WithMessage("The Phone Number is required");

            RuleFor(x => x.Gender)
                .NotEmpty()
                .WithMessage("The Gender value is required.");
        }
    }
}
