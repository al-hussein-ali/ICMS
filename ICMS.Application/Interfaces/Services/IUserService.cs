using ICMS.Application.DTOs.Pagination;
using ICMS.Application.DTOs.User;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ICMS.Application.Interfaces.Services
{
    public interface IUserService 
    {
        Task<IReadOnlyList<UserReadDto>> GetAllAsync(PaginationParams paginationParams, CancellationToken ct = default);

        Task<UserReadDto?> GetByIdAsync(int id, CancellationToken ct = default);

        Task<UserReadDto> AddAsync(UserCreateDto entity, CancellationToken ct = default);

        Task<bool> UpdateAsync(UserReadDto updatedEntity, CancellationToken ct = default);

        Task<bool> DeleteAsync(int id, CancellationToken ct = default);

        Task<bool> ActivateAsync(int id, CancellationToken ct = default);

        Task<bool> DeactivateAsync(int id, CancellationToken ct = default);

        Task<bool> ChangePasswordAsync(int id, UserChangePasswordDto changePasswordDto, CancellationToken ct = default);

        Task<bool> ChangeOwnPasswordAsync(int id, ChangeOwnPasswordDto changeOwnPasswordDto, CancellationToken ct = default);
    }
}
