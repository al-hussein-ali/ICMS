using ICMS.Application.Interfaces.Services;
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
        public UpdateFieldVisitIndividualValidator(ILocalizer localizer)
        {

            RuleFor(x => x.IndividualId)
                .GreaterThanOrEqualTo(1)
                .WithMessage(x => localizer["RequiredField", "This field"]);


            RuleFor(x => x.DoseId)
                .GreaterThanOrEqualTo(1)
                .WithMessage(x => localizer["RequiredField", "This field"]);


            RuleFor(x => x.FieldVisitId)
                .GreaterThanOrEqualTo(1)
                .WithMessage(x => localizer["InvalidField", "This field"]);


            RuleFor(x => x.VaccinationDate)
                .Must(d => d != default)
                .NotEmpty()
                .WithMessage(x => localizer["RequiredField", "This field"]);

            RuleFor(x => x.TakenIn)
                .NotEmpty()
                .WithMessage(x => localizer["RequiredField", "This field"])
                .MaximumLength(200)
                .WithMessage(x => localizer["InvalidField", "This field"]);

            When(x => x.Note != null, () =>
            {
                RuleFor(x => x.Note)
                    .MaximumLength(500)
                    .WithMessage(x => localizer["InvalidField", "This field"]);
            });
        }
    }
}

