using System;

namespace ICMS.Application.DTOs.ImmunizationRecord
{
    public record ImmunizationRecordDetailsDto(Guid Id, int IndividualId, int DoseId, int? FieldVisitId, DateOnly VaccinationDate, string TakenIn, string? Notes);
}
