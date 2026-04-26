using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICMS.Application.DTOs.Maternal;
using ICMS.Application.DTOs.Pagination;

namespace ICMS.Application.Interfaces.Services
{
    public interface IPregnancyDetailsService
    {
        Task<IReadOnlyList<PregnancyDetailsReadDto>> GetAllAsync(PaginationParams paginationParams, CancellationToken ct = default);

        Task<PregnancyDetailsReadDto?> GetByIdAsync(int id, CancellationToken ct = default);

        Task<PregnancyDetailsReadDto> AddAsync(PregnancyDetailsReadDto entity, CancellationToken ct = default);

        Task<bool> UpdateAsync(int id, PregnancyDetailsReadDto updatedEntity, CancellationToken ct = default);

        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
