using ICMS.Application.DTOs.Maternal;
using ICMS.Application.DTOs.Schedules;
using ICMS.Application.DTOs.Pagination;
using ICMS.Application.DTOs.Account;
using ICMS.Application.Interfaces;
using ICMS.Application.Interfaces.Repositories;
using ICMS.Application.Interfaces.Services;
using ICMS.Application.Extensions;
using ICMS.Application.Validators;
using ICMS.Application.Utilities;
using ICMS.Domain.Constants;
using ICMS.Domain.Entites.Maternal;
using ICMS.Domain.Entites.Identity;
using ICMS.Domain.Exceptions;
using ICMS.Domain.ValueObjects;
using FluentValidation;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ICMS.Application.Services
{
    public class ReproductiveHealthService : IReproductiveHealthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IImmunizationService _immunizationService;

        public ReproductiveHealthService(IUnitOfWork unitOfWork, IImmunizationService immunizationService)
        {
            _unitOfWork = unitOfWork;
            _immunizationService = immunizationService;
        }

        public async Task<PagedResult<PregnantWomanReadDto>> GetAllPregnantWomenAsync(PaginationParams paginationParams, CancellationToken ct = default)
        {
            var pagnationValidator = new PaginationValidator();
            await pagnationValidator.ValidateAndThrowAsync(paginationParams, cancellationToken: ct);

            var query = _unitOfWork.PregnantWomanRepository.GetQueryable(false, ct).Select(pw => pw.ToReadDto());
            return query.ApplyPagination(paginationParams.PageNumber, paginationParams.PageSize);
        }

        public async Task<PregnantWomanReadDto> GetPregnantWomanByIdAsync(int id, CancellationToken ct = default)
        {
            var pw = await _unitOfWork.PregnantWomanRepository.GetByIdAsync(id, ct);
            return pw?.ToReadDto() ?? throw new NotFoundException("Pregnant woman profile not found.");
        }

        public async Task<PregnantWomanDetailsDto> GetPregnantWomanDetailsAsync(int id, CancellationToken ct = default)
        {
            var pw = await _unitOfWork.PregnantWomanRepository.GetByIdWithDetailsAsync(id, ct);
            
            if (pw == null) throw new NotFoundException("Pregnant woman profile not found.");

            return pw.ToDetailsDto();
        }

        public async Task<PregnantWomanReadDto> CreatePregnantWomanAsync(PregnantWomanCreateDto request, CancellationToken ct = default)
        {
            int selectedPersonId;

            if (request.PersonId.HasValue && request.PersonId.Value > 0)
            {
                var person = await _unitOfWork.PersonRepository.GetByIdAsync(request.PersonId.Value, ct);
                if (person == null) throw new NotFoundException("This person was not found");
                selectedPersonId = person.Id;
            }
            else
            {
                if (request.PersonCreateDto == null) throw new DomainException("Person details required to create new person.");
                var newPerson = ICMS.Domain.Entites.Identity.Person.Create(
                    request.PersonCreateDto.FirstName, request.PersonCreateDto.SecondName, request.PersonCreateDto.ThirdName,
                    request.PersonCreateDto.LastName, Enum.Parse<ICMS.Domain.Enums.Gender>(request.PersonCreateDto.Gender, true), request.PersonCreateDto.DateOfBirth, request.PersonCreateDto.PhoneNumber);

                await _unitOfWork.PersonRepository.AddAsync(newPerson, ct);
                await _unitOfWork.SaveChangesAsync(ct);
                selectedPersonId = newPerson.Id;
            }

            var pregnantWoman = PregnantWoman.Create(request.AgeRange, request.PregnancyCount, request.BloodGroup, request.RhFactor, selectedPersonId, request.UserId);
            await _unitOfWork.PregnantWomanRepository.AddAsync(pregnantWoman, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return pregnantWoman.ToReadDto();
        }

        public async Task<bool> UpdatePregnantWomanAsync(int id, PregnantWomanCreateDto request, CancellationToken ct = default)
        {
            var pw = await _unitOfWork.PregnantWomanRepository.GetByIdAsync(id, ct);
            if (pw == null) throw new NotFoundException("Pregnant woman profile not found.");

            pw.Update(request.AgeRange, request.PregnancyCount, request.BloodGroup, request.RhFactor, request.UserId);
            
            await _unitOfWork.PregnantWomanRepository.UpdateAsync(pw, ct);
            await _unitOfWork.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> DeletePregnantWomanAsync(int id, CancellationToken ct = default)
        {
            var pw = await _unitOfWork.PregnantWomanRepository.GetByIdAsync(id, ct);
            if (pw == null) throw new NotFoundException("Pregnant woman profile not found.");

            await _unitOfWork.PregnantWomanRepository.DeleteAsync(pw, ct);
            await _unitOfWork.SaveChangesAsync(ct);
            return true;
        }

        public async Task StartPregnancyAsync(StartPregnancyDto request, CancellationToken ct = default)
        {
            var pregnantWoman = await _unitOfWork.PregnantWomanRepository.GetByPersonIdWithDetailsAsync(request.PersonId, ct);

            if (pregnantWoman == null)
            {
                throw new DomainException("Maternal profile not found. Please register pregnant woman profile first.");
            }

            PreviousPregnancyComplications? pregComps = null;
            if (request.PreviousPregnancyComplications != null)
            {
                var r = request.PreviousPregnancyComplications;
                pregComps = PreviousPregnancyComplications.CreateForNewPregnancy(r.VaginalBleeding, r.RecurrentMiscarriage, r.Diabetes, r.Epilepsy, r.HeartDisease, r.Preeclampsia, r.PretermBirth);
            }

            PreviousPregnancyDeliveryComplications? delivComps = null;
            if (request.PreviousPregnancyDeliveryComplications != null)
            {
                var r = request.PreviousPregnancyDeliveryComplications;
                delivComps = PreviousPregnancyDeliveryComplications.CreateForNewPregnancy(r.CesareanSection, r.AssistedDelivery, r.StillbirthOrMultipleDeaths);
            }

            PreviousPostpartumComplications? postComps = null;
            if (request.PreviousPostpartumComplications != null)
            {
                var r = request.PreviousPostpartumComplications;
                postComps = PreviousPostpartumComplications.CreateForNewPregnancy(r.VaginalBleeding, r.PlacentaRetention, r.VaginalFistula, r.PuerperalSepsis, r.NeonatalDeath);
            }

            pregnantWoman.StartNewPregnancy(request.LMP, request.EDD, pregComps, delivComps, postComps);

            await _unitOfWork.SaveChangesAsync(ct);
        }

        public async Task AddAncVisitAsync(int pregnancyId, AddAncVisitDto request, CancellationToken ct = default)
        {
            var pregnancy = await _unitOfWork.PregnancyDetailsRepository.GetPregnancyWithDetailsAsync(pregnancyId, ct);

            if (pregnancy == null)
            {
                throw new DomainException("Pregnancy details not found.");
            }

            pregnancy.AddVisit(
                visitDate: request.VisitDate,
                pregnancyDurationInWeeks: request.PregnancyDurationInWeeks,
                weight: request.WeightInKilo,
                bloodPressure: request.BloodPressure,
                doctorSuggestedNextVisit: request.DoctorSuggestedNextVisit,
                appInUrineTest: request.AppInUrineTest,
                ogttInUrineTest: request.OgttInUrineTest,
                fetalHeartbeat: request.FetalHeartbeat,
                fetalMovement: request.FetalMovement,
                fetalPosition: request.FetalPosition,
                anaemiaOrHemoglobinType: request.AnaemiaOrHemoglobinType
            );

            if (request.TetanusDoseId.HasValue)
            {
                var vaccinatedIndividual = await _unitOfWork.VaccinatedIndividualRepository.GetByPersonIdAsync(pregnancy.PregnantWoman.PersonId, ct);
                
                if (vaccinatedIndividual == null)
                {
                    throw new DomainException("A Vaccinated Individual profile must be registered for this person before they can receive ANC immunizations.");
                }

                var doseDto = new AdministerDoseDto(
                    IndividualId: vaccinatedIndividual.Id, 
                    DoseId: request.TetanusDoseId.Value,
                    Date: request.VisitDate,
                    TakenIn: "Clinic",
                    Notes: "ANC Immunization"
                );

                await _immunizationService.AdministerDoseAsync(doseDto, ct);
            }

            await _unitOfWork.SaveChangesAsync(ct);
        }

        public async Task<List<PregnancyDetailsReadDto>> GetPregnancyHistoryAsync(int pregnantWomanId, CancellationToken ct = default)
        {
            var pw = await _unitOfWork.PregnantWomanRepository.GetByIdWithDetailsAsync(pregnantWomanId, ct);
            if (pw == null) throw new NotFoundException("Pregnant woman profile not found.");

            return pw.PregnancyDetails.Select(p => p.ToReadDto()).ToList();
        }

        public async Task ConcludePregnancyAsync(int pregnancyId, ConcludePregnancyDto request, CancellationToken ct = default)
        {
            var pregnancy = await _unitOfWork.PregnancyDetailsRepository.GetPregnancyWithDetailsAsync(pregnancyId, ct);

            if (pregnancy == null)
            {
                throw new DomainException("Pregnancy details not found.");
            }

            var newborns = request.Newborns?.Select(n => n.ToDomain(pregnancyId)).ToList();

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
        }

        public async Task<GeneratedAccountDto> GenerateAccountAsync(int id, CancellationToken ct = default)
        {
            var pregnantWoman = await _unitOfWork.PregnantWomanRepository.GetByIdAsync(id, ct);
            if (pregnantWoman == null) throw new NotFoundException("Pregnant woman profile not found.");

            if (pregnantWoman.UserId.HasValue)
            {
                throw new UserAlreadyExistsException();
            }


            //var existingUser = await _unitOfWork.UserRepository.GetByIdAsync(pregnantWoman.UserId.Value, ct);
         

            string username = $"pw_{id}";
            string password = PasswordHasher.GenerateSimplePassword();
            string passwordHash = PasswordHasher.HashPassword(password);

            var user = User.Create(username, passwordHash, pregnantWoman.PersonId);
            
            var roles = await _unitOfWork.RoleRepository.GetAllAsync(false, ct);
            var targetRole = roles.FirstOrDefault(r => r.RoleName == Roles.PregnantWoman);
            
            if (targetRole != null)
            {
                user.AddUserRole(UserRole.Create(0, targetRole.Id));
            }

            await _unitOfWork.UserRepository.AddAsync(user, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            pregnantWoman.AssignUser(user);
            await _unitOfWork.SaveChangesAsync(ct);

            return new GeneratedAccountDto(username, password, true);
        }
    }
}
