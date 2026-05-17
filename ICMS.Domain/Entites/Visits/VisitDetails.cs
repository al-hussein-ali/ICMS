using ICMS.Domain.Exceptions;
using ICMS.Domain.Entites.Common;
using System;
using System.Collections.Generic;

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
        public int PregnancyDurationInWeeks { get; private set; }
        public string AnaemiaOrHemoglobinType { get; private set; } = string.Empty;
        public bool LegsSwelling { get; private set; }
        public bool VaginalBleeding { get; private set; }
        public string? TreatmentsGiven { get; private set; }
        public Maternal.PregnancyDetails? PregnancyDetails { get; private set; }

        public int UserId { get; private set; }
        public ICMS.Domain.Entites.Identity.User? User { get; private set; }

        private readonly List<FetalDetails> _fetalDetailsList = new();
        public IReadOnlyList<FetalDetails> FetalDetailsList => _fetalDetailsList.AsReadOnly();

        private VisitDetails()
        {
        }

        public static VisitDetails Create(int pregnancyDetailsId, DateOnly visitDate, decimal weightInKilo, int pregnancyDurationInWeeks, string bloodPressure, string appInUrineTest, string ogttInUrineTest, string anaemiaOrHemoglobinType, int userId, bool legsSwelling = false, bool vaginalBleeding = false, string? clinicalExaminationAndObservation = null, DateOnly? nextVisitDate = null, string? treatmentsGiven = null)
        {
            if (pregnancyDetailsId <= 0) throw new DomainException("Invalid pregnancy details id");
            if (userId <= 0) throw new DomainException("Invalid user id");
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
                PregnancyDurationInWeeks = pregnancyDurationInWeeks,
                AnaemiaOrHemoglobinType = anaemiaOrHemoglobinType,
                UserId = userId,
                LegsSwelling = legsSwelling,
                VaginalBleeding = vaginalBleeding,
                TreatmentsGiven = treatmentsGiven
            };
        }

        public void Update(DateOnly visitDate, decimal weightInKilo, int pregnancyDurationInWeeks, string bloodPressure, string appInUrineTest, string ogttInUrineTest, string anaemiaOrHemoglobinType, bool legsSwelling = false, bool vaginalBleeding = false, string? clinicalExaminationAndObservation = null, DateOnly? nextVisitDate = null, string? treatmentsGiven = null)
        {
            VisitDate = visitDate;
            WeightInKilo = weightInKilo;
            PregnancyDurationInWeeks = pregnancyDurationInWeeks;
            BloodPressure = bloodPressure;
            APPInUrineTest = appInUrineTest;
            OGTTInUrineTest = ogttInUrineTest;
            AnaemiaOrHemoglobinType = anaemiaOrHemoglobinType;
            LegsSwelling = legsSwelling;
            VaginalBleeding = vaginalBleeding;
            ClinicalExaminationAndObservation = clinicalExaminationAndObservation;
            NextVisitDate = nextVisitDate;
            TreatmentsGiven = treatmentsGiven;
        }

        public void AddFetalDetail(FetalDetails fd)
        {
            if (fd == null) throw new DomainException("Fetal details required");
            _fetalDetailsList.Add(fd);
        }

        public void AddFetalDetailsRange(IEnumerable<FetalDetails> fds)
        {
            if (fds == null) return;
            _fetalDetailsList.AddRange(fds);
        }

        public void ClearFetalDetails()
        {
            _fetalDetailsList.Clear();
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
