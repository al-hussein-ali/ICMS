using ICMS.Domain.ValueObjects;
using ICMS.Application.DTOs.HealthAdvisory;
using ICMS.Application.Extensions;
using ICMS.Application.Interfaces;
using ICMS.Application.Interfaces.Services;
using ICMS.Domain.Entites.Clinical;
using ICMS.Domain.Enums;
using ICMS.Domain.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ICMS.Application.Services
{
    public class HealthAdvisoryService : IHealthAdvisoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPushNotificationService _pushNotificationService;

        public HealthAdvisoryService(IUnitOfWork unitOfWork, IPushNotificationService pushNotificationService)
        {
            _unitOfWork = unitOfWork;
            _pushNotificationService = pushNotificationService;
        }

        public async Task<HealthAdvisoryDetailsDto> CreateAsync(HealthAdvisoryCreateDto dto, int currentUserId, CancellationToken ct = default)
        {
            var advisory = dto.ToDomain(currentUserId);
            
            await _unitOfWork.HealthAdvisoryRepository.AddAsync(advisory, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return advisory.ToDetailsDto();
        }

        public async Task<HealthAdvisoryDetailsDto> CreateAndSendNowAsync(HealthAdvisoryCreateDto dto, int currentUserId, CancellationToken ct = default)
        {
            var advisory = dto.ToDomain(currentUserId);
            
            // If it's "Send Now", we ensure it's marked as "today" just in case dispatch fails and background picks it up later
            // (The domain logic already defaults it to today if null)
            
            await _unitOfWork.HealthAdvisoryRepository.AddAsync(advisory, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            // Trigger immediate dispatch
            var tokens = GetDeviceTokensForTarget(advisory.Target);
            if (tokens.Any())
            {
                var success = await _pushNotificationService.SendMulticastNotificationAsync(
                    tokens,
                    advisory.Title,
                    advisory.Content,
                    ct);

                if (success)
                {
                    advisory.MarkAsSent();
                    await _unitOfWork.SaveChangesAsync(ct);
                }
            }
            else
            {
                // Mark as sent even if no tokens to prevent background service from trying
                advisory.MarkAsSent();
                await _unitOfWork.SaveChangesAsync(ct);
            }

            return advisory.ToDetailsDto();
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var advisory = await _unitOfWork.HealthAdvisoryRepository.GetByIdAsync(id, ct);
            if (advisory == null)
            {
                throw new DomainException("Health Advisory not found.");
            }

            await _unitOfWork.HealthAdvisoryRepository.DeleteAsync(advisory, ct);
            await _unitOfWork.SaveChangesAsync(ct);
        }

        public async Task<HealthAdvisoryDetailsDto> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var advisory = await _unitOfWork.HealthAdvisoryRepository.GetByIdAsync(id, ct);
            if (advisory == null)
            {
                throw new DomainException("Health Advisory not found.");
            }

            return advisory.ToDetailsDto();
        }

        public async Task<PagedResult<HealthAdvisoryReadDto>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken ct = default)
        {
            var query = _unitOfWork.HealthAdvisoryRepository.GetQueryable()
                .OrderByDescending(x => x.CreationDate);

            var totalCount = query.Count();
            var items = query.Skip((pageNumber - 1) * pageSize).Take(pageSize)
                .Select(x => x.ToReadDto())
                .ToList();

            return new PagedResult<HealthAdvisoryReadDto>(items, totalCount, pageNumber, pageSize);
        }

        private List<string> GetDeviceTokensForTarget(AdviceTarget target)
        {
            var userDeviceRepo = _unitOfWork.UserDeviceRepository.GetQueryable();

            switch (target)
            {
                case AdviceTarget.VaccinatedIndividuals:
                    var vaccinatedUsers = _unitOfWork.VaccinatedIndividualRepository.GetQueryable()
                        .Where(v => v.UserId != null)
                        .Select(v => v.UserId);
                        
                    return userDeviceRepo
                        .Where(ud => vaccinatedUsers.Contains(ud.UserId))
                        .Select(ud => ud.FcmToken)
                        .Distinct()
                        .ToList();

                case AdviceTarget.PregnantWomen:
                    var pregnantUsers = _unitOfWork.PregnantWomanRepository.GetQueryable()
                        .Where(p => p.UserId != null)
                        .Select(p => p.UserId);

                    return userDeviceRepo
                        .Where(ud => pregnantUsers.Contains(ud.UserId))
                        .Select(ud => ud.FcmToken)
                        .Distinct()
                        .ToList();

                case AdviceTarget.All:
                default:
                    return userDeviceRepo
                        .Select(ud => ud.FcmToken)
                        .Distinct()
                        .ToList();
            }
        }
    }
}
