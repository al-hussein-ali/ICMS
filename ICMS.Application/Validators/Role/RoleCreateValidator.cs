using FluentValidation;
using ICMS.Application.DTOs.Role;

namespace ICMS.Application.Validators.Role
{
    public class RoleCreateValidator : AbstractValidator<RoleCreateDto>
    {
        public RoleCreateValidator()
        {
            RuleFor(x => x.RoleName)
                .NotEmpty()
                .WithMessage("Role name is required.")
                .MaximumLength(50)
                .WithMessage("Role name must be at most 50 characters.");
        }
    }
}
