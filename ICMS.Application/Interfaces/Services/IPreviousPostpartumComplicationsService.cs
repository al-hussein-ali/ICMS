using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICMS.Application.DTOs.Maternal;
using ICMS.Application.DTOs.Pagination;

namespace ICMS.Application.Interfaces.Services
{
    public interface IPreviousPostpartumComplicationsService
    {
        Task<IReadOnlyList<PreviousPostpartumComplicationsDto>> GetAllAsync(PaginationParams paginationParams, CancellationToken ct = default);

        Task<PreviousPostpartumComplicationsDto?> GetByIdAsync(int id, CancellationToken ct = default);

        Task<PreviousPostpartumComplicationsDto> AddAsync(PreviousPostpartumComplicationsDto entity, CancellationToken ct = default);

        Task<bool> UpdateAsync(int id, PreviousPostpartumComplicationsDto updatedEntity, CancellationToken ct = default);

        Task<bool> DeleteAsync(int id, PreviousPostpartumComplicationsDto entity, CancellationToken ct = default);
    }
}
