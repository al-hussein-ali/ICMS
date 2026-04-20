using ICMS.Application.DTOs.Reports;
using System.Threading;
using System.Threading.Tasks;

namespace ICMS.Application.Interfaces.Services
{
    public interface IReportService
    {
        /// <summary>
        /// Enqueues a report generation job and returns a jobId.
        /// The caller should subscribe to SignalR hub to receive completion event.
        /// </summary>
        Task<ReportJobResponseDto> EnqueueReportAsync(ReportRequestDto dto, int requestingUserId, CancellationToken ct = default);

        /// <summary>Returns status and download URL if available.</summary>
        Task<ReportStatusDto?> GetStatusAsync(string jobId, CancellationToken ct = default);
    }
}
