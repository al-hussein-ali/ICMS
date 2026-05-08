using ICMS.Domain.Entites.Geography;
using ICMS.Domain.Entites.Audit;
using ICMS.Domain.Entites.Visits;
using ICMS.Domain.Entites.Maternal;
using ICMS.Domain.Entites.Clinical;
using ICMS.Domain.Entites.Identity;
using ICMS.Domain.Entites.Common;
using System;
using ICMS.Domain.Enums;

namespace ICMS.Domain.Entites.Clinical
{
    public class VaccinationSchedule : BaseEntity<int>
    {
        public int VaccinatedIndividualId { get; private set; }
        public int DoseId { get; private set; }
        public DateOnly ScheduledDate { get; private set; }
        public DateOnly? ActualDate { get; private set; }
        public ScheduleStatus Status { get; private set; }
        public Guid? ImmunizationRecordId { get; private set; }

        public VaccinatedIndividual VaccinatedIndividual { get; private set; } = null!;
        public Dose Dose { get; private set; } = null!;
        public ImmunizationRecord? ImmunizationRecord { get; private set; }

        private VaccinationSchedule()
        {
        }

        public static VaccinationSchedule Create(int individualId, int doseId, DateOnly scheduledDate)
        {
            return new VaccinationSchedule
            {
                VaccinatedIndividualId = individualId,
                DoseId = doseId,
                ScheduledDate = scheduledDate,
                Status = ScheduleStatus.Pending
            };
        }

        public void MarkAsCompleted(DateOnly actualDate, ImmunizationRecord record)
        {
            ActualDate = actualDate;
            Status = ScheduleStatus.Completed;
            ImmunizationRecord = record ?? throw new ArgumentNullException(nameof(record));
            ImmunizationRecordId = record.Id;
        }

        public void Reschedule(DateOnly newScheduledDate)
        {
            ScheduledDate = newScheduledDate;
            Status = ScheduleStatus.Pending;
        }

        public void MarkAsMissed()
        {
            Status = ScheduleStatus.Missed;
        }
    }
}
