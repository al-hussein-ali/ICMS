using ICMS.Application.Interfaces.Services;
using FluentValidation;
using ICMS.Application.DTOs.User;
using ICMS.Application.DTOs.Person;
using System.Linq;

namespace ICMS.Application.Validators.User
{
    public class UserCreateValidator : AbstractValidator<UserCreateDto>
    {
        public UserCreateValidator(IValidator<PersonCreateDto> personCreateValidator, ILocalizer localizer)
        {
            RuleFor(x => x.UserName)
                .NotEmpty()
                .WithMessage(x => localizer["RequiredField", "This field"])
                .MaximumLength(50)
                .WithMessage(x => localizer["InvalidField", "This field"]);

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage(x => localizer["RequiredField", "This field"])
                .MinimumLength(6)
                .WithMessage(x => localizer["PasswordTooShort", 6])
                .MaximumLength(256)
                .WithMessage(x => localizer["InvalidField", "This field"]);

            RuleFor(x => x)
                .Must(x => x.PersonId > 0 || x.PersonCreateDto != null)
                .WithMessage(x => localizer["RequiredField", "PersonId or Person details"]);

            When(x => x.PersonCreateDto != null && (!x.PersonId.HasValue || x.PersonId.Value <= 0), () =>
            {
                RuleFor(x => x.PersonCreateDto)
                    .SetValidator(personCreateValidator!);
            });

            RuleFor(x => x.Roles)
                .Must(roles => roles == null || roles.All(r => !string.IsNullOrWhiteSpace(r)))
                .WithMessage(x => localizer["ValidationError", "Role names cannot be empty."]);
        }
    }
}

