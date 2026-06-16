using ICMS.Application.DTOs.FieldVisit;
using ICMS.Domain.Entites.Visits;
using System.Linq;
using System.Collections.Generic;
using ICMS.Application.DTOs.User;

namespace ICMS.Application.Extensions
{
    public static class FieldVisitExtensions
    {
        public static FieldVisitReadDto ToReadDto(this FieldVisit fv)
        {
            string location = "Unknown";
            if (fv.SubNeighborhood != null && fv.SubNeighborhood.Neighborhood?.Directorate?.Governorate != null)
            {
                location = $"{fv.SubNeighborhood.Neighborhood.Directorate.Governorate.Name} - {fv.SubNeighborhood.Neighborhood.Directorate.Name} - {fv.SubNeighborhood.Neighborhood.Name} - {fv.SubNeighborhood.Name}";
            }
            else if (fv.SubNeighborhood != null)
            {
                location = fv.SubNeighborhood.Name;
            }

            return new FieldVisitReadDto(
                fv.Id,
                fv.CampaignName,
                fv.VisitDate,
                fv.FromDate,
                fv.ToDate,
                fv.SubNeighborhood?.Neighborhood?.DirectorateId ?? 0,
                fv.SubNeighborhood?.NeighborhoodId ?? 0,
                fv.SubNeighborhoodId,
                fv.SubNeighborhood?.Name,
                location,
                fv.TargetedCount, 
                fv.IsCompleted,
                fv.ReminderSent
            );
        }

        public static FieldVisitDetailsDto ToDetailsDto(this FieldVisit fv)
        {
            var selectedInds = (fv.FieldVisitIndividuals ?? Enumerable.Empty<FieldVisitIndividual>()).Select(fvi => {
                var ind = fvi.VaccinatedIndividual;
                if (ind == null) return null!;
                var doses = (ind.Schedules ?? Enumerable.Empty<ICMS.Domain.Entites.Clinical.VaccinationSchedule>())
                    .Where(s => s.Status == ICMS.Domain.Enums.ScheduleStatus.Missed)
                    .Select(s => s.Dose.DoseName)
                    .ToList();

                return new FieldVisitTargetedIndividualDto(
                    ind.Id,
                    ind.Person?.FullName ?? string.Empty,
                    ind.CardNumber,
                    ind.Person?.PhoneNumber ?? string.Empty,
                    doses
                );
            }).Where(x => x != null).ToList();

            var selectedIds = (fv.FieldVisitIndividuals ?? Enumerable.Empty<FieldVisitIndividual>())
                .Select(fvi => fvi.VaccinatedIndividualId)
                .ToList();

            var selectedWorkers = (fv.FieldVisitWorkers ?? Enumerable.Empty<FieldVisitWorker>()).Select(fvw => {
                var user = fvw.User;
                if (user == null) return null!;
                return user.ToReadDto(new List<string> { ICMS.Domain.Constants.Roles.FieldVisitWorker });
            }).Where(x => x != null).ToList();

            var selectedWorkerIds = (fv.FieldVisitWorkers ?? Enumerable.Empty<FieldVisitWorker>())
                .Select(fvw => fvw.UserId)
                .ToList();

            return new FieldVisitDetailsDto(
                fv.Id,
                fv.CampaignName,
                fv.VisitDate,
                fv.SubNeighborhoodId,
                fv.SubNeighborhood?.Name ?? "N/A",
                fv.SubNeighborhood?.NeighborhoodId ?? 0,
                fv.SubNeighborhood?.Neighborhood?.DirectorateId ?? 0,
                fv.SubNeighborhood?.Neighborhood?.Directorate?.GovernorateId ?? 0,
                fv.FromDate,
                fv.ToDate,
                fv.IsCompleted,
                fv.ReminderSent,
                fv.ImmunizationRecords.Count,
                selectedInds,
                selectedIds,
                selectedWorkers,
                selectedWorkerIds
            );
        }
    }
}
