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

            var query = _unitOfWork.ImmunizationRecordRepository
                .GetQueryable(false, ct, ir => ir.VaccinatedIndividual, ir => ir.Dose)
                .Where(ir => ir.VaccinationDate >= startDate && ir.VaccinationDate <= endDate);

            if (parameters != null)
            {
                if (parameters.TryGetValue("status", out var statusStr) && Enum.TryParse<ScheduleStatus>(statusStr, true, out var status))
                {
                    if (status != ScheduleStatus.Completed)
                    {
                        query = query.Where(ir => false);
                    }
                }

                if (parameters.TryGetValue("gender", out var genderStr) && Enum.TryParse<Gender>(genderStr, true, out var gender))
                {
                    query = query.Where(ir => ir.VaccinatedIndividual.Person.Gender == gender);
                }

                if (parameters.TryGetValue("vaccineId", out var vIdStr) && int.TryParse(vIdStr, out var vId))
                {
                    query = query.Where(ir => ir.Dose.VaccineId == vId);
                }

                if (parameters.TryGetValue("doseId", out var dIdStr) && int.TryParse(dIdStr, out var dId))
                {
                    query = query.Where(ir => ir.DoseId == dId);
                }
            }

            var records = await query
                .Include(ir => ir.VaccinatedIndividual.Person)
                .Include(ir => ir.Dose.Vaccine)
                .ToListAsync(ct);

            // ── Column headers based on Language ─────────────────────────
            var colDate     = isAr ? "التاريخ"             : "Date";
            var colCard     = isAr ? "رقم البطاقة"         : "Card Number";
            var colName     = isAr ? "المستفيد"           : "Beneficiary";
            var colAgeW     = isAr ? "العمر (بالأسابيع)"   : "Age (Weeks)";
            var colAgeY     = isAr ? "العمر (بالسنوات)"    : "Age (Years)";
            var colVaccine  = isAr ? "اللقاح"             : "Vaccine";
            var colDose     = isAr ? "الجرعة"             : "Dose";
            var colDoseNum  = isAr ? "رقم الجرعة"          : "Dose No.";
            var colLocation = isAr ? "الموقع"             : "Location";

            // Group summary by vaccine names
            var stats = new Dictionary<string, int>();

            var rows = records.Select(ir =>
            {
                var dob = ir.VaccinatedIndividual.Person.DateOfBirth;
                var adminDate = ir.VaccinationDate;

                // Calculate age at vaccination date
                var totalDays = (adminDate.ToDateTime(TimeOnly.MinValue) - dob.ToDateTime(TimeOnly.MinValue)).TotalDays;
                var ageWeeks = (int)Math.Floor(totalDays / 7.0);
                var ageYears = (int)Math.Floor(totalDays / 365.25);

                // Safe parsing of DoseNumber (e.g. "Dose 1" -> "1/3")
                var doseNumberStr = "1";
                if (ir.Dose != null)
                {
                    var match = Regex.Match(ir.Dose.DoseName, @"\d+");
                    var currentDoseNum = match.Success ? match.Value : "1";
                    var totalDoses = ir.Dose.Vaccine?.TotalDosages ?? 1;
                    doseNumberStr = $"{currentDoseNum}/{totalDoses}";
                }
                // Increment stat using localized vaccine name
                var vacName = LocalizationHelper.GetLocalizedValue(ir.Dose.Vaccine?.VaccineName, lang);
                if (!string.IsNullOrEmpty(vacName))
                {
                    stats[vacName] = stats.GetValueOrDefault(vacName) + 1;
                }

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
                        [colLocation] = NormalizeLocation(ir.TakenIn, isAr)
                    }
                };
            }).OrderBy(r => r.Columns[colDate]).ToList();

            // ── Secondary Table: Subtracted Inventory Transactions ────────
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

            if (parameters != null)
            {
                if (parameters.TryGetValue("vaccineId", out var vIdStr) && int.TryParse(vIdStr, out var vId))
                {
                    txQuery = txQuery.Where(t => t.Batch.Dose.VaccineId == vId);
                }

                if (parameters.TryGetValue("doseId", out var dIdStr) && int.TryParse(dIdStr, out var dId))
                {
                    txQuery = txQuery.Where(t => t.Batch.DoseId == dId);
                }
            }

            var transactions = await txQuery.ToListAsync(ct);

            var txColDate      = isAr ? "تاريخ العملية"     : "Transaction Date";
            var txColBatch     = isAr ? "اسم الدفعة"       : "Batch Name";
            var txColVaccine   = isAr ? "اللقاح"           : "Vaccine";
            var txColDose      = isAr ? "الجرعة"           : "Dose";
            var txColPermit    = isAr ? "رقم الإذن"        : "Permission No.";
            var txColQty       = isAr ? "الكمية المنصرفة"   : "Subtracted Qty";

            var txColumnHeaders = new List<string> { txColDate, txColBatch, txColVaccine, txColDose, txColPermit, txColQty };
            var txTableTitle    = isAr ? "الجرعات والتشغيلات المنصرفة من المخزون" : "Dose Batches Subtracted from Inventory";

            var secondaryRows = transactions.Select(t => new ReportRow
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

            return new ReportData
            {
                ReportType    = ReportType,
                StartDate     = startDate,
                EndDate       = endDate,
                Lang          = lang,
                GeneratedAt   = DateTime.UtcNow.AddHours(3).ToString("yyyy-MM-dd HH:mm"),
                TotalRecords  = records.Count,
                SummaryStats  = stats.ToDictionary(k => k.Key, v => v.Value.ToString()),
                ColumnHeaders = [colDate, colCard, colName, colAgeW, colAgeY, colVaccine, colDose, colDoseNum, colLocation],
                Rows          = rows,
                SecondaryTableTitle = txTableTitle,
                SecondaryColumnHeaders = txColumnHeaders,
                SecondaryRows = secondaryRows
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
                
                // Final fallback localized mapping for known segments if they come as raw i18n tails
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
