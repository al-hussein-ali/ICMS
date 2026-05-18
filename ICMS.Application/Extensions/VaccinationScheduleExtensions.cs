using ICMS.Application.DTOs.Schedules;
using ICMS.Domain.Entites.Clinical;
using System.Linq;

namespace ICMS.Application.Extensions
{
    public static class VaccinationScheduleExtensions
    {
        public static ScheduleReadDto ToReadDto(this VaccinationSchedule schedule)
        {
            return new ScheduleReadDto(
                ScheduleId: schedule.Id,
                VaccinatedIndividualId: schedule.VaccinatedIndividualId,
                DoseId: schedule.DoseId,
                VaccineName: LocalizationHelper.GetLocalizedValue(schedule.Dose?.Vaccine?.VaccineName) ?? string.Empty,
                DoseName: LocalizationHelper.GetLocalizedValue(schedule.Dose?.DoseName) ?? string.Empty,
                ScheduledDate: schedule.ScheduledDate,
                ActualDate: schedule.ActualDate,
                Status: schedule.Status,
                ImmunizationRecordId: schedule.ImmunizationRecordId
            );
        }
    }
}
