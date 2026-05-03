using ICMS.Application.DTOs.Maternal;
using ICMS.Application.DTOs.VisitDetails;
using ICMS.Domain.Entites.Visits;

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
                dto.FetalHeartbeat,
                dto.FetalMovement, 
                dto.FetalPosition,
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
                null, // TetanusDoseId - not stored directly in visit
                vd.NextVisitDate,
                vd.APPInUrineTest,
                vd.OGTTInUrineTest,
                vd.FetalHeartbeat,
                vd.FetalHeartbeatValue,
                vd.FetalMovement,
                vd.FetalPosition,
                vd.AnaemiaOrHemoglobinType,
                vd.Id,
                vd.ClinicalExaminationAndObservation,
                vd.TreatmentsGiven,
                vd.LegsSwelling,
                vd.VaginalBleeding);
    }
}
