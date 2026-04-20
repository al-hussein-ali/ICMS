using ICMS.Application.DTOs.Reports;

namespace ICMS.Application.Interfaces.Reports
{
    /// <summary>
    /// The Hangfire job interface. Infrastructure implements this so the Application
    /// layer can enqueue it without a hard reference to Infrastructure.
    /// </summary>
    public interface IReportGeneratorJob
    {
        Task GenerateAsync(string jobId, ReportRequestDto request, int userId, CancellationToken ct);
    }
}
