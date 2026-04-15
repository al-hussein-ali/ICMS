using ICMS.Domain.Entites.Geography;
using ICMS.Domain.Entites.Audit;
using ICMS.Domain.Entites.Visits;
using ICMS.Domain.Entites.Maternal;
using ICMS.Domain.Entites.Clinical;
using ICMS.Domain.Entites.Identity;
using ICMS.Domain.Entites.Common;
using ICMS.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ICMS.Domain.Entites.Visits
{
    public class FieldVisit : BaseEntity<int>
    {
        private readonly List<ImmunizationRecord> _immunizationRecords = new();

        public IReadOnlyList<ImmunizationRecord> ImmunizationRecords => _immunizationRecords.AsReadOnly();
        public DateOnly VisitDate { get; private set; }
        public int SubNeighborhoodId { get; private set; }
        public SubNeighborhood SubNeighborhood { get; private set; } = null!;
        public bool IsCompleted { get; private set; }


        private FieldVisit()
        {
        }

        public static FieldVisit Create(DateOnly visitDate, int subNeighborhoodId, bool isCompleted = false)
        {
            if (subNeighborhoodId <= 0) throw new DomainException("Valid SubNeighborhoodId is required");

            return new FieldVisit { VisitDate = visitDate, SubNeighborhoodId = subNeighborhoodId, IsCompleted = isCompleted };
        }

        public void AddImmunizationRecord(ImmunizationRecord ir)
        {
            if (ir == null) throw new DomainException("Immunization record is required");
            if (_immunizationRecords.Any(x => x.Id == ir.Id)) throw new DomainException("Immunization record already added");

            _immunizationRecords.Add(ir);
        }

        public void MarkCompleted()
        {
            IsCompleted = true;
        }

        public void UpdateVisitInfo(DateOnly visitDate, int subNeighborhoodId)
        {
            if (subNeighborhoodId <= 0) throw new DomainException("Valid SubNeighborhoodId is required");

            VisitDate = visitDate;
            SubNeighborhoodId = subNeighborhoodId;
        }

    }
}
