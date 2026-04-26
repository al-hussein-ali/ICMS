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
using ICMS.Application.DTOs.Account;
using ICMS.Domain.Entites.Common;
using ICMS.Domain.Entites.Identity;
using ICMS.Domain.Entites.Clinical;
using ICMS.Domain.Entites.Maternal;
using ICMS.Domain.Entites.Visits;
using ICMS.Domain.Entites.Audit;
using ICMS.Domain.Entites.Geography;
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

        Task<bool> DeleteAsync(int id, bool deletePersonalInfo = false, CancellationToken ct = default);

        Task<bool> GiveDose(ImmunizationRecordCreateDto dto, int userId, CancellationToken ct = default);
        Task<BulkInsertResult> BulkInsertIndividualAsync(List<NewFieldVaccinatedIndividualDto> newFieldVaccinatedIndividuals, int userId, CancellationToken ct = default);
        Task<BulkInsertResult> BulkUpdateFieldVisitIndividualAsync(List<UpdateFieldVisitIndividualDto> dtos, int userId, CancellationToken ct = default);
        Task<GeneratedAccountDto> GenerateAccountAsync(int id, CancellationToken ct = default);
    }
}
