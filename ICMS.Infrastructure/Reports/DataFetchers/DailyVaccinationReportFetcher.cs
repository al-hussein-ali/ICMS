using ICMS.Application.DTOs.Reports;
using ICMS.Application.Enums;
using ICMS.Application.Interfaces;
using ICMS.Application.Interfaces.Reports;
using ICMS.Application.Extensions;
using ICMS.Domain.Enums;
using ICMS.Domain.Entites.Audit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ICMS.Infrastructure.Reports.DataFetchers
{
    public class DailyVaccinationReportFetcher : IReportDataFetcher
    {
        private readonly IUnitOfWork _unitOfWork;

        public DailyVaccinationReportFetcher(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public ReportType ReportType => ReportType.DailyVaccination;

        public async Task<ReportData> FetchAsync(DateOnly startDate, DateOnly endDate, string lang = "en", Dictionary<string, string>? parameters = null, CancellationToken ct = default)
        {
            var isAr = lang.StartsWith("ar", StringComparison.OrdinalIgnoreCase);

            var pills = new List<string>();

            if (parameters != null)
            {
                parameters = new Dictionary<string, string>(parameters, StringComparer.OrdinalIgnoreCase);
            }

            // Parse optional filters
            ScheduleStatus? statusFilter = null;
            if (parameters != null && parameters.TryGetValue("status", out var statusStr) &&
                Enum.TryParse<ScheduleStatus>(statusStr, true, out var parsedStatus))
            {
                statusFilter = parsedStatus;
                var statusLabel = isAr
                    ? (parsedStatus == ScheduleStatus.Completed ? "مكتملة" : parsedStatus == ScheduleStatus.Missed ? "فائتة" : "معلقة")
                    : parsedStatus.ToString();
                pills.Add($"<span class='filter-pill'>{(isAr ? "الحالة" : "Status")}: {statusLabel}</span>");
            }

            Gender? genderFilter = null;
            if (parameters != null && parameters.TryGetValue("gender", out var genderStr) &&
                Enum.TryParse<Gender>(genderStr, true, out var parsedGender))
            {
                genderFilter = parsedGender;
                var genderLabel = isAr
                    ? (parsedGender == Gender.Male ? "ذكور" : "إناث")
                    : (parsedGender == Gender.Male ? "Males" : "Females");
                pills.Add($"<span class='filter-pill'>{(isAr ? "الجنس" : "Gender")}: {genderLabel}</span>");
            }

            int? vaccineIdFilter = null;
            if (parameters != null && parameters.TryGetValue("vaccineId", out var vIdStr) && int.TryParse(vIdStr, out var vId))
            {
                vaccineIdFilter = vId;
                var vaccine = await _unitOfWork.VaccineRepository.GetByIdAsync(vId, ct);
                if (vaccine != null)
                {
                    var vName = LocalizationHelper.GetLocalizedValue(vaccine.VaccineName, lang);
                    pills.Add($"<span class='filter-pill'>{(isAr ? "اللقاح" : "Vaccine")}: {vName}</span>");
                }
            }

            int? doseIdFilter = null;
            if (parameters != null && parameters.TryGetValue("doseId", out var dIdStr) && int.TryParse(dIdStr, out var dId))
            {
                doseIdFilter = dId;
                var dose = await _unitOfWork.DoseRepository.GetByIdAsync(dId, ct);
                if (dose != null)
                {
                    var dName = LocalizationHelper.GetLocalizedValue(dose.DoseName, lang);
                    pills.Add($"<span class='filter-pill'>{(isAr ? "الجرعة" : "Dose")}: {dName}</span>");
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

            // ── Dynamic report title based on active filters ────────────────
            string reportTitle;
            string baseTitle;
            if (!statusFilter.HasValue)
                baseTitle = isAr ? "تقرير التطعيم" : "Immunization Report";
            else if (statusFilter.Value == ScheduleStatus.Completed)
                baseTitle = isAr ? "تقرير الجرعات المكتملة" : "Completed Vaccinations Report";
            else if (statusFilter.Value == ScheduleStatus.Missed)
                baseTitle = isAr ? "تقرير الجرعات الفائتة" : "Missed Doses Report";
            else
                baseTitle = isAr ? "تقرير الجرعات المعلقة" : "Pending Doses Report";

            if (isAr)
            {
                reportTitle = baseTitle;
                if (!string.IsNullOrEmpty(periodPrefix))
                {
                    if (baseTitle == "تقرير التطعيم")
                        reportTitle = $"تقرير التطعيم {periodPrefix}";
                    else
                        reportTitle = $"{baseTitle} {periodPrefix}";
                }
            }
            else
            {
                reportTitle = string.IsNullOrEmpty(periodPrefix) ? baseTitle : $"{periodPrefix} {baseTitle}";
            }

            // ── Column headers ──────────────────────────────────────────────
            var colDate     = isAr ? "التاريخ"             : "Date";
            var colCard     = isAr ? "رقم البطاقة"         : "Card Number";
            var colName     = isAr ? "المستفيد"           : "Beneficiary";
            var colAgeW     = isAr ? "العمر (بالأسابيع)"   : "Age (Weeks)";
            var colAgeY     = isAr ? "العمر (بالسنوات)"    : "Age (Years)";
            var colVaccine  = isAr ? "اللقاح"             : "Vaccine";
            var colDose     = isAr ? "الجرعة"             : "Dose";
            var colDoseNum  = isAr ? "رقم الجرعة"          : "Dose No.";
            var colLocation = isAr ? "الموقع"             : "Location";
            var colStatus   = isAr ? "الحالة"             : "Status";

            List<ReportRow> rows;
            var stats = new Dictionary<string, int>();
            List<ReportRow> secondaryRows = new();
            string txTableTitle = string.Empty;
            List<string> txColumnHeaders = new();

            bool fetchSchedules = !statusFilter.HasValue || statusFilter.Value != ScheduleStatus.Completed;
            bool fetchCompleted = !statusFilter.HasValue || statusFilter.Value == ScheduleStatus.Completed;

            if (fetchSchedules)
            {
                var scheduleQuery = _unitOfWork.VaccinationScheduleRepository
                    .GetQueryable(false, ct, s => s.VaccinatedIndividual, s => s.Dose)
                    .Where(s => s.ScheduledDate >= startDate && s.ScheduledDate <= endDate);

                if (statusFilter.HasValue)
                    scheduleQuery = scheduleQuery.Where(s => s.Status == statusFilter.Value);
                else
                    scheduleQuery = scheduleQuery.Where(s => s.Status != ScheduleStatus.Completed);

                scheduleQuery = scheduleQuery
                    .Include(s => s.VaccinatedIndividual)
                        .ThenInclude(vi => vi.Person)
                    .Include(s => s.Dose)
                        .ThenInclude(d => d.Vaccine);

                if (genderFilter.HasValue)
                    scheduleQuery = scheduleQuery.Where(s => s.VaccinatedIndividual.Person.Gender == genderFilter.Value);

                if (vaccineIdFilter.HasValue)
                    scheduleQuery = scheduleQuery.Where(s => s.Dose.VaccineId == vaccineIdFilter.Value);

                if (doseIdFilter.HasValue)
                    scheduleQuery = scheduleQuery.Where(s => s.DoseId == doseIdFilter.Value);

                var schedules = await scheduleQuery.ToListAsync(ct);

                var schedRows = schedules.Select(s =>
                {
                    var statusLabel = s.Status == ScheduleStatus.Missed
                        ? (isAr ? "فائتة" : "Missed")
                        : (isAr ? "معلقة" : "Pending");

                    var dob = s.VaccinatedIndividual.Person.DateOfBirth;
                    var refDate = s.ScheduledDate;
                    var totalDays = (refDate.ToDateTime(TimeOnly.MinValue) - dob.ToDateTime(TimeOnly.MinValue)).TotalDays;
                    var ageWeeks = (int)Math.Floor(totalDays / 7.0);
                    var ageYears = (int)Math.Floor(totalDays / 365.25);

                    var doseNumberStr = "1";
                    if (s.Dose != null)
                    {
                        var match = Regex.Match(s.Dose.DoseName, @"\d+");
                        var currentDoseNum = match.Success ? match.Value : "1";
                        var totalDoses = s.Dose.Vaccine?.TotalDosages ?? 1;
                        doseNumberStr = $"{currentDoseNum}/{totalDoses}";
                    }

                    var vacName = LocalizationHelper.GetLocalizedValue(s.Dose?.Vaccine?.VaccineName, lang);
                    if (!string.IsNullOrEmpty(vacName))
                        stats[vacName] = stats.GetValueOrDefault(vacName) + 1;

                    return new ReportRow
                    {
                        Columns = new Dictionary<string, string?>
                        {
                            [colDate]     = s.ScheduledDate.ToString("yyyy-MM-dd"),
                            [colCard]     = s.VaccinatedIndividual.CardNumber,
                            [colName]     = s.VaccinatedIndividual.Person.FullName,
                            [colAgeW]     = ageWeeks.ToString(),
                            [colAgeY]     = ageYears.ToString(),
                            [colVaccine]  = vacName,
                            [colDose]     = LocalizationHelper.GetLocalizedValue(s.Dose?.DoseName, lang),
                            [colDoseNum]  = doseNumberStr,
                            [colLocation] = "—",
                            [colStatus]   = statusLabel
                        }
                    };
                });
                
                rows.AddRange(schedRows);
            }

            if (fetchCompleted)
            {
                var query = _unitOfWork.ImmunizationRecordRepository
                    .GetQueryable(false, ct, ir => ir.VaccinatedIndividual, ir => ir.Dose)
                    .Where(ir => DateOnly.FromDateTime(ir.VaccinationDate) >= startDate && DateOnly.FromDateTime(ir.VaccinationDate) <= endDate);

                // Apply remaining filters
                if (genderFilter.HasValue)
                    query = query.Where(ir => ir.VaccinatedIndividual.Person.Gender == genderFilter.Value);

                if (vaccineIdFilter.HasValue)
                    query = query.Where(ir => ir.Dose.VaccineId == vaccineIdFilter.Value);

                if (doseIdFilter.HasValue)
                    query = query.Where(ir => ir.DoseId == doseIdFilter.Value);

                var records = await query
                    .Include(ir => ir.VaccinatedIndividual.Person)
                    .Include(ir => ir.Dose.Vaccine)
                    .ToListAsync(ct);

                rows = records.Select(ir =>
                {
                    var dob = ir.VaccinatedIndividual.Person.DateOfBirth;
                    var adminDate = ir.VaccinationDate;

                    var totalDays = (adminDate.ToDateTime(TimeOnly.MinValue) - dob.ToDateTime(TimeOnly.MinValue)).TotalDays;
                    var ageWeeks = (int)Math.Floor(totalDays / 7.0);
                    var ageYears = (int)Math.Floor(totalDays / 365.25);

                    var doseNumberStr = "1";
                    if (ir.Dose != null)
                    {
                        var match = Regex.Match(ir.Dose.DoseName, @"\d+");
                        var currentDoseNum = match.Success ? match.Value : "1";
                        var totalDoses = ir.Dose.Vaccine?.TotalDosages ?? 1;
                        doseNumberStr = $"{currentDoseNum}/{totalDoses}";
                    }

                    var vacName = LocalizationHelper.GetLocalizedValue(ir.Dose.Vaccine?.VaccineName, lang);
                    if (!string.IsNullOrEmpty(vacName))
                        stats[vacName] = stats.GetValueOrDefault(vacName) + 1;

                    var completedStatusLabel = isAr ? "مكتملة" : "Completed";

                    return new ReportRow
                    {
                        Columns = new Dictionary<string, string?>
                        {
                            [colDate]     = adminDate.ToString("yyyy-MM-dd"),
                            [colCard]     = ir.VaccinatedIndividual.CardNumber,
                            [colName]     = ir.VaccinatedIndividual.Person.FullName,
                            [colAgeW]     = ageWeeks.ToString(),
                            [colAgeY]     = ageYears.ToString(),
                            [colVaccine]  = vacName,
                            [colDose]     = LocalizationHelper.GetLocalizedValue(ir.Dose.DoseName, lang),
                            [colDoseNum]  = doseNumberStr,
                            [colLocation] = NormalizeLocation(ir.TakenIn, isAr),
                            [colStatus]   = completedStatusLabel
                        }
                    };
                });
                
                rows.AddRange(compRows);
            }

            rows = rows.OrderBy(r => r.Columns[colDate]).ToList();

            // Additional logic for secondary table (inventory transactions) 
            // Only query transactions if we fetched completed records
            if (fetchCompleted)
            {
                // ── Secondary Table: Inventory Transactions ────────────────────
                var startDateTime = DateTime.SpecifyKind(startDate.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);
                var endDateTime   = DateTime.SpecifyKind(endDate.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc).AddDays(1);

                var txQuery = _unitOfWork.TransactionRepository
                    .GetQueryable(false, ct)
                    .Include(t => t.Batch)
                        .ThenInclude(b => b.Dose)
                            .ThenInclude(d => d.Vaccine)
                    .Where(t => t.TransactionType == TransactionType.Out
                             && t.TransactionDate >= startDateTime
                             && t.TransactionDate < endDateTime);

                if (vaccineIdFilter.HasValue)
                    txQuery = txQuery.Where(t => t.Batch.Dose.VaccineId == vaccineIdFilter.Value);

                if (doseIdFilter.HasValue)
                    txQuery = txQuery.Where(t => t.Batch.DoseId == doseIdFilter.Value);

                var transactions = await txQuery.ToListAsync(ct);

                var txColDate      = isAr ? "تاريخ العملية"     : "Transaction Date";
                var txColBatch     = isAr ? "اسم الدفعة"       : "Batch Name";
                var txColVaccine   = isAr ? "اللقاح"           : "Vaccine";
                var txColDose      = isAr ? "الجرعة"           : "Dose";
                var txColPermit    = isAr ? "رقم الإذن"        : "Permission No.";
                var txColQty       = isAr ? "الكمية المنصرفة"   : "Subtracted Qty";

                txColumnHeaders = new List<string> { txColDate, txColBatch, txColVaccine, txColDose, txColPermit, txColQty };
                txTableTitle    = isAr ? "الجرعات والتشغيلات المنصرفة من المخزون" : "Dose Batches Subtracted from Inventory";

                secondaryRows = transactions.Select(t => new ReportRow
                {
                    Columns = new Dictionary<string, string?>
                    {
                        [txColDate]    = t.TransactionDate.ToString("yyyy-MM-dd HH:mm"),
                        [txColBatch]   = t.Batch?.BatchName ?? "-",
                        [txColVaccine] = LocalizationHelper.GetLocalizedValue(t.Batch?.Dose?.Vaccine?.VaccineName, lang),
                        [txColDose]    = LocalizationHelper.GetLocalizedValue(t.Batch?.Dose?.DoseName, lang),
                        [txColPermit]  = t.PermissionNumber,
                        [txColQty]     = t.Quantity.ToString()
                    }
                }).OrderBy(r => r.Columns[txColDate]).ToList();

            }

            // Completed path has location column, schedules path does not, so if fetchSchedules is true we include colStatus
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
                SummaryStats  = stats.ToDictionary(k => k.Key, v => v.Value.ToString()),
                ColumnHeaders = [colDate, colCard, colName, colAgeW, colAgeY, colVaccine, colDose, colDoseNum, colLocation, colStatus],
                Rows          = rows,
                SecondaryTableTitle = secondaryRows.Any() ? txTableTitle : null,
                SecondaryColumnHeaders = secondaryRows.Any() ? txColumnHeaders : null,
                SecondaryRows = secondaryRows.Any() ? secondaryRows : null
            };
        }

        /// <summary>
        /// Maps i18n key strings (e.g. "immunization.modal.healthCenter") to
        /// a clean human-readable localized label for the printed report.
        /// </summary>
        private static string NormalizeLocation(string takenIn, bool isAr)
        {
            if (string.IsNullOrWhiteSpace(takenIn)) return "—";

            var key = takenIn.Trim().ToLowerInvariant();

            if (key.Contains("healthcenter") || key.Contains("health center") || key.Contains("health_center"))
                return isAr ? "المركز الصحي" : "Health Center";
            
            if (key.Contains("field"))
                return isAr ? "زيارة ميدانية" : "Field Visit";
            
            if (key.Contains("hospital"))
                return isAr ? "مستشفى" : "Hospital";
            
            if (key.Contains("clinic"))
                return isAr ? "عيادة" : "Clinic";
            
            if (key.Contains("home"))
                return isAr ? "زيارة منزلية" : "Home Visit";

            // If it looks like an i18n key (contains dots), extract last segment and un-camelCase it
            if (takenIn.Contains('.'))
            {
                var last = takenIn.Split('.')[^1];
                var name = Regex.Replace(last, "([A-Z])", " $1").Trim();
                
                if (isAr)
                {
                    if (name.Equals("Health Center", StringComparison.OrdinalIgnoreCase)) return "المركز الصحي";
                    if (name.Equals("Field Visit", StringComparison.OrdinalIgnoreCase)) return "زيارة ميدانية";
                }
                return name;
            }

            return takenIn;
        }
    }
}
