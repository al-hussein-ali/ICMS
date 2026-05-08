using ICMS.Domain.Enums;
using System;

namespace ICMS.Application.DTOs.Schedules
{
    public record ScheduleReadDto(
        int ScheduleId,
        int VaccinatedIndividualId,
        int DoseId,
        string VaccineName,
        string DoseName,
        DateOnly ScheduledDate,
        DateOnly? ActualDate,
        ScheduleStatus Status,
        Guid? ImmunizationRecordId
    );
}
