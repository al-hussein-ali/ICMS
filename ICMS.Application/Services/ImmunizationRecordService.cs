using FluentValidation;
using ICMS.Application.DTOs;
using ICMS.Application.DTOs.ImmunizationRecord;
using ICMS.Application.DTOs.Pagination;
using ICMS.Application.Extensions;
using ICMS.Application.Interfaces;
using ICMS.Application.Interfaces.Services;
using ICMS.Domain.Exceptions;
using ICMS.Domain.Entites.Clinical;
using ICMS.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ICMS.Application.Services
{
    public class ImmunizationRecordService(IUnitOfWork unitOfWork,
        IValidator<PaginationParams> paginationValidator,
        IValidator<ImmunizationRecordCreateDto> immunizationCreateValidator,
        ICacheService cacheService) : IImmunizationRecordService
    {
        public async Task<PagedResult<ImmunizationRecordReadDto>> GetAllAsync(PaginationParams paginationParams, int? individualId = null, CancellationToken ct = default)
        {
            await paginationValidator.ValidateAndThrowAsync(paginationParams);

            var query = unitOfWork.ImmunizationRecordRepository.GetQueryable()
                .Where(ir => !ir.VaccinatedIndividual!.Person.IsDeleted);

            if (individualId.HasValue)
            {
                query = query.Where(ir => ir.IndividualId == individualId.Value);
            }

            if (!string.IsNullOrWhiteSpace(paginationParams.Search))
            {
                var search = paginationParams.Search.Trim().ToLower();
                query = query.Where(ir => 
                    (ir.Notes != null && ir.Notes.ToLower().Contains(search)) || 
                    ir.TakenIn.ToLower().Contains(search));
            }

            var immunizationRecords = query.Select(ir => ir.ToReadDto());

            ct.ThrowIfCancellationRequested();

            return immunizationRecords.ApplyPagination(pageNumber: paginationParams.PageNumber, pageSize: paginationParams.PageSize);
        }

        public async Task<ImmunizationRecordReadDto> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var immunizationRecord = await unitOfWork.ImmunizationRecordRepository.GetByIdAsync(id, ct);

            ct.ThrowIfCancellationRequested();

            return immunizationRecord?.ToReadDto() ?? throw new NotFoundException("NotFound");
        }

        public async Task<ImmunizationRecordReadDto> AddAsync(ImmunizationRecordCreateDto entity, int userId, CancellationToken ct = default)
        {
            await immunizationCreateValidator.ValidateAndThrowAsync(entity);

            var individual = await unitOfWork.VaccinatedIndividualRepository.GetIndividualWithSchedulesAsync(entity.IndividualId, ct);
            if (individual == null)
                throw new NotFoundException("NotFound");

            var dose = await unitOfWork.DoseRepository.GetByIdAsync(entity.DoseId, ct, d => d.Vaccine);
            if (dose == null)
                throw new NotFoundException("NotFound");

            var allDoses = await unitOfWork.DoseRepository.GetAllAsync(dose.VaccineId, ct);
            var nextDose = allDoses.OrderBy(d => d.DoseOrder).FirstOrDefault(d => d.DoseOrder > dose.DoseOrder);

            return await unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                individual.AdministerDose(dose, entity.VaccinationDate, entity.TakenIn, userId, nextDose, entity.FieldVisitId, entity.Notes, entity.BatchId, entity.IsAdvancedDose, allDoses);

                // If a batch is selected, subtract 1 from inventory
                if (entity.BatchId.HasValue)
                {
                    var batch = await unitOfWork.BatchRepository.GetByIdAsync(entity.BatchId.Value, ct);
                    if (batch == null) throw new NotFoundException("BatchNotFound");

                    // Create subtraction transaction
                    batch.RemoveInventory(
                        1, 
                        $"IMM-REC-{entity.IndividualId}-{dose.Id}", 
                        "Beneficiary Administration", 
                        userId, 
                        DateTime.UtcNow, 
                        $"Automatic subtraction for vaccination of individual ID {entity.IndividualId}"
                    );

                    // Add the transaction to repository
                    var transaction = batch.Transactions.Last();
                    await unitOfWork.TransactionRepository.AddAsync(transaction, ct);
                }

                await unitOfWork.SaveChangesAsync(ct);
                InvalidateCache(entity.IndividualId);

                var newRecord = individual.ImmunizationRecords.FirstOrDefault(ir => ir.DoseId == dose.Id);
                return newRecord!.ToReadDto();
            });
        }
        public async Task<bool> UpdateAsync(Guid id, ImmunizationRecordCreateDto updatedEntity, CancellationToken ct = default)
        {
            await immunizationCreateValidator.ValidateAndThrowAsync(updatedEntity);

            var oldImmunizaitionRecord = await unitOfWork.ImmunizationRecordRepository.GetByIdAsync(id, ct);

            ct.ThrowIfCancellationRequested();

            if (oldImmunizaitionRecord == null)
                throw new NotFoundException("NotFound");

            if (!await unitOfWork.DoseRepository.ExistAsync(updatedEntity.DoseId))
                throw new NotFoundException("NotFound");

            if (!await unitOfWork.VaccinatedIndividualRepository.ExistAsync(updatedEntity.IndividualId))
                throw new NotFoundException("NotFound");



            oldImmunizaitionRecord.UpdateRecordInfo(updatedEntity.IndividualId,
                updatedEntity.DoseId,
                updatedEntity.VaccinationDate,
                updatedEntity.TakenIn,
                updatedEntity.FieldVisitId,
                updatedEntity.Notes);

            var result = await unitOfWork.SaveChangesAsync(ct) > 0;
            if (result)
            {
                InvalidateCache(updatedEntity.IndividualId);
            }
            return result;

        }
        public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
        {
            var existingRecord = await unitOfWork.ImmunizationRecordRepository.GetByIdAsync(id, ct);

            if (existingRecord == null)
                throw new NotFoundException("NotFound");

            ct.ThrowIfCancellationRequested();

            var individualId = existingRecord.IndividualId;
            // Revert the associated schedule status back to Pending
            var individual = await unitOfWork.VaccinatedIndividualRepository.GetIndividualWithSchedulesAsync(individualId, ct);
            if (individual != null)
            {
                individual.RevertAdministeredDose(id);
            }

            await unitOfWork.ImmunizationRecordRepository.DeleteAsync(existingRecord);

            // Revert inventory subtraction if linked to a batch
            if (existingRecord.BatchId.HasValue)
            {
                var batch = await unitOfWork.BatchRepository.GetByIdAsync(existingRecord.BatchId.Value, ct);
                if (batch != null)
                {
                    batch.AddInventory(
                        1, 
                        $"IMM-DEL-{existingRecord.IndividualId}-{existingRecord.DoseId}", 
                        "Record Deletion Revert", 
                        existingRecord.UserId, 
                        DateTime.UtcNow, 
                        $"Automatic revert for deletion of record ID {existingRecord.Id}"
                    );

                    var transaction = batch.Transactions.Last();
                    await unitOfWork.TransactionRepository.AddAsync(transaction, ct);
                }
            }

            var result = await unitOfWork.SaveChangesAsync(ct) > 0;
            if (result)
            {
                InvalidateCache(individualId);
            }
            return result;
        }

        private void InvalidateCache(int individualId)
        {
            cacheService.Remove($"schedules:individual:{individualId}:en");
            cacheService.Remove($"schedules:individual:{individualId}:ar");
        }
    }
}
