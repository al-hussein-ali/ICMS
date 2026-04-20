using ICMS.Application.DTOs.Reports;
using ICMS.Application.Interfaces.Reports;
using ICMS.Application.Interfaces.Services;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ICMS.Application.Services
{
    // This file intentionally left minimal — the actual implementation lives in
    // ICMS.Infrastructure.Reports.ReportService to keep Hangfire out of Application.
    // IReportService is registered in DI by InfrastructureExtensions.
}
