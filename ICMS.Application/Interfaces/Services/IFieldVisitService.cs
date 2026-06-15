using ICMS.Application.DTOs.FieldVisit;
using ICMS.Application.DTOs.Pagination;
using ICMS.Domain.ValueObjects;

namespace ICMS.Application.Interfaces.Services
{
    public interface IFieldVisitService
    {
        Task<PagedResult<FieldVisitReadDto>> GetAllAsync(PaginationParams paginationParams, bool? onlyUncompleted, int? workerId = null, CancellationToken ct = default);

        Task<FieldVisitDetailsDto> GetByIdAsync(int id, CancellationToken ct = default);

        Task<FieldVisitVaccinationsDto> GetVaccinationsAsync(int id, CancellationToken ct = default);

        Task<FieldVisitReadDto> AddAsync(FieldVisitCreateDto dto, CancellationToken ct = default);

        Task<bool> UpdateAsync(int id, FieldVisitCreateDto dto, CancellationToken ct = default);

        Task<bool> DeleteAsync(int id, CancellationToken ct = default);

        Task<bool> MarkCompletedAsync(int id, CancellationToken ct = default);
    }
}
