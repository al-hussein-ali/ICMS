using ICMS.Domain.Exceptions;
using ICMS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMS.Domain.Entites
{
    public class Transaction : BaseEntity<Guid>
    {
        public TransactionType TransactionType { get; private set; }
        public DateTime TransactionDate { get; private set; }
        public string PermissionNumber { get; private set; } = string.Empty;
        public string SourceorDestination { get; private set; } = string.Empty;
        public int Quantity { get; private set; }
        public int BatchId { get; private set; }
        public Batch? Batch { get; private set; }

        private Transaction()
        {
        }

        public static Transaction Create(int batchId, TransactionType transactionType, DateTime transactionDate, int quantity, string permissionNumber, string sourceOrDestination)
        {
            if (batchId <= 0) throw new DomainException("Invalid batch id");
            if (quantity <= 0) throw new DomainException("Quantity must be greater than zero");
            if (string.IsNullOrWhiteSpace(permissionNumber)) throw new DomainException("Permission number is required");

            return new Transaction { BatchId = batchId, TransactionType = transactionType, TransactionDate = transactionDate, Quantity = quantity, PermissionNumber = permissionNumber, SourceorDestination = sourceOrDestination };
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
