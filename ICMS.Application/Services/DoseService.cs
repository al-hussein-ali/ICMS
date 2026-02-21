using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public DoseService(IUnitOfWork unitOfWork, IValidator<DoseCreateDto> createDTOValidator)
        {
            _unitOfWork = unitOfWork;
            this.createDTOValidator = createDTOValidator;
        }

        public async Task<IReadOnlyList<DoseReadDto>> GetAllAsync(int? vaccineId, CancellationToken ct = default)
        {
            var doses = await _unitOfWork.DoseRepository.GetAllAsync(vaccineId, ct);

            return doses.Select(d => d.ToReadDto()).ToList();
        }

        public async Task<DoseReadDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var dose = await _unitOfWork.DoseRepository.GetByIdAsync(id);

            if (dose == null)
                throw new NotFoundException("Dose id was not found.");

            return dose.ToReadDto();
        }

        public async Task<DoseReadDto?> GetByNameAsync(string doseName, CancellationToken ct)
        {
            var dose = await _unitOfWork.DoseRepository.GetByAsync(d => d.DoseName == doseName,ct);

            if (dose == null)
                throw new NotFoundException("Dose id was not found.");

            return dose.ToReadDto();
        }

        public async Task<DoseReadDto> AddAsync(DoseCreateDto entity, CancellationToken ct = default)
        {
            await createDTOValidator.ValidateAndThrowAsync(entity);

            var vaccine = await _unitOfWork.VaccineRepository.GetByIdAsync(entity.VaccineId, ct);

            if (vaccine == null)
                throw new NotFoundException("Vaccine id was not found.");


            var newDose = entity.ToDomain();

            vaccine.AddDose(newDose);

            await _unitOfWork.SaveChangesAsync();

            return newDose.ToReadDto();
        }

        public async Task<bool> UpdateAsync(int id, DoseCreateDto updatedEntity, CancellationToken ct = default)
        {
            var existingDose = await _unitOfWork.DoseRepository.GetByIdAsync(id, ct);

            if (existingDose == null)
                throw new NotFoundException("This Dose was not found.");

            existingDose.UpdateDoseInfo(updatedEntity.VaccineId,
                updatedEntity.DoseName,
                updatedEntity.DoseOrder,
                updatedEntity.RecommendedAgeInMonths,
                updatedEntity.RecommendedAgeGroup,
                updatedEntity.Notes);

            return await _unitOfWork.SaveChangesAsync(ct) > 0;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var existingDose = await _unitOfWork.DoseRepository.GetByIdAsync(id, ct);

            if (existingDose == null)
                throw new NotFoundException("This Dose was not found");

            await _unitOfWork.DoseRepository.DeleteAsync(existingDose, ct);

            return await _unitOfWork.SaveChangesAsync(ct) > 0;

        }


    }
}
