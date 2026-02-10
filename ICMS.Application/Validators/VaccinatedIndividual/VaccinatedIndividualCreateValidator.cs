using FluentValidation;
using ICMS.Application.DTOs.Person;
using ICMS.Application.DTOs.VaccinatedIndividual;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMS.Application.Validators.VaccinatedIndividual
{
    internal class VaccinatedIndividualCreateValidator : AbstractValidator<VaccinatedIndividualCreateDto>
    {
        public VaccinatedIndividualCreateValidator(IValidator<PersonCreateDto> personCreateValidator)
        {

            RuleFor(x => x.CardNumber)
                .NotEmpty()
                .WithMessage("The Card Number is required")
                .Length(1, 100).WithMessage("The Card Number should be at least 1 and at most 100 length");

            RuleFor(x => x.Area)
                .NotEmpty()
                .WithMessage("The Area is required");

            RuleFor(x => x.Neighborhood)
                .NotEmpty()
                .WithMessage("The Neighborhood is required");


            RuleFor(x => x.Directorate)
                .NotEmpty()
                .WithMessage("The Neighborhood is required");


            RuleFor(x => x.PersonCreateDto)
                .NotNull()
                .SetValidator(personCreateValidator);

        }
    }
}
