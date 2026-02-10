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
    internal class VaccinatedIndividualValidator : AbstractValidator<VaccinatedIndividualCreateDto>
    {
        public VaccinatedIndividualValidator(IValidator<PersonCreateDto> personCreateValidator)
        {


            RuleFor(x => x.CardNumber)
                .NotEmpty()
                .WithMessage("The Card Number is required");



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
