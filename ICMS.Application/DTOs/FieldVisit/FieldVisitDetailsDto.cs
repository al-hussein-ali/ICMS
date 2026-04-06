namespace ICMS.Application.DTOs.FieldVisit
{
    public record FieldVisitDetailsDto(
        int Id,
        DateOnly VisitDate,
        string TargetedLocation,
        bool IsCompleted,
        List<FieldWorkerDto> FieldWorkers,
        int ImmunizationRecordsCount);

    public record FieldWorkerDto(int UserId, int FieldVisitId);
}
