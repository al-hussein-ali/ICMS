using FluentValidation;
using ICMS.Application.DTOs;
using ICMS.Application.DTOs.BulkResult;
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
    private readonly IValidator<NewFieldVaccinatedIndividualDto> _newFieldIndividualValidator;

    public VaccinatedIndividualService(IUnitOfWork unitOfWork, IValidator<PaginationParams> pagnationValidator, IValidator<ImmunizationRecordCreateDto> immunizationRecordCreateValidator, IValidator<VaccinatedIndividualCreateDto> vaccinatedCreateValidator, IValidator<NewFieldVaccinatedIndividualDto> newFieldIndividualValidator)

    {
        this._unitOfWork = unitOfWork;
        this.pagnationValidator = pagnationValidator;
        _vaccinatedCreateValidator = vaccinatedCreateValidator;
        _immunizationRecordCreateValidator = immunizationRecordCreateValidator;
        _newFieldIndividualValidator = newFieldIndividualValidator;
    }

    public async Task<PagedResult<VaccinatedIndividualReadDto>> GetAllAsync(PaginationParams paginationParams, CancellationToken ct = default)
    {
        await pagnationValidator.ValidateAndThrowAsync(paginationParams);


        var vaccinatedIndividuals = _unitOfWork.VaccinatedIndividualRepository.GetQueryable().Select(vi => vi.ToReadDto());

        return vaccinatedIndividuals.ApplyPagination(paginationParams.PageNumber, paginationParams.PageSize);
    }
    public async Task<VaccinatedIndividualDetailsDto> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var vaccinatedIndividual = await _unitOfWork.VaccinatedIndividualRepository.GetByIdAsync(id, ct);

        return vaccinatedIndividual?.ToDetailsDto() ?? throw new NotFoundException("This individual was not found");
    }

    public async Task<VaccinatedIndividualDetailsDto> GetByCardNumberAsync(string cardNumber, CancellationToken ct = default)
    {
        var vaccinatedIndividual = await _unitOfWork.VaccinatedIndividualRepository.GetDetailsByCardNumber(cardNumber, ct);

        return vaccinatedIndividual?.ToDetailsDto() ?? throw new NotFoundException("This individual was not found");
    }

    public async Task<VaccinatedIndividualReadDto> AddAsync(VaccinatedIndividualCreateDto entity, CancellationToken ct = default)
    {

        await _vaccinatedCreateValidator.ValidateAndThrowAsync(entity);


        var person = (entity.PersonCreateDto == null) ? await _unitOfWork.PersonRepository.GetByIdAsync(entity.PersonId!.Value, ct) : await _unitOfWork
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
            vaccinatedIndividual.AssignPerson(entity.PersonCreateDto!.ToDomain());
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

        if (existingVaccinatedIndividual == null)
            throw new NotFoundException("This record was not found!");



        if (existingVaccinatedIndividual.Person == null) throw new NotFoundException("Associated person not found.");


        if (updatedEntity.PersonId.HasValue)
        {
            if (existingVaccinatedIndividual.PersonId != updatedEntity.PersonId.Value)
                await AssignExsitingPersonAsync(existingVaccinatedIndividual, updatedEntity.PersonId.Value, ct);
        }

        if (updatedEntity.PersonCreateDto != null)
        {
            existingVaccinatedIndividual.Person.UpdatePersonInfo(
                updatedEntity.PersonCreateDto.FirstName,
                updatedEntity.PersonCreateDto.SecondName,
                updatedEntity.PersonCreateDto.ThirdName,
                updatedEntity.PersonCreateDto.LastName,
                updatedEntity.PersonCreateDto.Gender.FromStringToGenderEnum(),
                updatedEntity.PersonCreateDto.DateOfBirth,
                updatedEntity.PersonCreateDto.PhoneNumber);
        }

        existingVaccinatedIndividual.UpdateIndividualInfo(
            updatedEntity.Directorate,
            updatedEntity.Area,
            updatedEntity.Neighborhood);

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

    private async Task<bool> AssignExsitingPersonAsync(VaccinatedIndividual existingVaccinatedIndividual, int personId, CancellationToken ct = default)
    {

        var newPerson = await _unitOfWork.PersonRepository.GetByIdAsync(personId, ct);

        if (newPerson == null) throw new NotFoundException("Person was not found");


        existingVaccinatedIndividual.AssignExistingPersonById(newPerson.Id);
        return true;
    }

    public async Task<BulkInsertResult> BulkInsertIndividualAsync(List<NewFieldVaccinatedIndividualDto> dtos,CancellationToken ct = default)
    {
        var result = new BulkInsertResult();

        // 1. Validate and convert DTOs to Domain Entities
        var (validIndividuals, entityToDtoMap) = await ProcessAndValidateDtosAsync(dtos, result);

        if (!validIndividuals.Any())
            return result; // Exit early if nothing passed validation

        // 2. Persist valid entities to the database
        await PersistToDatabaseAsync(validIndividuals, entityToDtoMap, result);

        return result;
    }

    private async Task<(List<VaccinatedIndividual>, Dictionary<VaccinatedIndividual, NewFieldVaccinatedIndividualDto>)> ProcessAndValidateDtosAsync(
        List<NewFieldVaccinatedIndividualDto> dtos,
        BulkInsertResult result)
    {
        var validIndividuals = new List<VaccinatedIndividual>();
        var entityToDtoMap = new Dictionary<VaccinatedIndividual, NewFieldVaccinatedIndividualDto>();

        for (int i = 0; i < dtos.Count; i++)
        {
            var dto = dtos[i];

            // STEP A: Fluent Validation
            var validationResult = await _newFieldIndividualValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                var errorMessages = string.Join(" | ", validationResult.Errors.Select(e => e.ErrorMessage));
                result.Errors.Add($"Row {i + 1} Validation Failed: {errorMessages}");
                continue;
            }

            // STEP B: Domain Creation
            try
            {
                var person = Person.Create(
                    dto.FirstName, dto.SecondName, dto.ThirdName, dto.LastName,
                    dto.Gender.FromStringToGenderEnum(), dto.DateOfBirth, dto.PhoneNumber);

                var individual = VaccinatedIndividual.Create(dto.Directorate, dto.Area, dto.Neighborhood);

                individual.AssignPerson(person);

                validIndividuals.Add(individual);
                entityToDtoMap.Add(individual, dto);
            }
            catch (Exception ex)
            {
                result.Errors.Add($"Row {i + 1} Domain Error: {ex.Message}");
            }
        }

        return (validIndividuals, entityToDtoMap);
    }

    private async Task PersistToDatabaseAsync(
        List<VaccinatedIndividual> validIndividuals,
        Dictionary<VaccinatedIndividual, NewFieldVaccinatedIndividualDto> entityToDtoMap,
        BulkInsertResult result)
    {
        try
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                // Step A: Bulk insert Parents (and Persons via IncludeGraph)
                await _unitOfWork.VaccinatedIndividualRepository.BulkInsertAsync(validIndividuals);

                var recordsToInsert = new List<ImmunizationRecord>();

                // Step B: Call TakeDose now that IDs exist
                foreach (var parent in validIndividuals)
                {
                    var dto = entityToDtoMap[parent];

                    try
                    {
                        parent.TakeDose(dto.DoseId, dto.VaccinationDate, dto.TakenIn, dto.FieldVisitId, dto.Note);
                        recordsToInsert.AddRange(parent.ImmunizationRecords);
                    }
                    catch (Exception ex)
                    {
                        result.Errors.Add($"Row '{dto.FirstName} {dto.LastName}' Dose Failed: {ex.Message}");
                    }
                }

                // Step C: Bulk Insert Children
                if (recordsToInsert.Any())
                {
                    await _unitOfWork.ImmunizationRecordRepository.BulkInsertAsync(recordsToInsert);
                }
            });

            // Transaction successful
            result.InsertedCount = validIndividuals.Count;
        }
        catch (Exception ex)
        {
            // Transaction failed and rolled back
            result.Errors.Add($"Database Transaction Failed and Rolled Back: {ex.Message}");
            result.InsertedCount = 0;
        }
    }

}
