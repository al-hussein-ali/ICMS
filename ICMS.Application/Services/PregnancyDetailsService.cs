using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICMS.Application.DTOs;
using ICMS.Application.DTOs.Pagination;
using ICMS.Application.Interfaces.Services;

namespace ICMS.Application.Services
{
    public class PregnancyDetailsService : IPregnancyDetailsService
    {
        public Task<IReadOnlyList<TempDto>> GetAllAsync(PaginationParams paginationParams, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<TempDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
        public Task<TempDto> AddAsync(TempDto entity, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(TempDto entity, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }


        public Task<bool> UpdateAsync(TempDto updatedEntity, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}
