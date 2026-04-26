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
    public class NewbornSerivce : INewbornService
    {
        public Task<IReadOnlyList<NewbornDto>> GetAllAsync(PaginationParams paginationParams, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<NewbornDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
        
        public Task<NewbornDto> AddAsync(NewbornDto entity, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }


        public Task<bool> UpdateAsync(int id, NewbornDto updatedEntity, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}
