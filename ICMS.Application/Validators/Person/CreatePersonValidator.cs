using ICMS.Application.Interfaces.Services;
using FluentValidation;
using ICMS.Application.DTOs.Person;
using ICMS.Domain.Enums;

namespace ICMS.Application.Validators.Person
{
    public sealed class CreatePersonValidator : AbstractValidator<PersonCreateDto>
    {
        public CreatePersonValidator(ILocalizer localizer)
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage(x => localizer["RequiredField", "This field"])
                .MaximumLength(20)
                .WithMessage(x => localizer["InvalidField", "This field"]);

            RuleFor(x => x.SecondName)
                .NotEmpty()
                .WithMessage(x => localizer["RequiredField", "This field"])
                .MaximumLength(20)
                .WithMessage(x => localizer["InvalidField", "This field"]);

            When(x => x.ThirdName != null, () =>
            {
                RuleFor(x => x.ThirdName)
                    .MaximumLength(20)
                    .WithMessage(x => localizer["InvalidField", "This field"]);
            });

            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage(x => localizer["RequiredField", "This field"])
                .MaximumLength(20)
                .WithMessage(x => localizer["InvalidField", "This field"]);

            RuleFor(x => x.DateOfBirth)
                .NotNull()
                .WithMessage(x => localizer["RequiredField", "This field"])
                .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
                .WithMessage(x => localizer["FutureDateNotAllowed"]);

            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .WithMessage(x => localizer["RequiredField", "This field"])
                .Matches(@"^\d+$")
                .WithMessage(x => localizer["InvalidField", "This field"]);

            RuleFor(x => x.Gender)
                .NotEmpty()
                .WithMessage(x => localizer["RequiredField", "This field"]);
        }
    }
}

