namespace ICMS.Application.DTOs.FieldVisit
{
    public record FieldVisitDetailsDto(
        int Id,
        DateOnly VisitDate,
        int SubNeighborhoodId,
        string SubNeighborhoodName,
        bool IsCompleted,
        int ImmunizationRecordsCount);
}
