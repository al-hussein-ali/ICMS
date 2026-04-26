using ICMS.Application.DTOs.VisitDetails;
using ICMS.Application.DTOs.Pagination;
using ICMS.Application.Interfaces.Services;

namespace ICMS.Application.Services;

public class VisitDetailsService : IVisitDetailsService
{
    public async Task<IReadOnlyList<VisitDetailsReadDto>> GetAllAsync(PaginationParams paginationParams, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public async Task<VisitDetailsReadDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public async Task<VisitDetailsReadDto> AddAsync(VisitDetailsCreateDto entity, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> UpdateAsync(int id, VisitDetailsCreateDto updatedEntity, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}
