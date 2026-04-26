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
    public class PregnancyDetailsService : IPregnancyDetailsService
    {
        public Task<IReadOnlyList<PregnancyDetailsReadDto>> GetAllAsync(PaginationParams paginationParams, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<PregnancyDetailsReadDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
        public Task<PregnancyDetailsReadDto> AddAsync(PregnancyDetailsReadDto entity, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }


        public Task<bool> UpdateAsync(int id, PregnancyDetailsReadDto updatedEntity, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}
