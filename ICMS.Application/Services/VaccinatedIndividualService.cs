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

            // Fetch the full object with details to avoid NullReferenceException in ToReadDto
            var fullIndividual = await unitOfWork.VaccinatedIndividualRepository.GetByIdAsync(vaccinatedIndividual.Id, ct);
            if (fullIndividual == null) throw new NotFoundException("NotFound");

            return fullIndividual.ToReadDto();
        }

        public async Task<bool> UpdateAsync(int id, VaccinatedIndividualCreateDto updatedEntity, CancellationToken ct = default)
        {
            var individual = await unitOfWork.VaccinatedIndividualRepository.GetByIdAsync(id, ct);
            if (individual == null)
            {
                throw new NotFoundException("NotFound");
            }

            individual.UpdateIndividualInfo(updatedEntity.DirectorateId, updatedEntity.NeighborhoodId, updatedEntity.SubNeighborhoodId);

            // Handle switching to a different existing person
            if (updatedEntity.PersonId > 0 && updatedEntity.PersonId != individual.PersonId)
            {
                var newPerson = await unitOfWork.PersonRepository.GetByIdAsync(updatedEntity.PersonId.Value, ct);
                if (newPerson == null) throw new NotFoundException("NotFound");
                individual.AssignExistingPersonById(newPerson.Id);
            }
            else if (updatedEntity.PersonCreateDto != null)
            {
                var person = individual.Person;
                if (person == null && individual.PersonId > 0)
                {
                    person = await unitOfWork.PersonRepository.GetByIdAsync(individual.PersonId, ct);
                }

                if (person != null)
                {
                    person.UpdatePersonInfo(
                        updatedEntity.PersonCreateDto.FirstName,
                        updatedEntity.PersonCreateDto.SecondName,
                        updatedEntity.PersonCreateDto.ThirdName,
                        updatedEntity.PersonCreateDto.LastName,
                        updatedEntity.PersonCreateDto.Gender.FromStringToGenderEnum(),
                        updatedEntity.PersonCreateDto.DateOfBirth,
                        updatedEntity.PersonCreateDto.PhoneNumber
                    );
                    await unitOfWork.PersonRepository.UpdateAsync(person, ct);
                }
            }

            await unitOfWork.VaccinatedIndividualRepository.UpdateAsync(individual, ct);
            await unitOfWork.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> DeleteAsync(int id, bool deletePersonalInfo = false, CancellationToken ct = default)
        {
            var individual = await unitOfWork.VaccinatedIndividualRepository.GetByIdAsync(id, ct);
            if (individual == null)
                throw new NotFoundException("NotFound");

            int personId = individual.PersonId;

            await unitOfWork.VaccinatedIndividualRepository.DeleteAsync(individual, ct);
            
            if (deletePersonalInfo)
            {
                var person = await unitOfWork.PersonRepository.GetByIdAsync(personId, ct);
                if (person != null)
                {
                    await unitOfWork.PersonRepository.DeleteAsync(person, ct);
                }
            }

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

        public async Task<BulkSyncResultDto> BulkInsertIndividualAsync(List<NewFieldVaccinatedIndividualDto> newFieldVaccinatedIndividuals, int userId, CancellationToken ct = default)
        {
            var result = new BulkSyncResultDto();
            var allDoses = await unitOfWork.DoseRepository.GetAllAsync(false, ct);

            foreach (var dto in newFieldVaccinatedIndividuals)
            {
                var fullName = $"{dto.Person?.FirstName} {dto.Person?.LastName}".Trim();
                try
                {
                    var personDto = dto.Person;
                    if (personDto == null) throw new ArgumentNullException(nameof(dto.Person));
                    var person = Person.Create(
                        personDto.FirstName ?? "Unknown", 
                        personDto.SecondName, 
                        personDto.ThirdName, 
                        personDto.LastName ?? "Unknown", 
                        Enum.Parse<ICMS.Domain.Enums.Gender>(personDto.Gender ?? "Male", true), 
                        personDto.DateOfBirth, 
                        personDto.PhoneNumber ?? "");
                    var individual = VaccinatedIndividual.Create(dto.DirectorateId, dto.NeighborhoodId, dto.SubNeighborhoodId);
                    individual.AssignPerson(person);

                    individual.ScheduleInitialVaccines(allDoses, person.DateOfBirth);

                    var dose = await unitOfWork.DoseRepository.GetByIdAsync(dto.DoseId, ct);
                    if (dose != null)
                    {
                        var vaccineDoses = await unitOfWork.DoseRepository.GetAllAsync(dose.VaccineId, ct);
                        var nextDose = vaccineDoses.OrderBy(d => d.DoseOrder).FirstOrDefault(d => d.DoseOrder > dose.DoseOrder);

                        individual.AdministerDose(dose, dto.VaccinationDate, dto.TakenIn, userId, nextDose, notes: dto.Note);
                    }

                    await unitOfWork.VaccinatedIndividualRepository.AddAsync(individual, ct);
                    await unitOfWork.SaveChangesAsync(ct);

                    result.Successes.Add(new SyncSuccessDetail(dto.CorrelationId, individual.Id));
                    result.SuccessCount++;
                }
                catch (Exception ex)
                {
                    result.Failures.Add(new SyncFailureDetail(dto.CorrelationId, string.IsNullOrEmpty(fullName) ? "Unknown" : fullName, ex.InnerException?.Message ?? ex.Message));
                }
            }

            return result;
        }

        public async Task<BulkSyncResultDto> BulkUpdateFieldVisitIndividualAsync(List<UpdateFieldVisitIndividualDto> dtos, int userId, CancellationToken ct = default)
        {
            var result = new BulkSyncResultDto();
            var ids = dtos.Select(d => d.IndividualId).ToList();
            var individuals = await unitOfWork.VaccinatedIndividualRepository.GetByIdsWithImmunizationRecordsAsync(ids, ct);

            foreach (var dto in dtos)
            {
                try
                {
                    var individual = individuals.FirstOrDefault(vi => vi.Id == dto.IndividualId);
                    if (individual == null)
                    {
                        result.Failures.Add(new SyncFailureDetail(dto.CorrelationId, "Unknown", $"Individual ID {dto.IndividualId} not found."));
                        continue;
                    }

                    var dose = await unitOfWork.DoseRepository.GetByIdAsync(dto.DoseId, ct);
                    if (dose != null)
                    {
                        var allDoses = await unitOfWork.DoseRepository.GetAllAsync(dose.VaccineId, ct);
                        var nextDose = allDoses.OrderBy(d => d.DoseOrder).FirstOrDefault(d => d.DoseOrder > dose.DoseOrder);

                        individual.AdministerDose(dose, dto.VaccinationDate, dto.TakenIn, userId, nextDose, notes: dto.Note);
                        
                        await unitOfWork.SaveChangesAsync(ct);
                        
                        result.Successes.Add(new SyncSuccessDetail(dto.CorrelationId, individual.Id));
                        result.SuccessCount++;
                    }
                    else
                    {
                        result.Failures.Add(new SyncFailureDetail(dto.CorrelationId, $"{individual.Person?.FirstName} {individual.Person?.LastName}".Trim(), $"Dose ID {dto.DoseId} not found."));
                    }
                }
                catch (Exception ex)
                {
                    result.Failures.Add(new SyncFailureDetail(dto.CorrelationId, "Unknown", ex.InnerException?.Message ?? ex.Message));
                }
            }

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

            // Check if a user already exists for this person (e.g. from another module)
            var user = await unitOfWork.UserRepository.FirstOrDefaultAsync(u => u.PersonId == individual.PersonId, ct);

            string username;
            string password = PasswordHasher.GenerateSimplePassword();
            string passwordHash = PasswordHasher.HashPassword(password);

            if (user != null)
            {
                // Use existing user, but update password for this request
                username = user.UserName;
                user.ChangePassword(passwordHash);

                // Assign role if missing
                await identityService.AssignRolesToUserAsync(user.Id, [Roles.VaccinatedIndividual], ct);

                individual.AssignExistingUserById(user.Id);
                await unitOfWork.SaveChangesAsync(ct);
            }
            else
            {
                // Create new user
                username = $"iv_{id}";
                user = User.Create(username, passwordHash, individual.PersonId);

                await unitOfWork.UserRepository.AddAsync(user, ct);
                await unitOfWork.SaveChangesAsync(ct);

                // Assign Multi-Role using IdentityService
                await identityService.AssignRolesToUserAsync(user.Id, [Roles.VaccinatedIndividual], ct);

                individual.AssignExistingUserById(user.Id);
                await unitOfWork.SaveChangesAsync(ct);
            }

            return new GeneratedAccountDto(username, password, true);
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
