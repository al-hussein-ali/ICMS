namespace ICMS.Application.DTOs.FieldVisit
{
    public record FieldVisitReadDto(
        int Id,
        DateOnly VisitDate,
        string TargetedLocation,
        bool IsCompleted);
}
