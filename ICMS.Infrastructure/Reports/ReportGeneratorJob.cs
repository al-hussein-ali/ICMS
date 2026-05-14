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
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ICMS.Infrastructure.Reports
{
    /// <summary>
    /// Hangfire background job that:
    /// 1. Resolves the correct data fetcher and template renderer by ReportType
    /// 2. Fetches report data from repositories
    /// 3. Renders HTML and converts to PDF OR converts data directly to CSV
    /// 4. Saves file to wwwroot/reports/{jobId}.{ext}
    /// 5. Notifies the requesting user via SignalR
    /// </summary>
    public class ReportGeneratorJob : IReportGeneratorJob
    {
        private readonly IEnumerable<IReportDataFetcher> _fetchers;
        private readonly IEnumerable<IReportTemplateRenderer> _renderers;
        private readonly INotificationService _notificationService;
        private readonly ILogger<ReportGeneratorJob> _logger;

        public ReportGeneratorJob(
            IEnumerable<IReportDataFetcher> fetchers,
            IEnumerable<IReportTemplateRenderer> renderers,
            INotificationService notificationService,
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

                // 2. Fetch data
                var reportData = await fetcher.FetchAsync(request.StartDate, request.EndDate, request.Lang, ct);

                // 3. Render based on format
                byte[] fileBytes;
                string extension;

                if (request.Format == ReportFormat.Csv)
                {
                    fileBytes = ConvertDataToCsv(reportData);
                    extension = "csv";
                }
                else
                {
                    // 3b. Render HTML then PDF
                    var renderer = _renderers.FirstOrDefault(r => r.ReportType == request.ReportType)
                        ?? throw new InvalidOperationException($"No template renderer registered for report type: {request.ReportType}");
                    
                    var html = renderer.Render(reportData);
                    fileBytes = await ConvertHtmlToPdfAsync(html);
                    extension = "pdf";
                }

                // 4. Save to disk
                var reportsDir = Path.Combine("wwwroot", "reports");
                Directory.CreateDirectory(reportsDir);
                var filePath = Path.Combine(reportsDir, $"{jobId}.{extension}");
                await File.WriteAllBytesAsync(filePath, fileBytes, ct);

                _logger.LogInformation("Report generated successfully. JobId={JobId}, Format={Format}, File={File}", 
                    jobId, request.Format, filePath);

                // 5. Create Persistent Notification
                var downloadUrl = $"/api/Reports/download/{jobId}";
                var title = "common.notifications.report_ready_title";
                var message = $"{{\"key\":\"common.notifications.report_ready_msg\",\"params\":{{\"reportType\":\"{request.ReportType}\",\"month\":\"{DateTime.UtcNow.Month}\",\"year\":\"{DateTime.UtcNow.Year}\"}}}}";

                await _notificationService.CreateNotificationAsync(userId, title, message, downloadUrl, jobId, ct);
                
                // Also trigger specific SignalR event for immediate toast and UI state updates
                await _notificationService.NotifyReportReadyAsync(userId.ToString(), jobId, downloadUrl, ct);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Report generation cancelled by user. JobId={JobId}", jobId);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Report generation failed. JobId={JobId}", jobId);
                
                var title = "common.notifications.report_failed_title";
                var message = $"{{\"key\":\"common.notifications.report_failed_msg\",\"params\":{{\"reportType\":\"{request.ReportType}\"}}}}";

                await _notificationService.CreateNotificationAsync(userId, title, message, null, null, ct);
                
                // Also trigger specific SignalR failure event
                await _notificationService.NotifyReportFailedAsync(userId.ToString(), jobId, ex.Message, ct);
                
                throw; // Re-throw so Hangfire marks job as failed
            }
        }

        private async Task<byte[]> ConvertHtmlToPdfAsync(string html)
        {
            _logger.LogInformation("Starting Playwright PDF conversion...");
            using var playwright = await Playwright.CreateAsync();
            
            _logger.LogInformation("Launching browser...");
            await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true,
                Args = new[] { "--no-sandbox", "--disable-setuid-sandbox" }
            });

            try
            {
                _logger.LogInformation("Creating new page...");
                var page = await browser.NewPageAsync();

                _logger.LogInformation("Setting content (NetworkIdle)...");
                await page.SetContentAsync(html, new PageSetContentOptions
                {
                    WaitUntil = WaitUntilState.NetworkIdle,
                    Timeout = 60000 // 60 seconds
                });

                _logger.LogInformation("Generating PDF bytes...");
                var pdf = await page.PdfAsync(new PagePdfOptions
                {
                    Format = "A4",
                    PrintBackground = true,
                    Margin = new Margin { Top = "16px", Bottom = "16px", Left = "16px", Right = "16px" }
                });

                _logger.LogInformation("PDF conversion successful.");
                return pdf;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during Playwright PDF conversion");
                throw;
            }
            finally
            {
                _logger.LogInformation("Closing browser...");
                await browser.CloseAsync();
            }
        }

        private byte[] ConvertDataToCsv(ReportData data)
        {
            _logger.LogInformation("Starting CSV conversion...");
            var sb = new StringBuilder();

            // 1. Headers
            sb.AppendLine(string.Join(",", data.ColumnHeaders.Select(EscapeCsv)));

            // 2. Rows
            foreach (var row in data.Rows)
            {
                var values = data.ColumnHeaders.Select(h => EscapeCsv(row.Columns.GetValueOrDefault(h) ?? ""));
                sb.AppendLine(string.Join(",", values));
            }

            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        private string EscapeCsv(string? value)
        {
            if (string.IsNullOrEmpty(value)) return "";
            if (value.Contains(',') || value.Contains('"') || value.Contains('\n') || value.Contains('\r'))
            {
                return "\"" + value.Replace("\"", "\"\"") + "\"";
            }
            return value;
        }

    }
}
