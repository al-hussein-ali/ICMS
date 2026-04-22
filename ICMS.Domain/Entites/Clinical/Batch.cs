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

namespace ICMS.Domain.Entites.Clinical
{
    public class Batch : BaseEntity<int>
    {
        public IReadOnlyList<Transaction> Transactions => _transactions.AsReadOnly();
        private readonly List<Transaction> _transactions = new();
        public int DoseId { get; private set; }
        public int UserId { get; private set; }
        public string BatchName { get; private set; } = string.Empty;
        public string CountryOfOrigin { get; private set; } = string.Empty;
        public string CookNumber { get; private set; } = string.Empty;
        public DateOnly ExpiryDate { get; private set; }
        public DateOnly CreationDate { get; private set; }
        public int TotalQuantity { get; private set; }
        public string? Notes { get; private set; }
        public bool IsActive { get; private set; } = true;

        public Dose? Dose { get; private set; }
        public User? User { get; private set; }

        private Batch()
        {
        }

        public static Batch Create(int doseId, int userId, string batchName, DateOnly creationDate, DateOnly expiryDate, int totalQuantity, string countryOfOrigin, string cookNumber, string? notes = null)
        {
            if (doseId <= 0) throw new DomainException("Invalid dose id");
            if (userId <= 0) throw new DomainException("Invalid user id");
            if (string.IsNullOrWhiteSpace(batchName)) throw new DomainException("Batch name is required");
            if (totalQuantity < 0) throw new DomainException("Total quantity cannot be negative");
            if (string.IsNullOrWhiteSpace(countryOfOrigin)) throw new DomainException("Country of origin is required");
            if (string.IsNullOrWhiteSpace(cookNumber)) throw new DomainException("Cook number is required");

            return new Batch
            {
                DoseId = doseId,
                UserId = userId,
                BatchName = batchName,
                CreationDate = creationDate,
                ExpiryDate = expiryDate,
                TotalQuantity = totalQuantity,
                CountryOfOrigin = countryOfOrigin,
                CookNumber = cookNumber,
                Notes = notes
            };
        }

        public void UpdateBatchInfo(string batchName, string countryOfOrigin, string cookNumber, DateOnly expiryDate, string? notes)
        {
            if (string.IsNullOrWhiteSpace(batchName)) throw new DomainException("Batch name is required");
            if (string.IsNullOrWhiteSpace(countryOfOrigin)) throw new DomainException("Country of origin is required");
            if (string.IsNullOrWhiteSpace(cookNumber)) throw new DomainException("Cook number is required");

            BatchName = batchName;
            CountryOfOrigin = countryOfOrigin;
            CookNumber = cookNumber;
            ExpiryDate = expiryDate;
            Notes = notes;
        }

        public void AddInventory(int quantity, string permissionNumber, string source, int userId, DateTime? date = null)
        {
            if (quantity <= 0) throw new DomainException("Added quantity must be positive");
            
            var transaction = Transaction.Create(
                this.Id, 
                TransactionType.In, 
                date ?? DateTime.UtcNow, 
                quantity, 
                permissionNumber, 
                source, 
                userId);

            _transactions.Add(transaction);
            TotalQuantity += quantity;
        }

        public void RemoveInventory(int quantity, string permissionNumber, string destination, int userId, DateTime? date = null)
        {
            if (quantity <= 0) throw new DomainException("Removed quantity must be positive");
            if (TotalQuantity < quantity) throw new DomainException("Insufficient inventory in batch");

            var transaction = Transaction.Create(
                this.Id, 
                TransactionType.Out, 
                date ?? DateTime.UtcNow, 
                quantity, 
                permissionNumber, 
                destination, 
                userId);

            _transactions.Add(transaction);
            TotalQuantity -= quantity;
        }

        public void AssignDose(Dose dose)
        {
            if (dose == null) throw new DomainException("Dose is required");
            if (Dose != null) throw new DomainException("Dose already assigned");
            if (dose.Id != 0 && dose.Id != DoseId) throw new DomainException("Dose id mismatch");

            Dose = dose;
            DoseId = dose.Id;
        }

        public void AssignUser(User user)
        {
            if (user == null) throw new DomainException("User is required");
            if (User != null) throw new DomainException("User already assigned");
            if (user.Id != 0 && user.Id != UserId) throw new DomainException("User id mismatch");

            User = user;
            UserId = user.Id;
        }

        public void AddTransaction(Transaction transaction)
        {
            if (transaction == null) throw new DomainException("Transaction is required");
            if (_transactions.Any(t => t.Id == transaction.Id)) throw new DomainException("Transaction already added");

            _transactions.Add(transaction);
        }

        public void Deactivate()
        {
            if (!IsActive) return;
            IsActive = false;
        }
    }
}
