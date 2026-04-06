namespace ICMS.Application.DTOs.FieldVisit
{
    public record FieldVisitCreateDto(
        DateOnly VisitDate,
        string TargetedLocation,
        List<int>? FieldWorkerUserIds = null);
}
