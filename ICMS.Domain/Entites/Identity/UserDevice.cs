using ICMS.Domain.Entites.Common;
using ICMS.Domain.Exceptions;

namespace ICMS.Domain.Entites.Identity
{
    public class UserDevice : BaseEntity<int>
    {
        public int UserId { get; private set; }
        public string FcmToken { get; private set; } = string.Empty;
        public DateTime LastActiveAt { get; private set; }
        
        public User? User { get; private set; }

        private UserDevice() { }

        public static UserDevice Create(int userId, string fcmToken)
        {
            if (userId <= 0) throw new DomainException("Invalid user id");
            if (string.IsNullOrWhiteSpace(fcmToken)) throw new DomainException("FCM token is required");

            return new UserDevice
            {
                UserId = userId,
                FcmToken = fcmToken,
                LastActiveAt = DateTime.UtcNow
            };
        }

        public void UpdateLastActive()
        {
            LastActiveAt = DateTime.UtcNow;
        }
    }
}
