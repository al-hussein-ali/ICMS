using ICMS.Application.Interfaces.Services;
using FluentValidation;
using ICMS.Application.DTOs.Pagination;

namespace ICMS.Application.Validators
{
    public sealed class PaginationValidator : AbstractValidator<PaginationParams>
    {
        public PaginationValidator(ILocalizer localizer)
        {
            RuleFor(x => x.PageNumber).NotEqual(0).WithMessage(x => localizer["RequiredField", "This field"]);

            RuleFor(x => x.PageSize).LessThanOrEqualTo(200).WithMessage(x =>
                localizer["ValidationError", "The Page size cannot be greater than 200"]);

            RuleFor(x => x.PageSize).GreaterThanOrEqualTo(1).WithMessage(x =>
                localizer["ValidationError", "The Page size cannot be less than 1"]);
        }
    }
}

