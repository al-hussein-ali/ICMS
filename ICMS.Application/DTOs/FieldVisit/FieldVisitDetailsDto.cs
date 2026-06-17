using System;
using System.Collections.Generic;
using ICMS.Application.DTOs.User;

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
        bool ReminderSent,
        int ImmunizationRecordsCount,
        List<FieldVisitTargetedIndividualDto> SelectedIndividuals,
        List<int> SelectedIndividualIds,
        List<UserReadDto> SelectedWorkers,
        List<int> SelectedWorkerIds,
        List<int> NotGoingWorkerIds);
}
