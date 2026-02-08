using FluentValidation;
using ICMS.Application.DTOs;
using ICMS.Application.DTOs.Pagination;
using ICMS.Application.DTOs.VaccinatedIndividual;
using ICMS.Application.Extensions;
using ICMS.Application.Interfaces;
using ICMS.Application.Interfaces.Services;
using ICMS.Application.Validators;
using ICMS.Domain.Exceptions;
using ICMS.Domain.ValueObjects;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ICMS.Application.Services;

public class VaccinatedIndividualService : IVaccinatedIndividualService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<PaginationParams> pagnationValidator;
    private readonly IPersonService personService;

    public VaccinatedIndividualService(IUnitOfWork unitOfWork, IValidator<PaginationParams> pagnationValidator, IPersonService personService)
    {
        this._unitOfWork = unitOfWork;
        this.pagnationValidator = pagnationValidator;
        this.personService = personService;
    }


    public async Task<PagedResult<VaccinatedIndividualReadDto>> GetAllAsync(PaginationParams paginationParams, CancellationToken ct = default)
    {
        await pagnationValidator.ValidateAndThrowAsync(paginationParams);


        var vaccinatedIndividuals = _unitOfWork.VaccinatedIndividualRepository.GetQueryable().Select(vi => vi.ToReadDto());

        return vaccinatedIndividuals.ApplyPagination(paginationParams.PageNumber, paginationParams.PageSize);
    }

    public async Task<VaccinatedIndividualReadDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var vaccinatedIndividual = await _unitOfWork.VaccinatedIndividualRepository.GetByIdAsync(id, ct);

        return vaccinatedIndividual?.ToReadDto();
    }
    public async Task<VaccinatedIndividualReadDto> AddAsync(VaccinatedIndividualCreateDto entity, CancellationToken ct = default)
    {
        var person = await personService.GetByAsync(p =>
        p.FirstName == entity.PersonCreateDto.FirstName
        && p.LastName == entity.PersonCreateDto.LastName
        && p.PhoneNumber == entity.PersonCreateDto.PhoneNumber
        && p.DateOfBirth == entity.PersonCreateDto.DateOfBirth
        , ct);



        var vaccinatedIndividual = entity.ToDomain();

        

        ct.ThrowIfCancellationRequested();

        await _unitOfWork.VaccinatedIndividualRepository.AddAsync(vaccinatedIndividual);
        await personService.AddAsync(person)
        await _unitOfWork.SaveChangesAsync(ct);

        return vaccinatedIndividual.ToReadDto();
    }

    public async Task<bool> UpdateAsync(int id, VaccinatedIndividualCreateDto updatedEntity, CancellationToken ct = default)
    {
        var existingVaccinatedIndividual = await _unitOfWork.VaccinatedIndividualRepository.GetByIdAsync(id, ct);

        if (existingVaccinatedIndividual == null)
            throw new NotFoundException("This record was not found!");

        existingVaccinatedIndividual.UpdateIndividualInfo(
            updatedEntity.CardNumber,
            updatedEntity.Directorate,
            updatedEntity.Area,
            updatedEntity.Neighborhood
            );

        return await _unitOfWork.SaveChangesAsync(ct) > 0;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var existingVaccinatedIndividual = await _unitOfWork.VaccinatedIndividualRepository.GetByIdAsync(id, ct);

        if (existingVaccinatedIndividual is null)
            throw new NotFoundException("This record was not found!");

        await _unitOfWork.VaccinatedIndividualRepository.DeleteAsync(existingVaccinatedIndividual);

        return await _unitOfWork.SaveChangesAsync() > 0;
    }

}