using System.Linq;
using System.Collections.Generic;
using ICMS.Domain.Entites.Maternal;
using ICMS.Application.DTOs.Person;
using ICMS.Application.DTOs.Maternal;

namespace ICMS.Application.Extensions
{
    public static class MaternalExtensions
    {
        public static PregnantWomanReadDto ToReadDto(this PregnantWoman pw)
        {
            return new PregnantWomanReadDto(
                pw.Id,
                pw.AgeRange,
                pw.PregnancyCount,
                pw.BloodGroup,
                pw.RhFactor,
                pw.PersonId,
                pw.UserId,
                pw.Person?.FirstName,
                pw.Person?.SecondName,
                pw.Person?.ThirdName,
                pw.Person?.LastName,
                pw.Person?.Gender.ToString(),
                pw.Person?.PhoneNumber,
                pw.CurrentAddress
            );
        }

        public static PregnancyDetailsReadDto ToReadDto(this PregnancyDetails p)
        {
            return new PregnancyDetailsReadDto(
                p.Id,
                p.LastMenstrualPeriodDate,
                p.ExpectedDeliveryDate,
                p.DeliveryDate,
                p.VisitsCount,
                p.NewbornCount,
                p.IsPregnancyDone,
                p.PregnancyType,
                p.BirthNature,
                p.BirthLocationType,
                p.BirthLocationDetails,
                p.ComplicationsDuringChildbirth,
                p.PostpartumComplications,
                p.PreviousPregnancyComplications != null ? new PreviousPregnancyComplicationsDto(
                    p.PreviousPregnancyComplications.VaginalBleedingDuringPregnancy,
                    p.PreviousPregnancyComplications.RecurrentMiscarriageMoreThanThree,
                    p.PreviousPregnancyComplications.Diabetes,
                    p.PreviousPregnancyComplications.Epilepsy,
                    p.PreviousPregnancyComplications.HeartDisease,
                    p.PreviousPregnancyComplications.Preeclampsia,
                    p.PreviousPregnancyComplications.PretermBirthBefore8Months) : null,
                p.PreviousPregnancyDeliveryComplications != null ? new PreviousPregnancyDeliveryComplicationsDto(
                    p.PreviousPregnancyDeliveryComplications.CesareanSection,
                    p.PreviousPregnancyDeliveryComplications.AssistedDelivery,
                    p.PreviousPregnancyDeliveryComplications.StillbirthOrMultipleDeaths) : null,
                p.PreviousPostpartumComplications != null ? new PreviousPostpartumComplicationsDto(
                    p.PreviousPostpartumComplications.VaginalBleeding,
                    p.PreviousPostpartumComplications.PlacentaRetention,
                    p.PreviousPostpartumComplications.VaginalFistula,
                    p.PreviousPostpartumComplications.PuerperalSepsis,
                    p.PreviousPostpartumComplications.NeonatalDeathWithinFirstWeek) : null,
                p.VisitDetails.Select(v => new AddAncVisitDto(
                    v.VisitDate,
                    v.PregnancyDurationInWeeks,
                    v.WeightInKilo,
                    v.BloodPressure,
                    v.NextVisitDate,
                    v.APPInUrineTest,
                    v.OGTTInUrineTest,
                    v.FetalHeartbeat,
                    v.FetalHeartbeatValue,
                    v.FetalMovement,
                    v.FetalPosition,
                    v.AnaemiaOrHemoglobinType,
                    v.Id,
                    v.ClinicalExaminationAndObservation,
                    v.TreatmentsGiven,
                    v.LegsSwelling,
                    v.VaginalBleeding)).ToList(),
                p.Newborns.Select(n => new NewbornDto(
                    n.NewbornStatus,
                    n.NewbornWeightInGrams,
                    n.NewbornGender)).ToList()
            );
        }

        public static PregnantWomanDetailsDto ToDetailsDto(this PregnantWoman pw)
        {
            PersonReadDto? personDto = null;
            if (pw.Person != null)
            {
                personDto = new PersonReadDto(
                    pw.Person.Id,
                    pw.Person.FirstName,
                    pw.Person.SecondName,
                    pw.Person.ThirdName,
                    pw.Person.LastName,
                    pw.Person.Gender.ToString(),
                    pw.Person.DateOfBirth,
                    pw.Person.PhoneNumber
                );
            }

            var pregnancies = pw.PregnancyDetails.Select(p => p.ToReadDto()).ToList();

            return new PregnantWomanDetailsDto(
                pw.Id,
                pw.AgeRange,
                pw.PregnancyCount,
                pw.BloodGroup,
                pw.RhFactor,
                pw.PersonId,
                pw.UserId,
                pw.CurrentAddress,
                personDto,
                pregnancies
            );
        }

        public static Newborn ToDomain(this NewbornDto dto, int pregnancyDetailsId, int userId)
        {
            return Newborn.Create(
                pregnancyDetailsId: pregnancyDetailsId,
                newbornStatus: dto.Status,
                newbornWeightInGrams: dto.Weight,
                newbornGender: dto.Gender,
                userId: userId
            );
        }
    }
}
