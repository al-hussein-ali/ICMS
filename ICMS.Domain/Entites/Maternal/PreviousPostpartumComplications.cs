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

namespace ICMS.Domain.Entites.Maternal
{
    public class PreviousPostpartumComplications : BaseEntity<int>
    {

        public bool VaginalBleeding { get; private set; }
        public bool PlacentaRetention { get; private set; }
        public bool VaginalFistula { get; private set; }
        public bool PuerperalSepsis { get; private set; }
        public bool NeonatalDeathWithinFirstWeek { get; private set; }

        public int PregnancyDetailId { get; private set; }
        public PregnancyDetails? PregnancyDetails { get; private set; }

        private PreviousPostpartumComplications()
        {
        }

        public static PreviousPostpartumComplications Create(bool vaginalBleeding, bool placentaRetention, bool vaginalFistula, bool puerperalSepsis, bool neonatalDeathWithinFirstWeek, int pregnancyDetailId)
        {
            if (pregnancyDetailId <= 0) throw new DomainException("Invalid pregnancy detail id");

            return new PreviousPostpartumComplications
            {
                VaginalBleeding = vaginalBleeding,
                PlacentaRetention = placentaRetention,
                VaginalFistula = vaginalFistula,
                PuerperalSepsis = puerperalSepsis,
                NeonatalDeathWithinFirstWeek = neonatalDeathWithinFirstWeek,
                PregnancyDetailId = pregnancyDetailId
            };
        }

        public static PreviousPostpartumComplications CreateForNewPregnancy(bool vaginalBleeding, bool placentaRetention, bool vaginalFistula, bool puerperalSepsis, bool neonatalDeathWithinFirstWeek)
        {
            return new PreviousPostpartumComplications
            {
                VaginalBleeding = vaginalBleeding,
                PlacentaRetention = placentaRetention,
                VaginalFistula = vaginalFistula,
                PuerperalSepsis = puerperalSepsis,
                NeonatalDeathWithinFirstWeek = neonatalDeathWithinFirstWeek
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
