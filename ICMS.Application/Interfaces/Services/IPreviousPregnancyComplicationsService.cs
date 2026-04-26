using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICMS.Application.DTOs.Maternal;
using ICMS.Application.DTOs.Pagination;

namespace ICMS.Application.Interfaces.Services
{
    public interface IPreviousPregnancyComplicationsService
    {
        Task<IReadOnlyList<PreviousPregnancyComplicationsDto>> GetAllAsync(PaginationParams paginationParams, CancellationToken ct = default);

        Task<PreviousPregnancyComplicationsDto?> GetByIdAsync(int id, CancellationToken ct = default);

        Task<PreviousPregnancyComplicationsDto> AddAsync(PreviousPregnancyComplicationsDto entity, CancellationToken ct = default);

        Task<bool> UpdateAsync(int id, PreviousPregnancyComplicationsDto updatedEntity, CancellationToken ct = default);

        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
