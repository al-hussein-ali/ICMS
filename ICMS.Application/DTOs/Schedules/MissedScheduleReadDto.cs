using System;

namespace ICMS.Application.DTOs.Schedules
{
    public record MissedScheduleReadDto(
        int IndividualId,
        string FirstName,
        string LastName,
        string DoseName,
        DateOnly ScheduledDate,
        string? DirectorateName
    );
}
