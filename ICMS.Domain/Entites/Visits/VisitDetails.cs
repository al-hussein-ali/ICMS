using ICMS.Domain.Exceptions;
using ICMS.Domain.Entites.Common;
using System;

namespace ICMS.Domain.Entites.Visits
{
    public class VisitDetails : BaseEntity<int>
    {
        public int PregnancyDetailsId { get; private set; }
        public DateOnly VisitDate { get; private set; }
        public DateOnly? NextVisitDate { get; private set; }
        public string? ClinicalExaminationAndObservation { get; private set; }
        public decimal WeightInKilo { get; private set; }
        public string BloodPressure { get; private set; } = string.Empty;
        public string APPInUrineTest { get; private set; } = string.Empty;
        public string OGTTInUrineTest { get; private set; } = string.Empty;
        public string FetalHeartbeat { get; private set; } = string.Empty;
        public string FetalMovement { get; private set; } = string.Empty;
        public string FetalPosition { get; private set; } = string.Empty;
        public int PregnancyDurationInWeeks { get; private set; }
        public string AnaemiaOrHemoglobinType { get; private set; } = string.Empty;
        public bool LegsSwelling { get; private set; }
        public bool VaginalBleeding { get; private set; }
        public Maternal.PregnancyDetails? PregnancyDetails { get; private set; }


        private VisitDetails()
        {
        }

        public static VisitDetails Create(int pregnancyDetailsId, DateOnly visitDate, decimal weightInKilo, int pregnancyDurationInWeeks, string bloodPressure, string appInUrineTest, string ogttInUrineTest, string fetalHeartbeat, string fetalMovement, string fetalPosition, string anaemiaOrHemoglobinType, bool legsSwelling = false, bool vaginalBleeding = false, string? clinicalExaminationAndObservation = null, DateOnly? nextVisitDate = null)
        {
            if (pregnancyDetailsId <= 0) throw new DomainException("Invalid pregnancy details id");
            if (weightInKilo <= 0) throw new DomainException("Invalid weight");
            if (pregnancyDurationInWeeks < 0) throw new DomainException("Invalid pregnancy duration");
            if (string.IsNullOrWhiteSpace(bloodPressure)) throw new DomainException("Blood pressure is required");

            return new VisitDetails
            {
                PregnancyDetailsId = pregnancyDetailsId,
                VisitDate = visitDate,
                NextVisitDate = nextVisitDate,
                ClinicalExaminationAndObservation = clinicalExaminationAndObservation,
                WeightInKilo = weightInKilo,
                BloodPressure = bloodPressure,
                APPInUrineTest = appInUrineTest,
                OGTTInUrineTest = ogttInUrineTest,
                FetalHeartbeat = fetalHeartbeat,
                FetalMovement = fetalMovement,
                FetalPosition = fetalPosition,
                PregnancyDurationInWeeks = pregnancyDurationInWeeks,
                AnaemiaOrHemoglobinType = anaemiaOrHemoglobinType,
                LegsSwelling = legsSwelling,
                VaginalBleeding = vaginalBleeding
            };
        }

        public void AssignPregnancyDetails(Maternal.PregnancyDetails pd)
        {
            if (pd == null) throw new DomainException("Pregnancy details required");
            if (PregnancyDetails != null) throw new DomainException("Already assigned");
            if (pd.Id != 0 && pd.Id != PregnancyDetailsId) throw new DomainException("Pregnancy details id mismatch");

            PregnancyDetails = pd;
            PregnancyDetailsId = pd.Id;
        }
    }
}
