using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICMS.Application.DTOs.Maternal;
using ICMS.Application.DTOs.Pagination;

namespace ICMS.Application.Interfaces.Services
{
    public interface IPreviousPregnancyDeliveryComplicationsService
    {
        Task<IReadOnlyList<PreviousPregnancyDeliveryComplicationsDto>> GetAllAsync(PaginationParams paginationParams, CancellationToken ct = default);

        Task<PreviousPregnancyDeliveryComplicationsDto?> GetByIdAsync(int id, CancellationToken ct = default);

        Task<PreviousPregnancyDeliveryComplicationsDto> AddAsync(PreviousPregnancyDeliveryComplicationsDto entity, CancellationToken ct = default);

        Task<bool> UpdateAsync(int id, PreviousPregnancyDeliveryComplicationsDto updatedEntity, CancellationToken ct = default);

        Task<bool> DeleteAsync(int id, PreviousPregnancyDeliveryComplicationsDto entity, CancellationToken ct = default);
    }
}
