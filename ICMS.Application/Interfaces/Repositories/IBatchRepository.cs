using ICMS.Application.DTOs.Batch;
using ICMS.Application.DTOs.Pagination;
using ICMS.Domain.Entites.Clinical;
using ICMS.Domain.ValueObjects;
using System.Threading;
using System.Threading.Tasks;

namespace ICMS.Application.Interfaces.Repositories
{
    public interface IBatchRepository : IRepository<Batch, int>
    {
        Task<PagedResult<Batch>> GetAllAsync(BatchFilterDto filter, PaginationParams paginationParams, CancellationToken ct = default);
        Task<Batch?> GetByIdWithDetailsAsync(int id, CancellationToken ct = default, bool track = false);
    }
}
