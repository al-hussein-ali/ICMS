using ICMS.Domain.ValueObjects;
using ICMS.Application.DTOs.HealthAdvisory;
using System.Threading;
using System.Threading.Tasks;

namespace ICMS.Application.Interfaces.Services
{
    public interface IHealthAdvisoryService
    {
        Task<PagedResult<HealthAdvisoryReadDto>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken ct = default);
        Task<HealthAdvisoryDetailsDto> GetByIdAsync(int id, CancellationToken ct = default);
        Task<HealthAdvisoryDetailsDto> CreateAsync(HealthAdvisoryCreateDto dto, int currentUserId, CancellationToken ct = default);
        Task<HealthAdvisoryDetailsDto> CreateAndSendNowAsync(HealthAdvisoryCreateDto dto, int currentUserId, CancellationToken ct = default);
        Task<HealthAdvisoryDetailsDto> UpdateAsync(int id, HealthAdvisoryCreateDto dto, CancellationToken ct = default);
        Task<HealthAdvisoryDetailsDto> UpdateAndSendNowAsync(int id, HealthAdvisoryCreateDto dto, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
    }
}
