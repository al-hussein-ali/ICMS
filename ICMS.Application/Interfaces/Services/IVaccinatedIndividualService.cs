using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICMS.Application.DTOs;
using ICMS.Application.DTOs.BulkResult;
using ICMS.Application.DTOs.ImmunizationRecord;
using ICMS.Application.DTOs.Pagination;
using ICMS.Application.DTOs.VaccinatedIndividual;
using ICMS.Domain.Entites;
using ICMS.Domain.ValueObjects;

namespace ICMS.Application.Interfaces.Services
{
    public interface IVaccinatedIndividualService
    {
        Task<PagedResult<VaccinatedIndividualReadDto>> GetAllAsync(PaginationParams paginationParams, CancellationToken ct = default);

        Task<VaccinatedIndividualDetailsDto> GetByIdAsync(int id, CancellationToken ct = default);

        Task<VaccinatedIndividualDetailsDto> GetByCardNumberAsync(string cardNumber, CancellationToken ct = default);

        Task<VaccinatedIndividualReadDto> AddAsync(VaccinatedIndividualCreateDto entity, CancellationToken ct = default);

        Task<bool> UpdateAsync(int id,VaccinatedIndividualCreateDto updatedEntity, CancellationToken ct = default);

        Task<bool> DeleteAsync(int id, CancellationToken ct = default);

        Task<bool> GiveDose(ImmunizationRecordCreateDto dto, CancellationToken ct = default);
        Task<BulkInsertResult> BulkInsertIndividualAsync(List<NewFieldVaccinatedIndividualDto> newFieldVaccinatedIndividuals, CancellationToken ct = default);
    }
}
