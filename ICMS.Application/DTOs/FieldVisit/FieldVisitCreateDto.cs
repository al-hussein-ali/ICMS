using System;
using System.Collections.Generic;

namespace ICMS.Application.DTOs.FieldVisit
{
    public record FieldVisitCreateDto(
        string CampaignName,
        DateOnly VisitDate,
        int SubNeighborhoodId,
        DateOnly FromDate,
        DateOnly ToDate,
        List<int> SelectedIndividualIds,
        List<int> SelectedWorkerIds);
}
