using ICMS.Domain.Entites.Identity;
using ICMS.Domain.Entites.Common;
using ICMS.Domain.Exceptions;
using ICMS.Domain.Enums;

namespace ICMS.Domain.Entites.Clinical
{
    public class HealthAdvisory : BaseEntity<int>
    {
        public string Title { get; private set; } = string.Empty;
        public string Content { get; private set; } = string.Empty;
        public AdviceTarget Target { get; private set; }
        public DateOnly ScheduledDate { get; private set; }
        public bool IsSent { get; private set; }
        public DateTime CreationDate { get; private set; }
        public int UserId { get; private set; }
        public User? User { get; private set; }

        private HealthAdvisory()
        {
        }

        public static HealthAdvisory Create(string title, string content, AdviceTarget target, DateOnly? scheduledDate, int userId)
        {
            if (string.IsNullOrWhiteSpace(title)) throw new DomainException("Title is required");
            if (string.IsNullOrWhiteSpace(content)) throw new DomainException("Content is required");
            if (userId <= 0) throw new DomainException("Invalid user id");

            var finalDate = scheduledDate ?? DateOnly.FromDateTime(DateTime.UtcNow.AddHours(3));

            return new HealthAdvisory
                { Title = title, Content = content, Target = target, ScheduledDate = finalDate, IsSent = false, UserId = userId, CreationDate = DateTime.UtcNow };
        }


        public void AssignUser(User user)
        {
            if (user == null) throw new DomainException("User is required");
            if (User != null) throw new DomainException("User already assigned");
            if (user.Id != 0 && user.Id != UserId) throw new DomainException("User id mismatch");

            User = user;
            UserId = user.Id;
        }

        public void MarkAsSent()
        {
            if (IsSent) return;
            IsSent = true;
        }
    }
}
