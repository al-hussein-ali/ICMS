using FluentValidation;
using ICMS.Application.DTOs.VaccinatedIndividual;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMS.Application.Validators.VaccinatedIndividual
{
    internal class NewFieldVaccinatedIndividualCreateValidator : AbstractValidator<NewFieldVaccinatedIndividualDto>
    {
        public NewFieldVaccinatedIndividualCreateValidator()
        {


            RuleFor(x => x.Area)
                .NotEmpty()
                .WithMessage("The Area is required")
                .MaximumLength(100)
                .WithMessage("The Area must be at most 100 characters.");

            RuleFor(x => x.Neighborhood)
                .NotEmpty()
                .WithMessage("The Neighborhood is required")
                .MaximumLength(100)
                .WithMessage("The Neighborhood must be at most 100 characters.");

            RuleFor(x => x.Directorate)
                .NotEmpty()
                .WithMessage("The Directorate is required")
                .MaximumLength(60)
                .WithMessage("The Directorate must be at most 100 characters.");


            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("The First Name is required")
                .MaximumLength(20)
                .WithMessage("First Name must be at most 20 characters.");

            RuleFor(x => x.SecondName)
                .NotEmpty()
                .WithMessage("The Second Name is required")
                .MaximumLength(20)
                .WithMessage("Second Name must be at most 20 characters.");

            When(x => x.ThirdName != null, () =>
            {
                RuleFor(x => x.ThirdName)
                    .MaximumLength(20)
                    .WithMessage("Third Name must be at most 20 characters.");
            });

            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage("The Last Name is required")
                .MaximumLength(20)
                .WithMessage("Last Name must be at most 20 characters.");

            RuleFor(x => x.DateOfBirth)
                .NotNull()
                .WithMessage("The Date of Birth is required");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .WithMessage("The Phone Number is required");

            RuleFor(x => x.Gender)
                .NotEmpty()
                .WithMessage("The Gender value is required.");


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
                    .WithMessage("Note must be at most 500 characters.");
            });
        }
    }
}
