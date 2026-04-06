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
    public class Batch : BaseEntity<int>
    {
        public IReadOnlyList<Transaction> Transactions => _transactions.AsReadOnly();
        private readonly List<Transaction> _transactions = new();
        public int DoseId { get; private set; }
        public int UserId { get; private set; }
        public string CountryOfOrigin { get; private set; } = string.Empty;
        public string? CookNumber { get; private set; }
        public DateOnly ExpiryDate { get; private set; }
        public int TotalQuantity { get; private set; }
        public string? Notes { get; private set; }

        public Dose? Dose { get; private set; }
        public User? User { get; private set; }

        private Batch()
        {
        }

        public static Batch Create(int doseId, int userId, DateOnly expiryDate, int totalQuantity, string countryOfOrigin, string? cookNumber = null, string? notes = null)
        {
            if (doseId <= 0) throw new DomainException("Invalid dose id");
            if (userId <= 0) throw new DomainException("Invalid user id");
            if (totalQuantity < 0) throw new DomainException("Total quantity cannot be negative");
            if (string.IsNullOrWhiteSpace(countryOfOrigin)) throw new DomainException("Country of origin is required");

            return new Batch
            {
                DoseId = doseId,
                UserId = userId,
                ExpiryDate = expiryDate,
                TotalQuantity = totalQuantity,
                CountryOfOrigin = countryOfOrigin,
                CookNumber = cookNumber,
                Notes = notes
            };
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
    }
}
