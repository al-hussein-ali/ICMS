using ICMS.Domain.Entites.Identity;
using System;

namespace ICMS.Domain.Entites.Common
{
    public class Notification : BaseEntity<Guid>
    {
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? TargetUrl { get; set; }
        public string? JobId { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int UserId { get; set; }
        public virtual User? User { get; set; }
    }
}
