using ICMS.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMS.Domain.Entites
{
    public class PreviousPostartumComplications : BaseEntity<int>
    {

        public bool VaginalBleeding { get; private set; }
        public bool PlacentaRetention { get; private set; }
        public bool VaginalFistula { get; private set; }
        public bool PuerperalSepsis { get; private set; }
        public bool NeonatalDeathWithinFirstWeek { get; private set; }

        public int PregnancyDetailId { get; private set; }
        public PregnancyDetails? PregnancyDetails { get; private set; }

        private PreviousPostartumComplications()
        {
        }

        public static PreviousPostartumComplications Create(bool vaginalBleeding, bool placentaRetention, bool vaginalFistula, bool puerperalSepsis, bool neonatalDeathWithinFirstWeek, int pregnancyDetailId)
        {
            if (pregnancyDetailId <= 0) throw new DomainException("Invalid pregnancy detail id");

            return new PreviousPostartumComplications
            {
                VaginalBleeding = vaginalBleeding,
                PlacentaRetention = placentaRetention,
                VaginalFistula = vaginalFistula,
                PuerperalSepsis = puerperalSepsis,
                NeonatalDeathWithinFirstWeek = neonatalDeathWithinFirstWeek,
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
