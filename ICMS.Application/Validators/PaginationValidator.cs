using FluentValidation;
using ICMS.Application.DTOs.Pagination;
using ICMS.Application.DTOs.Person;
using ICMS.Domain.Entites;

namespace ICMS.Application.Validators
{
    public sealed class PaginationValidator : AbstractValidator<PaginationParams>
    {
        public PaginationValidator()
        {
            RuleFor(x => x.PageNumber).NotEqual(0).WithMessage("The Page Number is required");

            RuleFor(x => x.PageSize).LessThanOrEqualTo(50).WithMessage("The Page size cannot be greater than 50");

            RuleFor(x => x.PageSize).GreaterThanOrEqualTo(1).WithMessage("The Page size cannot be less than 1");
           
        }
    }
}
