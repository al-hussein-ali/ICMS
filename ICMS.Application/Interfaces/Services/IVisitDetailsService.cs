using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICMS.Application.DTOs.VisitDetails;
using ICMS.Application.DTOs.Pagination;

namespace ICMS.Application.Interfaces.Services
{
    public interface IVisitDetailsService
    {
        Task<IReadOnlyList<VisitDetailsReadDto>> GetAllAsync(PaginationParams paginationParams, CancellationToken ct = default);

        Task<VisitDetailsReadDto?> GetByIdAsync(int id, CancellationToken ct = default);

        Task<VisitDetailsReadDto> AddAsync(VisitDetailsCreateDto entity, CancellationToken ct = default);

        Task<bool> UpdateAsync(int id, VisitDetailsCreateDto updatedEntity, CancellationToken ct = default);

        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
