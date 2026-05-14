using ICMS.Application.DTOs.Reports;
using ICMS.Application.Enums;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ICMS.Application.Interfaces.Reports
{
    /// <summary>
    /// Strategy interface — each report type provides its own implementation
    /// to fetch data from the appropriate repositories.
    /// </summary>
    public interface IReportDataFetcher
    {
        ReportType ReportType { get; }
        Task<ReportData> FetchAsync(DateOnly startDate, DateOnly endDate, string lang = "en", Dictionary<string, string>? parameters = null, CancellationToken ct = default);
    }
}
