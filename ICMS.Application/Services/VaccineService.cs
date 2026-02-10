using ICMS.Application.DTOs;
using ICMS.Application.DTOs.Pagination;
using ICMS.Application.DTOs.Vaccine;
using ICMS.Application.Interfaces;
using ICMS.Application.Interfaces.Services;

namespace ICMS.Application.Services;

public class VaccineService(IUnitOfWork unitOfWork) : IVaccineService
{

    public Task<IReadOnlyList<VaccineReadDto>> GetAllAsync(PaginationParams paginationParams, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<VaccineReadDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<VaccineReadDto> AddAsync(VaccineCreateDto entity, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateAsync(int id, VaccineCreateDto updatedEntity, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
    public Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

}