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
    public class VaccinatedIndividualService(IUnitOfWork unitOfWork, IIdentityService identityService, ICacheService cacheService)
        : IVaccinatedIndividualService
    {
        public async Task<PagedResult<VaccinatedIndividualReadDto>> GetAllAsync(PaginationParams paginationParams,
            CancellationToken ct = default)
        {
            var pagedResult =
                await unitOfWork.VaccinatedIndividualRepository.GetPagedWithDetailsAsync(paginationParams.PageNumber,
                    paginationParams.PageSize, ct);

            var items = pagedResult.Items.Select(vi => vi.ToReadDto()).ToList();

            return new PagedResult<VaccinatedIndividualReadDto>(items, pagedResult.TotalCount, pagedResult.PageNumber,
                pagedResult.PageSize);
        }

        public async Task<VaccinatedIndividualDetailsDto> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var individual = await unitOfWork.VaccinatedIndividualRepository.GetDetailsById(id, ct);
            if (individual == null)
            {
                throw new NotFoundException("NotFound");
            }

            // Auto-correct schedules on profile load (backfill missing or clean up ineligible)
            var allDoses = await unitOfWork.DoseRepository.GetAllAsync(false, ct, d => d.Vaccine);
            var isPregnant = await unitOfWork.PregnantWomanRepository.ExistAsync(pw => pw.PersonId == individual.PersonId, ct);
            individual.ScheduleInitialVaccines(allDoses, individual.Person.DateOfBirth, isPregnant);
            await unitOfWork.SaveChangesAsync(ct);
            InvalidateCache(id);

            return individual.ToDetailsDto();
        }

        public async Task<VaccinatedIndividualDetailsDto> GetByCardNumberAsync(string cardNumber,
            CancellationToken ct = default)
        {
            var individual = await unitOfWork.VaccinatedIndividualRepository.GetDetailsByCardNumber(cardNumber, ct);
            if (individual == null)
            {
                throw new NotFoundException("NotFound");
            }

            // Auto-correct schedules on profile load (backfill missing or clean up ineligible)
            var allDoses = await unitOfWork.DoseRepository.GetAllAsync(false, ct, d => d.Vaccine);
            var isPregnant = await unitOfWork.PregnantWomanRepository.ExistAsync(pw => pw.PersonId == individual.PersonId, ct);
            individual.ScheduleInitialVaccines(allDoses, individual.Person.DateOfBirth, isPregnant);
            await unitOfWork.SaveChangesAsync(ct);
            InvalidateCache(individual.Id);

            return individual.ToDetailsDto();
        }

        public async Task<VaccinatedIndividualReadDto> AddAsync(
            VaccinatedIndividualCreateDto vaccinatedIndividualCreateDto, CancellationToken ct = default)
        {
            Person? person;

            if (vaccinatedIndividualCreateDto.PersonId is > 0)
            {
                person = await unitOfWork.PersonRepository.GetByIdAsync(vaccinatedIndividualCreateDto.PersonId.Value,
                    ct);
                if (person == null) throw new NotFoundException("NotFound");
            }
            else
            {
                if (vaccinatedIndividualCreateDto.PersonCreateDto == null)
                    throw new DomainException("MissingPersonData");

                // 1. Check if person already exists by details
                person = await unitOfWork.PersonRepository.GetByAsync(
                    vaccinatedIndividualCreateDto.PersonCreateDto.FirstName,
                    vaccinatedIndividualCreateDto.PersonCreateDto.LastName,
                    vaccinatedIndividualCreateDto.PersonCreateDto.PhoneNumber,
                    vaccinatedIndividualCreateDto.PersonCreateDto.DateOfBirth,
                    ct);

                if (person != null)
                {
                    // Check if this person is already registered as a VaccinatedIndividual
                    var existingVi = await unitOfWork.VaccinatedIndividualRepository.FirstOrDefaultAsync(vi => vi.PersonId == person.Id, ct);
                    if (existingVi != null)
                    {
                        throw new DomainException("IndividualAlreadyRegistered");
                    }
                }
                else
                {
                    person = Person.Create(
                        vaccinatedIndividualCreateDto.PersonCreateDto.FirstName,
                        vaccinatedIndividualCreateDto.PersonCreateDto.SecondName,
                        vaccinatedIndividualCreateDto.PersonCreateDto.ThirdName,
                        vaccinatedIndividualCreateDto.PersonCreateDto.LastName,
                        Enum.Parse<ICMS.Domain.Enums.Gender>(vaccinatedIndividualCreateDto.PersonCreateDto.Gender,
                            true),
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
                vaccinatedIndividualCreateDto.UserId,
                vaccinatedIndividualCreateDto.RegistrationDate);

            // 2. Assign the person object (sets both Id and Navigation property)
            // This prevents NullReferenceException in ScheduleAllInitialVaccinesAsync
            vaccinatedIndividual.AssignPerson(person);

            await ScheduleAllInitialVaccinesAsync([vaccinatedIndividual], ct);

            ct.ThrowIfCancellationRequested();

            await unitOfWork.VaccinatedIndividualRepository.AddAsync(vaccinatedIndividual, ct);
            await unitOfWork.SaveChangesAsync(ct);

            // Fetch the full object with details to avoid NullReferenceException in ToReadDto
            var fullIndividual =
                await unitOfWork.VaccinatedIndividualRepository.GetByIdAsync(vaccinatedIndividual.Id, ct);
            if (fullIndividual == null) throw new NotFoundException("NotFound");

            return fullIndividual.ToReadDto();
        }

        public async Task<bool> UpdateAsync(int id, VaccinatedIndividualCreateDto updatedEntity,
            CancellationToken ct = default)
        {
            var individual = await unitOfWork.VaccinatedIndividualRepository.GetByIdAsync(id, ct);
            if (individual == null)
            {
                throw new NotFoundException("NotFound");
            }

            individual.UpdateIndividualInfo(updatedEntity.DirectorateId, updatedEntity.NeighborhoodId,
                updatedEntity.SubNeighborhoodId);

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

        public async Task<bool> DeleteAsync(int id, bool deletePersonalInfo = false, bool isSoftDelete = true, CancellationToken ct = default)
        {
            var individual = await unitOfWork.VaccinatedIndividualRepository.GetByIdAsync(id, ct);
            if (individual == null)
                throw new NotFoundException("NotFound");

            if (isSoftDelete)
            {
                individual.MarkAsDeleted();
                await unitOfWork.VaccinatedIndividualRepository.UpdateAsync(individual, ct);
            }
            else
            {
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
            }

            await unitOfWork.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> GiveDose(ImmunizationRecordCreateDto dto, int userId, CancellationToken ct = default)
        {
            var individual =
                await unitOfWork.VaccinatedIndividualRepository.GetIndividualWithSchedulesAsync(dto.IndividualId, ct);
            if (individual == null)
                throw new NotFoundException("NotFound");

            var dose = await unitOfWork.DoseRepository.GetByIdAsync(dto.DoseId, cancellationToken: ct);
            if (dose == null)
                throw new NotFoundException("NotFound");

            var allDoses = await unitOfWork.DoseRepository.GetAllAsync(dose.VaccineId, ct);
            var nextDose = allDoses.OrderBy(d => d.DoseOrder).FirstOrDefault(d => d.DoseOrder > dose.DoseOrder);

            individual.AdministerDose(dose, dto.VaccinationDate, dto.TakenIn, userId, nextDose,
                fieldVisitId: dto.FieldVisitId, notes: dto.Notes, allDoses: allDoses);

            await unitOfWork.SaveChangesAsync(ct);
            InvalidateCache(dto.IndividualId);
            return true;
        }

        public async Task<BulkSyncResultDto> BulkInsertIndividualAsync(
            List<NewFieldVaccinatedIndividualDto> newFieldVaccinatedIndividuals, int userId,
            CancellationToken ct = default)
        {
            var result = new BulkSyncResultDto();
            var allDoses = await unitOfWork.DoseRepository.GetAllAsync(false, ct, d => d.Vaccine);

            // Step 1: Prepare all entities in memory
            var staging = new List<(NewFieldVaccinatedIndividualDto Dto, VaccinatedIndividual Entity)>();

            foreach (var dto in newFieldVaccinatedIndividuals)
            {
                try
                {
                    var personDto = dto.Person;
                    if (personDto == null) throw new ArgumentNullException(nameof(dto.Person));

                    // Check if person already exists
                    var person = await unitOfWork.PersonRepository.GetByAsync(
                        personDto.FirstName ?? "Unknown",
                        personDto.LastName ?? "Unknown",
                        personDto.PhoneNumber ?? "",
                        personDto.DateOfBirth,
                        ct);

                    if (person != null)
                    {
                        var existingVi = await unitOfWork.VaccinatedIndividualRepository.FirstOrDefaultAsync(vi => vi.PersonId == person.Id, ct);
                        if (existingVi != null)
                        {
                            throw new DomainException("IndividualAlreadyRegistered");
                        }
                    }
                    else
                    {
                        person = Person.Create(
                            personDto.FirstName ?? "Unknown",
                            personDto.SecondName,
                            personDto.ThirdName,
                            personDto.LastName ?? "Unknown",
                            Enum.Parse<ICMS.Domain.Enums.Gender>(personDto.Gender ?? "Male", true),
                            personDto.DateOfBirth,
                            personDto.PhoneNumber ?? "");
                    }

                    var individual =
                        VaccinatedIndividual.Create(dto.DirectorateId, dto.NeighborhoodId, dto.SubNeighborhoodId, registrationDate: dto.RegistrationDate);
                    individual.AssignPerson(person);

                    // Optimized scheduling using prefetched doses
                    var isPregnant = person.Id > 0 && await unitOfWork.PregnantWomanRepository.ExistAsync(pw => pw.PersonId == person.Id, ct);
                    individual.ScheduleInitialVaccines(allDoses, person.DateOfBirth, isPregnant);

                    if (dto.DoseId > 0)
                    {
                        var dose = allDoses.FirstOrDefault(d => d.Id == dto.DoseId);
                        if (dose != null)
                        {
                            var vaccineDoses = allDoses.Where(d => d.VaccineId == dose.VaccineId).ToList();
                            var nextDose = vaccineDoses.OrderBy(d => d.DoseOrder)
                                .FirstOrDefault(d => d.DoseOrder > dose.DoseOrder);

                            individual.AdministerDose(dose, dto.VaccinationDate, dto.TakenIn, userId, nextDose,
                                fieldVisitId: dto.FieldVisitId, notes: dto.Note, allDoses: allDoses);
                        }
                    }

                    await unitOfWork.VaccinatedIndividualRepository.AddAsync(individual, ct);
                    staging.Add((dto, individual));
                }
                catch (Exception ex)
                {
                    result.Failures.Add(new SyncFailureDetail(dto.CorrelationId,
                        $"{dto.Person?.FirstName} {dto.Person?.LastName}".Trim(),
                        ex.Message));
                }
            }

            // Step 2: Attempt Batch Save
            try
            {
                await unitOfWork.SaveChangesAsync(ct);

                // If we reach here, the whole batch succeeded
                foreach (var item in staging)
                {
                    result.Successes.Add(new SyncSuccessDetail(item.Dto.CorrelationId, item.Entity.Id));
                    result.SuccessCount++;
                    InvalidateCache(item.Entity.Id);
                }
            }
            catch (Exception)
            {
                // Step 3: Fallback to Individual Retry if Batch fails
                unitOfWork.RollbackTracker();

                foreach (var item in staging)
                {
                    try
                    {
                        // We re-create the entities for the individual retry to ensure a fresh state
                        var personDto = item.Dto.Person;
                        var person = await unitOfWork.PersonRepository.GetByAsync(
                            personDto.FirstName ?? "Unknown",
                            personDto.LastName ?? "Unknown",
                            personDto.PhoneNumber ?? "",
                            personDto.DateOfBirth,
                            ct);

                        if (person != null)
                        {
                            var existingVi = await unitOfWork.VaccinatedIndividualRepository.FirstOrDefaultAsync(vi => vi.PersonId == person.Id, ct);
                            if (existingVi != null) throw new DomainException("IndividualAlreadyRegistered");
                        }
                        else
                        {
                            person = Person.Create(
                                personDto.FirstName ?? "Unknown",
                                personDto.SecondName,
                                personDto.ThirdName,
                                personDto.LastName ?? "Unknown",
                                Enum.Parse<ICMS.Domain.Enums.Gender>(personDto.Gender ?? "Male", true),
                                personDto.DateOfBirth,
                                personDto.PhoneNumber ?? "");
                        }

                        var individual = VaccinatedIndividual.Create(item.Dto.DirectorateId, item.Dto.NeighborhoodId,
                            item.Dto.SubNeighborhoodId, registrationDate: item.Dto.RegistrationDate);
                        individual.AssignPerson(person);
                        var isPregnant = person.Id > 0 && await unitOfWork.PregnantWomanRepository.ExistAsync(pw => pw.PersonId == person.Id, ct);
                        individual.ScheduleInitialVaccines(allDoses, person.DateOfBirth, isPregnant);

                        if (item.Dto.DoseId > 0)
                        {
                            var dose = allDoses.FirstOrDefault(d => d.Id == item.Dto.DoseId);
                            if (dose != null)
                            {
                                var vaccineDoses = allDoses.Where(d => d.VaccineId == dose.VaccineId).ToList();
                                var nextDose = vaccineDoses.OrderBy(d => d.DoseOrder)
                                    .FirstOrDefault(d => d.DoseOrder > dose.DoseOrder);
                                individual.AdministerDose(dose, item.Dto.VaccinationDate, item.Dto.TakenIn, userId,
                                    nextDose, fieldVisitId: item.Dto.FieldVisitId, notes: item.Dto.Note, allDoses: allDoses);
                            }
                        }

                        await unitOfWork.VaccinatedIndividualRepository.AddAsync(individual, ct);
                        await unitOfWork.SaveChangesAsync(ct);

                        result.Successes.Add(new SyncSuccessDetail(item.Dto.CorrelationId, individual.Id));
                        result.SuccessCount++;
                        InvalidateCache(individual.Id);
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.RollbackTracker();
                        result.Failures.Add(new SyncFailureDetail(item.Dto.CorrelationId,
                            $"{item.Dto.Person?.FirstName} {item.Dto.Person?.LastName}".Trim(),
                            ex.InnerException?.Message ?? ex.Message));
                    }
                }
            }

            return result;
        }

        public async Task<BulkSyncResultDto> BulkUpdateFieldVisitIndividualAsync(
            List<UpdateFieldVisitIndividualDto> dtos, int userId, CancellationToken ct = default)
        {
            var result = new BulkSyncResultDto();
            var ids = dtos.Select(d => d.IndividualId).ToList();

            // Optimization: Fetch all needed data upfront
            var individuals =
                await unitOfWork.VaccinatedIndividualRepository.GetByIdsWithImmunizationRecordsAsync(ids, ct);
            var allDoses = await unitOfWork.DoseRepository.GetAllAsync(false, ct, d => d.Vaccine);

            var staging = new List<(UpdateFieldVisitIndividualDto Dto, VaccinatedIndividual Entity)>();

            foreach (var dto in dtos)
            {
                try
                {
                    var individual = individuals.FirstOrDefault(vi => vi.Id == dto.IndividualId);
                    if (individual == null)
                    {
                        result.Failures.Add(new SyncFailureDetail(dto.CorrelationId, "Unknown",
                            $"Individual ID {dto.IndividualId} not found."));
                        continue;
                    }

                    var dose = allDoses.FirstOrDefault(d => d.Id == dto.DoseId);
                    if (dose == null)
                    {
                        result.Failures.Add(new SyncFailureDetail(dto.CorrelationId,
                            $"{individual.Person?.FirstName} {individual.Person?.LastName}".Trim(),
                            $"Dose ID {dto.DoseId} not found."));
                        continue;
                    }

                    var vaccineDoses = allDoses.Where(d => d.VaccineId == dose.VaccineId).ToList();
                    var nextDose = vaccineDoses.OrderBy(d => d.DoseOrder)
                        .FirstOrDefault(d => d.DoseOrder > dose.DoseOrder);

                    individual.AdministerDose(dose, dto.VaccinationDate, dto.TakenIn, userId, nextDose,
                        fieldVisitId: dto.FieldVisitId, notes: dto.Note, allDoses: allDoses);
                    staging.Add((dto, individual));
                }
                catch (Exception ex)
                {
                    result.Failures.Add(new SyncFailureDetail(dto.CorrelationId, "Unknown", ex.Message));
                }
            }

            // Step 2: Attempt Batch Save
            try
            {
                await unitOfWork.SaveChangesAsync(ct);

                // Batch succeeded
                foreach (var item in staging)
                {
                    result.Successes.Add(new SyncSuccessDetail(item.Dto.CorrelationId, item.Entity.Id));
                    result.SuccessCount++;
                    InvalidateCache(item.Entity.Id);
                }
            }
            catch (Exception)
            {
                // Step 3: Fallback to Individual Retry
                unitOfWork.RollbackTracker();

                // Re-fetch clean entities for individual retries to avoid tracker conflicts
                var freshIndividuals =
                    await unitOfWork.VaccinatedIndividualRepository.GetByIdsWithImmunizationRecordsAsync(ids, ct);

                foreach (var dto in dtos)
                {
                    // Skip if already failed in logic loop
                    if (staging.All(s => s.Dto.CorrelationId != dto.CorrelationId)) continue;

                    try
                    {
                        var individual = freshIndividuals.FirstOrDefault(vi => vi.Id == dto.IndividualId);
                        if (individual == null) continue;

                        var dose = allDoses.FirstOrDefault(d => d.Id == dto.DoseId);
                        var vaccineDoses = allDoses.Where(d => d.VaccineId == dose!.VaccineId).ToList();
                        var nextDose = vaccineDoses.OrderBy(d => d.DoseOrder)
                            .FirstOrDefault(d => d.DoseOrder > dose!.DoseOrder);

                        individual.AdministerDose(dose!, dto.VaccinationDate, dto.TakenIn, userId, nextDose,
                            fieldVisitId: dto.FieldVisitId, notes: dto.Note, allDoses: allDoses);

                        await unitOfWork.SaveChangesAsync(ct);

                        result.Successes.Add(new SyncSuccessDetail(dto.CorrelationId, individual.Id));
                        result.SuccessCount++;
                        InvalidateCache(individual.Id);
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.RollbackTracker();
                        result.Failures.Add(new SyncFailureDetail(dto.CorrelationId, "Unknown",
                            ex.InnerException?.Message ?? ex.Message));
                    }
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
                var existingUser =
                    await unitOfWork.UserRepository.GetByIdAsync(individual.UserId.Value, cancellationToken: ct);
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
            var allDoses = await unitOfWork.DoseRepository.GetAllAsync(false, ct, d => d.Vaccine);
            foreach (var individual in individuals)
            {
                var dob = individual.Person.DateOfBirth;
                var isPregnant = individual.PersonId > 0 && await unitOfWork.PregnantWomanRepository.ExistAsync(pw => pw.PersonId == individual.PersonId, ct);
                individual.ScheduleInitialVaccines(allDoses, dob, isPregnant);
            }
        }
        private void InvalidateCache(int individualId)
        {
            cacheService.Remove($"schedules:individual:{individualId}:en");
            cacheService.Remove($"schedules:individual:{individualId}:ar");
        }

        public async Task<int?> GetIndividualIdByUserIdAsync(int userId, CancellationToken ct = default)
        {
            var individual = await unitOfWork.VaccinatedIndividualRepository.FirstOrDefaultAsync(x => x.UserId == userId, ct);
            return individual?.Id;
        }
    }
}
