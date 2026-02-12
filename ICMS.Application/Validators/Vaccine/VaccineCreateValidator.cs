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


            RuleFor(x => x.VaccineCode)
                .NotEmpty()
                .WithMessage("The Vaccine Code is required");


            RuleFor(x => x.TotalDosages)
                .NotNull()
                .WithMessage("This Total Dosages is required");


            RuleFor(x => x.IsActive)
                .NotNull()
                .WithMessage("The Active Field is required");

        }
    }
}
