using ICMS.Application.Interfaces;
using ICMS.Application.Interfaces.Services;
using ICMS.Domain.Entites.Identity;
using ICMS.Domain.Exceptions;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ICMS.Application.Services
{
    public class UserDeviceService : IUserDeviceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserDeviceService> _logger;

        public UserDeviceService(IUnitOfWork unitOfWork, IConfiguration configuration, ILogger<UserDeviceService> logger)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _logger = logger;
        }

        private bool IsValidFcmToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return false;
            
            // Real FCM tokens are long base64-like strings (typically > 100 characters)
            if (token.Length < 100) return false;

            // Reject typical test/mock patterns
            var lowerToken = token.ToLowerInvariant();
            if (lowerToken.Contains("mock") || 
                lowerToken.Contains("dummy") || 
                lowerToken.Contains("test") || 
                lowerToken.Contains("fake") || 
                lowerToken.Contains("temp"))
            {
                return false;
            }

            return true;
        }

        public async Task RegisterDeviceAsync(int userId, string fcmToken, CancellationToken ct = default)
        {
            var isMock = bool.TryParse(_configuration["Firebase:Mock"], out var mock) && mock;
            if (!isMock && !IsValidFcmToken(fcmToken))
            {
                _logger.LogWarning("Skipped registering invalid/mock FCM token (Length: {Length}) in real sending mode for User ID {UserId}.", fcmToken?.Length ?? 0, userId);
                return;
            }

            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId, ct);
            if (user == null)
            {
                throw new DomainException("UserNotFound");
            }

            // Check if this token is already registered to ANY user (unique constraint)
            var existingDevice = await _unitOfWork.UserDeviceRepository.GetQueryable()
                .FirstOrDefaultAsync(d => d.FcmToken == fcmToken, ct);

            if (existingDevice != null)
            {
                if (existingDevice.UserId == userId)
                {
                    // Same user, just update activity
                    existingDevice.UpdateLastActive();
                    await _unitOfWork.UserDeviceRepository.UpdateAsync(existingDevice, ct);
                }
                else
                {
                    // Token belongs to someone else (maybe a shared device or old login)
                    // We need to reassign it to the current user
                    await _unitOfWork.UserDeviceRepository.DeleteAsync(existingDevice, ct);
                    await _unitOfWork.SaveChangesAsync(ct); // Flush deletion first
                    
                    user.AddDevice(fcmToken);
                }
            }
            else
            {
                // New token entirely
                user.AddDevice(fcmToken);
            }

            await _unitOfWork.SaveChangesAsync(ct);
        }

        public async Task UnregisterDeviceAsync(int userId, string fcmToken, CancellationToken ct = default)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId, ct);
            if (user == null)
            {
                return;
            }

            user.RemoveDevice(fcmToken);
            await _unitOfWork.SaveChangesAsync(ct);
        }
    }
}
