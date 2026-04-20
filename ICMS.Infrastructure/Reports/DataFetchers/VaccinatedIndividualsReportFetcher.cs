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

namespace ICMS.Infrastructure.Reports.DataFetchers
{
    public class VaccinatedIndividualsReportFetcher : IReportDataFetcher
    {
        private readonly IUnitOfWork _unitOfWork;

        public VaccinatedIndividualsReportFetcher(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public ReportType ReportType => ReportType.VaccinatedIndividuals;

        public async Task<ReportData> FetchAsync(DateOnly startDate, DateOnly endDate, CancellationToken ct = default)
        {
            var individuals = await _unitOfWork.VaccinatedIndividualRepository
                .GetQueryable(false, ct, vi => vi.Person)
                .Where(vi => DateOnly.FromDateTime(vi.Person.CreatedAt) >= startDate
                          && DateOnly.FromDateTime(vi.Person.CreatedAt) <= endDate)
                .ToListAsync(ct);

            var genderGroups = individuals
                .GroupBy(vi => vi.Person.Gender.ToString())
                .ToDictionary(g => g.Key, g => g.Count().ToString());

            var rows = individuals.Select(vi => new ReportRow
            {
                Columns = new Dictionary<string, string?>
                {
                    ["Card Number"]    = vi.CardNumber,
                    ["Full Name"]      = vi.Person.FullName,
                    ["Gender"]         = vi.Person.Gender.ToString(),
                    ["Date of Birth"]  = vi.Person.DateOfBirth.ToString("yyyy-MM-dd"),
                    ["Phone"]          = vi.Person.PhoneNumber,
                    ["Registered On"]  = vi.Person.CreatedAt.ToString("yyyy-MM-dd"),
                }
            }).ToList();

            return new ReportData
            {
                ReportType    = ReportType,
                StartDate     = startDate,
                EndDate       = endDate,
                GeneratedAt   = DateTime.UtcNow.AddHours(3).ToString("yyyy-MM-dd HH:mm"),
                TotalRecords  = rows.Count,
                SummaryStats  = genderGroups,
                ColumnHeaders = ["Card Number", "Full Name", "Gender", "Date of Birth", "Phone", "Registered On"],
                Rows          = rows
            };
        }
    }
}
