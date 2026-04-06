using FluentValidation;
using ICMS.Application.DTOs;
using ICMS.Application.DTOs.Pagination;
using ICMS.Application.DTOs.Vaccine;
using ICMS.Application.Extensions;
using ICMS.Application.Interfaces;
using ICMS.Application.Interfaces.Services;
using ICMS.Domain.Exceptions;
using System.Collections.Immutable;

namespace ICMS.Application.Services;

public class VaccineService(IUnitOfWork unitOfWork,IValidator<VaccineCreateDto> vaccineCreateValidator) : IVaccineService
{

    public async Task<IReadOnlyList<VaccineReadDto>> GetAllAsync(CancellationToken ct = default)
    {
        var vaccines = await unitOfWork.VaccineRepository.GetAllAsync(cancellationToken:ct);

        return vaccines.Select(v => v.ToReadDto()).ToList();
    }

    public async Task<VaccineReadDto> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var vaccine = await unitOfWork.VaccineRepository.GetByIdAsync(id,ct);


        return vaccine?.ToReadDto() ?? throw new NotFoundException("Vaccine was not found");
        ;
    }

    public async Task<VaccineReadDto> AddAsync(VaccineCreateDto entity, CancellationToken ct = default)
    {
        await vaccineCreateValidator.ValidateAndThrowAsync(entity);


        var newVaccine = entity.ToDomain();


        await unitOfWork.VaccineRepository.AddAsync(newVaccine);

        await unitOfWork.SaveChangesAsync();

        return newVaccine.ToReadDto();
    }

    public async Task<bool> UpdateAsync(int id, VaccineCreateDto updatedEntity, CancellationToken ct = default)
    {
        var existingVaccine = await unitOfWork.VaccineRepository.GetByIdAsync(id,ct);

        if (existingVaccine == null)
            throw new NotFoundException("This Vaccine was not found");


        existingVaccine.UpdateVaccineInfo(updatedEntity.VaccineName,
            updatedEntity.VaccineCode,
            updatedEntity.Description,
            updatedEntity.IsActive,
            updatedEntity.TotalDosages,
            updatedEntity.Audience);


        return await unitOfWork.SaveChangesAsync() > 0;
            
    }
    public async Task<bool> DeactivateAsync(int id, CancellationToken ct = default)
    {
        var existingVaccine = await unitOfWork.VaccineRepository.GetByIdAsync(id, ct);

        if (existingVaccine == null)
            throw new NotFoundException("This Vaccine was not found");



        existingVaccine.Deactivate();

        return await unitOfWork.SaveChangesAsync() > 0;
    }


    public async Task<bool> ReactivateAsync(int id, CancellationToken ct = default)
    {
        var existingVaccine = await unitOfWork.VaccineRepository.GetByIdAsync(id, ct);

        if (existingVaccine == null)
            throw new NotFoundException("This Vaccine was not found");



        existingVaccine.Reactivate();

        return await unitOfWork.SaveChangesAsync() > 0;
    }

}