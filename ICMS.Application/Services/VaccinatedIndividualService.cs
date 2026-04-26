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
    public class VaccinatedIndividualService(IUnitOfWork unitOfWork, IIdentityService identityService) : IVaccinatedIndividualService
    {
        public async Task<PagedResult<VaccinatedIndividualReadDto>> GetAllAsync(PaginationParams paginationParams, CancellationToken ct = default)
        {
            var pagedResult = await unitOfWork.VaccinatedIndividualRepository.GetPagedWithDetailsAsync(paginationParams.PageNumber, paginationParams.PageSize, ct);
            
            var items = pagedResult.Items.Select(vi => vi.ToReadDto()).ToList();
            
            return new PagedResult<VaccinatedIndividualReadDto>(items, pagedResult.TotalCount, pagedResult.PageNumber, pagedResult.PageSize);
        }

        public async Task<VaccinatedIndividualDetailsDto> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var individual = await unitOfWork.VaccinatedIndividualRepository.GetDetailsById(id, ct);
            if (individual == null)
            {
                throw new NotFoundException("NotFound");
            }

            return individual.ToDetailsDto();
        }

        public async Task<VaccinatedIndividualDetailsDto> GetByCardNumberAsync(string cardNumber, CancellationToken ct = default)
        {
            var individual = await unitOfWork.VaccinatedIndividualRepository.GetDetailsByCardNumber(cardNumber, ct);
            if (individual == null)
            {
                throw new NotFoundException("NotFound");
            }

            return individual.ToDetailsDto();
        }

        public async Task<VaccinatedIndividualReadDto> AddAsync(VaccinatedIndividualCreateDto vaccinatedIndividualCreateDto, CancellationToken ct = default)
        {
            Person? person;

            if (vaccinatedIndividualCreateDto.PersonId is > 0)
            {
                person = await unitOfWork.PersonRepository.GetByIdAsync(vaccinatedIndividualCreateDto.PersonId.Value, ct);
                if (person == null) throw new NotFoundException("NotFound");
            }
            else
            {
                if (vaccinatedIndividualCreateDto.PersonCreateDto == null) throw new DomainException("MissingPersonData");

                // 1. Check if person already exists by details to avoid UniqueConstraintException
                person = await unitOfWork.PersonRepository.GetByAsync(
                    vaccinatedIndividualCreateDto.PersonCreateDto.FirstName,
                    vaccinatedIndividualCreateDto.PersonCreateDto.LastName,
                    vaccinatedIndividualCreateDto.PersonCreateDto.PhoneNumber,
                    vaccinatedIndividualCreateDto.PersonCreateDto.DateOfBirth,
                    ct);

                if (person == null)
                {
                    person = Person.Create(
                        vaccinatedIndividualCreateDto.PersonCreateDto.FirstName,
                        vaccinatedIndividualCreateDto.PersonCreateDto.SecondName,
                        vaccinatedIndividualCreateDto.PersonCreateDto.ThirdName,
                        vaccinatedIndividualCreateDto.PersonCreateDto.LastName,
                        Enum.Parse<ICMS.Domain.Enums.Gender>(vaccinatedIndividualCreateDto.PersonCreateDto.Gender, true),
                        vaccinatedIndividualCreateDto.PersonCreateDto.DateOfBirth,
                        vaccinatedIndividualCreateDto.PersonCreateDto.PhoneNumber);

                    await unitOfWork.PersonRepository.AddAsync(person, ct);
                    await unitOfWork.SaveChangesAsync(ct);
                }
            }

            var vaccinatedIndividual = VaccinatedIndividual.Create(
                vaccinatedIndividualCreateDto.DirectorateId,
                vaccinatedIndividualCreateDto.NeighborhoodId,
                vaccinatedIndividualCreateDto.SubNeighborhoodId,
                vaccinatedIndividualCreateDto.UserId);

            // 2. Assign the person object (sets both Id and Navigation property)
            // This prevents NullReferenceException in ScheduleAllInitialVaccinesAsync
            vaccinatedIndividual.AssignPerson(person);

            await ScheduleAllInitialVaccinesAsync([vaccinatedIndividual], ct);

            ct.ThrowIfCancellationRequested();

            await unitOfWork.VaccinatedIndividualRepository.AddAsync(vaccinatedIndividual, ct);
            await unitOfWork.SaveChangesAsync(ct);

            return vaccinatedIndividual.ToReadDto();
        }

        public async Task<bool> UpdateAsync(int id, VaccinatedIndividualCreateDto updatedEntity, CancellationToken ct = default)
        {
            var individual = await unitOfWork.VaccinatedIndividualRepository.GetByIdAsync(id, ct);
            if (individual == null)
            {
                throw new NotFoundException("NotFound");
            }

            individual.UpdateIndividualInfo(updatedEntity.DirectorateId, updatedEntity.NeighborhoodId, updatedEntity.SubNeighborhoodId);

            await unitOfWork.VaccinatedIndividualRepository.UpdateAsync(individual, ct);
            await unitOfWork.SaveChangesAsync(ct);

            return true;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var individual = await unitOfWork.VaccinatedIndividualRepository.GetByIdAsync(id, ct);
            if (individual == null)
                throw new NotFoundException("NotFound");

            await unitOfWork.VaccinatedIndividualRepository.DeleteAsync(individual, ct);
            await unitOfWork.SaveChangesAsync(ct);

            return true;
        }

        public async Task<bool> GiveDose(ImmunizationRecordCreateDto dto, int userId, CancellationToken ct = default)
        {
            var individual = await unitOfWork.VaccinatedIndividualRepository.GetIndividualWithSchedulesAsync(dto.IndividualId, ct);
            if (individual == null)
                throw new NotFoundException("NotFound");

            var dose = await unitOfWork.DoseRepository.GetByIdAsync(dto.DoseId, ct);
            if (dose == null)
                throw new NotFoundException("NotFound");

            var allDoses = await unitOfWork.DoseRepository.GetAllAsync(dose.VaccineId, ct);
            var nextDose = allDoses.OrderBy(d => d.DoseOrder).FirstOrDefault(d => d.DoseOrder > dose.DoseOrder);

            individual.AdministerDose(dose, dto.VaccinationDate, dto.TakenIn, userId, nextDose, notes: dto.Notes);

            await unitOfWork.SaveChangesAsync(ct);
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
                    result.Errors.Add($"Record for {dto.Person.PhoneNumber}: {ex.Message}");
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
            var individuals = await unitOfWork.VaccinatedIndividualRepository.GetByIdsWithImmunizationRecordsAsync(ids, ct);

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
                    var dose = await unitOfWork.DoseRepository.GetByIdAsync(dto.DoseId, ct);
                    if (dose != null)
                    {
                        var allDoses = await unitOfWork.DoseRepository.GetAllAsync(dose.VaccineId, ct);
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

            await unitOfWork.SaveChangesAsync(ct);
            return result;
        }

        public async Task<GeneratedAccountDto> GenerateAccountAsync(int id, CancellationToken ct = default)
        {
            var individual = await unitOfWork.VaccinatedIndividualRepository.GetIndividualWithSchedulesAsync(id, ct);
            if (individual == null)
                throw new NotFoundException("NotFound");

            if (individual.UserId.HasValue)
            {
                var existingUser = await unitOfWork.UserRepository.GetByIdAsync(individual.UserId.Value, ct);
                return new GeneratedAccountDto(existingUser?.UserName ?? "Unknown", "********", false);
            }

            string username = $"iv_{id}";
            string password = PasswordHasher.GenerateSimplePassword();
            string passwordHash = PasswordHasher.HashPassword(password);

            var user = User.Create(username, passwordHash, individual.PersonId);
            
            await unitOfWork.UserRepository.AddAsync(user, ct);
            await unitOfWork.SaveChangesAsync(ct);

            // Assign Multi-Role using IdentityService
            await identityService.AssignRolesToUserAsync(user.Id, [Roles.VaccinatedIndividual], ct);

            individual.AssignExistingUserById(user.Id);
            await unitOfWork.SaveChangesAsync(ct);

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
                await unitOfWork.ExecuteInTransactionAsync(async () =>
                {
                    await unitOfWork.VaccinatedIndividualRepository.BulkInsertAsync(validIndividuals, ct);

                    await ScheduleAllInitialVaccinesAsync(validIndividuals, ct);

                    var schedulesToInsert = validIndividuals.SelectMany(vi => vi.Schedules).ToList();
                    if (schedulesToInsert.Any())
                    {
                        await unitOfWork.VaccinationScheduleRepository.BulkInsertAsync(schedulesToInsert, ct);
                    }

                    var recordsToInsert = new List<ImmunizationRecord>();

                    foreach (var parent in validIndividuals)
                    {
                        var dto = entityToDtoMap[parent];
                        var dose = await unitOfWork.DoseRepository.GetByIdAsync(dto.DoseId, ct);
                        if (dose != null)
                        {
                            var allDoses = await unitOfWork.DoseRepository.GetAllAsync(dose.VaccineId, ct);
                            var nextDose = allDoses.OrderBy(d => d.DoseOrder).FirstOrDefault(d => d.DoseOrder > dose.DoseOrder);

                            parent.AdministerDose(dose, dto.VaccinationDate, dto.TakenIn, userId, nextDose, notes: dto.Note);
                        }

                        recordsToInsert.AddRange(parent.ImmunizationRecords.Where(ir => ir.Id == Guid.Empty));
                    }

                    if (recordsToInsert.Any())
                    {
                        await unitOfWork.ImmunizationRecordRepository.BulkInsertAsync(recordsToInsert, ct);
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
            var allDoses = await unitOfWork.DoseRepository.GetAllAsync(false, ct);
            foreach (var individual in individuals)
            {
                var dob = individual.Person.DateOfBirth;
                individual.ScheduleInitialVaccines(allDoses, dob);
            }
        }
    }
}
