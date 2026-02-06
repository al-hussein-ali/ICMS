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

            RuleFor(x => x.FirstName).Must(IsNotEmptyOrNull).WithMessage("First Name is required");
            RuleFor(x => x.SecondName).Must(IsNotEmptyOrNull).WithMessage("Second Name is required");
            RuleFor(x => x.LastName).Must(IsNotEmptyOrNull).WithMessage("Last Name is required");

            RuleFor(x => x.DateOfBirth).NotNull().WithMessage("Date of Birth is required");

            RuleFor(x => x.PhoneNumber).Must(IsNotEmptyOrNull).WithMessage("Phone Number is required");

            RuleFor(x => x.Gender).NotEmpty().WithMessage("Gender is required");
        }



        private bool IsNotEmptyOrNull(string proprty) => 
            !string.IsNullOrEmpty(proprty) 
            && !string.IsNullOrWhiteSpace(proprty);
    }
}
