using FluentValidation;
using ICMS.Application.DTOs;
using ICMS.Application.DTOs.Pagination;
using ICMS.Application.DTOs.VaccinatedIndividual;
using ICMS.Application.Extensions;
using ICMS.Application.Interfaces;
using ICMS.Application.Interfaces.Services;
using ICMS.Application.Validators;
using ICMS.Domain.Entites;
using ICMS.Domain.Exceptions;
using ICMS.Domain.ValueObjects;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ICMS.Application.Services;

public class VaccinatedIndividualService : IVaccinatedIndividualService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<PaginationParams> pagnationValidator;
    private readonly IValidator<VaccinatedIndividualCreateDto> _vaccinatedCreateValidator;
    private readonly IPersonService personService;

    public VaccinatedIndividualService(IUnitOfWork unitOfWork, IValidator<PaginationParams> pagnationValidator, IPersonService personService, IValidator<VaccinatedIndividualCreateDto> vaccinatedCreateValidator)
    {
        this._unitOfWork = unitOfWork;
        this.pagnationValidator = pagnationValidator;
        this.personService = personService;
        _vaccinatedCreateValidator = vaccinatedCreateValidator;
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

        await _vaccinatedCreateValidator.ValidateAndThrowAsync(entity);

        var person = await _unitOfWork
            .PersonRepository
            .GetByAsync(entity.PersonCreateDto.FirstName
            , entity.PersonCreateDto.LastName
            , entity.PersonCreateDto.PhoneNumber
            , entity.PersonCreateDto.DateOfBirth
            , ct);


        var vaccinatedIndividual = entity.ToDomain();

        if (person != null)
        {
            vaccinatedIndividual.AssignExistingPersonById(person.Id);
        }
        else
        {
            vaccinatedIndividual.AssignPerson(entity.PersonCreateDto.ToDomain());
        }


        ct.ThrowIfCancellationRequested();

        await _unitOfWork.VaccinatedIndividualRepository.AddAsync(vaccinatedIndividual, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return vaccinatedIndividual.ToReadDto();
    }

    public async Task<bool> UpdateAsync(int id, VaccinatedIndividualCreateDto updatedEntity, CancellationToken ct = default)
    {
        await _vaccinatedCreateValidator.ValidateAndThrowAsync(updatedEntity);


        var existingVaccinatedIndividual = await _unitOfWork.VaccinatedIndividualRepository.GetByIdAsync(id, ct);

        if (existingVaccinatedIndividual == null || existingVaccinatedIndividual.Person.IsDeleted)
            throw new NotFoundException("This record was not found!");


        existingVaccinatedIndividual.UpdateIndividualInfo(
            updatedEntity.CardNumber,
            updatedEntity.Directorate,
            updatedEntity.Area,
            updatedEntity.Neighborhood,
            updatedEntity.PersonCreateDto.FirstName,
            updatedEntity.PersonCreateDto.SecondName,
            updatedEntity.PersonCreateDto.ThirdName,
            updatedEntity.PersonCreateDto.LastName,
            updatedEntity.PersonCreateDto.Gender.FromStringToGenderEnum(),
            updatedEntity.PersonCreateDto.DateOfBirth);

        return await _unitOfWork.SaveChangesAsync(ct) > 0;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {

        var existingVaccinatedIndividual = await _unitOfWork.VaccinatedIndividualRepository.GetByIdAsync(id, ct);

        if (existingVaccinatedIndividual == null || existingVaccinatedIndividual.Person.IsDeleted)
            throw new NotFoundException("This record was not found!");

        await _unitOfWork.VaccinatedIndividualRepository.DeleteAsync(existingVaccinatedIndividual,ct);

        return await _unitOfWork.SaveChangesAsync() > 0;
    }

}