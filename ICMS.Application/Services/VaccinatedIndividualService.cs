using FluentValidation;
using ICMS.Application.DTOs;
using ICMS.Application.DTOs.ImmunizationRecord;
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
    private readonly IValidator<ImmunizationRecordCreateDto> _immunizationRecordCreateValidator;

<<<<<<< HEAD
    public VaccinatedIndividualService(IUnitOfWork unitOfWork, IValidator<PaginationParams> pagnationValidator, IPersonService personService, IValidator<VaccinatedIndividualCreateDto> vaccinatedCreateValidator, IValidator<ImmunizationRecordCreateDto> immunizationRecordCreateValidator)
=======
    public VaccinatedIndividualService(IUnitOfWork unitOfWork, IValidator<PaginationParams> pagnationValidator, IValidator<VaccinatedIndividualCreateDto> vaccinatedCreateValidator)
>>>>>>> dfd769b3e85ddebe02d7efecbd351527db621e99
    {
        this._unitOfWork = unitOfWork;
        this.pagnationValidator = pagnationValidator;
        _vaccinatedCreateValidator = vaccinatedCreateValidator;
        _immunizationRecordCreateValidator = immunizationRecordCreateValidator;
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

        // ensure person exists
        var person = await _unitOfWork.PersonRepository.GetByIdAsync(entity.PersonId, ct);
        if (person == null)
            throw new NotFoundException("Person was not found");

        var vaccinatedIndividual = entity.ToDomain();

        vaccinatedIndividual.AssignExistingPersonById(person.Id);

        if (entity.UserId.HasValue && entity.UserId.Value > 0)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(entity.UserId.Value, ct);
            if (user == null) throw new NotFoundException("User was not found");
            vaccinatedIndividual.AssignUser(user);
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

        // If PersonId changed ensure it exists and assign
        if (existingVaccinatedIndividual.PersonId != updatedEntity.PersonId)
        {
            var newPerson = await _unitOfWork.PersonRepository.GetByIdAsync(updatedEntity.PersonId, ct);
            if (newPerson == null) throw new NotFoundException("Person was not found");
            existingVaccinatedIndividual.AssignExistingPersonById(newPerson.Id);
        }

        // update vaccinated individual's own fields. Domain only exposes UpdateIndividualInfo which also updates person info.
        // We'll call it with existing person details to keep person unchanged.
        var person = existingVaccinatedIndividual.Person;
        if (person == null) throw new NotFoundException("Associated person not found.");

        existingVaccinatedIndividual.UpdateIndividualInfo(
            updatedEntity.CardNumber,
            updatedEntity.Directorate,
            updatedEntity.Area,
            updatedEntity.Neighborhood,
            person.FirstName,
            person.SecondName,
            person.ThirdName,
            person.LastName,
            person.Gender,
            person.DateOfBirth,
            person.PhoneNumber);

        // if user changed
        if (updatedEntity.UserId.HasValue)
        {
            if (!existingVaccinatedIndividual.UserId.HasValue || existingVaccinatedIndividual.UserId.Value != updatedEntity.UserId.Value)
            {
                var user = await _unitOfWork.UserRepository.GetByIdAsync(updatedEntity.UserId.Value, ct);
                if (user == null) throw new NotFoundException("User was not found");
                existingVaccinatedIndividual.AssignUser(user);
            }
        }


        return await _unitOfWork.SaveChangesAsync(ct) > 0;
    }
    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {

        var existingVaccinatedIndividual = await _unitOfWork.VaccinatedIndividualRepository.GetByIdAsync(id, ct);

        if (existingVaccinatedIndividual == null || existingVaccinatedIndividual.Person.IsDeleted)
            throw new NotFoundException("This record was not found!");


        await _unitOfWork.VaccinatedIndividualRepository.DeleteAsync(existingVaccinatedIndividual, ct);

        return await _unitOfWork.SaveChangesAsync() > 0;
    }
    public async Task<bool> GiveDose(ImmunizationRecordCreateDto dto, CancellationToken ct = default)
    {
        await _immunizationRecordCreateValidator.ValidateAndThrowAsync(dto);
        
        var vaccinatedIndividual = await _unitOfWork.VaccinatedIndividualRepository.GetDetailsById(dto.IndividualId);

        if (vaccinatedIndividual == null)
            throw new NotFoundException("This record was not found!");

        if (!await _unitOfWork.DoseRepository.ExistAsync(dto.DoseId))
            throw new NotFoundException("The Dose Id was invalid or not found.");


        vaccinatedIndividual.TakeDose(dto.DoseId, dto.VaccinationDate, dto.TakenIn, dto.FieldVisitId, dto.Notes);

        return await _unitOfWork.SaveChangesAsync() > 0;
    }


}