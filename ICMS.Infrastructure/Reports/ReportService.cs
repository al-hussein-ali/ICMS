using ICMS.Application.DTOs.Reports;
using ICMS.Application.Interfaces.Reports;
using ICMS.Application.Interfaces.Services;
using Hangfire;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ICMS.Infrastructure.Reports
{
    /// <summary>
    /// Infrastructure implementation of IReportService.
    /// Uses Hangfire's IBackgroundJobClient to enqueue the actual generator job.
    /// This keeps Hangfire entirely in the Infrastructure layer.
    /// </summary>
    public class ReportService : IReportService
    {
        private readonly IBackgroundJobClient _backgroundJobClient;

        public ReportService(IBackgroundJobClient backgroundJobClient)
        {
            _backgroundJobClient = backgroundJobClient;
        }

        public Task<ReportJobResponseDto> EnqueueReportAsync(ReportRequestDto dto, int requestingUserId, CancellationToken ct = default)
        {
            var jobId = Guid.NewGuid().ToString("N");

            _backgroundJobClient.Enqueue<IReportGeneratorJob>(
                job => job.GenerateAsync(jobId, dto, requestingUserId, CancellationToken.None));

            return Task.FromResult(new ReportJobResponseDto(
                jobId,
                "Report generation has been queued. You will be notified via real-time connection when it is ready."));
        }

        public Task<ReportStatusDto?> GetStatusAsync(string jobId, CancellationToken ct = default)
        {
            var reportsDir = Path.Combine("wwwroot", "reports");
            
            // Check for PDF or CSV
            string? fileName = null;
            if (File.Exists(Path.Combine(reportsDir, $"{jobId}.pdf"))) fileName = $"{jobId}.pdf";
            else if (File.Exists(Path.Combine(reportsDir, $"{jobId}.csv"))) fileName = $"{jobId}.csv";

            if (fileName != null)
            {
                return Task.FromResult<ReportStatusDto?>(new ReportStatusDto(
                    jobId, "Completed", $"/api/Reports/download/{jobId}", null));
            }

            return Task.FromResult<ReportStatusDto?>(new ReportStatusDto(
                jobId, "Pending", null, null));
        }

        public Task<bool> CancelJobAsync(string jobId, CancellationToken ct = default)
        {
            // Hangfire's Delete returns true if successfully cancelled/deleted
            var deleted = _backgroundJobClient.Delete(jobId);
            return Task.FromResult(deleted);
        }
    }
}
