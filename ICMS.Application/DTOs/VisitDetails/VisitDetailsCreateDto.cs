using System;

namespace ICMS.Application.DTOs.VisitDetails
{
    public record VisitDetailsCreateDto(int PregnancyDetailsId, DateOnly VisitDate,
        DateOnly? NextVisitDate,
        string? ClinicalExaminationAndObservation,
        decimal WeightInKilo,
        string BloodPressure,
        string APPInUrineTest,
        string OGTTInUrineTest,
        string FetalHeartbeat,
        string FetalMovement,
        string FetalPosition,
        int PregnancyDurationInWeeks,
        string AnaemiaOrHemoglobinType,
        bool LegsSwelling,
        bool VaginalBleeding
        );



}
