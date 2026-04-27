using System;

namespace ICMS.Application.DTOs.FieldVisit
{
    public record FieldVisitCreateDto(
        string CampaignName,
        DateOnly VisitDate,
        int SubNeighborhoodId,
        DateOnly FromDate,
        DateOnly ToDate);
}
