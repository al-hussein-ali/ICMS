using ICMS.Domain.Exceptions;
using ICMS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMS.Domain.Entites
{
    public class PregnancyDetails : BaseEntity<int>
    {
        private readonly List<VisitDetails> _visitDetails = new();
        public IReadOnlyList<VisitDetails> VisitDetails => _visitDetails.AsReadOnly();

        public DateOnly LastMenstrualPeriodDate { get; private set; }
        public DateOnly ExpectedDeliveryDate { get; private set; }
        public DateOnly? DeliveryDate { get; private set; }
        public byte VisitsCount { get; private set; } = 0;
        public PregnancyType PregnancyType { get; private set; }
        public BirthNature BirthNature { get; private set; }
        public BirthLocationType BirthLocationType { get; private set; }
        public string? BirthLocationDetails { get; private set; } = string.Empty;
        public string BirthNatureReason { get; private set; } = string.Empty;
        public string PregnancyComplications { get; private set; } = string.Empty;
        public string Interferences { get; private set; } = string.Empty;
        public byte NewbornCount { get; private set; } = 0;
        public bool IsPregnancyDone { get; private set; }
        public string? ComplicationsDuringChildbirth { get; private set; }
        public string? PostpartumComplications { get; private set; }

        public int PregnantWomanId { get; private set; }
        public int? PreviousPregnancyComplicationsId { get; private set; }
        public int? PreviousPostartumComplicationsId { get; private set; }
        public int? PreviousPregnancyDeliveryComplicationsId { get; private set; }
        public PregnantWoman? PregnantWoman { get; private set; }
        public PreviousPregnancyComplications? PreviousPregnancyComplications { get; private set; }
        public PreviousPostartumComplications? PreviousPostartumComplications { get; private set; }
        public PreviousPregnancyDeliveryComplications? PreviousPregnancyDelivaryComplications { get; private set; }

        private PregnancyDetails()
        {
        }

        public static PregnancyDetails Create(DateOnly lastMenstrualPeriodDate,
            DateOnly expectedDeliveryDate,
            PregnancyType pregnancyType,
            BirthNature birthNature,
            BirthLocationType birthLocationType, 
            int pregnantWomanId)
        {
            if (pregnantWomanId <= 0) throw new DomainException("Invalid pregnant woman id");
            if (expectedDeliveryDate < lastMenstrualPeriodDate) throw new DomainException("Expected delivery date cannot be before last menstrual period date");

            return new PregnancyDetails
            {
                LastMenstrualPeriodDate = lastMenstrualPeriodDate,
                ExpectedDeliveryDate = expectedDeliveryDate,
                PregnancyType = pregnancyType,
                BirthNature = birthNature,
                BirthLocationType = birthLocationType,
                PregnantWomanId = pregnantWomanId
            };
        }

        public void AssignPregnantWoman(PregnantWoman pw)
        {
            if (pw == null) throw new DomainException("Pregnant woman is required");
            if (PregnantWoman != null) throw new DomainException("Pregnant woman already assigned");
            if (pw.Id != 0 && pw.Id != PregnantWomanId) throw new DomainException("Pregnant woman id mismatch");

            PregnantWoman = pw;
            PregnantWomanId = pw.Id;
        }

        public void MarkDelivery(DateOnly deliveryDate,
            string? complicationsDuringChildbirth = null,
            string? postpartumComplications = null)
        {
            if (deliveryDate < LastMenstrualPeriodDate) throw new DomainException("Delivery date cannot be before last menstrual period date");

            DeliveryDate = deliveryDate;
            ComplicationsDuringChildbirth = complicationsDuringChildbirth;
            PostpartumComplications = postpartumComplications;
            IsPregnancyDone = true;
        }

        public void AddVisit(VisitDetails vd)
        {
            if (vd == null) throw new DomainException("Visit details required");
            if (_visitDetails.Any(x => x.Id == vd.Id)) throw new DomainException("Visit already added");

            _visitDetails.Add(vd);
            VisitsCount = (byte)_visitDetails.Count;
        }

        public void AssignPreviousPregnancyComplications(PreviousPregnancyComplications p)
        {
            if (p == null) throw new DomainException("Previous pregnancy complications required");
            if (PreviousPregnancyComplications != null) throw new DomainException("Already assigned");
            if (p.Id != 0 && p.PregnancyDetailId != 0 && p.PregnancyDetailId != Id) throw new DomainException("Pregnancy detail id mismatch");

            PreviousPregnancyComplications = p;
            PreviousPregnancyComplicationsId = p.Id;
        }

        public void AssignPreviousPostartumComplications(PreviousPostartumComplications p)
        {
            if (p == null) throw new DomainException("Previous postartum complications required");
            if (PreviousPostartumComplications != null) throw new DomainException("Already assigned");
            if (p.Id != 0 && p.PregnancyDetailId != 0 && p.PregnancyDetailId != Id) throw new DomainException("Pregnancy detail id mismatch");

            PreviousPostartumComplications = p;
            PreviousPostartumComplicationsId = p.Id;
        }

        public void AssignPreviousPregnancyDeliveryComplications(PreviousPregnancyDeliveryComplications p)
        {
            if (p == null) throw new DomainException("Previous pregnancy delivery complications required");
            if (PreviousPregnancyDelivaryComplications != null) throw new DomainException("Already assigned");
            if (p.Id != 0 && p.PregnancyDetailId != 0 && p.PregnancyDetailId != Id) throw new DomainException("Pregnancy detail id mismatch");

            PreviousPregnancyDelivaryComplications = p;
            PreviousPregnancyDeliveryComplicationsId = p.Id;
        }
    }
}
