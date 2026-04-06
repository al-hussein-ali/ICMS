using System;

namespace ICMS.Application.DTOs.Schedules
{
    public record AdministerDoseDto(
        int IndividualId,
        int DoseId,
        DateOnly Date,
        string TakenIn,
        string Notes,
        int? FieldVisitId = null
    );
}
