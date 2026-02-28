using FluentValidation;
using ICMS.Application.DTOs;
using ICMS.Application.DTOs.ImmunizationRecord;
using ICMS.Application.DTOs.Pagination;
using ICMS.Application.Extensions;
using ICMS.Application.Interfaces;
using ICMS.Application.Interfaces.Services;
using ICMS.Domain.Exceptions;
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
        IValidator<ImmunizationRecordCreateDto> immunizationCreateValidator) : IImmunizationRecordService
    {

        public async Task<PagedResult<ImmunizationRecordReadDto>> GetAllAsync(PaginationParams paginationParams, CancellationToken ct = default)
        {
            await paginationValidator.ValidateAndThrowAsync(paginationParams);

            var immunizationRecords = unitOfWork.ImmunizationRecordRepository.GetQueryable()
                .Where(ir => !ir.VaccinatedIndividual.Person.IsDeleted)
                .Select(ir => ir.ToReadDto());

            ct.ThrowIfCancellationRequested();

            return immunizationRecords.ApplyPagination(pageNumber: paginationParams.PageNumber, pageSize: paginationParams.PageSize);
        }

        public async Task<ImmunizationRecordReadDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var immunizationRecord = await unitOfWork.ImmunizationRecordRepository.GetByIdAsync(id, ct);

            ct.ThrowIfCancellationRequested();

            return immunizationRecord?.ToReadDto();
        }

        //public async Task<ImmunizationRecordReadDto> AddAsync(ImmunizationRecordCreateDto entity, CancellationToken ct = default)
        //{
        //    await immunizationCreateValidator.ValidateAndThrowAsync(entity);

        //    var immunizationRecord = entity.ToDomain();



        //}
        public async Task<bool> UpdateAsync(Guid id, ImmunizationRecordCreateDto updatedEntity, CancellationToken ct = default)
        {
            await immunizationCreateValidator.ValidateAndThrowAsync(updatedEntity);

            var oldImmunizaitionRecord = await unitOfWork.ImmunizationRecordRepository.GetByIdAsync(id, ct);

            ct.ThrowIfCancellationRequested();

            if (oldImmunizaitionRecord == null)
                throw new NotFoundException("This Record was not found");

            if (!await unitOfWork.DoseRepository.ExistAsync(updatedEntity.DoseId))
                throw new NotFoundException($"The dose with Id {updatedEntity.DoseId} was not found!");

            if (!await unitOfWork.VaccinatedIndividualRepository.ExistAsync(updatedEntity.IndividualId))
                throw new NotFoundException($"The Vaccinated Individual with Id {updatedEntity.IndividualId} was not found!");



            oldImmunizaitionRecord.UpdateRecordInfo(updatedEntity.IndividualId,
                updatedEntity.DoseId,
                updatedEntity.VaccinationDate,
                updatedEntity.TakenIn,
                updatedEntity.FieldVisitId,
                updatedEntity.Notes);

            return await unitOfWork.SaveChangesAsync(ct) > 0;

        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
        {
            var existingRecord = await unitOfWork.ImmunizationRecordRepository.GetByIdAsync(id, ct);

            if (existingRecord == null)
                throw new NotFoundException("This record was not found!");

            ct.ThrowIfCancellationRequested();

            await unitOfWork.ImmunizationRecordRepository.DeleteAsync(existingRecord);

            return await unitOfWork.SaveChangesAsync(ct) > 0;

        }

    }
}
