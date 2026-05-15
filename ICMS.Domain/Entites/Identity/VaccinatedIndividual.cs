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
        public DateOnly RegistrationDate { get; private set; }
        public User? User { get; private set; }
        public Person Person { get; private set; } = null!;
        public bool IsDeleted { get; private set; }
        public Directorate Directorate { get; private set; } = null!;
        public Neighborhood Neighborhood { get; private set; } = null!;
        public SubNeighborhood? SubNeighborhood { get; private set; }


        private VaccinatedIndividual()
        {
        }


        public static VaccinatedIndividual Create(int directorateId, int neighborhoodId, int? subNeighborhoodId = null,
            int? userId = null, DateOnly? registrationDate = null)
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
                UserId = userId,
                RegistrationDate = registrationDate ?? DateOnly.FromDateTime(DateTime.UtcNow),
                IsDeleted = false
            };
        }

        public void MarkAsDeleted()
        {
            if (IsDeleted) return;
            IsDeleted = true;
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

        public void ScheduleInitialVaccines(IEnumerable<Dose> requiredDoses, DateOnly dateOfBirth, bool isPregnant = false)
        {
            if (dateOfBirth > DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)))
                throw new DomainException("Date of birth cannot be in the future.");

            // Calculate current age in years for TT eligibility
            int ageInYears = DateTime.Today.Year - dateOfBirth.Year;
            if (dateOfBirth > DateOnly.FromDateTime(DateTime.Today.AddYears(-ageInYears))) ageInYears--;

            var eligibleDoses = requiredDoses.Where(d =>
            {
                if (d.Vaccine == null) return true;

                // Tetanus Toxoid (TT) Business Rule: 
                // Available for all females of reproductive age (15-49), regardless of pregnancy status.
                if (d.Vaccine.VaccineCode == "TT")
                {
                    return Person.Gender == Gender.Female && ageInYears >= 15 && ageInYears <= 49;
                }

                // Standard Pregnancy vaccines
                if (d.Vaccine.Audience == Enums.TargetAudience.Pregnancy) return isPregnant;

                // Routine infant/childhood vaccines or General adult vaccines
                return true;
            }).ToList();

            var eligibleDoseIds = eligibleDoses.Select(d => d.Id).ToHashSet();

            // Clean up any existing schedules that the individual is no longer eligible for
            _schedules.RemoveAll(s => !eligibleDoseIds.Contains(s.DoseId) && s.Status != ScheduleStatus.Completed);

            foreach (var dose in eligibleDoses.OrderBy(d => d.RecommendedAgeInWeeks).ThenBy(d => d.DoseOrder))
            {
                if (_schedules.Any(s => s.DoseId == dose.Id)) continue;

                // Business Change: Scheduling now starts from RegistrationDate instead of DateOfBirth
                var scheduledDate = RegistrationDate.AddDays(dose.RecommendedAgeInWeeks * 7);
                
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
            int userId, Dose? nextSequenceDose = null, int? fieldVisitId = null, string? notes = null, int? batchId = null, bool isAdvancedDose = false, IEnumerable<Dose>? allDoses = null)
        {
            if (currentDose == null || currentDose.Id <= 0)
                throw new DomainException("Invalid Dose!");

            if (Person == null)
                throw new DomainException("PersonRequired");

            // 1. Gender check for specific vaccines
            if (currentDose.Vaccine?.VaccineCode == "TT" && Person.Gender != Gender.Female)
            {
                throw new DomainException("TetanusOnlyForFemales");
            }

            // 2. Calculate biological age and weeks since registration
            int biologicalAgeInWeeks = (int)((administrationDate.ToDateTime(TimeOnly.MinValue) - Person.DateOfBirth.ToDateTime(TimeOnly.MinValue)).TotalDays / 7);
            int weeksSinceRegistration = (int)((administrationDate.ToDateTime(TimeOnly.MinValue) - RegistrationDate.ToDateTime(TimeOnly.MinValue)).TotalDays / 7);

            // Rule: Biological age must be within vaccine's allowed range (Safety Rule)
            if (currentDose.Vaccine != null)
            {
                int minAgeInWeeks = currentDose.Vaccine.MinEligibleAgeInMonths * 4; 
                if (biologicalAgeInWeeks < minAgeInWeeks)
                    throw new DomainException("TooYoungForVaccine", biologicalAgeInWeeks, minAgeInWeeks, currentDose.Vaccine.VaccineName);
            }

            // Rule: Progress check relative to registration timeline (Scheduling Rule)
            if (!isAdvancedDose && weeksSinceRegistration < currentDose.RecommendedAgeInWeeks)
            {
                throw new DomainException("TooYoung", weeksSinceRegistration, currentDose.RecommendedAgeInWeeks, currentDose.DoseName);
            }

            // Rule: Actual date cannot be before scheduled date
            var scheduleToComplete = _schedules.FirstOrDefault(s => s.DoseId == currentDose.Id && s.Status != ScheduleStatus.Completed);
            if (!isAdvancedDose && scheduleToComplete != null && administrationDate < scheduleToComplete.ScheduledDate)
            {
                throw new DomainException("TooEarly", administrationDate, scheduleToComplete.ScheduledDate, currentDose.DoseName);
            }

            if (administrationDate > DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)))
                throw new DomainException("FutureDateNotAllowed");

            if (string.IsNullOrEmpty(takenIn))
                throw new DomainException("RequiredField", nameof(takenIn));

            // Invariant: Cannot take the same dose twice
            if (_immunizationRecords.Any(ir => ir.DoseId == currentDose.Id))
                throw new InvalidDoubleDoseException("DoubleDose", currentDose.DoseName);

            // 1. Record the immunization
            var newRecord = ImmunizationRecord.Create(this.Id, currentDose.Id, administrationDate, takenIn, userId,
                fieldVisitId, notes, batchId);
            _immunizationRecords.Add(newRecord);

            // 2. Complete the corresponding schedule (find it whether it's Pending or Missed)
            if (scheduleToComplete != null)
            {
                scheduleToComplete.MarkAsCompleted(administrationDate, newRecord);
            }

            // 3. Handle late doses and cascading schedule updates
            // Business Rule: If a Primary dose is taken late, shift ALL subsequent Primary doses across ALL vaccines.
            if (currentDose.IsPrimary && scheduleToComplete != null && administrationDate > scheduleToComplete.ScheduledDate)
            {
                var delayDays = (administrationDate.ToDateTime(TimeOnly.MinValue) - scheduleToComplete.ScheduledDate.ToDateTime(TimeOnly.MinValue)).Days;
                if (delayDays > 0)
                {
                    // Find all subsequent schedules that are not completed and were scheduled after this one
                    var subsequentSchedules = _schedules
                        .Where(s => s.Status != ScheduleStatus.Completed && s.ScheduledDate > scheduleToComplete.ScheduledDate)
                        .ToList();

                    foreach (var schedule in subsequentSchedules)
                    {
                        var doseInfo = schedule.Dose ?? allDoses?.FirstOrDefault(d => d.Id == schedule.DoseId);
                        if (doseInfo != null && doseInfo.IsPrimary)
                        {
                            schedule.Reschedule(schedule.ScheduledDate.AddDays(delayDays));
                        }
                    }
                }
            }

            // Fallback: If the next dose doesn't have a schedule yet, create it (legacy support)
            if (nextSequenceDose != null && !_schedules.Any(s => s.DoseId == nextSequenceDose.Id))
            {
                int recommendedIntervalWeeks = nextSequenceDose.RecommendedAgeInWeeks - currentDose.RecommendedAgeInWeeks;
                if (recommendedIntervalWeeks < 0) recommendedIntervalWeeks = 0;
                var minNextDate = administrationDate.AddDays(recommendedIntervalWeeks * 7);
                _schedules.Add(VaccinationSchedule.Create(this.Id, nextSequenceDose.Id, minNextDate));
            }
        }

    }
}
