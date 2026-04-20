using ICMS.Application.DTOs.Reports;
using ICMS.Application.Enums;
using ICMS.Application.Interfaces.Reports;
using ICMS.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ICMS.Infrastructure.Reports
{
    /// <summary>
    /// Hangfire background job that:
    /// 1. Resolves the correct data fetcher and template renderer by ReportType
    /// 2. Fetches report data from repositories
    /// 3. Renders HTML and converts to PDF via Playwright headless Chromium
    /// 4. Saves PDF to wwwroot/reports/{jobId}.pdf
    /// 5. Notifies the requesting user via SignalR
    /// </summary>
    public class ReportGeneratorJob : IReportGeneratorJob
    {
        private readonly IEnumerable<IReportDataFetcher> _fetchers;
        private readonly IEnumerable<IReportTemplateRenderer> _renderers;
        private readonly IReportNotificationService _notificationService;
        private readonly ILogger<ReportGeneratorJob> _logger;

        public ReportGeneratorJob(
            IEnumerable<IReportDataFetcher> fetchers,
            IEnumerable<IReportTemplateRenderer> renderers,
            IReportNotificationService notificationService,
            ILogger<ReportGeneratorJob> logger)
        {
            _fetchers = fetchers;
            _renderers = renderers;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task GenerateAsync(string jobId, ReportRequestDto request, int userId, CancellationToken ct)
        {
            _logger.LogInformation("Starting report generation. JobId={JobId}, Type={Type}, User={UserId}",
                jobId, request.ReportType, userId);

            var userIdStr = userId.ToString();

            try
            {
                // 1. Resolve strategy implementations
                var fetcher = _fetchers.FirstOrDefault(f => f.ReportType == request.ReportType)
                    ?? throw new InvalidOperationException($"No data fetcher registered for report type: {request.ReportType}");

                var renderer = _renderers.FirstOrDefault(r => r.ReportType == request.ReportType)
                    ?? throw new InvalidOperationException($"No template renderer registered for report type: {request.ReportType}");

                // 2. Fetch data
                var reportData = await fetcher.FetchAsync(request.StartDate, request.EndDate, ct);

                // 3. Render HTML
                var html = renderer.Render(reportData);

                // 4. Convert HTML → PDF via Playwright
                var pdfBytes = await ConvertHtmlToPdfAsync(html);

                // 5. Save to disk
                var reportsDir = Path.Combine("wwwroot", "reports");
                Directory.CreateDirectory(reportsDir);
                var filePath = Path.Combine(reportsDir, $"{jobId}.pdf");
                await File.WriteAllBytesAsync(filePath, pdfBytes, ct);

                _logger.LogInformation("Report generated successfully. JobId={JobId}, File={File}", jobId, filePath);

                // 6. Notify user via SignalR
                var downloadUrl = $"/api/Reports/download/{jobId}";
                await _notificationService.NotifyReportReadyAsync(userIdStr, jobId, downloadUrl, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Report generation failed. JobId={JobId}", jobId);
                await _notificationService.NotifyReportFailedAsync(userIdStr, jobId, ex.Message, ct);
                throw; // Re-throw so Hangfire marks job as failed
            }
        }

        private static async Task<byte[]> ConvertHtmlToPdfAsync(string html)
        {
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true
            });

            var page = await browser.NewPageAsync();

            // Set content directly (avoids needing a running web server)
            await page.SetContentAsync(html, new PageSetContentOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle
            });

            var pdf = await page.PdfAsync(new PagePdfOptions
            {
                Format = "A4",
                PrintBackground = true,
                Margin = new Margin { Top = "16px", Bottom = "16px", Left = "16px", Right = "16px" }
            });

            return pdf;
        }
    }
}
