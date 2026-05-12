using ICMS.Domain.Entites.Clinical;
using ICMS.Domain.Entites.Common;
using ICMS.Domain.Entites.Geography;
using ICMS.Domain.Enums;
using ICMS.Domain.Exceptions;

namespace ICMS.Domain.Entites.Identity
{
    public class VaccinatedIndividual : BaseEntity<int>
    {
        private readonly List<ImmunizationRecord> _immunizationRecords = new();
        public IReadOnlyList<ImmunizationRecord> ImmunizationRecords => _immunizationRecords.AsReadOnly();

        private readonly List<VaccinationSchedule> _schedules = new();
        public IReadOnlyCollection<VaccinationSchedule> Schedules => _schedules.AsReadOnly();

        public string CardNumber { get; private set; } = string.Empty;

        public int DirectorateId { get; private set; }
        public int NeighborhoodId { get; private set; }
        public int? SubNeighborhoodId { get; private set; }
        public int? UserId { get; private set; }
        public int PersonId { get; private set; }
        public User? User { get; private set; }
        public Person Person { get; private set; } = null!;
        public Directorate Directorate { get; private set; } = null!;
        public Neighborhood Neighborhood { get; private set; } = null!;
        public SubNeighborhood? SubNeighborhood { get; private set; }


        private VaccinatedIndividual()
        {
        }


        public static VaccinatedIndividual Create(int directorateId, int neighborhoodId, int? subNeighborhoodId = null,
            int? userId = null)
        {
            if (directorateId <= 0 || neighborhoodId <= 0)
            {
                throw new DomainException("Valid Directorate and Neighborhood are required!");
            }

            return new VaccinatedIndividual
            {
                DirectorateId = directorateId,
                NeighborhoodId = neighborhoodId,
                SubNeighborhoodId = subNeighborhoodId,
                UserId = userId
            };
        }

        public void AssignPerson(Person person)
        {
            if (Person != null) throw new DomainException("Person already assigned");

            Person = person ?? throw new DomainException("Person is required");
        }

        public void AssignExistingPersonById(int personId)
        {
            if (personId == 0)
                throw new DomainException("Person Id cannot be zero");

            PersonId = personId;
        }


        public void AssignExistingUserById(int userId)
        {
            if (userId <= 0) throw new DomainException("Invalid user id");
            if (UserId.HasValue) throw new DomainException("This record is already linked to a user");

            UserId = userId;
        }

        public void UpdateIndividualInfo(int directorateId, int neighborhoodId, int? subNeighborhoodId)
        {
            DirectorateId = directorateId;
            NeighborhoodId = neighborhoodId;
            SubNeighborhoodId = subNeighborhoodId;
        }

        public void ScheduleInitialVaccines(IEnumerable<Dose> requiredDoses, DateOnly dateOfBirth)
        {
            if (dateOfBirth > DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)))
                throw new DomainException("Date of birth cannot be in the future.");

            foreach (var dose in requiredDoses.OrderBy(d => d.RecommendedAgeInMonths).ThenBy(d => d.DoseOrder))
            {
                if (_schedules.Any(s => s.DoseId == dose.Id)) continue;

                var scheduledDate = dateOfBirth.AddMonths(dose.RecommendedAgeInMonths);
                var schedule = VaccinationSchedule.Create(Id, dose.Id, scheduledDate);
                _schedules.Add(schedule);
            }
        }

        /// <summary>
        /// Administers a dose and automatically recalculates subsequent schedules in the sequence.
        /// Business Rule: If a dose is taken late, subsequent doses in the same sequence should be shifted 
        /// to maintain the recommended minimum interval.
        /// </summary>
        public void AdministerDose(Dose currentDose, DateOnly administrationDate, string takenIn,
            int userId, Dose? nextSequenceDose = null, int? fieldVisitId = null, string? notes = null, int? batchId = null, bool isAdvancedDose = false)
        {
            if (currentDose == null || currentDose.Id <= 0)
                throw new DomainException("Invalid Dose!");

            if (Person == null)
                throw new DomainException("PersonRequired");

            // Calculate age in months at administration date
            int ageInMonths = (administrationDate.Year - Person.DateOfBirth.Year) * 12 + administrationDate.Month - Person.DateOfBirth.Month;
            if (administrationDate.Day < Person.DateOfBirth.Day) ageInMonths--;

            // Rule: Age must be within vaccine's allowed range
            if (currentDose.Vaccine != null)
            {
                if (ageInMonths < currentDose.Vaccine.MinEligibleAgeInMonths)
                    throw new DomainException("TooYoungForVaccine", ageInMonths, currentDose.Vaccine.MinEligibleAgeInMonths, currentDose.Vaccine.VaccineName);
                
                // Business Logic Change: Removed hard limit for MaxEligibleAgeInMonths to allow catch-up vaccinations 
                // for individuals who missed their routine schedule but still need the dose.
                // if (ageInMonths > currentDose.Vaccine.MaxEligibleAgeInMonths)
                //     throw new DomainException("TooOldForVaccine", ageInMonths, currentDose.Vaccine.MaxEligibleAgeInMonths, currentDose.Vaccine.VaccineName);
            }

            // Rule: Age must be at least the dose's recommended age
            if (!isAdvancedDose && ageInMonths < currentDose.RecommendedAgeInMonths)
            {
                throw new DomainException("TooYoung", ageInMonths, currentDose.RecommendedAgeInMonths, currentDose.DoseName);
            }

            // Rule: Actual date cannot be before scheduled date
            var scheduleToComplete = _schedules.FirstOrDefault(s => s.DoseId == currentDose.Id && s.Status != ScheduleStatus.Completed);
            if (!isAdvancedDose && scheduleToComplete != null && administrationDate < scheduleToComplete.ScheduledDate)
            {
                throw new DomainException("TooEarly", administrationDate, scheduleToComplete.ScheduledDate, currentDose.DoseName);
            }

            // FIXME: Temporarily disabled due to persistent timezone/server clock discrepancies causing false positives.
            // if (administrationDate > DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)))
            //     throw new DomainException("Administration date cannot be in the future.");

            if (string.IsNullOrEmpty(takenIn))
                throw new DomainException("A required field is missing (takenIn)!");

            // Invariant: Cannot take the same dose twice
            if (_immunizationRecords.Any(ir => ir.DoseId == currentDose.Id))
                throw new InvalidDoubleDoseException($"Dose {currentDose.DoseName} has already been administered.");

            // 1. Record the immunization
            var newRecord = ImmunizationRecord.Create(this.Id, currentDose.Id, administrationDate, takenIn, userId,
                fieldVisitId, notes, batchId);
            _immunizationRecords.Add(newRecord);

            // 2. Complete the corresponding schedule (find it whether it's Pending or Missed)
            if (scheduleToComplete != null)
            {
                scheduleToComplete.MarkAsCompleted(administrationDate, newRecord);
            }

            // 3. Handle late doses and cascading schedule updates (Task 1: Reschedule ALL subsequent doses)
            var currentDoseOrder = currentDose.DoseOrder;
            var vaccineId = currentDose.VaccineId;

            // Find all subsequent schedules for the same vaccine that are not completed
            var subsequentSchedules = _schedules
                .Where(s => s.Dose.VaccineId == vaccineId && s.Dose.DoseOrder > currentDoseOrder && s.Status != ScheduleStatus.Completed)
                .OrderBy(s => s.Dose.DoseOrder)
                .ToList();

            var prevDose = currentDose;
            var prevReferenceDate = administrationDate;

            foreach (var schedule in subsequentSchedules)
            {
                int recommendedIntervalMonths = schedule.Dose.RecommendedAgeInMonths - prevDose.RecommendedAgeInMonths;
                if (recommendedIntervalMonths < 0) recommendedIntervalMonths = 0;

                var newScheduledDate = prevReferenceDate.AddMonths(recommendedIntervalMonths);

                // If the previous dose was taken late, or if we need to shift subsequent ones to maintain intervals
                if (newScheduledDate > schedule.ScheduledDate)
                {
                    schedule.Reschedule(newScheduledDate);
                }

                prevDose = schedule.Dose;
                prevReferenceDate = schedule.ScheduledDate; // Use the (potentially updated) scheduled date for the next interval calculation
            }

            // Fallback: If the next dose doesn't have a schedule yet, create it (legacy support)
            if (nextSequenceDose != null && !_schedules.Any(s => s.DoseId == nextSequenceDose.Id))
            {
                int recommendedIntervalMonths = nextSequenceDose.RecommendedAgeInMonths - currentDose.RecommendedAgeInMonths;
                if (recommendedIntervalMonths < 0) recommendedIntervalMonths = 0;
                var minNextDate = administrationDate.AddMonths(recommendedIntervalMonths);
                _schedules.Add(VaccinationSchedule.Create(this.Id, nextSequenceDose.Id, minNextDate));
            }
        }

    }
}
