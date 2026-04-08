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
    public class PregnancyDetails : BaseEntity<int>
    {
        private readonly List<VisitDetails> _visitDetails = new();
        public IReadOnlyList<VisitDetails> VisitDetails => _visitDetails.AsReadOnly();


        private readonly List<Newborn> _newborns = new();
        public IReadOnlyList<Newborn> Newborns => _newborns.AsReadOnly();


        public DateOnly LastMenstrualPeriodDate { get; private set; }
        public DateOnly ExpectedDeliveryDate { get; private set; }
        public DateOnly? DeliveryDate { get; private set; }
        public byte VisitsCount { get; private set; } = 0;
        public PregnancyType PregnancyType { get; private set; }
        public BirthNature? BirthNature { get; private set; }
        public BirthLocationType? BirthLocationType { get; private set; }
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
        public int? PreviousPostpartumComplicationsId { get; private set; }
        public int? PreviousPregnancyDeliveryComplicationsId { get; private set; }
        public int UserId { get; private set; }
        public PregnantWoman? PregnantWoman { get; private set; }
        public PreviousPregnancyComplications? PreviousPregnancyComplications { get; private set; }
        public PreviousPostpartumComplications? PreviousPostpartumComplications { get; private set; }
        public PreviousPregnancyDeliveryComplications? PreviousPregnancyDeliveryComplications { get; private set; }
        public User? User { get; private set; }

        private PregnancyDetails()
        {
        }

        public static PregnancyDetails Create(DateOnly lastMenstrualPeriodDate,
            DateOnly expectedDeliveryDate,
            PregnancyType pregnancyType,
            BirthNature birthNature,
            BirthLocationType birthLocationType,
            int pregnantWomanId,
            int userId)
        {
            if (pregnantWomanId <= 0) throw new DomainException("Invalid pregnant woman id");
            if (userId <= 0) throw new DomainException("Invalid user id");
            if (expectedDeliveryDate < lastMenstrualPeriodDate)
                throw new DomainException("Expected delivery date cannot be before last menstrual period date");

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
            if (deliveryDate < LastMenstrualPeriodDate)
                throw new DomainException("Delivery date cannot be before last menstrual period date");

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
            if (p.Id != 0 && p.PregnancyDetailId != 0 && p.PregnancyDetailId != Id)
                throw new DomainException("Pregnancy detail id mismatch");

            PreviousPregnancyComplications = p;
            PreviousPregnancyComplicationsId = p.Id;
        }

        public void AssignPreviousPostpartumComplications(PreviousPostpartumComplications p)
        {
            if (p == null) throw new DomainException("Previous postpartum complications required");
            if (this.PreviousPostpartumComplications != null) throw new DomainException("Already assigned");
            if (p.Id != 0 && p.PregnancyDetailId != 0 && p.PregnancyDetailId != Id)
                throw new DomainException("Pregnancy detail id mismatch");

            this.PreviousPostpartumComplications = p;
            this.PreviousPostpartumComplicationsId = p.Id;
        }

        public void AssignPreviousPregnancyDeliveryComplications(PreviousPregnancyDeliveryComplications p)
        {
            if (p == null) throw new DomainException("Previous pregnancy delivery complications required");
            if (this.PreviousPregnancyDeliveryComplications != null) throw new DomainException("Already assigned");
            if (p.Id != 0 && p.PregnancyDetailId != 0 && p.PregnancyDetailId != Id)
                throw new DomainException("Pregnancy detail id mismatch");

            this.PreviousPregnancyDeliveryComplications = p;
            this.PreviousPregnancyDeliveryComplicationsId = p.Id;
        }

        public static PregnancyDetails CreateForNewPregnancy(DateOnly lmp, DateOnly edd, int pregnantWomanId, int userId)
        {
            if (pregnantWomanId <= 0) throw new DomainException("Invalid pregnant woman id");
            if (userId <= 0) throw new DomainException("Invalid user id");
            if (edd < lmp)
                throw new DomainException("Expected delivery date cannot be before last menstrual period date");

            return new PregnancyDetails
            {
                LastMenstrualPeriodDate = lmp,
                ExpectedDeliveryDate = edd,
                PregnantWomanId = pregnantWomanId,
                UserId = userId,
                IsPregnancyDone = false,
                VisitsCount = 0,
                NewbornCount = 0
            };
        }

        public void AddVisit(
            DateOnly visitDate,
            int pregnancyDurationInWeeks,
            decimal weight,
            string bloodPressure,
            int userId,
            DateOnly? doctorSuggestedNextVisit = null,
            string appInUrineTest = "N/A",
            string ogttInUrineTest = "N/A",
            string fetalHeartbeat = "N/A",
            string fetalMovement = "N/A",
            string fetalPosition = "N/A",
            string anaemiaOrHemoglobinType = "N/A")
        {
            if (IsPregnancyDone)
                throw new InvalidOperationException("Cannot add visit: Pregnancy is already marked as done.");
            if (userId <= 0) throw new DomainException("Invalid user id");

            DateOnly nextVisitDate;

            if (doctorSuggestedNextVisit.HasValue)
            {
                nextVisitDate = doctorSuggestedNextVisit.Value;
            }
            else
            {
                if (pregnancyDurationInWeeks < 20)
                {
                    nextVisitDate = LastMenstrualPeriodDate.AddDays(140);
                }
                else
                {
                    nextVisitDate = visitDate.AddDays(14);
                }
            }

            var visit = Visits.VisitDetails.Create(
                pregnancyDetailsId: this.Id,
                visitDate: visitDate,
                weightInKilo: weight,
                pregnancyDurationInWeeks: pregnancyDurationInWeeks,
                bloodPressure: bloodPressure,
                appInUrineTest: appInUrineTest,
                ogttInUrineTest: ogttInUrineTest,
                fetalHeartbeat: fetalHeartbeat,
                fetalMovement: fetalMovement,
                fetalPosition: fetalPosition,
                anaemiaOrHemoglobinType: anaemiaOrHemoglobinType,
                nextVisitDate: nextVisitDate,
                userId: userId
            );

            _visitDetails.Add(visit);
            VisitsCount++;
        }

        public void ConcludePregnancy(
            DateOnly deliveryDate,
            BirthNature birthNature,
            BirthLocationType locationType,
            string locationDetails,
            string intrapartumComplications,
            string postpartumComplications,
            List<Newborn> newborns)
        {
            if (IsPregnancyDone)
                throw new InvalidOperationException("Pregnancy is already concluded.");

            if (deliveryDate < LastMenstrualPeriodDate)
                throw new DomainException("Delivery date cannot be before last menstrual period date");

            DeliveryDate = deliveryDate;
            BirthNature = birthNature;
            BirthLocationType = locationType;
            BirthLocationDetails = locationDetails;
            ComplicationsDuringChildbirth = intrapartumComplications;
            PostpartumComplications = postpartumComplications;

            if (newborns != null && newborns.Any())
            {
                _newborns.AddRange(newborns);
                NewbornCount = (byte)_newborns.Count;
            }

            IsPregnancyDone = true;
        }
    }
}
