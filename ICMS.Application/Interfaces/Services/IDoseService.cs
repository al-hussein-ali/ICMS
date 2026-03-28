using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICMS.Application.DTOs;
using ICMS.Application.DTOs.Dose;
using ICMS.Application.DTOs.Pagination;

namespace ICMS.Application.Interfaces.Services
{
    public interface IDoseService
    {

        Task<IReadOnlyList<DoseReadDto>> GetAllAsync(int? vaccineId, CancellationToken ct = default);

        Task<DoseReadDto> GetByIdAsync(int id, CancellationToken ct = default);

        Task<DoseReadDto> GetByNameAsync(string doseName, CancellationToken ct = default);

        Task<DoseReadDto> AddAsync(DoseCreateDto entity, CancellationToken ct = default);

        Task<bool> UpdateAsync(int id, DoseCreateDto updatedEntity, CancellationToken ct = default);

        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
