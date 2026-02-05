using ICMS.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMS.Domain.Entites
{
    public class PreviousPregnancyComplications : BaseEntity<int>
    {
        public bool VaginalBleedingDuringPregnancy { get; private set; }
        public bool RecurrentMiscarriageMoreThanThree { get; private set; }
        public bool Diabetes { get; private set; }
        public bool Epilepsy { get; private set; }
        public bool HeartDisease { get; private set; }
        public bool Preeclampsia { get; private set; }
        public bool PretermBirthBefore8Months { get; private set; }

        public int PregnancyDetailId { get; private set; }
        public PregnancyDetails? PregnancyDetails { get; private set; }

        private PreviousPregnancyComplications()
        {
        }

        public static PreviousPregnancyComplications Create(bool vaginalBleedingDuringPregnancy, bool recurrentMiscarriageMoreThanThree, bool diabetes, bool epilepsy, bool heartDisease, bool preeclampsia, bool pretermBirthBefore8Months, int pregnancyDetailId)
        {
            if (pregnancyDetailId <= 0) throw new DomainException("Invalid pregnancy detail id");

            return new PreviousPregnancyComplications
            {
                VaginalBleedingDuringPregnancy = vaginalBleedingDuringPregnancy,
                RecurrentMiscarriageMoreThanThree = recurrentMiscarriageMoreThanThree,
                Diabetes = diabetes,
                Epilepsy = epilepsy,
                HeartDisease = heartDisease,
                Preeclampsia = preeclampsia,
                PretermBirthBefore8Months = pretermBirthBefore8Months,
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
