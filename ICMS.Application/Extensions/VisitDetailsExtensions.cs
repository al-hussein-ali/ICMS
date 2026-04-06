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

        public static VisitDetails ToDomain(this VisitDetailsCreateDto dto)
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
                dto.LegsSwelling,
                dto.VaginalBleeding, 
                dto.ClinicalExaminationAndObservation, 
                dto.NextVisitDate);
    }
}
