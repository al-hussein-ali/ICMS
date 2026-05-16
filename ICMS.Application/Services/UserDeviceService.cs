using ICMS.Application.Interfaces;
using ICMS.Application.Interfaces.Services;
using ICMS.Domain.Entites.Identity;
using ICMS.Domain.Exceptions;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace ICMS.Application.Services
{
    public class UserDeviceService : IUserDeviceService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserDeviceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task RegisterDeviceAsync(int userId, string fcmToken, CancellationToken ct = default)
        {
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
