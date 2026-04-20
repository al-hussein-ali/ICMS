using ICMS.Application.Interfaces.Services;
using FluentValidation;
using ICMS.Application.DTOs.Role;

namespace ICMS.Application.Validators.Role
{
    public class RoleCreateValidator : AbstractValidator<RoleCreateDto>
    {
        public RoleCreateValidator(ILocalizer localizer)
        {
            RuleFor(x => x.RoleName)
                .NotEmpty()
                .WithMessage(x => localizer["RequiredField", "This field"])
                .MaximumLength(50)
                .WithMessage(x => localizer["InvalidField", "This field"]);
        }
    }
}

