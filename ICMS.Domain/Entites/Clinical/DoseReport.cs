using ICMS.Domain.Entites.Geography;
using ICMS.Domain.Entites.Audit;
using ICMS.Domain.Entites.Visits;
using ICMS.Domain.Entites.Maternal;
using ICMS.Domain.Entites.Clinical;
using ICMS.Domain.Entites.Identity;
using ICMS.Domain.Entites.Common;
using ICMS.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMS.Domain.Entites.Clinical
{
    public class DoseReport : BaseEntity<int>
    {
        public int BatchId { get; private set; }
        public int UserId { get; private set; }
        public string? Description { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public Batch? Batch { get; private set; }
        public User? User { get; private set; }

        private DoseReport()
        {
        }

        public static DoseReport Create(int batchId, int userId, string? description = null)
        {
            if (batchId <= 0) throw new DomainException("Invalid batch id");
            if (userId <= 0) throw new DomainException("Invalid user id");

            return new DoseReport { BatchId = batchId, UserId = userId, Description = description, CreatedAt = DateTime.UtcNow };
        }

        public void AssignBatch(Batch batch)
        {
            if (batch == null) throw new DomainException("Batch is required");
            if (Batch != null) throw new DomainException("Batch already assigned");
            if (batch.Id != 0 && batch.Id != BatchId) throw new DomainException("Batch id mismatch");

            Batch = batch;
            BatchId = batch.Id;
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
