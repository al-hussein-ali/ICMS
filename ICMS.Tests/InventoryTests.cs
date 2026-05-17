using FluentAssertions;
using ICMS.Application.DTOs.Batch;
using ICMS.Application.DTOs.Pagination;
using ICMS.Application.DTOs.Transaction;
using ICMS.Application.Interfaces;
using ICMS.Application.Interfaces.Services;
using ICMS.Application.Services;
using ICMS.Application.Extensions;
using ICMS.Domain.Entites.Clinical;
using ICMS.Domain.Entites.Identity;
using ICMS.Domain.Enums;
using ICMS.Application.DTOs.DoseReport;
using ICMS.Application.Interfaces.Repositories;
using ICMS.Infrastructure.Persistence.Data;
using ICMS.Infrastructure.Repositories;
using ICMS.Infrastructure.Repositories.Clinical;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ICMS.Tests
{
    public class InventoryTests
    {
        private readonly AppDbContext _context;
        private readonly BatchService _batchService;
        private readonly DoseReportService _reportService;

        public InventoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);

            // Seed Person and User to satisfy the global query filter !b.User.Person.IsDeleted
            var person = Person.Create("Test", "User", "One", "Admin", Gender.Male, new DateOnly(1990, 1, 1), "1234567890");
            _context.People.Add(person);
            _context.SaveChanges();

            var user = User.Create("admin", "hashed_password", person.Id, true);
            user.AssignPerson(person);
            _context.Users.Add(user);
            _context.SaveChanges();

            // Seed Vaccine and Dose for DoseId = 1
            var vaccine = Vaccine.Create("BCG", "BCG-01", "Tuberculosis vaccine", true, 5, 0, 12, TargetAudience.InfantRoutine);
            _context.Vaccines.Add(vaccine);
            _context.SaveChanges();

            var dose = Dose.Create(vaccine.Id, "BCG Dose 1", 1, 0, "At Birth", true, "First dose");
            _context.Doses.Add(dose);
            _context.SaveChanges();

            var unitOfWork = new UnitOfWork(_context);
            _batchService = new BatchService(unitOfWork, new FakeCacheService());
            _reportService = new DoseReportService(unitOfWork);
        }

        private class FakeCacheService : ICMS.Application.Interfaces.Services.ICacheService
        {
            public T? Get<T>(string key) => default;
            public void Set<T>(string key, T value, TimeSpan? duration = null) { }
            public void Remove(string key) { }
            public bool TryGet<T>(string key, out T? value)
            {
                value = default;
                return false;
            }
        }


        [Fact]
        public async Task RemoveStockByDoseAsync_ShouldSubtractFromMultipleBatches_WhenQuantityExceedsOneBatch()
        {
            // Arrange
            int doseId = 1;
            int userId = 1;

            var batch1 = Batch.Create(doseId, userId, "Batch1", DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now.AddMonths(1)), 10, "Country",
                "Cook1");
            var batch2 = Batch.Create(doseId, userId, "Batch2", DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now.AddMonths(2)), 20, "Country",
                "Cook2");

            _context.Batches.AddRange(batch1, batch2);
            await _context.SaveChangesAsync();

            var dto = new InventoryRemoveByDoseDto(doseId, 15, "Perm123", "Dest1");

            // Act
            await _batchService.RemoveStockByDoseAsync(dto, userId);

            // Assert
            batch1.TotalQuantity.Should().Be(0);
            batch2.TotalQuantity.Should().Be(15);

            _context.Transactions.Count().Should().Be(2);
            var transactions = _context.Transactions.ToList();
            transactions.Should().Contain(t =>
                t.BatchId == batch1.Id && t.Quantity == 10 && t.TransactionType == TransactionType.Out);
            transactions.Should().Contain(t =>
                t.BatchId == batch2.Id && t.Quantity == 5 && t.TransactionType == TransactionType.Out);
        }

        [Fact]
        public async Task GetTransactionsAsync_ShouldFilterByType()
        {
            // Arrange
            int doseId = 1;
            int userId = 1;
            var batch = Batch.Create(doseId, userId, "Batch1", DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now.AddMonths(1)), 100, "Country",
                "Cook1");
            _context.Batches.Add(batch);
            await _context.SaveChangesAsync();

            batch.AddInventory(50, "P1", "Source", userId); // Transaction 1 (In)
            batch.RemoveInventory(20, "P2", "Dest", userId); // Transaction 2 (Out)
            
            var user = await _context.Users.Include(u => u.Person).FirstOrDefaultAsync(u => u.Id == userId);
            batch.GetType().GetProperty("User")?.SetValue(batch, user);
            foreach (var t in batch.Transactions)
            {
                t.GetType().GetProperty("User")?.SetValue(t, user);
            }

            await _context.SaveChangesAsync();

            var filterIn = new TransactionFilterDto(TransactionType.In);
            var pagination = new PaginationParams { PageNumber = 1, PageSize = 10 };

            // Act
            var result = await _batchService.GetTransactionsAsync(batch.Id, filterIn, pagination);

            // Assert
            result.Items.Should().HaveCount(1);
            result.Items.First().TransactionType.Should().Be(TransactionType.In);
            result.Items.First().Quantity.Should().Be(50);
        }

        [Fact]
        public async Task AddDoseReport_ShouldPersistReport()
        {
            // Arrange
            int doseId = 1;
            int userId = 1;
            var batch = Batch.Create(doseId, userId, "Batch1", DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now.AddMonths(1)), 100, "Country",
                "Cook1");
            _context.Batches.Add(batch);
            await _context.SaveChangesAsync();

            var dto = new DoseReportCreateDto(batch.Id, "Side effects reported");

            // Act
            var result = await _reportService.AddAsync(dto, userId);

            // Assert
            result.BatchId.Should().Be(batch.Id);
            result.Description.Should().Be("Side effects reported");

            _context.DoseReports.Count().Should().Be(1);
            _context.DoseReports.First().Description.Should().Be("Side effects reported");

            // Verify stock was zeroed out
            batch.TotalQuantity.Should().Be(0);
            _context.Transactions.Should().Contain(t =>
                t.BatchId == batch.Id && t.TransactionType == TransactionType.Out && t.PermissionNumber == "REPORTED");
        }

        [Fact]
        public void ToReadDto_ShouldReturnZeroRemainingQuantity_WhenBatchIsExpired()
        {
            // Arrange
            var doseId = 1;
            var userId = 1;
            var expiredDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));
            var batch = Batch.Create(doseId, userId, "ExpiredBatch", DateOnly.FromDateTime(DateTime.Today.AddDays(-10)), expiredDate, 50, "Jordan", "CN-EXP");

            // Act
            var dto = batch.ToReadDto();

            // Assert
            dto.RemainingQuantity.Should().Be(0);
            dto.Status.Should().Be("expired");
            dto.TotalQuantity.Should().Be(50); // Original Total should remain
            dto.ConsumedQuantity.Should().Be(0); // Consumed should not be incremented
        }

        [Fact]
        public async Task RemoveStockByDoseAsync_ShouldExcludeExpiredBatches()
        {
            // Arrange
            int doseId = 2;
            int userId = 1;

            // Batch 1 is expired (50 available, but expired yesterday)
            var expiredBatch = Batch.Create(doseId, userId, "ExpiredBatch", DateOnly.FromDateTime(DateTime.Today.AddDays(-10)), DateOnly.FromDateTime(DateTime.Today.AddDays(-1)), 50, "Country", "CN1");
            // Batch 2 is active (20 available, expiring in 1 month)
            var activeBatch = Batch.Create(doseId, userId, "ActiveBatch", DateOnly.FromDateTime(DateTime.Today), DateOnly.FromDateTime(DateTime.Today.AddMonths(1)), 20, "Country", "CN2");

            _context.Batches.AddRange(expiredBatch, activeBatch);
            await _context.SaveChangesAsync();

            var dto = new InventoryRemoveByDoseDto(doseId, 10, "Perm123", "Dest1");

            // Act
            await _batchService.RemoveStockByDoseAsync(dto, userId);

            // Assert
            expiredBatch.TotalQuantity.Should().Be(50); // Expired batch was not touched
            activeBatch.TotalQuantity.Should().Be(10); // 10 was subtracted from the active batch
        }
    }
}
