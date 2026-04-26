using ICMS.Application.DTOs.Maternal;
using ICMS.Application.DTOs.Pagination;
using ICMS.Application.Interfaces.Services;

namespace ICMS.Application.Services;

public class PreviousPregnancyDeliveryComplicationsService : IPreviousPregnancyDeliveryComplicationsService
{
    public async Task<IReadOnlyList<PreviousPregnancyDeliveryComplicationsDto>> GetAllAsync(PaginationParams paginationParams, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public async Task<PreviousPregnancyDeliveryComplicationsDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public async Task<PreviousPregnancyDeliveryComplicationsDto> AddAsync(PreviousPregnancyDeliveryComplicationsDto entity, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> UpdateAsync(int id, PreviousPregnancyDeliveryComplicationsDto updatedEntity, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteAsync(int id, PreviousPregnancyDeliveryComplicationsDto entity, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}
