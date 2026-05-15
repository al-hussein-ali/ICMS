using ICMS.Application.DTOs.BulkResult;
using ICMS.Application.DTOs.VaccinatedIndividual;
using ICMS.Application.DTOs.Pagination;
using ICMS.Application.DTOs.ImmunizationRecord;
using ICMS.Application.DTOs.Account;
using ICMS.Domain.ValueObjects;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ICMS.Application.Interfaces.Services
{
    public interface IVaccinatedIndividualService
    {
        Task<PagedResult<VaccinatedIndividualReadDto>> GetAllAsync(PaginationParams paginationParams, CancellationToken ct = default);

        Task<VaccinatedIndividualDetailsDto> GetByIdAsync(int id, CancellationToken ct = default);

        Task<VaccinatedIndividualDetailsDto> GetByCardNumberAsync(string cardNumber, CancellationToken ct = default);

        Task<VaccinatedIndividualReadDto> AddAsync(VaccinatedIndividualCreateDto entity, CancellationToken ct = default);

        Task<bool> UpdateAsync(int id,VaccinatedIndividualCreateDto updatedEntity, CancellationToken ct = default);

        Task<bool> DeleteAsync(int id, bool deletePersonalInfo = false, bool isSoftDelete = true, CancellationToken ct = default);

        Task<bool> GiveDose(ImmunizationRecordCreateDto dto, int userId, CancellationToken ct = default);
        Task<BulkSyncResultDto> BulkInsertIndividualAsync(List<NewFieldVaccinatedIndividualDto> newFieldVaccinatedIndividuals, int userId, CancellationToken ct = default);
        Task<BulkSyncResultDto> BulkUpdateFieldVisitIndividualAsync(List<UpdateFieldVisitIndividualDto> dtos, int userId, CancellationToken ct = default);
        Task<GeneratedAccountDto> GenerateAccountAsync(int id, CancellationToken ct = default);
        Task<int?> GetIndividualIdByUserIdAsync(int userId, CancellationToken ct = default);
    }
}
