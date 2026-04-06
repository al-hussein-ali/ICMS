using FluentValidation;
using ICMS.Application.DTOs.User;
using System.Linq;

namespace ICMS.Application.Validators.User
{
    public class UserUpdateValidator : AbstractValidator<UserReadDto>
    {
        public UserUpdateValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Valid User Id is required for updates.");

            RuleFor(x => x.UserName)
                .NotEmpty()
                .WithMessage("User name is required.")
                .MaximumLength(50)
                .WithMessage("User name must be at most 50 characters.");

            RuleFor(x => x.Roles)
                .Must(roles => roles == null || roles.All(r => !string.IsNullOrWhiteSpace(r)))
                .WithMessage("Role names cannot be empty.");
        }
    }
}
