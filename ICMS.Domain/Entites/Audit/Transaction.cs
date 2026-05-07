using ICMS.Domain.Entites.Geography;
using ICMS.Domain.Entites.Audit;
using ICMS.Domain.Entites.Visits;
using ICMS.Domain.Entites.Maternal;
using ICMS.Domain.Entites.Clinical;
using ICMS.Domain.Entites.Identity;
using ICMS.Domain.Entites.Common;
using ICMS.Domain.Exceptions;
using ICMS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMS.Domain.Entites.Audit
{
    public class Transaction : BaseEntity<Guid>
    {
        public TransactionType TransactionType { get; private set; }
        public DateTime TransactionDate { get; private set; }
        public string PermissionNumber { get; private set; } = string.Empty;
        public string SourceOrDestination { get; private set; } = string.Empty;
        public int Quantity { get; private set; }
        public string? Notes { get; private set; }
        public int BatchId { get; private set; }
        public Batch? Batch { get; private set; }

        public int UserId { get; private set; }
        public User? User { get; private set; }

        private Transaction()
        {
        }

        public static Transaction Create(int batchId, TransactionType transactionType, DateTime transactionDate, int quantity, string permissionNumber, string sourceOrDestination, int userId, string? notes = null)
        {
            if (batchId <= 0) throw new DomainException("Invalid batch id");
            if (userId <= 0) throw new DomainException("Invalid user id");
            if (quantity <= 0) throw new DomainException("Quantity must be greater than zero");
            if (string.IsNullOrWhiteSpace(permissionNumber)) throw new DomainException("Permission number is required");

            return new Transaction 
            { 
                BatchId = batchId, 
                TransactionType = transactionType, 
                TransactionDate = transactionDate, 
                Quantity = quantity, 
                PermissionNumber = permissionNumber, 
                SourceOrDestination = sourceOrDestination, 
                UserId = userId,
                Notes = notes
            };
        }

        public void AssignBatch(Batch batch)
        {
            if (batch == null) throw new DomainException("Batch is required");
            if (Batch != null) throw new DomainException("Batch already assigned");
            if (batch.Id != 0 && batch.Id != BatchId) throw new DomainException("Batch id mismatch");

            Batch = batch;
            BatchId = batch.Id;
        }
    }
}
