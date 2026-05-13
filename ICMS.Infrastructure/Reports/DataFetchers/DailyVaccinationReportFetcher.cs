using ICMS.Application.DTOs.Reports;
using ICMS.Application.Enums;
using ICMS.Application.Interfaces;
using ICMS.Application.Interfaces.Reports;
using ICMS.Domain.Enums;
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

        public async Task<ReportData> FetchAsync(DateOnly startDate, DateOnly endDate, string lang = "en", CancellationToken ct = default)
        {
            var isAr = lang.StartsWith("ar", StringComparison.OrdinalIgnoreCase);

            var records = await _unitOfWork.ImmunizationRecordRepository
                .GetQueryable(false, ct)
                .Include(ir => ir.Dose)
                .ThenInclude(d => d.Vaccine)
                .Include(ir => ir.VaccinatedIndividual)
                .ThenInclude(vi => vi.Person)
                .Where(ir => ir.VaccinationDate >= startDate && ir.VaccinationDate <= endDate)
                .ToListAsync(ct);

            // ── Labels (EN / AR) ─────────────────────────────────────────
            var labelUnder1  = isAr ? "أقل من سنة"        : "Children Under 1 yr";
            var label1to2    = isAr ? "من 1 إلى 2 سنة"    : "Children 1–2 yrs";
            var label2to5    = isAr ? "من 2 إلى 5 سنوات"  : "Children 2–5 yrs";
            var labelWomen   = isAr ? "نساء 15–49 سنة"     : "Women 15–49 yrs";
            var labelTotal   = isAr ? "إجمالي الجرعات"    : "Total Doses";

            var colDate      = isAr ? "التاريخ"           : "Date";
            var colCard      = isAr ? "رقم البطاقة"       : "Card No.";
            var colName      = isAr ? "المستفيد"          : "Beneficiary";
            var colAgeM      = isAr ? "العمر (بالأشهر)"   : "Age (Months)";
            var colAgeY      = isAr ? "العمر (بالسنوات)"  : "Age (Years)";
            var colVaccine   = isAr ? "اللقاح"            : "Vaccine";
            var colDose      = isAr ? "الجرعة"            : "Dose";
            var colDoseNum   = isAr ? "رقم الجرعة"        : "Dose Number";
            var colLocation  = isAr ? "الموقع"            : "Location";

            // ── Age-group summary ─────────────────────────────────────────
            var stats = new Dictionary<string, int>
            {
                [labelUnder1] = 0,
                [label1to2]   = 0,
                [label2to5]   = 0,
                [labelWomen]  = 0,
                [labelTotal]  = records.Count
            };

            foreach (var record in records)
            {
                var person   = record.VaccinatedIndividual.Person;
                var dob      = person.DateOfBirth;
                var adminDate = record.VaccinationDate;

                int ageInMonths = (adminDate.Year - dob.Year) * 12 + adminDate.Month - dob.Month;
                if (adminDate.Day < dob.Day) ageInMonths--;

                if      (ageInMonths < 12) stats[labelUnder1]++;
                else if (ageInMonths < 24) stats[label1to2]++;
                else if (ageInMonths < 60) stats[label2to5]++;

                if (person.Gender == Gender.Female && ageInMonths >= 180 && ageInMonths <= 588)
                    stats[labelWomen]++;
            }

            // ── Detail rows ───────────────────────────────────────────────
            var rows = records.Select(ir =>
            {
                var dob         = ir.VaccinatedIndividual.Person.DateOfBirth;
                var adminDate   = ir.VaccinationDate;
                int ageMonths   = (adminDate.Year - dob.Year) * 12 + adminDate.Month - dob.Month;
                if (adminDate.Day < dob.Day) ageMonths--;
                int ageYears    = ageMonths / 12;

                var totalDoses = ir.Dose.Vaccine?.TotalDosages ?? 0;
                var doseNumberStr = totalDoses > 0 ? $"{ir.Dose.DoseOrder}/{totalDoses}" : ir.Dose.DoseOrder.ToString();

                return new ReportRow
                {
                    Columns = new Dictionary<string, string?>
                    {
                        [colDate]     = adminDate.ToString("yyyy-MM-dd"),
                        [colCard]     = ir.VaccinatedIndividual.CardNumber,
                        [colName]     = ir.VaccinatedIndividual.Person.FullName,
                        [colAgeM]     = ageMonths.ToString(),
                        [colAgeY]     = ageYears.ToString(),
                        [colVaccine]  = ir.Dose.Vaccine?.VaccineName ?? "N/A",
                        [colDose]     = ir.Dose.DoseName,
                        [colDoseNum]  = doseNumberStr,
                        [colLocation] = NormalizeLocation(ir.TakenIn, isAr)
                    }
                };
            }).OrderBy(r => r.Columns[colDate]).ToList();

            return new ReportData
            {
                ReportType    = ReportType,
                StartDate     = startDate,
                EndDate       = endDate,
                Lang          = lang,
                GeneratedAt   = DateTime.UtcNow.AddHours(3).ToString("yyyy-MM-dd HH:mm"),
                TotalRecords  = records.Count,
                SummaryStats  = stats.ToDictionary(k => k.Key, v => v.Value.ToString()),
                ColumnHeaders = [colDate, colCard, colName, colAgeM, colAgeY, colVaccine, colDose, colDoseNum, colLocation],
                Rows          = rows
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
