using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICMS.Application.DTOs;
using ICMS.Application.DTOs.Pagination;

namespace ICMS.Application.Interfaces.Services
{
    public interface IPregnantWomanService
    {
        Task<IReadOnlyList<TempDto>> GetAllAsync(PaginationParams paginationParams, CancellationToken ct = default);

        Task<TempDto?> GetByIdAsync(int id, CancellationToken ct = default);

        Task<TempDto> AddAsync(TempDto entity, CancellationToken ct = default);

        Task<bool> UpdateAsync(TempDto updatedEntity, CancellationToken ct = default);

        Task<bool> DeleteAsync(TempDto entity, CancellationToken ct = default);
    }
}
