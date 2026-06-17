using ICMS.Application.DTOs.FieldVisit;
using ICMS.Application.DTOs.Pagination;
using ICMS.Domain.ValueObjects;

namespace ICMS.Application.Interfaces.Services
{
    public interface IFieldVisitService
    {
        Task<PagedResult<FieldVisitReadDto>> GetAllAsync(PaginationParams paginationParams, bool? onlyUncompleted, int? workerId = null, CancellationToken ct = default);

        Task<FieldVisitDetailsDto> GetByIdAsync(int id, int? workerId = null, CancellationToken ct = default);

        Task<bool> ToggleWorkerGoingAsync(int id, int workerId, CancellationToken ct = default);

        Task<FieldVisitVaccinationsDto> GetVaccinationsAsync(int id, CancellationToken ct = default);

        Task<FieldVisitReadDto> AddAsync(FieldVisitCreateDto dto, CancellationToken ct = default);

        Task<bool> UpdateAsync(int id, FieldVisitCreateDto dto, CancellationToken ct = default);

        Task<bool> DeleteAsync(int id, CancellationToken ct = default);

        Task<bool> MarkCompletedAsync(int id, CancellationToken ct = default);

        Task<int> CloseExpiredVisitsAsync(CancellationToken ct = default);

        Task<bool> SendWorkerNotificationsAsync(int id, CancellationToken ct = default);

        Task<bool> ShiftWorkerPeopleAsync(int fieldVisitId, int fromWorkerId, int toWorkerId, CancellationToken ct = default);
    }
}
