using FluentValidation;
using ICMS.Application.DTOs.Person;
using ICMS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMS.Application.Validators.Person
{
    public sealed class CreatePersonValidator : AbstractValidator<PersonCreateDto>
    {
        public CreatePersonValidator()
        {

            RuleFor(x => x.FirstName).NotEmpty().WithMessage("The First Name is required");
            RuleFor(x => x.SecondName).NotEmpty().WithMessage("The Second Name is required");
            RuleFor(x => x.LastName).NotEmpty().WithMessage("The Last Name is required");

            RuleFor(x => x.DateOfBirth).NotNull().WithMessage("The Date of Birth is required");

            RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("The Phone Number is required");

            RuleFor(x => x.Gender).NotEmpty().WithMessage("The Gender is required");
        }



       
    }
}
