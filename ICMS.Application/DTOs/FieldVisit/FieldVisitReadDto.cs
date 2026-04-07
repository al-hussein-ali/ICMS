namespace ICMS.Application.DTOs.FieldVisit
{
    public record FieldVisitReadDto(
        int Id,
        DateOnly VisitDate,
        int SubNeighborhoodId,
        string SubNeighborhoodName,
        bool IsCompleted);
}
