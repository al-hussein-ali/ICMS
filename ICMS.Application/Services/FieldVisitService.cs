using ICMS.Application.DTOs.FieldVisit;
using ICMS.Application.DTOs.Pagination;
using ICMS.Application.Interfaces;
using ICMS.Application.Interfaces.Services;
using ICMS.Application.Extensions;
using ICMS.Domain.Entites.Visits;
using ICMS.Domain.Exceptions;
using ICMS.Domain.ValueObjects;

using FluentValidation;
using Microsoft.Extensions.Logging;

namespace ICMS.Application.Services
{
    public class FieldVisitService : IFieldVisitService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<FieldVisitCreateDto> _validator;
        private readonly IPushNotificationService _pushNotificationService;
        private readonly ILocalizer _localizer;
        private readonly ILogger<FieldVisitService> _logger;

        public FieldVisitService(
            IUnitOfWork unitOfWork,
            IValidator<FieldVisitCreateDto> validator,
            IPushNotificationService pushNotificationService,
            ILocalizer localizer,
            ILogger<FieldVisitService> logger)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
            _pushNotificationService = pushNotificationService;
            _localizer = localizer;
            _logger = logger;
        }

        public async Task<PagedResult<FieldVisitReadDto>> GetAllAsync(PaginationParams paginationParams,
            bool? onlyUncompleted = null, int? workerId = null, CancellationToken ct = default)
        {
            var pagedResult =
                await _unitOfWork.FieldVisitRepository.GetPagedWithDetailsAsync(paginationParams.PageNumber,
                    paginationParams.PageSize, onlyUncompleted, workerId, ct);

            var items = pagedResult.Items.Select(fv => fv.ToReadDto()).ToList();

            return new PagedResult<FieldVisitReadDto>(items, pagedResult.TotalCount, pagedResult.PageNumber,
                pagedResult.PageSize);
        }

        public async Task<FieldVisitDetailsDto> GetByIdAsync(int id, int? workerId = null, CancellationToken ct = default)
        {
            var fieldVisit = await _unitOfWork.FieldVisitRepository.GetByIdWithDetailsAsync(id, ct);

            if (fieldVisit == null)
                throw new NotFoundException("NotFound");

            return fieldVisit.ToDetailsDto(workerId);
        }

        public async Task<bool> ToggleWorkerGoingAsync(int id, int workerId, CancellationToken ct = default)
        {
            var fieldVisit = await _unitOfWork.FieldVisitRepository.GetByIdWithDetailsAsync(id, ct);
            if (fieldVisit == null)
                throw new NotFoundException("Field Visit Not Found");

            var worker = fieldVisit.FieldVisitWorkers.FirstOrDefault(w => w.UserId == workerId);
            if (worker == null)
                throw new NotFoundException("Worker not assigned to this field visit");

            worker.IsGoing = !worker.IsGoing;

            var allIndividuals = fieldVisit.FieldVisitIndividuals.OrderBy(fvi => fvi.VaccinatedIndividualId).ToList();
            var activeWorkers = fieldVisit.FieldVisitWorkers.Where(w => w.IsGoing).OrderBy(w => w.UserId).ToList();

            if (!worker.IsGoing)
            {
                foreach (var ind in allIndividuals.Where(i => i.AssignedWorkerId == worker.UserId))
                {
                    ind.AssignedWorkerId = null;
                }
            }

            var workerAssignments = activeWorkers.ToDictionary(w => w.UserId, w => new List<FieldVisitIndividual>());
            foreach (var ind in allIndividuals)
            {
                if (ind.AssignedWorkerId.HasValue && workerAssignments.ContainsKey(ind.AssignedWorkerId.Value))
                {
                    workerAssignments[ind.AssignedWorkerId.Value].Add(ind);
                }
            }

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
                    foreach (var ind in chunk)
                    {
                        ind.AssignedWorkerId = activeWorkers[i].UserId;
                    }
                    start += count;
                }
            }

            await _unitOfWork.FieldVisitRepository.UpdateAsync(fieldVisit, ct);
            return await _unitOfWork.SaveChangesAsync(ct) > 0;
        }

        /// <summary>
        /// Returns a purpose-built vaccination summary for a given field visit:
        /// the full list of targeted/vaccinated individuals and the workers who
        /// administered the vaccines. Reuses GetByIdWithDetailsAsync — no extra DB hit.
        /// </summary>
        public async Task<FieldVisitVaccinationsDto> GetVaccinationsAsync(int id, CancellationToken ct = default)
        {
            var fieldVisit = await _unitOfWork.FieldVisitRepository.GetByIdAsync(id, ct);

            if (fieldVisit == null)
                throw new NotFoundException("NotFound");

            // Query immunization records for this field visit with all required details
            var records = await _unitOfWork.ImmunizationRecordRepository.GetRecordsWithDetailsForVisitAsync(id, ct);

            return fieldVisit.ToVaccinationsDto(records);
        }

        public async Task<FieldVisitReadDto> AddAsync(FieldVisitCreateDto dto, CancellationToken ct = default)
        {
            await _validator.ValidateAndThrowAsync(dto, ct);
            var fieldVisit = FieldVisit.Create(
                dto.CampaignName,
                dto.VisitDate,
                dto.SubNeighborhoodId,
                dto.FromDate,
                dto.ToDate);

            var individuals = dto.SelectedIndividualIds ?? new List<int>();
            var workers = dto.SelectedWorkerIds ?? new List<int>();
            int n = individuals.Count;
            int m = workers.Count;

            if (m > 0 && n > 0)
            {
                int baseCount = n / m;
                int remainder = n % m;
                int start = 0;

                for (int i = 0; i < m; i++)
                {
                    int count = baseCount + (i < remainder ? 1 : 0);
                    var chunk = individuals.Skip(start).Take(count);
                    foreach (var ind in chunk)
                    {
                        fieldVisit.AddIndividual(ind, workers[i]);
                    }
                    start += count;
                }
            }
            else
            {
                foreach (var individualId in individuals)
                {
                    fieldVisit.AddIndividual(individualId);
                }
            }

            foreach (var workerId in dto.SelectedWorkerIds ?? new List<int>())
            {
                fieldVisit.AddWorker(workerId);
            }

            await _unitOfWork.FieldVisitRepository.AddAsync(fieldVisit, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            // Notify workers who are assigned to contribute to the field visit
            try
            {
                var workerUserIds = dto.SelectedWorkerIds ?? new List<int>();
                if (workerUserIds.Any())
                {
                    var deviceTokens = await _unitOfWork.UserDeviceRepository.GetFcmTokensForUsersAsync(workerUserIds, ct);

                    if (deviceTokens.Any())
                    {
                        var subNeighborhood = await _unitOfWork.SubNeighborhoodRepository.GetByIdAsync(dto.SubNeighborhoodId, ct);
                        var placeName = subNeighborhood?.Name ?? "Unknown";
                        var dateStr = dto.VisitDate.ToString("yyyy-MM-dd");

                        var title = _localizer["NewFieldVisitAssignmentTitle"];
                        var body = _localizer["NewFieldVisitAssignmentBody", dto.CampaignName, dateStr, placeName];

                        var data = new Dictionary<string, string>
                        {
                            { "type", "field_visit_assignment" },
                            { "visitId", fieldVisit.Id.ToString() }
                        };

                        _logger.LogInformation("Sending push notifications to {Count} worker devices for FieldVisit ID {Id}", deviceTokens.Count, fieldVisit.Id);
                        var pushResult = await _pushNotificationService.SendMulticastNotificationAsync(
                            deviceTokens,
                            title,
                            body,
                            null,
                            data,
                            ct);

                        if (pushResult.UnregisteredTokens.Any())
                        {
                            _logger.LogInformation("Cleaning up {Count} expired worker FCM tokens.", pushResult.UnregisteredTokens.Count);
                            var expiredDevices = await _unitOfWork.UserDeviceRepository.GetDevicesByTokensAsync(pushResult.UnregisteredTokens, ct);

                            foreach (var device in expiredDevices)
                            {
                                await _unitOfWork.UserDeviceRepository.DeleteAsync(device, ct);
                            }
                            await _unitOfWork.SaveChangesAsync(ct);
                        }
                    }
                    else
                    {
                        _logger.LogWarning("No registered devices found for assigned workers of FieldVisit ID {Id}", fieldVisit.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send notifications to workers for FieldVisit ID {Id}. Custom notification failure was caught and swallowed.", fieldVisit.Id);
            }

            return fieldVisit.ToReadDto();
        }

        public async Task<bool> UpdateAsync(int id, FieldVisitCreateDto dto, CancellationToken ct = default)
        {
            await _validator.ValidateAndThrowAsync(dto, ct);
            var fieldVisit = await _unitOfWork.FieldVisitRepository.GetByIdWithDetailsAsync(id, ct);

            if (fieldVisit == null)
                throw new NotFoundException("NotFound");

            fieldVisit.UpdateVisitInfo(
                dto.CampaignName,
                dto.VisitDate,
                dto.SubNeighborhoodId,
                dto.FromDate,
                dto.ToDate);

            fieldVisit.ClearIndividuals();
            var individuals = dto.SelectedIndividualIds ?? new List<int>();
            var workers = dto.SelectedWorkerIds ?? new List<int>();
            int n = individuals.Count;
            int m = workers.Count;

            if (m > 0 && n > 0)
            {
                int baseCount = n / m;
                int remainder = n % m;
                int start = 0;

                for (int i = 0; i < m; i++)
                {
                    int count = baseCount + (i < remainder ? 1 : 0);
                    var chunk = individuals.Skip(start).Take(count);
                    foreach (var ind in chunk)
                    {
                        fieldVisit.AddIndividual(ind, workers[i]);
                    }
                    start += count;
                }
            }
            else
            {
                foreach (var individualId in individuals)
                {
                    fieldVisit.AddIndividual(individualId);
                }
            }

            fieldVisit.ClearWorkers();
            foreach (var workerId in dto.SelectedWorkerIds ?? new List<int>())
            {
                fieldVisit.AddWorker(workerId);
            }

            return await _unitOfWork.SaveChangesAsync(ct) > 0;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var fieldVisit = await _unitOfWork.FieldVisitRepository.GetByIdAsync(id, ct);

            if (fieldVisit == null)
                throw new NotFoundException("NotFound");

            await _unitOfWork.FieldVisitRepository.DeleteAsync(fieldVisit, ct);

            return await _unitOfWork.SaveChangesAsync(ct) > 0;
        }

        public async Task<bool> MarkCompletedAsync(int id, CancellationToken ct = default)
        {
            var fieldVisit = await _unitOfWork.FieldVisitRepository.GetByIdAsync(id, ct);

            if (fieldVisit == null)
                throw new NotFoundException("Field Visit Not Found");

            fieldVisit.MarkCompleted();
            await _unitOfWork.FieldVisitRepository.UpdateAsync(fieldVisit, ct);

            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result >= 0;
        }

        public async Task<int> CloseExpiredVisitsAsync(CancellationToken ct = default)
        {
            _logger.LogInformation("Starting expired field visits auto-closure job at {Time} (UTC)", DateTime.UtcNow);
            
            var threeDaysAgo = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-3));
            var expiredVisits = await _unitOfWork.FieldVisitRepository.GetExpiredUncompletedVisitsAsync(threeDaysAgo, ct);

            if (!expiredVisits.Any())
            {
                _logger.LogInformation("No uncompleted field visits found that have been expired for more than 3 days.");
                return 0;
            }

            _logger.LogInformation("Found {Count} expired field visits to close.", expiredVisits.Count);

            foreach (var visit in expiredVisits)
            {
                _logger.LogInformation("Closing expired field visit ID {Id} ('{CampaignName}') - Expiration date was {ToDate}", visit.Id, visit.CampaignName, visit.ToDate);
                visit.MarkCompleted();
                await _unitOfWork.FieldVisitRepository.UpdateAsync(visit, ct);
            }

            var result = await _unitOfWork.SaveChangesAsync(ct);
            _logger.LogInformation("Successfully closed {Count} expired field visits.", result);
            return result;
        }

        public async Task<bool> SendWorkerNotificationsAsync(int id, CancellationToken ct = default)
        {
            var fieldVisit = await _unitOfWork.FieldVisitRepository.GetByIdWithDetailsAsync(id, ct);
            if (fieldVisit == null)
                throw new NotFoundException("Field Visit Not Found");

            var workerUserIds = fieldVisit.FieldVisitWorkers.Select(w => w.UserId).ToList();
            if (!workerUserIds.Any())
            {
                _logger.LogWarning("No workers assigned to FieldVisit ID {Id}", fieldVisit.Id);
                return false;
            }

            var deviceTokens = await _unitOfWork.UserDeviceRepository.GetFcmTokensForUsersAsync(workerUserIds, ct);

            if (!deviceTokens.Any())
            {
                _logger.LogWarning("No registered devices found for assigned workers of FieldVisit ID {Id}", fieldVisit.Id);
                return false;
            }

            var subNeighborhood = await _unitOfWork.SubNeighborhoodRepository.GetByIdAsync(fieldVisit.SubNeighborhoodId, ct);
            var placeName = subNeighborhood?.Name ?? "Unknown";
            var dateStr = fieldVisit.VisitDate.ToString("yyyy-MM-dd");

            var title = _localizer["FieldVisitAssignmentTitle"];
            var body = _localizer["FieldVisitAssignmentBody", fieldVisit.CampaignName, dateStr, placeName];

            var data = new Dictionary<string, string>
            {
                { "type", "field_visit_assignment" },
                { "visitId", fieldVisit.Id.ToString() }
            };

            _logger.LogInformation("Sending push notifications to {Count} worker devices for FieldVisit ID {Id}", deviceTokens.Count, fieldVisit.Id);
            var pushResult = await _pushNotificationService.SendMulticastNotificationAsync(
                deviceTokens,
                title,
                body,
                null,
                data,
                ct);

            if (pushResult.UnregisteredTokens.Any())
            {
                _logger.LogInformation("Cleaning up {Count} expired worker FCM tokens.", pushResult.UnregisteredTokens.Count);
                var expiredDevices = await _unitOfWork.UserDeviceRepository.GetDevicesByTokensAsync(pushResult.UnregisteredTokens, ct);

                foreach (var device in expiredDevices)
                {
                    await _unitOfWork.UserDeviceRepository.DeleteAsync(device, ct);
                }
                await _unitOfWork.SaveChangesAsync(ct);
            }

            return pushResult.IsSuccess;
        }

        public async Task<bool> ShiftWorkerPeopleAsync(int fieldVisitId, int fromWorkerId, int toWorkerId, CancellationToken ct = default)
        {
            var fieldVisit = await _unitOfWork.FieldVisitRepository.GetByIdWithDetailsAsync(fieldVisitId, ct);
            if (fieldVisit == null)
                throw new NotFoundException("Field Visit Not Found");

            var activeWorkers = fieldVisit.FieldVisitWorkers
                .Where(w => w.IsGoing)
                .OrderBy(w => w.UserId)
                .ToList();

            if (!activeWorkers.Any(w => w.UserId == toWorkerId))
                throw new DomainException("Destination worker must be assigned and active.");

            var allIndividuals = fieldVisit.FieldVisitIndividuals
                .OrderBy(fvi => fvi.VaccinatedIndividualId)
                .ToList();

            // If none of the individuals have an assigned worker, we materialize the current dynamic partition first
            if (allIndividuals.All(fvi => fvi.AssignedWorkerId == null))
            {
                int n = allIndividuals.Count;
                int m = activeWorkers.Count;
                if (m > 0 && n > 0)
                {
                    int baseCount = n / m;
                    int remainder = n % m;
                    int start = 0;
                    for (int i = 0; i < m; i++)
                    {
                        int count = baseCount + (i < remainder ? 1 : 0);
                        var chunk = allIndividuals.Skip(start).Take(count);
                        foreach (var ind in chunk)
                        {
                            ind.AssignedWorkerId = activeWorkers[i].UserId;
                        }
                        start += count;
                    }
                }
            }
            else
            {
                // If some individuals are already assigned, but some are null (e.g. newly added individuals),
                // we should materialize the null ones using the dynamic partition first so that all individuals have an AssignedWorkerId
                // before we do the shift.
                var workerAssignments = activeWorkers.ToDictionary(w => w.UserId, w => new List<FieldVisitIndividual>());
                foreach (var ind in allIndividuals)
                {
                    if (ind.AssignedWorkerId.HasValue && workerAssignments.ContainsKey(ind.AssignedWorkerId.Value))
                    {
                        workerAssignments[ind.AssignedWorkerId.Value].Add(ind);
                    }
                }

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
                        foreach (var ind in chunk)
                        {
                            ind.AssignedWorkerId = activeWorkers[i].UserId;
                        }
                        start += count;
                    }
                }
            }

            // Now perform the shift
            foreach (var ind in allIndividuals)
            {
                if (ind.AssignedWorkerId == fromWorkerId)
                {
                    ind.AssignedWorkerId = toWorkerId;
                }
            }

            await _unitOfWork.FieldVisitRepository.UpdateAsync(fieldVisit, ct);
            return await _unitOfWork.SaveChangesAsync(ct) > 0;
        }

        public async Task<object> GetDiagnosticDbAsync(CancellationToken ct = default)
        {
            return await _unitOfWork.FieldVisitRepository.GetDiagnosticDbAsync(ct);
        }
    }
}
