using ICMS.Application.DTOs.Vaccine;

namespace ICMS.Application.Interfaces.Services
{
    public interface IVaccineService
    {
        Task<IReadOnlyList<VaccineReadDto>> GetAllAsync(string? name = null, CancellationToken ct = default);

        Task<VaccineReadDto> GetByIdAsync(int id, CancellationToken ct = default);

        Task<VaccineReadDto> AddAsync(VaccineCreateDto entity, CancellationToken ct = default);

        Task<bool> UpdateAsync(int id, VaccineCreateDto updatedEntity, CancellationToken ct = default);

        Task<bool> DeactivateAsync(int id, CancellationToken ct = default);
        Task<bool> ReactivateAsync(int id, CancellationToken ct = default);
    }
}
