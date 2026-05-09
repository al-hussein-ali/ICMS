using System;

namespace ICMS.Application.DTOs.ImmunizationRecord
{
    public record ImmunizationRecordReadDto(Guid Id, int IndividualId, int DoseId, int? FieldVisitId, DateOnly VaccinationDate, string TakenIn, int? BatchId = null);
}
