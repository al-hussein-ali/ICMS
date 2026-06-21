using ICMS.Application.DTOs.FieldVisit;
using ICMS.Domain.Entites.Visits;
using ICMS.Domain.Entites.Clinical;
using System;
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

        public static FieldVisitDetailsDto ToDetailsDto(this FieldVisit fv, int? workerId = null)
        {
            var activeWorkers = (fv.FieldVisitWorkers ?? Enumerable.Empty<FieldVisitWorker>())
                .Where(w => w.IsGoing)
                .OrderBy(w => w.UserId)
                .ToList();

            var allIndividuals = (fv.FieldVisitIndividuals ?? Enumerable.Empty<FieldVisitIndividual>())
                .OrderBy(fvi => fvi.VaccinatedIndividualId)
                .ToList();

            // Build worker-to-individuals dictionary for active workers
            var workerAssignments = activeWorkers.ToDictionary(
                w => w.UserId, 
                w => new List<FieldVisitIndividual>());

            // Put manually assigned individuals to their active worker
            foreach (var ind in allIndividuals)
            {
                if (ind.AssignedWorkerId.HasValue && workerAssignments.ContainsKey(ind.AssignedWorkerId.Value))
                {
                    workerAssignments[ind.AssignedWorkerId.Value].Add(ind);
                }
            }

            // Distribute unassigned individuals (those with null AssignedWorkerId, or assigned to inactive workers)
            var unassignedInds = allIndividuals
                .Where(ind => !ind.AssignedWorkerId.HasValue || !workerAssignments.ContainsKey(ind.AssignedWorkerId.Value))
                .OrderBy(ind => ind.VaccinatedIndividualId)
                .ToList();

            int n = unassignedInds.Count;
            int m = activeWorkers.Count;
            if (m > 0 && n > 0)
            {
                int baseCount = n / m;
                int remainder = n % m;
                int start = 0;
                for (int i = 0; i < m; i++)
                {
                    int count = baseCount + (i < remainder ? 1 : 0);
                    var chunk = unassignedInds.Skip(start).Take(count);
                    workerAssignments[activeWorkers[i].UserId].AddRange(chunk);
                    start += count;
                }
            }

            // Map every individual to their computed AssignedWorkerId
            var computedAssignments = new Dictionary<int, int>(); // VaccinatedIndividualId -> WorkerUserId
            foreach (var kvp in workerAssignments)
            {
                foreach (var ind in kvp.Value)
                {
                    computedAssignments[ind.VaccinatedIndividualId] = kvp.Key;
                }
            }

            // If workerId is provided, filter list to show only this worker's portion.
            // If workerId is null, show all individuals.
            List<FieldVisitIndividual> selectedIndividualsToMap;
            if (workerId.HasValue)
            {
                if (workerAssignments.ContainsKey(workerId.Value))
                {
                    selectedIndividualsToMap = workerAssignments[workerId.Value];
                }
                else
                {
                    selectedIndividualsToMap = new List<FieldVisitIndividual>();
                }
            }
            else
            {
                selectedIndividualsToMap = allIndividuals;
            }

            var selectedInds = selectedIndividualsToMap.Select(fvi => {
                var ind = fvi.VaccinatedIndividual;
                if (ind == null) return null!;
                var doses = (ind.Schedules ?? Enumerable.Empty<ICMS.Domain.Entites.Clinical.VaccinationSchedule>())
                    .Where(s => s.Status == ICMS.Domain.Enums.ScheduleStatus.Missed)
                    .Select(s => s.Dose.DoseName)
                    .ToList();

                int? assignedWorkerId = computedAssignments.TryGetValue(fvi.VaccinatedIndividualId, out var wId) ? wId : null;

                return new FieldVisitTargetedIndividualDto(
                    ind.Id,
                    ind.Person?.FullName ?? string.Empty,
                    ind.CardNumber,
                    ind.Person?.PhoneNumber ?? string.Empty,
                    doses,
                    assignedWorkerId
                );
            }).Where(x => x != null).ToList();

            var selectedIds = selectedIndividualsToMap
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

            var notGoingWorkerIds = (fv.FieldVisitWorkers ?? Enumerable.Empty<FieldVisitWorker>())
                .Where(fvw => !fvw.IsGoing)
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
                selectedWorkerIds,
                notGoingWorkerIds
            );
        }

        public static FieldVisitVaccinationsDto ToVaccinationsDto(this FieldVisit fieldVisit, IEnumerable<ImmunizationRecord> records)
        {
            // Group by vaccinated individual
            var vaccinatedPersons = records
                .GroupBy(ir => ir.IndividualId)
                .Select(group =>
                {
                    var firstRecord = group.First();
                    var ind = firstRecord.VaccinatedIndividual;
                    if (ind == null) return null!;

                    var doseNames = group
                        .Select(ir => ir.Dose?.DoseName ?? string.Empty)
                        .Where(name => !string.IsNullOrEmpty(name))
                        .Distinct()
                        .ToList();

                    var individualAdministeredBy = group
                        .Select(ir => ir.User)
                        .Where(u => u != null)
                        .GroupBy(u => u.Id)
                        .Select(g => g.First())
                        .Select(user =>
                        {
                            var firstName = user!.Person?.FirstName ?? string.Empty;
                            var lastName  = user.Person?.LastName  ?? string.Empty;

                            return new FieldVisitWorkerDto(
                                user.Id,
                                firstName,
                                lastName,
                                $"{firstName} {lastName}".Trim(),
                                user.UserName
                            );
                        })
                        .ToList();

                    return new FieldVisitVaccinatedPersonDto(
                        ind.Id,
                        ind.Person?.FullName ?? string.Empty,
                        ind.CardNumber,
                        ind.Person?.PhoneNumber ?? string.Empty,
                        doseNames,
                        individualAdministeredBy
                    );
                })
                .Where(x => x != null)
                .ToList();

            // Project field workers who administered vaccinations in this visit
            var administeredBy = records
                .Select(ir => ir.User)
                .Where(u => u != null)
                .GroupBy(u => u.Id)
                .Select(g => g.First())
                .Select(user =>
                {
                    var firstName = user!.Person?.FirstName ?? string.Empty;
                    var lastName  = user.Person?.LastName  ?? string.Empty;

                    return new FieldVisitWorkerDto(
                        user.Id,
                        firstName,
                        lastName,
                        $"{firstName} {lastName}".Trim(),
                        user.UserName
                    );
                })
                .ToList();

            return new FieldVisitVaccinationsDto(
                fieldVisit.Id,
                fieldVisit.CampaignName,
                fieldVisit.VisitDate,
                vaccinatedPersons.Count,
                vaccinatedPersons,
                administeredBy
            );
        }
    }
}
