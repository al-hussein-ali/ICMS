using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICMS.Application.DTOs.Maternal;
using ICMS.Application.DTOs.Pagination;

namespace ICMS.Application.Interfaces.Services
{
    public interface IPregnantWomanService
    {
        Task<IReadOnlyList<PregnantWomanReadDto>> GetAllAsync(PaginationParams paginationParams, CancellationToken ct = default);

        Task<PregnantWomanReadDto?> GetByIdAsync(int id, CancellationToken ct = default);

        Task<PregnantWomanReadDto> AddAsync(PregnantWomanCreateDto entity, CancellationToken ct = default);

        Task<bool> UpdateAsync(int id, PregnantWomanCreateDto updatedEntity, CancellationToken ct = default);

        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
