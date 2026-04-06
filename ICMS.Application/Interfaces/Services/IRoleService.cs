using ICMS.Application.DTOs.Pagination;
using ICMS.Application.DTOs.Role;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ICMS.Application.Interfaces.Services
{
    public interface IRoleService
    {
        Task<IReadOnlyList<RoleReadDto>> GetAllAsync(PaginationParams paginationParams, CancellationToken ct = default);

        Task<RoleReadDto?> GetByIdAsync(int id, CancellationToken ct = default);

        Task<RoleReadDto> AddAsync(RoleCreateDto entity, CancellationToken ct = default);

        Task<bool> UpdateAsync(int id, RoleCreateDto updatedEntity, CancellationToken ct = default);

        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
