namespace ICMS.Application.DTOs.FieldVisit
{
    public record FieldVisitDetailsDto(
        int Id,
        string CampaignName,
        DateOnly VisitDate,
        int SubNeighborhoodId,
        string SubNeighborhoodName,
        int NeighborhoodId,
        int DirectorateId,
        int GovernorateId,
        DateOnly FromDate,
        DateOnly ToDate,
        bool IsCompleted,
        int ImmunizationRecordsCount);
}
