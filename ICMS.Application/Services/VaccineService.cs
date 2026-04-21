using FluentValidation;
using ICMS.Application.DTOs.Vaccine;
using ICMS.Application.Extensions;
using ICMS.Application.Interfaces;
using ICMS.Application.Interfaces.Services;
using ICMS.Domain.Exceptions;
using ICMS.Domain.Entites.Clinical;

namespace ICMS.Application.Services;

public class VaccineService(
    IUnitOfWork unitOfWork,
    IValidator<VaccineCreateDto> vaccineCreateValidator,
    ICacheService cacheService) : IVaccineService
{
    private const string CacheKeyAll = "vaccines:all";

    public async Task<IReadOnlyList<VaccineReadDto>> GetAllAsync(string? name = null, CancellationToken ct = default)
    {
        string cacheKey = string.IsNullOrWhiteSpace(name) ? CacheKeyAll : $"{CacheKeyAll}:{name}";

        if (cacheService.TryGet(cacheKey, out IReadOnlyList<VaccineReadDto>? cachedVaccines) && cachedVaccines != null)
        {
            return cachedVaccines;
        }

        IReadOnlyList<Vaccine> vaccines;
        if (!string.IsNullOrWhiteSpace(name))
        {
            vaccines = await unitOfWork.VaccineRepository.SearchByNameAsync(name, ct);
        }
        else
        {
            vaccines = await unitOfWork.VaccineRepository.GetAllAsync(cancellationToken: ct);
        }

        var dtos = vaccines.Select(v => v.ToReadDto()).ToList();

        cacheService.Set(cacheKey, dtos);
        return dtos;
    }

    public async Task<VaccineReadDto> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var vaccine = await unitOfWork.VaccineRepository.GetByIdAsync(id, ct);


        return vaccine?.ToReadDto() ?? throw new NotFoundException("NotFound");
    }

    public async Task<VaccineReadDto> AddAsync(VaccineCreateDto entity, CancellationToken ct = default)
    {
        await vaccineCreateValidator.ValidateAndThrowAsync(entity);


        var newVaccine = entity.ToDomain();


        await unitOfWork.VaccineRepository.AddAsync(newVaccine);

        await unitOfWork.SaveChangesAsync();

        cacheService.Remove(CacheKeyAll);

        return newVaccine.ToReadDto();
    }

    public async Task<bool> UpdateAsync(int id, VaccineCreateDto updatedEntity, CancellationToken ct = default)
    {
        var existingVaccine = await unitOfWork.VaccineRepository.GetByIdAsync(id, ct);

        if (existingVaccine == null)
            throw new NotFoundException("NotFound");


        existingVaccine.UpdateVaccineInfo(updatedEntity.VaccineName,
            updatedEntity.VaccineCode,
            updatedEntity.Description,
            updatedEntity.IsActive,
            updatedEntity.TotalDosages,
            updatedEntity.MinEligibleAgeInMonths,
            updatedEntity.MaxEligibleAgeInMonths,
            updatedEntity.Audience);


        var result = await unitOfWork.SaveChangesAsync() > 0;
        if (result) cacheService.Remove(CacheKeyAll);
        return result;
    }

    public async Task<bool> DeactivateAsync(int id, CancellationToken ct = default)
    {
        var existingVaccine = await unitOfWork.VaccineRepository.GetByIdAsync(id, ct);

        if (existingVaccine == null)
            throw new NotFoundException("NotFound");


        existingVaccine.Deactivate();

        var result = await unitOfWork.SaveChangesAsync() > 0;
        if (result) cacheService.Remove(CacheKeyAll);
        return result;
    }


    public async Task<bool> ReactivateAsync(int id, CancellationToken ct = default)
    {
        var existingVaccine = await unitOfWork.VaccineRepository.GetByIdAsync(id, ct);

        if (existingVaccine == null)
            throw new NotFoundException("NotFound");


        existingVaccine.Reactivate();

        var result = await unitOfWork.SaveChangesAsync() > 0;
        if (result) cacheService.Remove(CacheKeyAll);
        return result;
    }
}
