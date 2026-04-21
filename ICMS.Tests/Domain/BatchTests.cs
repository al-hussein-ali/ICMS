using ICMS.Domain.Entites.Clinical;
using ICMS.Domain.Entites.Audit;
using ICMS.Domain.Enums;
using ICMS.Domain.Exceptions;
using Xunit;

namespace ICMS.Tests.Domain
{
    public class BatchTests
    {
        [Fact]
        public void Create_WithValidData_ReturnsBatch()
        {
            // Arrange
            var doseId = 1;
            var userId = 10;
            var expiryDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(1));
            var totalQuantity = 0;
            var country = "Jordan";
            var cookNumber = "B123-X";

            // Act
            var batch = Batch.Create(doseId, userId, "Batch Name", DateOnly.FromDateTime(DateTime.UtcNow), expiryDate, totalQuantity, country, cookNumber);

            // Assert
            Assert.Equal(doseId, batch.DoseId);
            Assert.Equal(country, batch.CountryOfOrigin);
            Assert.Equal(cookNumber, batch.CookNumber);
            Assert.Equal(0, batch.TotalQuantity);
        }

        [Fact]
        public void AddInventory_IncreasesQuantityAndCreatesTransaction()
        {
            // Arrange
            var batch = Batch.Create(1, 1, "Batch Name", DateOnly.FromDateTime(DateTime.UtcNow), DateOnly.FromDateTime(DateTime.UtcNow), 100, "Jordan", "CN-1");
            batch.Id = 1; // Simulate existing batch
            var addAmount = 50;

            // Act
            batch.AddInventory(addAmount, "PERM-001", "Central Store", 5);

            // Assert
            Assert.Equal(150, batch.TotalQuantity);
            Assert.Single(batch.Transactions);
            var tx = batch.Transactions[0];
            Assert.Equal(TransactionType.In, tx.TransactionType);
            Assert.Equal(addAmount, tx.Quantity);
        }

        [Fact]
        public void RemoveInventory_DecreasesQuantityAndCreatesTransaction()
        {
            // Arrange
            var batch = Batch.Create(1, 1, "Batch Name", DateOnly.FromDateTime(DateTime.UtcNow), DateOnly.FromDateTime(DateTime.UtcNow), 100, "Jordan", "CN-1");
            batch.Id = 1; // Simulate existing batch
            var removeAmount = 30;

            // Act
            batch.RemoveInventory(removeAmount, "PERM-OUT", "Clinic A", 5);

            // Assert
            Assert.Equal(70, batch.TotalQuantity);
            Assert.Single(batch.Transactions);
            var tx = batch.Transactions[0];
            Assert.Equal(TransactionType.Out, tx.TransactionType);
            Assert.Equal(removeAmount, tx.Quantity);
        }

        [Fact]
        public void RemoveInventory_InsufficientQuantity_ThrowsDomainException()
        {
            // Arrange
            var batch = Batch.Create(1, 1, "Batch Name", DateOnly.FromDateTime(DateTime.UtcNow), DateOnly.FromDateTime(DateTime.UtcNow), 20, "Jordan", "CN-1");

            // Act & Assert
            Assert.Throws<DomainException>(() => batch.RemoveInventory(30, "P", "D", 1));
        }
    }
}
