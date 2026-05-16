using ICMS.Domain.ValueObjects;
using ICMS.Application.DTOs.HealthAdvisory;
using ICMS.Application.Extensions;
using ICMS.Application.Interfaces;
using ICMS.Application.Interfaces.Services;
using ICMS.Domain.Entites.Clinical;
using ICMS.Domain.Enums;
using ICMS.Domain.Exceptions;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.IO;

namespace ICMS.Application.Services
{
    public class HealthAdvisoryService : IHealthAdvisoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPushNotificationService _pushNotificationService;
        private readonly IConfiguration _configuration;

        public HealthAdvisoryService(IUnitOfWork unitOfWork, IPushNotificationService pushNotificationService,
            IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _pushNotificationService = pushNotificationService;
            _configuration = configuration;
        }

        public async Task<HealthAdvisoryDetailsDto> CreateAsync(HealthAdvisoryCreateDto dto, int currentUserId,
            CancellationToken ct = default)
        {
            string? imageUrl = null;
            if (!string.IsNullOrEmpty(dto.ImageBase64))
            {
                imageUrl = await SaveImage(dto.ImageBase64);
            }

            var advisory = HealthAdvisory.Create(dto.Title, dto.Content, dto.Target, dto.ScheduledDate, currentUserId,
                imageUrl);

            await _unitOfWork.HealthAdvisoryRepository.AddAsync(advisory, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return advisory.ToDetailsDto();
        }

        public async Task<HealthAdvisoryDetailsDto> CreateAndSendNowAsync(HealthAdvisoryCreateDto dto,
            int currentUserId, CancellationToken ct = default)
        {
            string? imageUrl = null;
            if (!string.IsNullOrEmpty(dto.ImageBase64))
            {
                imageUrl = await SaveImage(dto.ImageBase64);
            }

            var advisory = HealthAdvisory.Create(dto.Title, dto.Content, dto.Target, dto.ScheduledDate, currentUserId,
                imageUrl);

            await _unitOfWork.HealthAdvisoryRepository.AddAsync(advisory, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            // Trigger immediate dispatch
            var tokens = GetDeviceTokensForTarget(advisory.Target);
            if (tokens.Any())
            {
                var baseUrl = _configuration["ApiBaseUrl"]?.TrimEnd('/');
                var fullImageUrl = !string.IsNullOrEmpty(advisory.ImageUrl) && !string.IsNullOrEmpty(baseUrl)
                    ? $"{baseUrl}{advisory.ImageUrl}"
                    : null;

                var data = new Dictionary<string, string>
                {
                    { "type", "advisory" },
                    { "id", advisory.Id.ToString() }
                };

                var success = await _pushNotificationService.SendMulticastNotificationAsync(
                    tokens,
                    advisory.Title,
                    advisory.Content,
                    fullImageUrl,
                    data,
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
                throw new DomainException("HealthAdvisoryNotFound");
            }

            await _unitOfWork.HealthAdvisoryRepository.DeleteAsync(advisory, ct);
            await _unitOfWork.SaveChangesAsync(ct);
        }

        public async Task<HealthAdvisoryDetailsDto> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var advisory = await _unitOfWork.HealthAdvisoryRepository.GetByIdAsync(id, ct);
            if (advisory == null)
            {
                throw new DomainException("HealthAdvisoryNotFound");
            }

            return advisory.ToDetailsDto();
        }

        public async Task<PagedResult<HealthAdvisoryReadDto>> GetPagedAsync(int pageNumber, int pageSize,
            CancellationToken ct = default)
        {
            var query = _unitOfWork.HealthAdvisoryRepository.GetQueryable()
                .OrderByDescending(x => x.CreationDate);

            var totalCount = query.Count();
            var items = query.Skip((pageNumber - 1) * pageSize).Take(pageSize)
                .Select(x => x.ToReadDto())
                .ToList();

            return new PagedResult<HealthAdvisoryReadDto>(items, totalCount, pageNumber, pageSize);
        }

        public async Task<HealthAdvisoryDetailsDto> UpdateAsync(int id, HealthAdvisoryCreateDto dto,
            CancellationToken ct = default)
        {
            var advisory = await _unitOfWork.HealthAdvisoryRepository.GetByIdAsync(id, ct);
            if (advisory == null)
            {
                throw new DomainException("HealthAdvisoryNotFound");
            }

            string? imageUrl = advisory.ImageUrl;
            if (!string.IsNullOrEmpty(dto.ImageBase64))
            {
                if (dto.ImageBase64.StartsWith("data:image"))
                {
                    imageUrl = await SaveImage(dto.ImageBase64);
                }
            }
            else
            {
                imageUrl = null;
            }

            var finalDate = dto.ScheduledDate ?? DateOnly.FromDateTime(DateTime.UtcNow.AddHours(3));
            advisory.Update(dto.Title, dto.Content, dto.Target, finalDate, imageUrl);

            await _unitOfWork.HealthAdvisoryRepository.UpdateAsync(advisory, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return advisory.ToDetailsDto();
        }

        public async Task<HealthAdvisoryDetailsDto> UpdateAndSendNowAsync(int id, HealthAdvisoryCreateDto dto,
            CancellationToken ct = default)
        {
            var advisory = await _unitOfWork.HealthAdvisoryRepository.GetByIdAsync(id, ct);
            if (advisory == null)
            {
                throw new DomainException("HealthAdvisoryNotFound");
            }

            string? imageUrl = advisory.ImageUrl;
            if (!string.IsNullOrEmpty(dto.ImageBase64))
            {
                if (dto.ImageBase64.StartsWith("data:image"))
                {
                    imageUrl = await SaveImage(dto.ImageBase64);
                }
            }
            else
            {
                imageUrl = null;
            }

            var finalDate = dto.ScheduledDate ?? DateOnly.FromDateTime(DateTime.UtcNow.AddHours(3));
            advisory.Update(dto.Title, dto.Content, dto.Target, finalDate, imageUrl);

            await _unitOfWork.HealthAdvisoryRepository.UpdateAsync(advisory, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            // Trigger immediate dispatch
            var tokens = GetDeviceTokensForTarget(advisory.Target);
            if (tokens.Any())
            {
                var baseUrl = _configuration["ApiBaseUrl"]?.TrimEnd('/');
                var fullImageUrl = !string.IsNullOrEmpty(advisory.ImageUrl) && !string.IsNullOrEmpty(baseUrl)
                    ? $"{baseUrl}{advisory.ImageUrl}"
                    : null;

                var data = new Dictionary<string, string>
                {
                    { "type", "advisory" },
                    { "id", advisory.Id.ToString() }
                };

                var success = await _pushNotificationService.SendMulticastNotificationAsync(
                    tokens,
                    advisory.Title,
                    advisory.Content,
                    fullImageUrl,
                    data,
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

        private async Task<string> SaveImage(string base64Image)
        {
            try
            {
                // Standard folder for uploads
                var uploadPath = Path.Combine("wwwroot", "uploads", "advisories");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                // Clean the base64 string if it contains data:image/...;base64,
                var base64Data = base64Image;
                var extension = ".jpg";

                if (base64Image.Contains(","))
                {
                    var parts = base64Image.Split(',');
                    base64Data = parts[1];

                    // Try to extract extension
                    var meta = parts[0];
                    if (meta.Contains("image/png")) extension = ".png";
                    else if (meta.Contains("image/gif")) extension = ".gif";
                    else if (meta.Contains("image/webp")) extension = ".webp";
                }

                var fileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadPath, fileName);

                var bytes = Convert.FromBase64String(base64Data);
                await File.WriteAllBytesAsync(filePath, bytes);

                return $"/uploads/advisories/{fileName}";
            }
            catch (Exception ex)
            {
                // Log and return null or handle appropriately. 
                // For now, we'll just return null so the advisory is created without an image if saving fails.
                return null;
            }
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
