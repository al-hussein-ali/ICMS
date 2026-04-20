using ICMS.Application.Interfaces.Services;
using FluentValidation;
using ICMS.Application.DTOs.User;

namespace ICMS.Application.Validators.User
{
    public class UserChangePasswordValidator : AbstractValidator<UserChangePasswordDto>
    {
        public UserChangePasswordValidator(ILocalizer localizer)
        {
            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .WithMessage(x => localizer["RequiredField", "This field"])
                .MaximumLength(256)
                .WithMessage(x => localizer["InvalidField", "This field"]);

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.NewPassword)
                .WithMessage(x => localizer["ValidationError", "The new password and confirmation password do not match."]);
        }
    }
}

