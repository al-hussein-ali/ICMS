using ICMS.Application.Interfaces.Services;
using FluentValidation;
using ICMS.Application.DTOs.User;
using System.Linq;

namespace ICMS.Application.Validators.User
{
    public class UserUpdateValidator : AbstractValidator<UserReadDto>
    {
        public UserUpdateValidator(ILocalizer localizer)
        {
            RuleFor(x => x.Id)
                .GreaterThanOrEqualTo(1)
                .WithMessage(x => localizer["RequiredField", "This field"]);

            RuleFor(x => x.UserName)
                .NotEmpty()
                .WithMessage(x => localizer["RequiredField", "This field"])
                .MaximumLength(50)
                .WithMessage(x => localizer["InvalidField", "This field"]);

            RuleFor(x => x.Roles)
                .Must(roles => roles == null || roles.All(r => !string.IsNullOrWhiteSpace(r)))
                .WithMessage(x => localizer["ValidationError", "Role names cannot be empty."]);
        }
    }
}

