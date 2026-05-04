namespace ICMS.Application.DTOs.FieldVisit
{
    public record FieldVisitReadDto(
        int Id,
        string CampaignName,
        DateOnly VisitDate,
        int DirectorateId,
        int NeighborhoodId,
        int? SubNeighborhoodId,
        string TargetedLocation,
        int TargetedCount,
        bool IsCompleted);
}
