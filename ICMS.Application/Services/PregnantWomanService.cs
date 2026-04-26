using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICMS.Application.DTOs.Maternal;
using ICMS.Application.DTOs.Pagination;
using ICMS.Application.Interfaces.Services;

namespace ICMS.Application.Services
{
    public class PregnantWomanService : IPregnantWomanService
    {

        public Task<IReadOnlyList<PregnantWomanReadDto>> GetAllAsync(PaginationParams paginationParams, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<PregnantWomanReadDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<PregnantWomanReadDto> AddAsync(PregnantWomanCreateDto entity, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(int id, PregnantWomanCreateDto updatedEntity, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}
