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
    public class PreviousPostpartumComplicationsService : IPreviousPostpartumComplicationsService
    {
        public Task<IReadOnlyList<PreviousPostpartumComplicationsDto>> GetAllAsync(PaginationParams paginationParams, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<PreviousPostpartumComplicationsDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<PreviousPostpartumComplicationsDto> AddAsync(PreviousPostpartumComplicationsDto entity, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(int id, PreviousPostpartumComplicationsDto updatedEntity, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int id, PreviousPostpartumComplicationsDto entity, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}
