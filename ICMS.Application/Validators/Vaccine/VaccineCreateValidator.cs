using FluentValidation;
using ICMS.Application.DTOs.Vaccine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMS.Application.Validators.Vaccine
{
    internal class VaccineCreateValidator : AbstractValidator<VaccineCreateDto>
    {
        public VaccineCreateValidator()
        {
            RuleFor(x => x.VaccineName)
                .NotEmpty()
                .WithMessage("The Vaccine Name is required");
        }
    }
}
