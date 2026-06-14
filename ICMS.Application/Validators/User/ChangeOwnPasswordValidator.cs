using ICMS.Application.Interfaces.Services;
using FluentValidation;
using ICMS.Application.DTOs.User;

namespace ICMS.Application.Validators.User
{
    public class ChangeOwnPasswordValidator : AbstractValidator<ChangeOwnPasswordDto>
    {
        public ChangeOwnPasswordValidator(ILocalizer localizer)
        {
            RuleFor(x => x.OldPassword)
                .NotEmpty()
                .WithMessage(x => localizer["RequiredField", "Old Password"])
                .MaximumLength(256)
                .WithMessage(x => localizer["InvalidField", "Old Password"]);

            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .WithMessage(x => localizer["RequiredField", "New Password"])
                .MinimumLength(6)
                .WithMessage(x => localizer["PasswordTooShort", 6])
                .MaximumLength(256)
                .WithMessage(x => localizer["InvalidField", "New Password"]);

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.NewPassword)
                .WithMessage(x => localizer["ValidationError", "The new password and confirmation password do not match."]);
        }
    }
}
