using ICMS.Domain.Entites.Geography;
using ICMS.Domain.Entites.Audit;
using ICMS.Domain.Entites.Visits;
using ICMS.Domain.Entites.Maternal;
using ICMS.Domain.Entites.Clinical;
using ICMS.Domain.Entites.Identity;
using ICMS.Domain.Entites.Common;
using ICMS.Domain.Exceptions;

namespace ICMS.Domain.Entites.Clinical
{
    public class ImmunizationRecord : BaseEntity<Guid>
    {
        public int IndividualId { get; private set; }
        public int DoseId { get; private set; }
        public int? FieldVisitId { get; private set; }
        public DateOnly VaccinationDate { get; private set; }
        public string TakenIn { get; private set; } = string.Empty;
        public string? Notes { get; private set; }
        public VaccinatedIndividual? VaccinatedIndividual { get; private set; }
        public Dose Dose { get; private set; }

        public int? BatchId { get; private set; }
        public Batch? Batch { get; private set; }

        public FieldVisit? FieldVisit { get; private set; }


        public int UserId { get; private set; }
        public User? User { get; private set; }

        private ImmunizationRecord()
        {
        }

        public static ImmunizationRecord Create(int individualId, int doseId, DateOnly vaccinationDate, string takenIn, int userId, int? fieldVisitId = null, string? notes = null, int? batchId = null)
        {
            // Allowed to be 0 for new individuals being created in the same transaction
            if (doseId <= 0) throw new DomainException("Invalid dose id");
            if (userId <= 0) throw new DomainException("Invalid user id");
            if (string.IsNullOrWhiteSpace(takenIn)) throw new DomainException("TakenIn is required");

            return new ImmunizationRecord { IndividualId = individualId, DoseId = doseId, FieldVisitId = fieldVisitId, VaccinationDate = vaccinationDate, TakenIn = takenIn, UserId = userId, Notes = notes, BatchId = batchId };
        }

        public void UpdateRecordInfo(int individualId, int doseId, DateOnly vaccinationDate, string takenIn, int? fieldVisitId = null, string? notes = null, int? batchId = null)
        {
            this.IndividualId = individualId;
            this.VaccinationDate = vaccinationDate;
            this.DoseId = doseId;
            this.TakenIn = takenIn;
            this.FieldVisitId = fieldVisitId;
            this.Notes = notes;
            this.BatchId = batchId;
        }

        //public void AssignDose(Dose dose)
        //{
        //    if (dose == null) throw new DomainException("Dose is required");
        //    if (Dose != null) throw new DomainException("Dose already assigned");
        //    if (dose.Id != 0 && dose.Id != DoseId) throw new DomainException("Dose id mismatch");

        //    Dose = dose;
        //    DoseId = dose.Id;
        //}

        //public void AssignFieldVisit(FieldVisit fv)
        //{
        //    if (fv == null) throw new DomainException("FieldVisit is required");
        //    if (FieldVisit != null) throw new DomainException("FieldVisit already assigned");
        //    if (fv.Id != 0 && fv.Id != FieldVisitId) throw new DomainException("FieldVisit id mismatch");

        //    FieldVisit = fv;
        //    FieldVisitId = fv.Id;
        //}

    }
}
