using System;

namespace ICMS.Application.DTOs.ImmunizationRecord
{
    public record ImmunizationRecordCreateDto(int IndividualId, int DoseId, int? FieldVisitId, DateOnly VaccinationDate, string TakenIn, string? Notes, int? BatchId = null);
}
