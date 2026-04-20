using FluentAssertions;
using ICMS.Application.DTOs.Batch;
using ICMS.Application.DTOs.Pagination;
using ICMS.Application.DTOs.Transaction;
using ICMS.Application.Interfaces;
using ICMS.Application.Interfaces.Services;
using ICMS.Application.Services;
using ICMS.Domain.Entites.Clinical;
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

            var batch1 = Batch.Create(doseId, userId, DateOnly.FromDateTime(DateTime.Now.AddMonths(1)), 10, "Country",
                "Cook1");
            var batch2 = Batch.Create(doseId, userId, DateOnly.FromDateTime(DateTime.Now.AddMonths(2)), 20, "Country",
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
            var batch = Batch.Create(doseId, userId, DateOnly.FromDateTime(DateTime.Now.AddMonths(1)), 100, "Country",
                "Cook1");
            _context.Batches.Add(batch);
            await _context.SaveChangesAsync();

            batch.AddInventory(50, "P1", "Source", userId); // Transaction 1 (In)
            batch.RemoveInventory(20, "P2", "Dest", userId); // Transaction 2 (Out)
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
            var batch = Batch.Create(doseId, userId, DateOnly.FromDateTime(DateTime.Now.AddMonths(1)), 100, "Country",
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
    }
}
