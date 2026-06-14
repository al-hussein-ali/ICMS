using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using ICMS.Application.DTOs;
using ICMS.Application.DTOs.Dose;
using ICMS.Application.DTOs.Pagination;
using ICMS.Application.Extensions;
using ICMS.Application.Interfaces;
using ICMS.Application.Interfaces.Services;
using ICMS.Application.Validators.Dose;
using ICMS.Domain.Exceptions;
using Microsoft.VisualBasic;

namespace ICMS.Application.Services
{
    public class DoseService : IDoseService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<DoseCreateDto> createDTOValidator;
        private readonly ICacheService _cacheService;

        public DoseService(IUnitOfWork unitOfWork, IValidator<DoseCreateDto> createDTOValidator, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            this.createDTOValidator = createDTOValidator;
            _cacheService = cacheService;
        }

        private void InvalidateDoseCache(int vaccineId)
        {
            _cacheService.Remove("doses:all:vaccine:all:en");
            _cacheService.Remove("doses:all:vaccine:all:ar");
            _cacheService.Remove($"doses:all:vaccine:{vaccineId}:en");
            _cacheService.Remove($"doses:all:vaccine:{vaccineId}:ar");
        }

        public async Task<IReadOnlyList<DoseReadDto>> GetAllAsync(int? vaccineId, CancellationToken ct = default)
        {
            var lang = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToLower();
            string cacheKey = $"doses:all:vaccine:{vaccineId?.ToString() ?? "all"}:{lang}";
            if (_cacheService.TryGet(cacheKey, out IReadOnlyList<DoseReadDto>? cached) && cached != null)
                return cached;

            var doses = await _unitOfWork.DoseRepository.GetAllAsync(vaccineId, ct);
            var dtos = doses.Select(d => d.ToReadDto()).ToList();

            _cacheService.Set(cacheKey, dtos);
            return dtos;
        }

        public async Task<DoseReadDto> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var dose = await _unitOfWork.DoseRepository.GetByIdAsync(id);

            return dose.ToReadDto() ?? throw new NotFoundException("NotFound"); ;
        }

        public async Task<DoseReadDto> GetByNameAsync(string doseName, CancellationToken ct)
        {
            var dose = await _unitOfWork.DoseRepository.GetByAsync(d => d.DoseName == doseName, ct);

            return dose.ToReadDto() ?? throw new NotFoundException("NotFound"); ;
        }

        public async Task<DoseReadDto> AddAsync(DoseCreateDto entity, CancellationToken ct = default)
        {
            await createDTOValidator.ValidateAndThrowAsync(entity);

            var vaccine = await _unitOfWork.VaccineRepository.GetByIdAsync(entity.VaccineId, ct);

            if (vaccine == null)
                throw new NotFoundException("NotFound");


            var newDose = entity.ToDomain();

            vaccine.AddDose(newDose);

            await _unitOfWork.SaveChangesAsync();

            InvalidateDoseCache(entity.VaccineId);

            return newDose.ToReadDto();
        }

        public async Task<bool> UpdateAsync(int id, DoseCreateDto updatedEntity, CancellationToken ct = default)
        {
            await createDTOValidator.ValidateAndThrowAsync(updatedEntity, ct);

            var existingDose = await _unitOfWork.DoseRepository.GetByIdAsync(id, ct);

            if (existingDose == null)
                throw new NotFoundException("NotFound");

            existingDose.UpdateDoseInfo(updatedEntity.VaccineId,
                updatedEntity.DoseName,
                updatedEntity.DoseOrder,
                updatedEntity.RecommendedAgeInWeeks,
                updatedEntity.RecommendedAgeGroup,
                updatedEntity.IsPrimary,
                updatedEntity.Notes);

            var result = await _unitOfWork.SaveChangesAsync(ct) > 0;
            if (result)
            {
                InvalidateDoseCache(updatedEntity.VaccineId);
                if (existingDose.VaccineId != updatedEntity.VaccineId)
                    InvalidateDoseCache(existingDose.VaccineId);
            }
            return result;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var existingDose = await _unitOfWork.DoseRepository.GetByIdAsync(id, ct);

            if (existingDose == null)
                throw new NotFoundException("NotFound");

            // 1. Check if anyone has already taken this dose
            bool hasImmunizations = await _unitOfWork.ImmunizationRecordRepository.ExistAsync(ir => ir.DoseId == id, ct);
            if (hasImmunizations)
            {
                throw new DomainException("CannotDeleteDoseWithImmunizationRecords");
            }

            // 2. Check if there are physical vaccine batches associated with this dose in the inventory
            bool hasBatches = await _unitOfWork.BatchRepository.ExistAsync(b => b.DoseId == id, ct);
            if (hasBatches)
            {
                throw new DomainException("CannotDeleteDoseWithBatches");
            }

            // 3. Since no one has taken the dose and there are no inventory batches,
            // we can safely clean up all auto-generated future schedules for this dose first!
            var schedules = await _unitOfWork.VaccinationScheduleRepository
                .GetQueryable(track: true, ct)
                .Where(s => s.DoseId == id)
                .ToListAsync(ct);

            foreach (var schedule in schedules)
            {
                await _unitOfWork.VaccinationScheduleRepository.DeleteAsync(schedule, ct);
            }

            // 4. Finally, delete the dose itself
            await _unitOfWork.DoseRepository.DeleteAsync(existingDose, ct);

            var result = await _unitOfWork.SaveChangesAsync(ct) > 0;
            if (result) InvalidateDoseCache(existingDose.VaccineId);
            return result;
        }


    }
}
