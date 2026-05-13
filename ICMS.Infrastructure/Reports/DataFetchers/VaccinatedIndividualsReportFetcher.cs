using ICMS.Application.DTOs.Reports;
using ICMS.Application.Enums;
using ICMS.Application.Interfaces;
using ICMS.Application.Interfaces.Reports;
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

        public async Task<ReportData> FetchAsync(DateOnly startDate, DateOnly endDate, string lang = "en", CancellationToken ct = default)
        {
            var isAr = lang.StartsWith("ar", StringComparison.OrdinalIgnoreCase);

            var individuals = await _unitOfWork.VaccinatedIndividualRepository
                .GetQueryable(false, ct, vi => vi.Person)
                .Where(vi => DateOnly.FromDateTime(vi.Person.CreatedAt) >= startDate
                          && DateOnly.FromDateTime(vi.Person.CreatedAt) <= endDate)
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
