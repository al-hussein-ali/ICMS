using ICMS.Domain.Entites.Geography;
using ICMS.Domain.Entites.Audit;
using ICMS.Domain.Entites.Visits;
using ICMS.Domain.Entites.Maternal;
using ICMS.Domain.Entites.Clinical;
using ICMS.Domain.Entites.Identity;
using ICMS.Domain.Entites.Common;
using ICMS.Domain.Exceptions;
using ICMS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMS.Domain.Entites.Maternal
{
    public class Newborn : BaseEntity<int>
    {
        public int PregnancyDetailsId { get; private set; }
        public NewbornStatus NewbornStatus { get; private set; }
        public decimal NewbornWeightInGrams { get; private set; }
        public Gender NewbornGender { get; private set; }
        public PregnancyDetails? PregnancyDetails { get; private set; }


        private Newborn()
        {
        }

        public static Newborn Create(int pregnancyDetailsId, NewbornStatus newbornStatus, decimal newbornWeightInGrams, Gender newbornGender)
        {
            if (pregnancyDetailsId <= 0) throw new DomainException("Invalid pregnancy details id");
            if (newbornWeightInGrams <= 0) throw new DomainException("Invalid newborn weight");

            return new Newborn { PregnancyDetailsId = pregnancyDetailsId, NewbornStatus = newbornStatus, NewbornWeightInGrams = newbornWeightInGrams, NewbornGender = newbornGender };
        }

        public void AssignPregnancyDetails(PregnancyDetails pd)
        {
            if (pd == null) throw new DomainException("Pregnancy details required");
            if (PregnancyDetails != null) throw new DomainException("Pregnancy details already assigned");
            if (pd.Id != 0 && pd.Id != PregnancyDetailsId) throw new DomainException("Pregnancy details id mismatch");

            PregnancyDetails = pd;
            PregnancyDetailsId = pd.Id;
        }
    }
}
