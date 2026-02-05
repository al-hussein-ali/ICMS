using ICMS.Application.DTOs;
using ICMS.Application.DTOs.Pagination;
using ICMS.Application.Interfaces.Services;

namespace ICMS.Application.Services;

public class UserService : IUserService
{
    public async Task<IReadOnlyList<TempDto>> GetAllAsync(PaginationParams paginationParams, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public async Task<TempDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public async Task<TempDto> AddAsync(TempDto entity, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> UpdateAsync(TempDto updatedEntity, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteAsync(TempDto entity, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}