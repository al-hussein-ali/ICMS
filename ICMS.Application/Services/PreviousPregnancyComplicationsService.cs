using ICMS.Application.DTOs.Maternal;
using ICMS.Application.DTOs.Pagination;
using ICMS.Application.Interfaces.Services;

namespace ICMS.Application.Services;

public class PreviousPregnancyComplicationsService : IPreviousPregnancyComplicationsService
{
    public async Task<IReadOnlyList<PreviousPregnancyComplicationsDto>> GetAllAsync(PaginationParams paginationParams, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public async Task<PreviousPregnancyComplicationsDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public async Task<PreviousPregnancyComplicationsDto> AddAsync(PreviousPregnancyComplicationsDto entity, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> UpdateAsync(int id, PreviousPregnancyComplicationsDto updatedEntity, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}
