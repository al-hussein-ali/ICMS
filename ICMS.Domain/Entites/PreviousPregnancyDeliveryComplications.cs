using ICMS.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMS.Domain.Entites
{
    public class PreviousPregnancyDeliveryComplications : BaseEntity<int>
    {
        public bool CesareanSection { get; private set; }
        public bool AssistedDelivery { get; private set; }
        public bool StillbirthOrMultipleDeaths { get; private set; }
        public int PregnancyDetailId { get; private set; }
        public PregnancyDetails? PregnancyDetails { get; private set; }

        private PreviousPregnancyDeliveryComplications()
        {
        }

        public static PreviousPregnancyDeliveryComplications Create(bool cesareanSection, bool assistedDelivery, bool stillbirthOrMultipleDeaths, int pregnancyDetailId)
        {
            if (pregnancyDetailId <= 0) throw new DomainException("Invalid pregnancy detail id");

            return new PreviousPregnancyDeliveryComplications
            {
                CesareanSection = cesareanSection,
                AssistedDelivery = assistedDelivery,
                StillbirthOrMultipleDeaths = stillbirthOrMultipleDeaths,
                PregnancyDetailId = pregnancyDetailId
            };
        }

        public void AssignPregnancyDetails(PregnancyDetails pd)
        {
            if (pd == null) throw new DomainException("Pregnancy details required");
            if (PregnancyDetails != null) throw new DomainException("Already assigned");
            if (pd.Id != 0 && pd.Id != PregnancyDetailId) throw new DomainException("Pregnancy details id mismatch");

            PregnancyDetails = pd;
            PregnancyDetailId = pd.Id;
        }
    }
}
