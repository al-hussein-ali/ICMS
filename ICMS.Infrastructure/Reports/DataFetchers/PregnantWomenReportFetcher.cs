using ICMS.Application.DTOs.Reports;
using ICMS.Application.Enums;
using ICMS.Application.Interfaces;
using ICMS.Application.Interfaces.Reports;
using ICMS.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ICMS.Infrastructure.Reports.DataFetchers
{
    public class PregnantWomenReportFetcher(IUnitOfWork unitOfWork) : IReportDataFetcher
    {
        public ReportType ReportType => ReportType.PregnantWomen;

        public async Task<ReportData> FetchAsync(DateOnly startDate, DateOnly endDate, string lang = "en", CancellationToken ct = default)
        {
            var isAr = lang.StartsWith("ar", StringComparison.OrdinalIgnoreCase);
            int currentYear = DateTime.UtcNow.AddHours(3).Year;

            var queryable = unitOfWork.PregnantWomanRepository
                .GetQueryable(false, ct, pw => pw.Person, pw => pw.PregnancyDetails);

            if (queryable == null)
                throw new InvalidOperationException("Failed to get queryable.");

            var women = await queryable
                .Include(pw => pw.PregnancyDetails)
                    .ThenInclude(pd => pd.Newborns)
                .Where(pw => pw.Person != null
                          && DateOnly.FromDateTime(pw.Person.CreatedAt) >= startDate
                          && DateOnly.FromDateTime(pw.Person.CreatedAt) <= endDate)
                .ToListAsync(ct);

            // ── Labels ───────────────────────────────────────────────────
            var colName      = isAr ? "الاسم الكامل"         : "Full Name";
            var colAgeRange  = isAr ? "الفئة العمرية"        : "Age Range";
            var colBlood     = isAr ? "فصيلة الدم"           : "Blood Group";
            var colRh        = isAr ? "عامل الريسوس"         : "Rh Factor";
            var colPregs     = isAr ? "عدد مرات الحمل"       : "Pregnancies";
            var colAlive     = isAr ? $"أحياء ({currentYear})" : $"Alive ({currentYear})";
            var colDead      = isAr ? $"وفيات ({currentYear})" : $"Deaths ({currentYear})";
            var colReg       = isAr ? "تاريخ التسجيل"        : "Registered On";

            // ── Summary Labels ───────────────────────────────────────────
            var lblBloodA    = isAr ? "فصيلة A" : "Blood Group A";
            var lblBloodB    = isAr ? "فصيلة B" : "Blood Group B";
            var lblBloodAB   = isAr ? "فصيلة AB" : "Blood Group AB";
            var lblBloodO    = isAr ? "فصيلة O" : "Blood Group O";
            var lblTotalNB   = isAr ? "إجمالي المواليد" : "Total Newborns";

            // ── Summary Stats ────────────────────────────────────────────
            var bloodCounts = new Dictionary<string, int>
            {
                ["A"] = 0, ["B"] = 0, ["AB"] = 0, ["O"] = 0
            };
            int totalNewbornsOverall = 0;

            foreach (var pw in women)
            {
                var bg = pw.BloodGroup.ToString();
                if (bloodCounts.ContainsKey(bg)) bloodCounts[bg]++;
                
                totalNewbornsOverall += pw.PregnancyDetails.Sum(pd => pd.NewbornCount);
            }

            var summaryStats = new Dictionary<string, string>
            {
                [lblBloodA]  = bloodCounts["A"].ToString(),
                [lblBloodB]  = bloodCounts["B"].ToString(),
                [lblBloodAB] = bloodCounts["AB"].ToString(),
                [lblBloodO]  = bloodCounts["O"].ToString(),
                [lblTotalNB] = totalNewbornsOverall.ToString()
            };

            // ── Detail rows ───────────────────────────────────────────────
            var rows = women.Select(pw =>
            {
                var thisYearNewborns = pw.PregnancyDetails
                    .Where(pd => pd.DeliveryDate.HasValue && pd.DeliveryDate.Value.Year == currentYear)
                    .SelectMany(pd => pd.Newborns)
                    .ToList();

                int aliveCount = thisYearNewborns.Count(n => n.NewbornStatus == NewbornStatus.Alive);
                int deadCount  = thisYearNewborns.Count(n => n.NewbornStatus == NewbornStatus.Dead);

                return new ReportRow
                {
                    Columns = new Dictionary<string, string?>
                    {
                        [colName]     = pw.Person?.FullName ?? "-",
                        [colAgeRange] = pw.AgeRange,
                        [colBlood]    = pw.BloodGroup.ToString(),
                        [colRh]       = pw.RhFactor == RhFactor.Positive ? "+" : "-",
                        [colPregs]    = pw.PregnancyCount.ToString(),
                        [colAlive]    = aliveCount > 0 ? aliveCount.ToString() : "—",
                        [colDead]     = deadCount  > 0 ? deadCount.ToString()  : "—",
                        [colReg]      = pw.Person?.CreatedAt.ToString("yyyy-MM-dd") ?? "-",
                    }
                };
            }).ToList();

            return new ReportData
            {
                ReportType    = ReportType,
                StartDate     = startDate,
                EndDate       = endDate,
                Lang          = lang,
                GeneratedAt   = DateTime.UtcNow.AddHours(3).ToString("yyyy-MM-dd HH:mm"),
                TotalRecords  = rows.Count,
                SummaryStats  = summaryStats,
                ColumnHeaders = [colName, colAgeRange, colBlood, colRh, colPregs, colAlive, colDead, colReg],
                Rows          = rows
            };
        }
    }
}
