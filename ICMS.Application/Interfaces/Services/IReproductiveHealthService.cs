using System.Threading;
using System.Threading.Tasks;
using ICMS.Application.DTOs.Maternal;
using ICMS.Application.DTOs.Account;

namespace ICMS.Application.Interfaces.Services
{
    public interface IReproductiveHealthService
    {
        Task<ICMS.Domain.ValueObjects.PagedResult<PregnantWomanReadDto>> GetAllPregnantWomenAsync(
            ICMS.Application.DTOs.Pagination.PaginationParams paginationParams, CancellationToken ct = default);

        Task<PregnantWomanReadDto> GetPregnantWomanByIdAsync(int id, int? authenticatedUserId = null, string? userRole = null, CancellationToken ct = default);
        Task<PregnantWomanDetailsDto> GetPregnantWomanDetailsAsync(int id, int? authenticatedUserId = null, string? userRole = null, CancellationToken ct = default);

        Task<PregnantWomanReadDto> CreatePregnantWomanAsync(PregnantWomanCreateDto request,
            CancellationToken ct = default);

        Task<bool> UpdatePregnantWomanAsync(int id, PregnantWomanCreateDto request, CancellationToken ct = default);
        Task<bool> DeletePregnantWomanAsync(int id, CancellationToken ct = default);

        Task StartPregnancyAsync(StartPregnancyDto request, int userId, CancellationToken ct = default);
        Task AddAncVisitAsync(int pregnancyId, AddAncVisitDto request, int userId, CancellationToken ct = default);

        Task ConcludePregnancyAsync(int pregnancyId, ConcludePregnancyDto request, int userId,
            CancellationToken ct = default);

        Task<GeneratedAccountDto> GenerateAccountAsync(int id, CancellationToken ct = default);

        Task<List<PregnancyDetailsReadDto>> GetPregnancyHistoryAsync(int pregnantWomanId,
            int? authenticatedUserId = null, string? userRole = null, CancellationToken ct = default);

        Task<PregnancyDetailsReadDto> GetPregnancyByIdAsync(int id, int? authenticatedUserId = null, string? userRole = null, CancellationToken ct = default);
        Task<List<AddAncVisitDto>> GetVisitsAsync(int pregnancyId, int? authenticatedUserId = null, string? userRole = null, CancellationToken ct = default);
        Task<AddAncVisitDto> GetVisitByIdAsync(int visitId, int? authenticatedUserId = null, string? userRole = null, CancellationToken ct = default);

        Task UpdatePregnancyAsync(int id, UpdatePregnancyDto request, CancellationToken ct = default);
        Task DeletePregnancyAsync(int id, CancellationToken ct = default);
        Task UpdateVisitAsync(int id, AddAncVisitDto request, CancellationToken ct = default);
        Task DeleteVisitAsync(int id, CancellationToken ct = default);
        Task<int?> GetWomanIdByUserIdAsync(int userId, CancellationToken ct = default);
        Task<ReproductiveHealthStatisticsDto> GetStatisticsAsync(CancellationToken ct = default);
    }
}
