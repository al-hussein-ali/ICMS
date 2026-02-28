using FluentValidation;
using ICMS.Application.DTOs.ImmunizationRecord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMS.Application.Validators.ImmunizationRecord
{
    public class ImmunizationCreateValidator : AbstractValidator<ImmunizationRecordCreateDto>
    {
        public ImmunizationCreateValidator()
        {
            RuleFor(x => x.DoseId)
                .NotNull()
                .WithMessage("The Dose Id is required.")
                .GreaterThanOrEqualTo(1);


            RuleFor(x => x.TakenIn)
                .NotEmpty()
                .WithMessage("The Taken in is required.")
                .Length(1, 100);



            RuleFor(x => x.IndividualId)
                .NotNull()
                .WithMessage("The Individual Id is required.")
                .GreaterThanOrEqualTo(1);
        }
    }
}
