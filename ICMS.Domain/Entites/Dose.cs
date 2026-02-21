using ICMS.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMS.Domain.Entites
{
    public class Dose : BaseEntity<int>
    {
        public int VaccineId { get; private set; }
        public byte DoseOrder { get; private set; }
        public string RecommendedAgeGroup { get; private set; } = string.Empty;
        public int RecommendedAgeInMonths { get; private set; }
        public string? Notes { get; private set; }
        public string DoseName { get; private set; } = string.Empty;
        public Vaccine Vaccine { get; private set; }

        private Dose()
        {   
        }

        public static Dose Create(int vaccineId, string doseName, byte doseOrder, int recommendedAgeInMonths, string recommendedAgeGroup, string? notes = null)
        {
            if (vaccineId <= 0) throw new DomainException("Invalid vaccine id");

            if (string.IsNullOrWhiteSpace(doseName)) throw new DomainException("Dose name is required");
            if (recommendedAgeInMonths < 0) throw new DomainException("Recommended age cannot be negative");
            if (string.IsNullOrWhiteSpace(recommendedAgeGroup)) throw new DomainException("Recommended age group is required");

            return new Dose
            {
                VaccineId = vaccineId,
                DoseName = doseName,
                DoseOrder = doseOrder,
                RecommendedAgeInMonths = recommendedAgeInMonths,
                RecommendedAgeGroup = recommendedAgeGroup,
                Notes = notes
            };
        }

        public void UpdateDoseInfo(int vaccineId, string doseName, byte doseOrder, int recommendedAgeInMonths, string recommendedAgeGroup, string? notes = null)
        {
            VaccineId = vaccineId;
            DoseName = doseName;
            DoseOrder = doseOrder;
            RecommendedAgeInMonths = recommendedAgeInMonths;
            RecommendedAgeGroup = recommendedAgeGroup;
            Notes = notes;
        }
    }
}
