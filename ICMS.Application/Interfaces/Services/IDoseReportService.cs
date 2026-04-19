using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ICMS.Application.DTOs.DoseReport;
using ICMS.Application.DTOs.Pagination;
using ICMS.Domain.ValueObjects;

namespace ICMS.Application.Interfaces.Services
{
    public interface IDoseReportService
    {
        Task<PagedResult<DoseReportReadDto>> GetAllAsync(PaginationParams paginationParams, CancellationToken ct = default);

        Task<DoseReportReadDto?> GetByIdAsync(int id, CancellationToken ct = default);

        Task<DoseReportReadDto> AddAsync(DoseReportCreateDto entity, int userId, CancellationToken ct = default);

        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
