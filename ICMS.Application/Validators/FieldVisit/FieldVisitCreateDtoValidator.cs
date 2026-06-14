using ICMS.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using ICMS.Application.DTOs.FieldVisit;

namespace ICMS.Application.Validators.FieldVisit
{
    public class FieldVisitCreateDtoValidator : AbstractValidator<FieldVisitCreateDto>
    {
        public FieldVisitCreateDtoValidator(ILocalizer localizer)
        {
            RuleFor(x => x.CampaignName)
                .NotEmpty().WithMessage(x => localizer["RequiredField", "Campaign name"]);

            RuleFor(x => x.VisitDate)
                .NotEmpty().WithMessage(x => localizer["RequiredField", "This field"])
                .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
                .WithMessage(x => localizer["ValidationError", "Visit date cannot be in the past."]);

            RuleFor(x => x.FromDate)
                .NotEmpty().WithMessage(x => localizer["RequiredField", "From date"]);

            RuleFor(x => x.ToDate)
                .NotEmpty().WithMessage(x => localizer["RequiredField", "To date"])
                .GreaterThanOrEqualTo(x => x.FromDate)
                .WithMessage(x => localizer["InvalidField", "To date"]);

            RuleFor(x => x.SubNeighborhoodId)
                .GreaterThan(0).WithMessage(x => localizer["RequiredField", "This field"]);
        }
    }
}

