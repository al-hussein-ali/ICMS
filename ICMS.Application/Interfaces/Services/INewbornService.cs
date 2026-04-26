using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICMS.Application.DTOs.Maternal;
using ICMS.Application.DTOs.Pagination;

namespace ICMS.Application.Interfaces.Services
{
    public interface INewbornService
    {
        Task<IReadOnlyList<NewbornDto>> GetAllAsync(PaginationParams paginationParams, CancellationToken ct = default);

        Task<NewbornDto?> GetByIdAsync(int id, CancellationToken ct = default);

        Task<NewbornDto> AddAsync(NewbornDto entity, CancellationToken ct = default);

        Task<bool> UpdateAsync(int id, NewbornDto updatedEntity, CancellationToken ct = default);

        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
