using ICMS.Domain.Entites.Geography;
using ICMS.Domain.Entites.Clinical;
using ICMS.Domain.Entites.Common;
using ICMS.Domain.Exceptions;
using System.ComponentModel.DataAnnotations.Schema;

namespace ICMS.Domain.Entites.Visits
{
    public class FieldVisit : BaseEntity<int>
    {
        private readonly List<ImmunizationRecord> _immunizationRecords = new();
        private readonly List<FieldVisitIndividual> _fieldVisitIndividuals = new();
        private readonly List<FieldVisitWorker> _fieldVisitWorkers = new();

        public IReadOnlyList<ImmunizationRecord> ImmunizationRecords => _immunizationRecords.AsReadOnly();
        public IReadOnlyList<FieldVisitIndividual> FieldVisitIndividuals => _fieldVisitIndividuals.AsReadOnly();
        public IReadOnlyList<FieldVisitWorker> FieldVisitWorkers => _fieldVisitWorkers.AsReadOnly();

        public string CampaignName { get; private set; } = string.Empty;
        public DateOnly VisitDate { get; private set; }
        public int SubNeighborhoodId { get; private set; }
        public SubNeighborhood SubNeighborhood { get; private set; } = null!;
        public DateOnly FromDate { get; private set; }
        public DateOnly ToDate { get; private set; }
        public bool IsCompleted { get; private set; }
        [NotMapped]
        public int TargetedCount { get; set; } // Added to carry the count from repository


        private FieldVisit()
        {
        }

        public static FieldVisit Create(
            string campaignName,
            DateOnly visitDate,
            int subNeighborhoodId,
            DateOnly fromDate,
            DateOnly toDate)
        {
            if (string.IsNullOrWhiteSpace(campaignName)) throw new DomainException("Campaign name is required");
            if (subNeighborhoodId <= 0) throw new DomainException("Valid SubNeighborhoodId is required");

            return new FieldVisit
            {
                CampaignName = campaignName,
                VisitDate = visitDate,
                SubNeighborhoodId = subNeighborhoodId,
                FromDate = fromDate,
                ToDate = toDate,
                IsCompleted = false
            };
        }

        public void AddImmunizationRecord(ImmunizationRecord ir)
        {
            if (ir == null) throw new DomainException("Immunization record is required");
            if (_immunizationRecords.Any(x => x.Id == ir.Id))
                throw new DomainException("Immunization record already added");

            _immunizationRecords.Add(ir);
        }

        public void MarkCompleted()
        {
            IsCompleted = true;
        }

        public void ClearIndividuals() => _fieldVisitIndividuals.Clear();

        public void AddIndividual(int vaccinatedIndividualId)
        {
            if (_fieldVisitIndividuals.Any(x => x.VaccinatedIndividualId == vaccinatedIndividualId))
                return;
            _fieldVisitIndividuals.Add(FieldVisitIndividual.Create(this.Id, vaccinatedIndividualId));
        }

        public void ClearWorkers() => _fieldVisitWorkers.Clear();

        public void AddWorker(int userId)
        {
            if (_fieldVisitWorkers.Any(x => x.UserId == userId))
                return;
            _fieldVisitWorkers.Add(FieldVisitWorker.Create(this.Id, userId));
        }

        public void UpdateVisitInfo(
            string campaignName,
            DateOnly visitDate,
            int subNeighborhoodId,
            DateOnly fromDate,
            DateOnly toDate)
        {
            if (string.IsNullOrWhiteSpace(campaignName)) throw new DomainException("Campaign name is required");
            if (subNeighborhoodId <= 0) throw new DomainException("Valid SubNeighborhoodId is required");

            CampaignName = campaignName;
            VisitDate = visitDate;
            SubNeighborhoodId = subNeighborhoodId;
            FromDate = fromDate;
            ToDate = toDate;
        }
    }
}
