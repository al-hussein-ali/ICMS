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

namespace ICMS.Domain.Entites.Clinical
{
    public class Dose : BaseEntity<int>
    {
        public int VaccineId { get; private set; }
        public byte DoseOrder { get; private set; }
        public string RecommendedAgeGroup { get; private set; } = string.Empty;
        public int RecommendedAgeInWeeks { get; private set; }
        public bool IsPrimary { get; private set; }
        public string? Notes { get; private set; }
        public string DoseName { get; private set; } = string.Empty;
        public Vaccine Vaccine { get; private set; }

        private Dose()
        {   
        }

        public static Dose Create(int vaccineId, string doseName, byte doseOrder, int recommendedAgeInWeeks, string recommendedAgeGroup, bool isPrimary = true, string? notes = null)
        {
            if (vaccineId <= 0) throw new DomainException("Invalid vaccine id");

            if (string.IsNullOrWhiteSpace(doseName)) throw new DomainException("Dose name is required");
            if (recommendedAgeInWeeks < 0) throw new DomainException("Recommended age cannot be negative");
            if (string.IsNullOrWhiteSpace(recommendedAgeGroup)) throw new DomainException("Recommended age group is required");

            return new Dose
            {
                VaccineId = vaccineId,
                DoseName = doseName,
                DoseOrder = doseOrder,
                RecommendedAgeInWeeks = recommendedAgeInWeeks,
                RecommendedAgeGroup = recommendedAgeGroup,
                IsPrimary = isPrimary,
                Notes = notes
            };
        }

        public void UpdateDoseInfo(int vaccineId, string doseName, byte doseOrder, int recommendedAgeInWeeks, string recommendedAgeGroup, bool isPrimary, string? notes = null)
        {
            VaccineId = vaccineId;
            DoseName = doseName;
            DoseOrder = doseOrder;
            RecommendedAgeInWeeks = recommendedAgeInWeeks;
            RecommendedAgeGroup = recommendedAgeGroup;
            IsPrimary = isPrimary;
            Notes = notes;
        }
    }
}
