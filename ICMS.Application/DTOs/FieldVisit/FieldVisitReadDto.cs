namespace ICMS.Application.DTOs.FieldVisit
{
    public record FieldVisitReadDto(
        int Id,
        string CampaignName,
        DateOnly VisitDate,
        int SubNeighborhoodId,
        string TargetedLocation,
        int TargetedCount,
        bool IsCompleted);
}
