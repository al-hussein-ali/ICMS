using ICMS.Domain.Exceptions;
using ICMS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMS.Domain.Entites
{
    public class HealthAdvisory : BaseEntity<int>
    {
        public string Title { get; private set; } = string.Empty;
        public string Content { get; private set; } = string.Empty;
        public AdviceTarget Target { get; private set; }
        public DateTime CreationDate { get; private set; }
        public int UserId { get; private set; }
        public User? User { get; private set; }

        private HealthAdvisory()
        {   
        }

        public static HealthAdvisory Create(string title, string content, AdviceTarget target, int userId)
        {
            if (string.IsNullOrWhiteSpace(title)) throw new DomainException("Title is required");
            if (string.IsNullOrWhiteSpace(content)) throw new DomainException("Content is required");
            if (userId <= 0) throw new DomainException("Invalid user id");

            return new HealthAdvisory { Title = title, Content = content, Target = target, UserId = userId, CreationDate = DateTime.UtcNow };
        }

        public void AssignUser(User user)
        {
            if (user == null) throw new DomainException("User is required");
            if (User != null) throw new DomainException("User already assigned");
            if (user.Id != 0 && user.Id != UserId) throw new DomainException("User id mismatch");

            User = user;
            UserId = user.Id;
        }
    }
}
