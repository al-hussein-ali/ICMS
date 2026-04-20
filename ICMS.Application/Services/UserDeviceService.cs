using ICMS.Application.Interfaces;
using ICMS.Application.Interfaces.Services;
using ICMS.Domain.Entites.Identity;
using ICMS.Domain.Exceptions;
using System.Threading;
using System.Threading.Tasks;

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
                throw new DomainException("User not found.");
            }

            user.AddDevice(fcmToken);
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
