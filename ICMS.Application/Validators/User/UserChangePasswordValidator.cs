using FluentValidation;
using ICMS.Application.DTOs.User;

namespace ICMS.Application.Validators.User
{
    public class UserChangePasswordValidator : AbstractValidator<UserChangePasswordDto>
    {
        public UserChangePasswordValidator()
        {
            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .WithMessage("New password is required.")
                .MaximumLength(256)
                .WithMessage("Password must be at most 256 characters.");

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.NewPassword)
                .WithMessage("The new password and confirmation password do not match.");
        }
    }
}
