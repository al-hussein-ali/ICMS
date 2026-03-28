using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICMS.Application.DTOs;
using ICMS.Application.DTOs.ImmunizationRecord;
using ICMS.Application.DTOs.Pagination;
using ICMS.Domain.ValueObjects;

namespace ICMS.Application.Interfaces.Services
{
    public interface IImmunizationRecordService
    {
        Task<PagedResult<ImmunizationRecordReadDto>> GetAllAsync(PaginationParams paginationParams, CancellationToken ct = default);

        Task<ImmunizationRecordReadDto> GetByIdAsync(Guid id, CancellationToken ct = default);

        Task<bool> UpdateAsync(Guid id,ImmunizationRecordCreateDto updatedEntity, CancellationToken ct = default);

        Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
    }
}
