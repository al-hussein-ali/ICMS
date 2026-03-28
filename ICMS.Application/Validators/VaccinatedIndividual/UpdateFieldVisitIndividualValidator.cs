using FluentValidation;
using ICMS.Application.DTOs.VaccinatedIndividual;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMS.Application.Validators.VaccinatedIndividual
{
    internal class UpdateFieldVisitIndividualValidator : AbstractValidator<UpdateFieldVisitIndividualDto>
    {
        public UpdateFieldVisitIndividualValidator()
        {

            RuleFor(x => x.IndividualId)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Individual Id is required.");


            RuleFor(x => x.DoseId)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Dose Id is required.");


            RuleFor(x => x.FieldVisitId)
                .GreaterThanOrEqualTo(1)
                .WithMessage("FieldVisitId must be greater than 0 when provided.");


            RuleFor(x => x.VaccinationDate)
                .Must(d => d != default)
                .NotEmpty()
                .WithMessage("Vaccination date is required.");

            RuleFor(x => x.TakenIn)
                .NotEmpty()
                .WithMessage("Taken In is required.")
                .MaximumLength(200)
                .WithMessage("TakenIn must be at most 200 characters.");

            When(x => x.Note != null, () =>
            {
                RuleFor(x => x.Note)
                    .MaximumLength(500)
                    .WithMessage("Notes must be at most 500 characters.");
            });
        }
    }
}
