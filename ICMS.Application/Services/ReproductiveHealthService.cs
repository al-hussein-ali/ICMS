using Microsoft.EntityFrameworkCore;
using ICMS.Application.DTOs.Maternal;
using ICMS.Application.DTOs.Schedules;
using ICMS.Application.DTOs.Pagination;
using ICMS.Application.DTOs.Account;
using ICMS.Application.Interfaces;
using ICMS.Application.Interfaces.Services;
using ICMS.Application.Extensions;
using ICMS.Application.Utilities;
using ICMS.Domain.Constants;
using ICMS.Domain.Entites.Maternal;
using ICMS.Domain.Entites.Visits;
using ICMS.Domain.Entites.Identity;
using ICMS.Domain.Exceptions;
using ICMS.Domain.ValueObjects;
using FluentValidation;


namespace ICMS.Application.Services
{
    public class ReproductiveHealthService : IReproductiveHealthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IImmunizationService _immunizationService;
        private readonly ICMS.Application.Interfaces.Services.ICacheService _cacheService;
        private readonly IValidator<PaginationParams> _paginationValidator;
        private readonly IIdentityService _identityService;

        public ReproductiveHealthService(
            IUnitOfWork unitOfWork,
            IImmunizationService immunizationService,
            ICMS.Application.Interfaces.Services.ICacheService cacheService,
            IValidator<PaginationParams> paginationValidator,
            IIdentityService identityService)
        {
            _unitOfWork = unitOfWork;
            _immunizationService = immunizationService;
            _cacheService = cacheService;
            _paginationValidator = paginationValidator;
            _identityService = identityService;
        }

        private void InvalidateCache(int id)
        {
            _cacheService.Remove($"pregnant_woman:{id}");
            _cacheService.Remove($"pregnant_woman:details:{id}");
        }

        public async Task<PagedResult<PregnantWomanReadDto>> GetAllPregnantWomenAsync(PaginationParams paginationParams,
            CancellationToken ct = default)
        {
            await _paginationValidator.ValidateAndThrowAsync(paginationParams, cancellationToken: ct);

            var query = _unitOfWork.PregnantWomanRepository.GetQueryable(false, ct)
                .Include(pw => pw.Person)
                .Select(pw => pw.ToReadDto());

            return query.ApplyPagination(paginationParams.PageNumber, paginationParams.PageSize);
        }

        public async Task<PregnantWomanReadDto> GetPregnantWomanByIdAsync(int id, CancellationToken ct = default)
        {
            string cacheKey = $"pregnant_woman:{id}";
            if (_cacheService.TryGet(cacheKey, out PregnantWomanReadDto? cached) && cached != null)
                return cached;

            var pw = await _unitOfWork.PregnantWomanRepository.GetQueryable(true, ct)
                .Include(pw => pw.Person)
                .Include(pw => pw.User)
                .FirstOrDefaultAsync(x => x.Id == id, ct);

            if (pw == null) throw new NotFoundException("NotFound");

            if (pw.UserId == null)
            {
                var user = await _unitOfWork.UserRepository.GetQueryable(true, ct)
                    .FirstOrDefaultAsync(u => u.PersonId == pw.PersonId, ct);
                if (user != null)
                {
                    pw.AssignUser(user);
                    await _unitOfWork.SaveChangesAsync(ct);
                }
            }

            var dto = pw.ToReadDto();
            _cacheService.Set(cacheKey, dto, TimeSpan.FromMinutes(10));
            return dto;
        }

        public async Task<PregnantWomanDetailsDto> GetPregnantWomanDetailsAsync(int id, CancellationToken ct = default)
        {
            string cacheKey = $"pregnant_woman:details:{id}";
            if (_cacheService.TryGet(cacheKey, out PregnantWomanDetailsDto? cached) && cached != null)
                return cached;

            var pw = await _unitOfWork.PregnantWomanRepository.GetByIdWithDetailsAsync(id, ct);

            if (pw == null) throw new NotFoundException("NotFound");

            if (pw.UserId == null)
            {
                var user = await _unitOfWork.UserRepository.GetQueryable(true, ct)
                    .FirstOrDefaultAsync(u => u.PersonId == pw.PersonId, ct);
                if (user != null)
                {
                    pw.AssignUser(user);
                    await _unitOfWork.SaveChangesAsync(ct);
                }
            }

            var dto = pw.ToDetailsDto();
            _cacheService.Set(cacheKey, dto, TimeSpan.FromMinutes(10));
            return dto;
        }

        public async Task<PregnantWomanReadDto> CreatePregnantWomanAsync(PregnantWomanCreateDto request,
            CancellationToken ct = default)
        {
            int selectedPersonId;

            if (request.PersonId.HasValue && request.PersonId.Value > 0)
            {
                var person = await _unitOfWork.PersonRepository.GetByIdAsync(request.PersonId.Value, ct);
                if (person == null) throw new NotFoundException("NotFound");
                selectedPersonId = person.Id;
            }
            else
            {
                if (request.PersonCreateDto == null) throw new DomainException("MissingPersonData");
                var newPerson = ICMS.Domain.Entites.Identity.Person.Create(
                    request.PersonCreateDto.FirstName, request.PersonCreateDto.SecondName,
                    request.PersonCreateDto.ThirdName,
                    request.PersonCreateDto.LastName,
                    Enum.Parse<ICMS.Domain.Enums.Gender>(request.PersonCreateDto.Gender, true),
                    request.PersonCreateDto.DateOfBirth, request.PersonCreateDto.PhoneNumber);

                await _unitOfWork.PersonRepository.AddAsync(newPerson, ct);
                await _unitOfWork.SaveChangesAsync(ct);
                selectedPersonId = newPerson.Id;
            }

            var pregnantWoman = PregnantWoman.Create(request.AgeRange, request.PregnancyCount, request.BloodGroup,
                request.RhFactor, selectedPersonId, request.CurrentAddress, request.UserId > 0 ? request.UserId : null);
            await _unitOfWork.PregnantWomanRepository.AddAsync(pregnantWoman, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            // Check if there is an existing VaccinatedIndividual for this person to update schedules
            var vaccinatedIndividual = await _unitOfWork.VaccinatedIndividualRepository.GetQueryable(true, ct)
                .Include(vi => vi.Person)
                .Include(vi => vi.Schedules)
                .FirstOrDefaultAsync(vi => vi.PersonId == selectedPersonId, ct);

            if (vaccinatedIndividual != null)
            {
                var allDoses = await _unitOfWork.DoseRepository.GetAllAsync(false, ct, d => d.Vaccine);
                vaccinatedIndividual.ScheduleInitialVaccines(allDoses, vaccinatedIndividual.Person.DateOfBirth, true);
                await _unitOfWork.SaveChangesAsync(ct);
            }

            return await GetPregnantWomanByIdAsync(pregnantWoman.Id, ct);
        }

        public async Task<bool> UpdatePregnantWomanAsync(int id, PregnantWomanCreateDto request,
            CancellationToken ct = default)
        {
            var pw = await _unitOfWork.PregnantWomanRepository.GetQueryable(true, ct)
                .Include(pw => pw.Person)
                .FirstOrDefaultAsync(x => x.Id == id, ct);

            if (pw == null) throw new NotFoundException("NotFound");

            // Update PregnantWoman fields
            pw.Update(request.AgeRange, request.PregnancyCount, request.BloodGroup, request.RhFactor,
                request.CurrentAddress, request.UserId);

            // Update linked Person fields if data is provided
            if (request.PersonCreateDto != null && pw.Person != null)
            {
                pw.Person.UpdatePersonInfo(
                    request.PersonCreateDto.FirstName,
                    request.PersonCreateDto.SecondName,
                    request.PersonCreateDto.ThirdName,
                    request.PersonCreateDto.LastName,
                    Enum.Parse<ICMS.Domain.Enums.Gender>(request.PersonCreateDto.Gender, true),
                    request.PersonCreateDto.DateOfBirth,
                    request.PersonCreateDto.PhoneNumber
                );
            }

            await _unitOfWork.PregnantWomanRepository.UpdateAsync(pw, ct);
            await _unitOfWork.SaveChangesAsync(ct);
            InvalidateCache(id);
            return true;
        }

        public async Task<bool> DeletePregnantWomanAsync(int id, CancellationToken ct = default)
        {
            var pw = await _unitOfWork.PregnantWomanRepository.GetByIdAsync(id, ct);
            if (pw == null) throw new NotFoundException("NotFound");

            await _unitOfWork.PregnantWomanRepository.DeleteAsync(pw, ct);
            await _unitOfWork.SaveChangesAsync(ct);
            InvalidateCache(id);
            return true;
        }

        public async Task StartPregnancyAsync(StartPregnancyDto request, int userId, CancellationToken ct = default)
        {
            var pregnantWoman =
                await _unitOfWork.PregnantWomanRepository.GetByPersonIdWithDetailsAsync(request.PersonId, ct);

            if (pregnantWoman == null)
            {
                var person = await _unitOfWork.PersonRepository.GetByIdAsync(request.PersonId, ct);
                if (person == null) throw new DomainException("PersonNotFound");

                int age = DateTime.Today.Year - person.DateOfBirth.Year;
                string ageRange = age < 20 ? "16-20" : age < 40 ? "20-39" : "40+";

                pregnantWoman = PregnantWoman.Create(
                    ageRange: ageRange,
                    pregnancyCount: 0,
                    bloodGroup: ICMS.Domain.Enums.BloodGroup.O,
                    rhFactor: ICMS.Domain.Enums.RhFactor.Positive,
                    personId: request.PersonId,
                    userId: null
                );

                await _unitOfWork.PregnantWomanRepository.AddAsync(pregnantWoman, ct);
                await _unitOfWork.SaveChangesAsync(ct);

                // Check if there is an existing VaccinatedIndividual for this person to update schedules
                var vaccinatedIndividual = await _unitOfWork.VaccinatedIndividualRepository.GetQueryable(true, ct)
                    .Include(vi => vi.Person)
                    .Include(vi => vi.Schedules)
                    .FirstOrDefaultAsync(vi => vi.PersonId == request.PersonId, ct);

                if (vaccinatedIndividual != null)
                {
                    var allDoses = await _unitOfWork.DoseRepository.GetAllAsync(false, ct, d => d.Vaccine);
                    vaccinatedIndividual.ScheduleInitialVaccines(allDoses, vaccinatedIndividual.Person.DateOfBirth,
                        true);
                    await _unitOfWork.SaveChangesAsync(ct);
                }
            }

            PreviousPregnancyComplications? pregComps = null;
            if (request.PreviousPregnancyComplications != null)
            {
                var r = request.PreviousPregnancyComplications;
                pregComps = PreviousPregnancyComplications.CreateForNewPregnancy(r.VaginalBleeding,
                    r.RecurrentMiscarriage, r.Diabetes, r.Epilepsy, r.HeartDisease, r.Preeclampsia, r.PretermBirth);
            }

            PreviousPregnancyDeliveryComplications? delivComps = null;
            if (request.PreviousPregnancyDeliveryComplications != null)
            {
                var r = request.PreviousPregnancyDeliveryComplications;
                delivComps = PreviousPregnancyDeliveryComplications.CreateForNewPregnancy(r.CesareanSection,
                    r.AssistedDelivery, r.StillbirthOrMultipleDeaths);
            }

            PreviousPostpartumComplications? postComps = null;
            if (request.PreviousPostpartumComplications != null)
            {
                var r = request.PreviousPostpartumComplications;
                postComps = PreviousPostpartumComplications.CreateForNewPregnancy(r.VaginalBleeding,
                    r.PlacentaRetention, r.VaginalFistula, r.PuerperalSepsis, r.NeonatalDeath);
            }

            pregnantWoman.StartNewPregnancy(request.LMP, request.EDD, userId, pregComps, delivComps, postComps);

            await _unitOfWork.SaveChangesAsync(ct);
            InvalidateCache(pregnantWoman.Id);
        }

        public async Task AddAncVisitAsync(int pregnancyId, AddAncVisitDto request, int userId,
            CancellationToken ct = default)
        {
            var pregnancy = await _unitOfWork.PregnancyDetailsRepository.GetPregnancyWithDetailsAsync(pregnancyId, ct);

            if (pregnancy == null)
            {
                throw new DomainException("MotherNotFound");
            }

            var fetalDetailsList = request.FetalDetails.Select(fd => FetalDetails.Create(
                visitDetailsId: 0,
                fetusLabel: fd.FetusLabel,
                fetalHeartbeat: fd.FetalHeartbeat,
                fetalMovement: fd.FetalMovement,
                fetalPosition: fd.FetalPosition,
                fetalHeartbeatValue: fd.FetalHeartbeatValue
            )).ToList();

            pregnancy.AddVisit(
                visitDate: request.VisitDate,
                pregnancyDurationInWeeks: request.PregnancyDurationInWeeks,
                weight: request.WeightInKilo,
                bloodPressure: request.BloodPressure,
                userId: userId,
                fetalDetails: fetalDetailsList,
                doctorSuggestedNextVisit: request.DoctorSuggestedNextVisit,
                appInUrineTest: request.AppInUrineTest,
                ogttInUrineTest: request.OgttInUrineTest,
                anaemiaOrHemoglobinType: request.AnaemiaOrHemoglobinType,
                clinicalExaminationAndObservation: request.ClinicalExaminationAndObservation,
                treatmentsGiven: request.TreatmentsGiven,
                legsSwelling: request.LegsSwelling,
                vaginalBleeding: request.VaginalBleeding
            );

            await _unitOfWork.SaveChangesAsync(ct);
            InvalidateCache(pregnancy.PregnantWomanId);
        }

        public async Task<List<PregnancyDetailsReadDto>> GetPregnancyHistoryAsync(int pregnantWomanId,
            CancellationToken ct = default)
        {
            var pw = await _unitOfWork.PregnantWomanRepository.GetByIdWithDetailsAsync(pregnantWomanId, ct);
            if (pw == null) throw new NotFoundException("NotFound");

            return pw.PregnancyDetails.Select(p => p.ToReadDto()).ToList();
        }

        public async Task<PregnancyDetailsReadDto> GetPregnancyByIdAsync(int id, CancellationToken ct = default)
        {
            var pregnancy = await _unitOfWork.PregnancyDetailsRepository.GetPregnancyWithDetailsAsync(id, ct);
            if (pregnancy == null) throw new NotFoundException("PregnancyNotFound");

            return pregnancy.ToReadDto();
        }

        public async Task<List<AddAncVisitDto>> GetVisitsAsync(int pregnancyId, CancellationToken ct = default)
        {
            var pregnancy = await _unitOfWork.PregnancyDetailsRepository.GetPregnancyWithDetailsAsync(pregnancyId, ct);
            if (pregnancy == null) throw new NotFoundException("NotFound");

            return pregnancy.VisitDetails.Select(v => v.ToAncVisitDto()).ToList();
        }

        public async Task<AddAncVisitDto> GetVisitByIdAsync(int visitId, CancellationToken ct = default)
        {
            var visit = await _unitOfWork.VisitDetailsRepository.GetQueryable(false, ct)
                .Include(v => v.FetalDetailsList)
                .FirstOrDefaultAsync(v => v.Id == visitId, ct);
            if (visit == null) throw new NotFoundException("VisitNotFound");

            return visit.ToAncVisitDto();
        }

        public async Task ConcludePregnancyAsync(int pregnancyId, ConcludePregnancyDto request, int userId,
            CancellationToken ct = default)
        {
            var pregnancy = await _unitOfWork.PregnancyDetailsRepository.GetPregnancyWithDetailsAsync(pregnancyId, ct);

            if (pregnancy == null)
            {
                throw new DomainException("MotherNotFound");
            }

            var newborns = request.Newborns?.Select(n => n.ToDomain(pregnancyId, userId)).ToList();

            pregnancy.ConcludePregnancy(
                deliveryDate: request.DeliveryDate,
                birthNature: request.BirthNature,
                locationType: request.BirthLocationType,
                locationDetails: request.BirthLocationDetails,
                intrapartumComplications: request.IntrapartumComplications,
                postpartumComplications: request.PostpartumComplications,
                newborns: newborns
            );

            await _unitOfWork.SaveChangesAsync(ct);
            InvalidateCache(pregnancy.PregnantWomanId);
        }

        public async Task UpdatePregnancyAsync(int id, UpdatePregnancyDto request, CancellationToken ct = default)
        {
            var pregnancy = await _unitOfWork.PregnancyDetailsRepository.GetByIdAsync(id, ct);
            if (pregnancy == null) throw new NotFoundException("PregnancyNotFound");

            pregnancy.UpdateDetails(request.LMP, request.EDD, request.PregnancyType);

            await _unitOfWork.PregnancyDetailsRepository.UpdateAsync(pregnancy, ct);
            await _unitOfWork.SaveChangesAsync(ct);
            InvalidateCache(pregnancy.PregnantWomanId);
        }

        public async Task DeletePregnancyAsync(int id, CancellationToken ct = default)
        {
            var pregnancy = await _unitOfWork.PregnancyDetailsRepository.GetByIdAsync(id, ct);
            if (pregnancy == null) throw new NotFoundException("PregnancyNotFound");

            var woman = await _unitOfWork.PregnantWomanRepository.GetByIdAsync(pregnancy.PregnantWomanId, ct);
            if (woman != null)
            {
                woman.DecrementPregnancyCount();
                await _unitOfWork.PregnantWomanRepository.UpdateAsync(woman, ct);
            }

            await _unitOfWork.PregnancyDetailsRepository.DeleteAsync(pregnancy, ct);
            await _unitOfWork.SaveChangesAsync(ct);
            InvalidateCache(pregnancy.PregnantWomanId);
        }

        public async Task<GeneratedAccountDto> GenerateAccountAsync(int id, CancellationToken ct = default)
        {
            var pregnantWoman = await _unitOfWork.PregnantWomanRepository.GetByIdAsync(id, ct);
            if (pregnantWoman == null) throw new NotFoundException("NotFound");

            // Check if this pregnant woman record already has a user linked
            if (pregnantWoman.UserId.HasValue)
            {
                throw new UserAlreadyExistsException();
            }

            // Check if a user already exists for this person (e.g. from another module)
            var existingUser =
                await _unitOfWork.UserRepository.FirstOrDefaultAsync(u => u.PersonId == pregnantWoman.PersonId, ct);

            string username;
            string password = PasswordHasher.GenerateSimplePassword();
            string passwordHash = PasswordHasher.HashPassword(password);

            if (existingUser != null)
            {
                // Use existing user, but update password for this request
                username = existingUser.UserName;
                existingUser.ChangePassword(passwordHash);

                // Assign role if missing
                await _identityService.AssignRolesToUserAsync(existingUser.Id, [Roles.PregnantWoman], ct);

                pregnantWoman.AssignUser(existingUser);
                await _unitOfWork.SaveChangesAsync(ct);
            }
            else
            {
                // Create new user
                username = $"pw_{id}";
                var user = User.Create(username, passwordHash, pregnantWoman.PersonId);

                await _unitOfWork.UserRepository.AddAsync(user, ct);
                await _unitOfWork.SaveChangesAsync(ct);

                // Assign PregnantWoman Role
                await _identityService.AssignRolesToUserAsync(user.Id, [Roles.PregnantWoman], ct);

                pregnantWoman.AssignUser(user);
                await _unitOfWork.SaveChangesAsync(ct);
            }

            InvalidateCache(id);
            return new GeneratedAccountDto(username, password, true);
        }

        public async Task UpdateVisitAsync(int id, AddAncVisitDto request, CancellationToken ct = default)
        {
            var visit = await _unitOfWork.VisitDetailsRepository.GetQueryable(true, ct)
                            .Include(v => v.FetalDetailsList)
                            .FirstOrDefaultAsync(v => v.Id == id, ct)
                        ?? throw new NotFoundException("Visit not found");

            visit.Update(
                visitDate: request.VisitDate,
                weightInKilo: request.WeightInKilo,
                pregnancyDurationInWeeks: request.PregnancyDurationInWeeks,
                bloodPressure: request.BloodPressure,
                appInUrineTest: request.AppInUrineTest,
                ogttInUrineTest: request.OgttInUrineTest,
                anaemiaOrHemoglobinType: request.AnaemiaOrHemoglobinType,
                legsSwelling: request.LegsSwelling,
                vaginalBleeding: request.VaginalBleeding,
                clinicalExaminationAndObservation: request.ClinicalExaminationAndObservation,
                nextVisitDate: request.DoctorSuggestedNextVisit,
                treatmentsGiven: request.TreatmentsGiven
            );

            visit.ClearFetalDetails();
            if (request.FetalDetails != null)
            {
                var newFetalDetails = request.FetalDetails.Select(fd => FetalDetails.Create(
                    visitDetailsId: visit.Id,
                    fetusLabel: fd.FetusLabel,
                    fetalHeartbeat: fd.FetalHeartbeat,
                    fetalMovement: fd.FetalMovement,
                    fetalPosition: fd.FetalPosition,
                    fetalHeartbeatValue: fd.FetalHeartbeatValue
                )).ToList();
                visit.AddFetalDetailsRange(newFetalDetails);
            }

            await _unitOfWork.VisitDetailsRepository.UpdateAsync(visit, ct);
            await _unitOfWork.SaveChangesAsync(ct);
        }

        public async Task DeleteVisitAsync(int id, CancellationToken ct = default)
        {
            var visit = await _unitOfWork.VisitDetailsRepository.GetByIdAsync(id, ct)
                        ?? throw new NotFoundException("Visit not found");

            var pregnancy = await _unitOfWork.PregnancyDetailsRepository.GetByIdAsync(visit.PregnancyDetailsId, ct);
            if (pregnancy != null)
            {
                pregnancy.DecrementVisitsCount();
                await _unitOfWork.PregnancyDetailsRepository.UpdateAsync(pregnancy, ct);
            }

            await _unitOfWork.VisitDetailsRepository.DeleteAsync(visit, ct);
            await _unitOfWork.SaveChangesAsync(ct);
        }

        public async Task<int?> GetWomanIdByUserIdAsync(int userId, CancellationToken ct = default)
        {
            // 1. Try finding by direct UserId link first (fastest)
            var pw = await _unitOfWork.PregnantWomanRepository.FirstOrDefaultAsync(x => x.UserId == userId, ct);
            if (pw != null) return pw.Id;

            // 2. If not found, resolve via PersonId (this handles cases where a user account was 
            // created in another module but not yet linked to this record)
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId, ct);
            if (user != null)
            {
                pw = await _unitOfWork.PregnantWomanRepository.FirstOrDefaultAsync(x => x.PersonId == user.PersonId,
                    ct);
                if (pw != null)
                {
                    // Repair the link for future fast lookups
                    pw.AssignUser(user);
                    await _unitOfWork.SaveChangesAsync(ct);
                    return pw.Id;
                }
            }

            return null;
        }

        public async Task<ReproductiveHealthStatisticsDto> GetStatisticsAsync(CancellationToken ct = default)
        {
            var activePregnancies = await _unitOfWork.PregnancyDetailsRepository.GetQueryable(false, ct)
                .CountAsync(pd => !pd.IsPregnancyDone, ct);

            var totalVisits = await _unitOfWork.VisitDetailsRepository.GetQueryable(false, ct)
                .CountAsync(ct);

            var totalNewborns = await _unitOfWork.PregnancyDetailsRepository.GetQueryable(false, ct)
                .SumAsync(pd => (int)pd.NewbornCount, ct);

            var successfulDeliveries = await _unitOfWork.PregnancyDetailsRepository.GetQueryable(false, ct)
                .CountAsync(pd => pd.IsPregnancyDone, ct);

            return new ReproductiveHealthStatisticsDto(
                ActivePregnancies: activePregnancies,
                HighRiskCases: 0, // Simplified for now
                SuccessfulDeliveries: successfulDeliveries,
                TotalVisits: totalVisits,
                TotalNewborns: totalNewborns
            );
        }
    }
}
