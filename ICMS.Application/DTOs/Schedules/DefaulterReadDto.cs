using System;

namespace ICMS.Application.DTOs.Schedules
{
    public record DefaulterReadDto(
        int ScheduleId,
        int VaccinatedIndividualId,
        string FirstName,
        string LastName,
        string PhoneNumber,
        string VaccineName,
        string DoseName,
        DateOnly ScheduledDate,
        int DaysLate
    );
}
