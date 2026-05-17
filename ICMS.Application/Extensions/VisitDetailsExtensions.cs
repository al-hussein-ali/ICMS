using ICMS.Application.DTOs.Maternal;
using ICMS.Application.DTOs.VisitDetails;
using ICMS.Domain.Entites.Visits;
using System.Linq;

namespace ICMS.Application.Extensions
{
    public static class VisitDetailsExtensions
    {
        public static VisitDetailsReadDto ToReadDto(this VisitDetails vd)
            => new(vd.Id, vd.PregnancyDetailsId, vd.VisitDate, vd.NextVisitDate, vd.WeightInKilo);

        public static VisitDetailsDetailsDto ToDetailsDto(this VisitDetails vd)
            => new(vd.Id, vd.PregnancyDetailsId, vd.VisitDate, vd.NextVisitDate, vd.WeightInKilo);

        public static VisitDetails ToDomain(this VisitDetailsCreateDto dto, int userId)
            => VisitDetails.Create(
                dto.PregnancyDetailsId, 
                dto.VisitDate, 
                dto.WeightInKilo, 
                dto.PregnancyDurationInWeeks, 
                dto.BloodPressure, 
                dto.APPInUrineTest,
                dto.OGTTInUrineTest, 
                dto.AnaemiaOrHemoglobinType,
                userId,
                dto.LegsSwelling,
                dto.VaginalBleeding, 
                dto.ClinicalExaminationAndObservation, 
                dto.NextVisitDate);

        public static AddAncVisitDto ToAncVisitDto(this VisitDetails vd)
            => new(
                vd.VisitDate,
                vd.PregnancyDurationInWeeks,
                vd.WeightInKilo,
                vd.BloodPressure,
                vd.FetalDetailsList.Select(fd => new FetalDetailDto(
                    fd.Id,
                    fd.FetusLabel,
                    fd.FetalHeartbeat,
                    fd.FetalMovement,
                    fd.FetalPosition,
                    fd.FetalHeartbeatValue
                )).ToList(),
                vd.NextVisitDate,
                vd.APPInUrineTest,
                vd.OGTTInUrineTest,
                vd.AnaemiaOrHemoglobinType,
                vd.Id,
                vd.ClinicalExaminationAndObservation,
                vd.TreatmentsGiven,
                vd.LegsSwelling,
                vd.VaginalBleeding);
    }
}
