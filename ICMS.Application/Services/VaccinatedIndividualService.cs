using ICMS.Application.DTOs.BulkResult;
using ICMS.Application.DTOs.ImmunizationRecord;
using ICMS.Application.DTOs.Pagination;
using ICMS.Application.DTOs.VaccinatedIndividual;
using ICMS.Application.DTOs.Account;
using ICMS.Application.Interfaces;
using ICMS.Application.Interfaces.Services;
using ICMS.Application.Utilities;
using ICMS.Domain.Constants;
using ICMS.Domain.Entites.Clinical;
using ICMS.Domain.Entites.Identity;
using ICMS.Domain.Exceptions;
using ICMS.Domain.ValueObjects;
using ICMS.Application.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace ICMS.Application.Services
{
    public class VaccinatedIndividualService : IVaccinatedIndividualService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdentityService _identityService;

        public VaccinatedIndividualService(IUnitOfWork unitOfWork, IIdentityService identityService)
        {
            _unitOfWork = unitOfWork;
            _identityService = identityService;
        }

        public async Task<PagedResult<VaccinatedIndividualReadDto>> GetAllAsync(PaginationParams paginationParams, CancellationToken ct = default)
        {
            var query = _unitOfWork.VaccinatedIndividualRepository.GetQueryable(false, ct)
                .Select(vi => vi.ToReadDto());

            return query.ApplyPagination(paginationParams.PageNumber, paginationParams.PageSize);
        }

        public async Task<VaccinatedIndividualDetailsDto> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var individual = await _unitOfWork.VaccinatedIndividualRepository.GetDetailsById(id, ct);
            if (individual == null)
            {
                throw new NotFoundException("NotFound");
            }

            return individual.ToDetailsDto();
        }

        public async Task<VaccinatedIndividualDetailsDto> GetByCardNumberAsync(string cardNumber, CancellationToken ct = default)
        {
            var individual = await _unitOfWork.VaccinatedIndividualRepository.GetDetailsByCardNumber(cardNumber, ct);
            if (individual == null)
            {
                throw new NotFoundException("NotFound");
            }

            return individual.ToDetailsDto();
        }

        public async Task<VaccinatedIndividualReadDto> AddAsync(VaccinatedIndividualCreateDto vaccinatedIndividualCreateDto, CancellationToken ct = default)
        {
            int selectedPersonId;

            if (vaccinatedIndividualCreateDto.PersonId.HasValue && vaccinatedIndividualCreateDto.PersonId.Value > 0)
            {
                var person = await _unitOfWork.PersonRepository.GetByIdAsync(vaccinatedIndividualCreateDto.PersonId.Value, ct);
                if (person == null) throw new NotFoundException("NotFound");
                selectedPersonId = person.Id;
            }
            else
            {
                if (vaccinatedIndividualCreateDto.PersonCreateDto == null) throw new DomainException("DomainError");
                var newPerson = Person.Create(
                    vaccinatedIndividualCreateDto.PersonCreateDto.FirstName,
                    vaccinatedIndividualCreateDto.PersonCreateDto.SecondName,
                    vaccinatedIndividualCreateDto.PersonCreateDto.ThirdName,
                    vaccinatedIndividualCreateDto.PersonCreateDto.LastName,
                    Enum.Parse<ICMS.Domain.Enums.Gender>(vaccinatedIndividualCreateDto.PersonCreateDto.Gender, true),
                    vaccinatedIndividualCreateDto.PersonCreateDto.DateOfBirth,
                    vaccinatedIndividualCreateDto.PersonCreateDto.PhoneNumber);

                await _unitOfWork.PersonRepository.AddAsync(newPerson, ct);
                await _unitOfWork.SaveChangesAsync(ct);
                selectedPersonId = newPerson.Id;
            }

            var vaccinatedIndividual = VaccinatedIndividual.Create(
                vaccinatedIndividualCreateDto.DirectorateId,
                vaccinatedIndividualCreateDto.NeighborhoodId,
                vaccinatedIndividualCreateDto.SubNeighborhoodId,
                vaccinatedIndividualCreateDto.UserId);

            vaccinatedIndividual.AssignExistingPersonById(selectedPersonId);

            await ScheduleAllInitialVaccinesAsync(new List<VaccinatedIndividual> { vaccinatedIndividual }, ct);

            ct.ThrowIfCancellationRequested();

            await _unitOfWork.VaccinatedIndividualRepository.AddAsync(vaccinatedIndividual, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return vaccinatedIndividual.ToReadDto();
        }

        public async Task<bool> UpdateAsync(int id, VaccinatedIndividualCreateDto updatedEntity, CancellationToken ct = default)
        {
            var individual = await _unitOfWork.VaccinatedIndividualRepository.GetByIdAsync(id, ct);
            if (individual == null)
            {
                throw new NotFoundException("NotFound");
            }

            individual.UpdateIndividualInfo(updatedEntity.DirectorateId, updatedEntity.NeighborhoodId, updatedEntity.SubNeighborhoodId);

            await _unitOfWork.VaccinatedIndividualRepository.UpdateAsync(individual, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return true;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var individual = await _unitOfWork.VaccinatedIndividualRepository.GetByIdAsync(id, ct);
            if (individual == null)
                throw new NotFoundException("NotFound");

            await _unitOfWork.VaccinatedIndividualRepository.DeleteAsync(individual, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return true;
        }

        public async Task<bool> GiveDose(ImmunizationRecordCreateDto dto, int userId, CancellationToken ct = default)
        {
            var individual = await _unitOfWork.VaccinatedIndividualRepository.GetIndividualWithSchedulesAsync(dto.IndividualId, ct);
            if (individual == null)
                throw new NotFoundException("NotFound");

            var dose = await _unitOfWork.DoseRepository.GetByIdAsync(dto.DoseId, ct);
            if (dose == null)
                throw new NotFoundException("NotFound");

            var allDoses = await _unitOfWork.DoseRepository.GetAllAsync(dose.VaccineId, ct);
            var nextDose = allDoses.OrderBy(d => d.DoseOrder).FirstOrDefault(d => d.DoseOrder > dose.DoseOrder);

            individual.AdministerDose(dose, dto.VaccinationDate, dto.TakenIn, userId, nextDose, notes: dto.Notes);

            await _unitOfWork.SaveChangesAsync(ct);
            return true;
        }

        public async Task<BulkInsertResult> BulkInsertIndividualAsync(List<NewFieldVaccinatedIndividualDto> newFieldVaccinatedIndividuals, int userId, CancellationToken ct = default)
        {
            var result = new BulkInsertResult();
            var validIndividuals = new List<VaccinatedIndividual>();
            var entityToDtoMap = new Dictionary<VaccinatedIndividual, NewFieldVaccinatedIndividualDto>();

            foreach (var dto in newFieldVaccinatedIndividuals)
            {
                try
                {
                    var personDto = dto.Person;
                    var person = Person.Create(personDto.FirstName, personDto.SecondName, personDto.ThirdName, personDto.LastName, Enum.Parse<ICMS.Domain.Enums.Gender>(personDto.Gender, true), personDto.DateOfBirth, personDto.PhoneNumber);
                    var individual = VaccinatedIndividual.Create(dto.DirectorateId, dto.NeighborhoodId, dto.SubNeighborhoodId);
                    individual.AssignPerson(person);

                    validIndividuals.Add(individual);
                    entityToDtoMap.Add(individual, dto);
                }
                catch (Exception ex)
                {
                    result.Errors.Add($"Record for {dto.Person.PhoneNumber ?? "Unknown"}: {ex.Message}");
                }
            }

            if (!validIndividuals.Any())
                return result;

            await PersistToDatabaseAsync(validIndividuals, entityToDtoMap, result, userId, ct);

            return result;
        }

        public async Task<BulkInsertResult> BulkUpdateFieldVisitIndividualAsync(List<UpdateFieldVisitIndividualDto> dtos, int userId, CancellationToken ct = default)
        {
            var result = new BulkInsertResult();
            var ids = dtos.Select(d => d.IndividualId).ToList();
            var individuals = await _unitOfWork.VaccinatedIndividualRepository.GetByIdsWithImmunizationRecordsAsync(ids, ct);

            foreach (var dto in dtos)
            {
                var individual = individuals.FirstOrDefault(vi => vi.Id == dto.IndividualId);
                if (individual == null)
                {
                    result.Errors.Add($"Individual ID {dto.IndividualId} not found.");
                    continue;
                }

                try
                {
                    var dose = await _unitOfWork.DoseRepository.GetByIdAsync(dto.DoseId, ct);
                    if (dose != null)
                    {
                        var allDoses = await _unitOfWork.DoseRepository.GetAllAsync(dose.VaccineId, ct);
                        var nextDose = allDoses.OrderBy(d => d.DoseOrder).FirstOrDefault(d => d.DoseOrder > dose.DoseOrder);

                        individual.AdministerDose(dose, dto.VaccinationDate, dto.TakenIn, userId, nextDose, notes: dto.Note);
                    }
                    result.InsertedCount++;
                }
                catch (Exception ex)
                {
                    result.Errors.Add($"Individual ID {dto.IndividualId}: {ex.Message}");
                }
            }

            await _unitOfWork.SaveChangesAsync(ct);
            return result;
        }

        public async Task<GeneratedAccountDto> GenerateAccountAsync(int id, CancellationToken ct = default)
        {
            var individual = await _unitOfWork.VaccinatedIndividualRepository.GetIndividualWithSchedulesAsync(id, ct);
            if (individual == null)
                throw new NotFoundException("NotFound");

            if (individual.UserId.HasValue)
            {
                var existingUser = await _unitOfWork.UserRepository.GetByIdAsync(individual.UserId.Value, ct);
                return new GeneratedAccountDto(existingUser?.UserName ?? "Unknown", "********", false);
            }

            string username = $"iv_{id}";
            string password = PasswordHasher.GenerateSimplePassword();
            string passwordHash = PasswordHasher.HashPassword(password);

            var user = User.Create(username, passwordHash, individual.PersonId);
            
            await _unitOfWork.UserRepository.AddAsync(user, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            // Assign Multi-Role using IdentityService
            await _identityService.AssignRolesToUserAsync(user.Id, new[] { Roles.VaccinatedIndividual }, ct);

            individual.AssignExistingUserById(user.Id);
            await _unitOfWork.SaveChangesAsync(ct);

            return new GeneratedAccountDto(username, password, true);
        }

        private async Task PersistToDatabaseAsync(
            List<VaccinatedIndividual> validIndividuals,
            Dictionary<VaccinatedIndividual, NewFieldVaccinatedIndividualDto> entityToDtoMap,
            BulkInsertResult result,
            int userId,
            CancellationToken ct = default)
        {
            try
            {
                await _unitOfWork.ExecuteInTransactionAsync(async () =>
                {
                    await _unitOfWork.VaccinatedIndividualRepository.BulkInsertAsync(validIndividuals, ct);

                    await ScheduleAllInitialVaccinesAsync(validIndividuals, ct);

                    var schedulesToInsert = validIndividuals.SelectMany(vi => vi.Schedules).ToList();
                    if (schedulesToInsert.Any())
                    {
                        await _unitOfWork.VaccinationScheduleRepository.BulkInsertAsync(schedulesToInsert, ct);
                    }

                    var recordsToInsert = new List<ImmunizationRecord>();

                    foreach (var parent in validIndividuals)
                    {
                        var dto = entityToDtoMap[parent];
                        var dose = await _unitOfWork.DoseRepository.GetByIdAsync(dto.DoseId, ct);
                        if (dose != null)
                        {
                            var allDoses = await _unitOfWork.DoseRepository.GetAllAsync(dose.VaccineId, ct);
                            var nextDose = allDoses.OrderBy(d => d.DoseOrder).FirstOrDefault(d => d.DoseOrder > dose.DoseOrder);

                            parent.AdministerDose(dose, dto.VaccinationDate, dto.TakenIn, userId, nextDose, notes: dto.Note);
                        }

                        recordsToInsert.AddRange(parent.ImmunizationRecords.Where(ir => ir.Id == Guid.Empty));
                    }

                    if (recordsToInsert.Any())
                    {
                        await _unitOfWork.ImmunizationRecordRepository.BulkInsertAsync(recordsToInsert, ct);
                    }

                    result.InsertedCount = validIndividuals.Count;
                });
            }
            catch (Exception ex)
            {
                result.InsertedCount = 0;
                result.Errors.Add($"Transaction failed: {ex.Message}");
            }
        }

        private async Task ScheduleAllInitialVaccinesAsync(List<VaccinatedIndividual> individuals, CancellationToken ct)
        {
            var allDoses = await _unitOfWork.DoseRepository.GetAllAsync(false, ct);
            foreach (var individual in individuals)
            {
                var dob = individual.Person?.DateOfBirth ?? (await _unitOfWork.PersonRepository.GetByIdAsync(individual.PersonId, ct))?.DateOfBirth;
                
                if (dob.HasValue)
                {
                    individual.ScheduleInitialVaccines(allDoses, dob.Value);
                }
            }
        }
    }
}
