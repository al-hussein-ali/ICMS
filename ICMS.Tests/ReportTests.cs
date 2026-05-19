using FluentAssertions;
using ICMS.Application.DTOs.Reports;
using ICMS.Application.Enums;
using ICMS.Domain.Entites.Audit;
using ICMS.Domain.Entites.Clinical;
using ICMS.Domain.Entites.Identity;
using ICMS.Domain.Enums;
using ICMS.Infrastructure.Persistence.Data;
using ICMS.Infrastructure.Repositories;
using ICMS.Infrastructure.Reports.DataFetchers;
using ICMS.Infrastructure.Reports.Templates;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ICMS.Tests
{
    public class ReportTests
    {
        private readonly AppDbContext _context;
        private readonly UnitOfWork _unitOfWork;

        public ReportTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _unitOfWork = new UnitOfWork(_context);
        }

        [Fact]
        public async Task DailyVaccinationReportFetcher_ShouldFetchBothPrimaryAndSecondaryData()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var startDateTime = DateTime.SpecifyKind(today.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);

            // Arrange: Seed some vaccinated individuals/immunization records
            var person = Person.Create("John", "Doe", "M", "Admin", Gender.Male, new DateOnly(2025, 1, 1), "0987654321");
            _context.People.Add(person);
            _context.SaveChanges();

            // Seed User to satisfy global query filters
            var user = User.Create("admin", "hashed_password", person.Id, true);
            user.AssignPerson(person);
            _context.Users.Add(user);
            _context.SaveChanges();

            var vi = VaccinatedIndividual.Create(1, 1, null, null);
            vi.AssignPerson(person);
            vi.GetType().GetProperty("CardNumber")?.SetValue(vi, "CARD-100");
            _context.VaccinatedIndividuals.Add(vi);
            _context.SaveChanges();

            var vaccine = Vaccine.Create("Hepatitis B", "HEP-B", "Hep B vaccine", true, 3, 0, 12, TargetAudience.InfantRoutine);
            _context.Vaccines.Add(vaccine);
            _context.SaveChanges();

            var dose = Dose.Create(vaccine.Id, "Dose 1", 1, 0, "At birth", true, "First dose");
            _context.Doses.Add(dose);
            _context.SaveChanges();

            // Seed some Out transactions (subtracted inventory)
            var batch = Batch.Create(dose.Id, user.Id, "Batch-HEP-12", DateOnly.FromDateTime(DateTime.Today), DateOnly.FromDateTime(DateTime.Today.AddMonths(6)), 100, "USA", "COOK-B12");
            _context.Batches.Add(batch);
            _context.SaveChanges();

            var record = ImmunizationRecord.Create(vi.Id, dose.Id, DateOnly.FromDateTime(DateTime.Today), "Health Center", user.Id, null, "admin", batch.Id);
            _context.ImmunizationRecords.Add(record);
            _context.SaveChanges();

            // Add Out transaction (subtracted) - ensuring the datetime is strictly in range
            var tx = Transaction.Create(batch.Id, TransactionType.Out, startDateTime.AddHours(12), 15, "PERM-OUT-1", "Clinic A", user.Id, "Administered dose");
            _context.Transactions.Add(tx);
            _context.SaveChanges();

            var fetcher = new DailyVaccinationReportFetcher(_unitOfWork);

            // Act
            var reportData = await fetcher.FetchAsync(today, today, "en");

            // Assert
            reportData.Should().NotBeNull();
            reportData.TotalRecords.Should().Be(1);
            reportData.Rows.Should().HaveCount(1);
            reportData.Rows.First().Columns["Beneficiary"].Should().Be("John Doe M Admin");

            // Verify secondary inventory reduction transactions
            reportData.SecondaryRows.Should().NotBeNull();
            reportData.SecondaryRows.Should().HaveCount(1);
            reportData.SecondaryRows.First().Columns["Batch Name"].Should().Be("Batch-HEP-12");
            reportData.SecondaryRows.First().Columns["Subtracted Qty"].Should().Be("15");
            reportData.SecondaryRows.First().Columns["Permission No."].Should().Be("PERM-OUT-1");
        }

        [Fact]
        public void DailyVaccinationReportTemplate_ShouldRenderThreeSectionsInOrder()
        {
            // Arrange
            var reportData = new ReportData
            {
                ReportType = ReportType.DailyVaccination,
                StartDate = DateOnly.FromDateTime(DateTime.Today),
                EndDate = DateOnly.FromDateTime(DateTime.Today),
                Lang = "en",
                GeneratedAt = "2026-05-19 12:00",
                TotalRecords = 1,
                ColumnHeaders = new List<string> { "Beneficiary", "Vaccine" },
                Rows = new List<ReportRow>
                {
                    new ReportRow
                    {
                        Columns = new Dictionary<string, string?>
                        {
                            ["Beneficiary"] = "Alice Smith",
                            ["Vaccine"] = "Polio"
                        }
                    }
                },
                SecondaryTableTitle = "Dose Batches Subtracted from Inventory",
                SecondaryColumnHeaders = new List<string> { "Batch Name", "Subtracted Qty" },
                SecondaryRows = new List<ReportRow>
                {
                    new ReportRow
                    {
                        Columns = new Dictionary<string, string?>
                        {
                            ["Batch Name"] = "BATCH-P-1",
                            ["Subtracted Qty"] = "5"
                        }
                    }
                },
                SummaryStats = new Dictionary<string, string>
                {
                    ["Total Doses"] = "1"
                }
            };

            var template = new DailyVaccinationReportTemplate();

            // Act
            var html = template.Render(reportData);

            // Assert
            html.Should().NotBeNullOrEmpty();
            html.Should().Contain("Vaccinated Beneficiary Records");
            html.Should().Contain("Alice Smith");
            html.Should().Contain("Dose Batches Subtracted from Inventory");
            html.Should().Contain("BATCH-P-1");
            html.Should().Contain("Report Summary");
            html.Should().Contain("Total Doses");

            // Verify order of sections
            int idxPrimary = html.IndexOf("Vaccinated Beneficiary Records");
            int idxSecondary = html.IndexOf("Dose Batches Subtracted from Inventory");
            int idxSummary = html.IndexOf("Report Summary");

            idxPrimary.Should().BeLessThan(idxSecondary);
            idxSecondary.Should().BeLessThan(idxSummary);
        }
    }
}
