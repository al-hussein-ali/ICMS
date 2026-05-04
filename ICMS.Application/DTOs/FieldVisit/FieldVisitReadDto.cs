namespace ICMS.Application.DTOs.FieldVisit
{
    public record FieldVisitReadDto(
        int Id,
        string CampaignName,
        DateOnly VisitDate,
        int DirectorateId,
        int NeighborhoodId,
        int? SubNeighborhoodId,
        string? SubNeighborhoodName,
        string TargetedLocation,
        int TargetedCount,
        bool IsCompleted);
}
