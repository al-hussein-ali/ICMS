using ICMS.Application.DTOs.Reports;
using ICMS.Application.Enums;
using ICMS.Application.Interfaces;
using ICMS.Application.Interfaces.Reports;
using ICMS.Application.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ICMS.Domain.Enums;

namespace ICMS.Infrastructure.Reports.DataFetchers
{
    public class VaccinatedIndividualsReportFetcher : IReportDataFetcher
    {
        private readonly IUnitOfWork _unitOfWork;

        public VaccinatedIndividualsReportFetcher(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public ReportType ReportType => ReportType.VaccinatedIndividuals;

        public async Task<ReportData> FetchAsync(DateOnly startDate, DateOnly endDate, string lang = "en", Dictionary<string, string>? parameters = null, CancellationToken ct = default)
        {
            var isAr = lang.StartsWith("ar", StringComparison.OrdinalIgnoreCase);

            var query = _unitOfWork.VaccinatedIndividualRepository
                .GetQueryable(false, ct, vi => vi.Person)
                .Where(vi => DateOnly.FromDateTime(vi.Person.CreatedAt) >= startDate
                          && DateOnly.FromDateTime(vi.Person.CreatedAt) <= endDate);

            var safeParams = parameters != null 
                ? new Dictionary<string, string>(parameters, StringComparer.OrdinalIgnoreCase) 
                : new Dictionary<string, string>();

            string? period = null;
            safeParams.TryGetValue("period", out period);

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
                    ? $"تقرير الأفراد الملقحين ({startDate:yyyy-MM-dd} - {endDate:yyyy-MM-dd})" 
                    : $"تقرير الأفراد الملقحين {periodPrefix.Trim()} ({startDate:yyyy-MM-dd} - {endDate:yyyy-MM-dd})";
            }
            else
            {
                reportTitle = string.IsNullOrEmpty(periodPrefix) 
                    ? $"Vaccinated Individuals Report ({startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd})" 
                    : $"{periodPrefix} Vaccinated Individuals Report ({startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd})";
            }

            var pills = new List<string>();

            if (safeParams.Count > 0)
            {
                if (safeParams.TryGetValue("gender", out var genderStr) && Enum.TryParse<Gender>(genderStr, true, out var gender))
                {
                    query = query.Where(vi => vi.Person.Gender == gender);
                    var genderLabel = isAr
                        ? (gender == Gender.Male ? "ذكور" : "إناث")
                        : (gender == Gender.Male ? "Males" : "Females");
                    pills.Add($"<span class='filter-pill'>{(isAr ? "الجنس" : "Gender")}: {genderLabel}</span>");
                }

                if (safeParams.TryGetValue("status", out var statusStr) && Enum.TryParse<ScheduleStatus>(statusStr, true, out var status))
                {
                    query = query.Where(vi => vi.Schedules.Any(s => s.Status == status));
                    var statusLabel = isAr
                        ? (status == ScheduleStatus.Completed ? "مكتملة" : status == ScheduleStatus.Missed ? "فائتة" : "معلقة")
                        : status.ToString();
                    pills.Add($"<span class='filter-pill'>{(isAr ? "الحالة" : "Status")}: {statusLabel}</span>");
                }

                if (safeParams.TryGetValue("vaccineId", out var vIdStr) && int.TryParse(vIdStr, out var vId))
                {
                    query = query.Where(vi => vi.Schedules.Any(s => s.Dose.VaccineId == vId));
                    var vaccine = await _unitOfWork.VaccineRepository.GetByIdAsync(vId, ct);
                    if (vaccine != null)
                    {
                        var vName = LocalizationHelper.GetLocalizedValue(vaccine.VaccineName, lang);
                        pills.Add($"<span class='filter-pill'>{(isAr ? "اللقاح" : "Vaccine")}: {vName}</span>");
                    }
                }

                if (safeParams.TryGetValue("doseId", out var dIdStr) && int.TryParse(dIdStr, out var dId))
                {
                    query = query.Where(vi => vi.Schedules.Any(s => s.DoseId == dId));
                    var dose = await _unitOfWork.DoseRepository.GetByIdAsync(dId, ct);
                    if (dose != null)
                    {
                        var dName = LocalizationHelper.GetLocalizedValue(dose.DoseName, lang);
                        pills.Add($"<span class='filter-pill'>{(isAr ? "الجرعة" : "Dose")}: {dName}</span>");
                    }
                }

                if (safeParams.TryGetValue("neighborhoodId", out var nIdStr) && int.TryParse(nIdStr, out var nId))
                {
                    query = query.Where(vi => vi.NeighborhoodId == nId);
                    var neighborhood = await _unitOfWork.NeighborhoodRepository.GetByIdAsync(nId, ct);
                    if (neighborhood != null)
                    {
                        pills.Add($"<span class='filter-pill'>{(isAr ? "الحي" : "Neighborhood")}: {neighborhood.Name}</span>");
                    }
                }

                if (safeParams.TryGetValue("subNeighborhoodId", out var snIdStr) && int.TryParse(snIdStr, out var snId))
                {
                    query = query.Where(vi => vi.SubNeighborhoodId == snId);
                    var subNeighborhood = await _unitOfWork.SubNeighborhoodRepository.GetByIdAsync(snId, ct);
                    if (subNeighborhood != null)
                    {
                        pills.Add($"<span class='filter-pill'>{(isAr ? "الحي الفرعي" : "Sub-Neighborhood")}: {subNeighborhood.Name}</span>");
                    }
                }
            }

            if (pills.Count == 0)
            {
                pills.Add($"<span class='filter-pill'>{(isAr ? "جميع السجلات" : "All Records")}</span>");
            }

            var subtitle = string.Join(" ", pills);

            var individuals = await query
                .Include(vi => vi.Schedules)
                    .ThenInclude(s => s.Dose)
                .ToListAsync(ct);

            // ── Labels ───────────────────────────────────────────────────
            var colCard      = isAr ? "رقم البطاقة"         : "Card Number";
            var colName      = isAr ? "الاسم الكامل"         : "Full Name";
            var colGender    = isAr ? "الجنس"               : "Gender";
            var colDob       = isAr ? "تاريخ الميلاد"        : "Date of Birth";
            var colPhone     = isAr ? "الهاتف"              : "Phone";
            var colReg       = isAr ? "تاريخ التسجيل"        : "Registered On";

            var lblMale      = isAr ? "ذكور"                : "Males";
            var lblFemale    = isAr ? "إناث"                : "Females";
            var lblTotal     = isAr ? "إجمالي الأفراد"       : "Total Individuals";

            var stats = new Dictionary<string, string>
            {
                [lblMale]   = individuals.Count(vi => vi.Person.Gender == Gender.Male).ToString(),
                [lblFemale] = individuals.Count(vi => vi.Person.Gender == Gender.Female).ToString(),
                [lblTotal]  = individuals.Count.ToString()
            };

            var rows = individuals.Select(vi => new ReportRow
            {
                Columns = new Dictionary<string, string?>
                {
                    [colCard]    = vi.CardNumber,
                    [colName]    = vi.Person.FullName,
                    [colGender]  = isAr ? (vi.Person.Gender == Gender.Male ? "ذكر" : "أنثى") : vi.Person.Gender.ToString(),
                    [colDob]     = vi.Person.DateOfBirth.ToString("yyyy-MM-dd"),
                    [colPhone]   = vi.Person.PhoneNumber,
                    [colReg]     = vi.Person.CreatedAt.ToString("yyyy-MM-dd"),
                }
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
                SummaryStats  = stats,
                ColumnHeaders = [colCard, colName, colGender, colDob, colPhone, colReg],
                Rows          = rows
            };
        }
    }
}
