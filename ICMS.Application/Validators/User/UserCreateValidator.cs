using FluentValidation;
using ICMS.Application.DTOs.User;
using System.Linq;

namespace ICMS.Application.Validators.User
{
    public class UserCreateValidator : AbstractValidator<UserCreateDto>
    {
        public UserCreateValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty()
                .WithMessage("User name is required.")
                .MaximumLength(50)
                .WithMessage("User name must be at most 50 characters.");

            RuleFor(x => x.PasswordHash)
                .NotEmpty()
                .WithMessage("Password hash is required.")
                .MaximumLength(256)
                .WithMessage("Password hash must be at most 256 characters.");

            RuleFor(x => x.PersonId)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Person Id is required.");

            RuleFor(x => x.Roles)
                .Must(roles => roles == null || roles.All(r => !string.IsNullOrWhiteSpace(r)))
                .WithMessage("Role names cannot be empty.");
        }
    }
}
