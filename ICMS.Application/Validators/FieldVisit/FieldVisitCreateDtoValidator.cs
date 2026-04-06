using System;
using System.Collections.Generic;
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

            RuleFor(x => x.TargetedLocation)
                .NotEmpty().WithMessage("Targeted location is required.")
                .MaximumLength(200).WithMessage("Targeted location must be at most 200 characters.");

            RuleFor(x => x.FieldWorkerUserIds)
                .Must(ids => ids == null || ids.Count > 0)
                .WithMessage("At least one field worker must be assigned if providing a list.")
                .Must(ids => ids == null || !ids.Any(id => id <= 0))
                .WithMessage("Invalid User Id found in field workers list.");
        }
    }
}
