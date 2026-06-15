using ICMS.Application.DTOs.FieldVisit;
using ICMS.Application.DTOs.Pagination;
using ICMS.Application.Interfaces;
using ICMS.Application.Interfaces.Services;
using ICMS.Application.Extensions;
using ICMS.Domain.Entites.Visits;
using ICMS.Domain.Exceptions;
using ICMS.Domain.ValueObjects;

using FluentValidation;

namespace ICMS.Application.Services
{
    public class FieldVisitService : IFieldVisitService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<FieldVisitCreateDto> _validator;

        public FieldVisitService(IUnitOfWork unitOfWork, IValidator<FieldVisitCreateDto> validator)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
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

        public async Task<FieldVisitDetailsDto> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var fieldVisit = await _unitOfWork.FieldVisitRepository.GetByIdWithDetailsAsync(id, ct);

            if (fieldVisit == null)
                throw new NotFoundException("NotFound");

            return fieldVisit.ToDetailsDto();
        }

        /// <summary>
        /// Returns a purpose-built vaccination summary for a given field visit:
        /// the full list of targeted/vaccinated individuals and the workers who
        /// administered the vaccines. Reuses GetByIdWithDetailsAsync — no extra DB hit.
        /// </summary>
        public async Task<FieldVisitVaccinationsDto> GetVaccinationsAsync(int id, CancellationToken ct = default)
        {
            var fieldVisit = await _unitOfWork.FieldVisitRepository.GetByIdWithDetailsAsync(id, ct);

            if (fieldVisit == null)
                throw new NotFoundException("NotFound");

            // Project vaccinated individuals
            var vaccinatedPersons = (fieldVisit.FieldVisitIndividuals ?? Enumerable.Empty<FieldVisitIndividual>())
                .Select(fvi =>
                {
                    var ind = fvi.VaccinatedIndividual;
                    if (ind == null) return null!;

                    var doses = (ind.Schedules ?? Enumerable.Empty<ICMS.Domain.Entites.Clinical.VaccinationSchedule>())
                        .Where(s => s.Status == ICMS.Domain.Enums.ScheduleStatus.Missed)
                        .Select(s => s.Dose.DoseName)
                        .ToList();

                    return new FieldVisitVaccinatedPersonDto(
                        ind.Id,
                        ind.Person?.FullName ?? string.Empty,
                        ind.CardNumber,
                        ind.Person?.PhoneNumber ?? string.Empty,
                        doses
                    );
                })
                .Where(x => x != null)
                .ToList();

            // Project field workers who administered vaccinations
            var administeredBy = (fieldVisit.FieldVisitWorkers ?? Enumerable.Empty<FieldVisitWorker>())
                .Select(fvw =>
                {
                    var user = fvw.User;
                    if (user == null) return null!;

                    var firstName = user.Person?.FirstName ?? string.Empty;
                    var lastName  = user.Person?.LastName  ?? string.Empty;

                    return new FieldVisitWorkerDto(
                        user.Id,
                        firstName,
                        lastName,
                        $"{firstName} {lastName}".Trim(),
                        user.UserName
                    );
                })
                .Where(x => x != null)
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

        public async Task<FieldVisitReadDto> AddAsync(FieldVisitCreateDto dto, CancellationToken ct = default)
        {
            await _validator.ValidateAndThrowAsync(dto, ct);
            var fieldVisit = FieldVisit.Create(
                dto.CampaignName,
                dto.VisitDate,
                dto.SubNeighborhoodId,
                dto.FromDate,
                dto.ToDate);

            foreach (var individualId in dto.SelectedIndividualIds ?? new List<int>())
            {
                fieldVisit.AddIndividual(individualId);
            }

            foreach (var workerId in dto.SelectedWorkerIds ?? new List<int>())
            {
                fieldVisit.AddWorker(workerId);
            }

            await _unitOfWork.FieldVisitRepository.AddAsync(fieldVisit, ct);
            await _unitOfWork.SaveChangesAsync(ct);

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
            foreach (var individualId in dto.SelectedIndividualIds ?? new List<int>())
            {
                fieldVisit.AddIndividual(individualId);
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
    }
}
