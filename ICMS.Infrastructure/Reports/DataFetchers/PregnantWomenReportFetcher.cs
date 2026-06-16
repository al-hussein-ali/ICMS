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

        public async Task<ReportData> FetchAsync(DateOnly startDate, DateOnly endDate, string lang = "en", Dictionary<string, string>? parameters = null, CancellationToken ct = default)
        {
            var isAr = lang.StartsWith("ar", StringComparison.OrdinalIgnoreCase);
            int currentYear = DateTime.UtcNow.AddHours(3).Year;

            var queryable = unitOfWork.PregnantWomanRepository
                .GetQueryable(false, ct, pw => pw.Person, pw => pw.PregnancyDetails);

            if (queryable == null)
                throw new InvalidOperationException("Failed to get queryable.");

            var query = queryable
                .Include(pw => pw.PregnancyDetails)
                    .ThenInclude(pd => pd.Newborns)
                .Where(pw => pw.Person != null
                          && DateOnly.FromDateTime(pw.Person.CreatedAt) >= startDate
                          && DateOnly.FromDateTime(pw.Person.CreatedAt) <= endDate);

            var pills = new List<string>();

            if (parameters != null)
            {
                if (parameters.TryGetValue("bloodGroup", out var bgStr) && Enum.TryParse<BloodGroup>(bgStr, true, out var bg))
                {
                    query = query.Where(pw => pw.BloodGroup == bg);
                    pills.Add($"<span class='filter-pill'>{(isAr ? "فصيلة الدم" : "Blood Group")}: {bg}</span>");
                }

                if (parameters.TryGetValue("rhFactor", out var rhStr) && Enum.TryParse<RhFactor>(rhStr, true, out var rh))
                {
                    query = query.Where(pw => pw.RhFactor == rh);
                    var rhLabel = rh == RhFactor.Positive ? "+" : "-";
                    pills.Add($"<span class='filter-pill'>{(isAr ? "عامل الريسوس" : "Rh Factor")}: {rhLabel}</span>");
                }

                if (parameters.TryGetValue("isPregnancyDone", out var doneStr) && bool.TryParse(doneStr, out var isDone))
                {
                    query = query.Where(pw => pw.PregnancyDetails.Any(pd => pd.IsPregnancyDone == isDone));
                    var doneLabel = isAr
                        ? (isDone ? "حمل منتهي" : "حمل جاري")
                        : (isDone ? "Completed Pregnancy" : "Ongoing Pregnancy");
                    pills.Add($"<span class='filter-pill'>{(isAr ? "الحالة" : "Status")}: {doneLabel}</span>");
                }

                if (parameters.TryGetValue("pregnancyType", out var ptStr) && Enum.TryParse<PregnancyType>(ptStr, true, out var pt))
                {
                    query = query.Where(pw => pw.PregnancyDetails.Any(pd => pd.PregnancyType == pt));
                    var ptLabel = isAr
                        ? (pt == PregnancyType.Multiple ? "متعدد" : "مفرد")
                        : pt.ToString();
                    pills.Add($"<span class='filter-pill'>{(isAr ? "نوع الحمل" : "Pregnancy Type")}: {ptLabel}</span>");
                }

                if (parameters.TryGetValue("birthNature", out var bnStr) && Enum.TryParse<BirthNature>(bnStr, true, out var bn))
                {
                    query = query.Where(pw => pw.PregnancyDetails.Any(pd => pd.BirthNature == bn));
                    var bnLabel = isAr
                        ? (bn == BirthNature.Natural ? "طبيعية" : "قيصرية")
                        : bn.ToString();
                    pills.Add($"<span class='filter-pill'>{(isAr ? "طبيعة الولادة" : "Birth Nature")}: {bnLabel}</span>");
                }

                if (parameters.TryGetValue("riskLevel", out var riskStr) && !string.IsNullOrEmpty(riskStr))
                {
                    if (riskStr == "HighRisk")
                    {
                        query = query.Where(pw => pw.PregnancyCount >= 5 || 
                            pw.PregnancyDetails.Any(pd => 
                                pd.PreviousPregnancyComplicationsId != null || 
                                pd.PregnancyType == PregnancyType.Multiple));
                        pills.Add($"<span class='filter-pill'>{(isAr ? "الخطورة: عالية الخطورة" : "Risk: High Risk")}</span>");
                    }
                }
            }

            if (pills.Count == 0)
            {
                pills.Add($"<span class='filter-pill'>{(isAr ? "جميع السجلات" : "All Records")}</span>");
            }

            var subtitle = string.Join(" ", pills);

            // Retrieve period parameter
            string? period = null;
            if (parameters != null && parameters.TryGetValue("period", out var pStr))
            {
                period = pStr;
            }

            string periodPrefix = "";
            if (!string.IsNullOrEmpty(period))
            {
                periodPrefix = isAr
                    ? (period == "daily" ? "اليومي" : period == "weekly" ? "الأسبوعي" : period == "monthly" ? "الشهري" : "السنوي")
                    : (period == "daily" ? "Daily" : period == "weekly" ? "Weekly" : period == "monthly" ? "Monthly" : "Yearly");
            }

            string reportTitle;
            if (isAr)
            {
                reportTitle = string.IsNullOrEmpty(periodPrefix)
                    ? "تقرير صحة الأم والحوامل"
                    : $"تقرير صحة الأم والحوامل {periodPrefix.Trim()}";
            }
            else
            {
                reportTitle = string.IsNullOrEmpty(periodPrefix)
                    ? "Maternal Health & Pregnancy Report"
                    : $"{periodPrefix} Maternal Health & Pregnancy Report";
            }

            var women = await query.ToListAsync(ct);

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
                ReportTitle   = reportTitle,
                Subtitle      = subtitle,
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
