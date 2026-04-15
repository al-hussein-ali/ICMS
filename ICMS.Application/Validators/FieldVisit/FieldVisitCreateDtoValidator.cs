using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using ICMS.Application.DTOs.FieldVisit;

namespace ICMS.Application.Validators.FieldVisit
{
    public class FieldVisitCreateDtoValidator : AbstractValidator<FieldVisitCreateDto>
    {
        public FieldVisitCreateDtoValidator()
        {
            RuleFor(x => x.VisitDate)
                .NotEmpty().WithMessage("Visit date is required.")
                .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
                .WithMessage("Visit date cannot be in the past.");

            RuleFor(x => x.SubNeighborhoodId)
                .GreaterThan(0).WithMessage("A valid SubNeighborhood ID is required.");
        }
    }
}
