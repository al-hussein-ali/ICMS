using ICMS.Application.DTOs.Reports;
using ICMS.Application.Enums;
using ICMS.Application.Interfaces;
using ICMS.Application.Interfaces.Reports;
using Microsoft.EntityFrameworkCore;

namespace ICMS.Infrastructure.Reports.DataFetchers
{
    public class PregnantWomenReportFetcher(IUnitOfWork unitOfWork) : IReportDataFetcher
    {
        public ReportType ReportType => ReportType.PregnantWomen;

        public async Task<ReportData> FetchAsync(DateOnly startDate, DateOnly endDate, CancellationToken ct = default)
        {
            var queryable = unitOfWork.PregnantWomanRepository
                .GetQueryable(false, ct, pw => pw.Person, pw => pw.PregnancyDetails);

            if (queryable == null)
                throw new InvalidOperationException("Failed to get queryable.");

            var women = await queryable
                .Where(pw => pw.Person != null
                          && DateOnly.FromDateTime(pw.Person.CreatedAt) >= startDate
                          && DateOnly.FromDateTime(pw.Person.CreatedAt) <= endDate)
                .ToListAsync(ct);

            var bloodGroupSummary = women
                .GroupBy(pw => pw.BloodGroup.ToString())
                .ToDictionary(g => $"Blood Group {g.Key}", g => g.Count().ToString());

            var rows = women.Select(pw => new ReportRow
            {
                Columns = new Dictionary<string, string?>
                {
                    ["Full Name"]         = pw.Person?.FullName ?? "-",
                    ["Age Range"]         = pw.AgeRange,
                    ["Blood Group"]       = pw.BloodGroup.ToString(),
                    ["Rh Factor"]         = pw.RhFactor.ToString(),
                    ["Pregnancy Count"]   = pw.PregnancyCount.ToString(),
                    ["Active Pregnancy"]  = pw.PregnancyDetails.Any(p => !p.IsPregnancyDone) ? "Yes" : "No",
                    ["Registered On"]     = pw.Person?.CreatedAt.ToString("yyyy-MM-dd") ?? "-",
                }
            }).ToList();

            return new ReportData
            {
                ReportType    = ReportType,
                StartDate     = startDate,
                EndDate       = endDate,
                GeneratedAt   = DateTime.UtcNow.AddHours(3).ToString("yyyy-MM-dd HH:mm"),
                TotalRecords  = rows.Count,
                SummaryStats  = bloodGroupSummary,
                ColumnHeaders = ["Full Name", "Age Range", "Blood Group", "Rh Factor", "Pregnancy Count", "Active Pregnancy", "Registered On"],
                Rows          = rows
            };
        }
    }
}
