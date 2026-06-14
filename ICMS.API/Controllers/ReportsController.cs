using ICMS.Domain.Constants;
using ICMS.Application.DTOs.Reports;
using ICMS.Application.Interfaces.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace ICMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Roles.Admin + "," + Roles.InventoryManager + "," + Roles.VaccinationManager + "," + Roles.ReproductiveHealthManager)]
    [EnableRateLimiting("stricter")]
    public class ReportsController(IReportService reportService, IValidator<ReportRequestDto> validator) : ControllerBase
    {

        /// <summary>
        /// Enqueues a report generation job.
        /// Returns 202 Accepted with a jobId.
        /// Subscribe to /hubs/reports to receive ReportReady notification.
        /// </summary>
        [HttpPost("generate")]
        public async Task<IActionResult> Generate(
            [FromBody] ReportRequestDto dto,
            CancellationToken ct = default)
        {
            var validation = await validator.ValidateAsync(dto, ct);
            if (!validation.IsValid)
                return ValidationProblem(new ValidationProblemDetails(
                    validation.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())));

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out var userId))
                return Unauthorized();

            var result = await reportService.EnqueueReportAsync(dto, userId, ct);
            return Accepted(result);
        }

        /// <summary>
        /// Polling fallback: check report status by jobId.
        /// </summary>
        [HttpGet("{jobId}")]
        public async Task<ActionResult<ReportStatusDto>> GetStatus(string jobId, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(jobId) || !System.Text.RegularExpressions.Regex.IsMatch(jobId, @"^[a-zA-Z0-9\-]+$"))
            {
                return BadRequest("Invalid Job ID.");
            }

            var status = await reportService.GetStatusAsync(jobId, ct);
            if (status == null) return NotFound();
            return Ok(status);
        }

        /// <summary>
        /// Download the generated report (PDF or CSV).
        /// </summary>
        [HttpGet("download/{jobId}")]
        public IActionResult Download(string jobId)
        {
            if (string.IsNullOrWhiteSpace(jobId) || !System.Text.RegularExpressions.Regex.IsMatch(jobId, @"^[a-zA-Z0-9\-]+$"))
            {
                return BadRequest("Invalid Job ID.");
            }

            var reportsDir = Path.GetFullPath(Path.Combine("wwwroot", "reports"));
            var pdfPath = Path.GetFullPath(Path.Combine(reportsDir, $"{jobId}.pdf"));
            var csvPath = Path.GetFullPath(Path.Combine(reportsDir, $"{jobId}.csv"));

            if (!pdfPath.StartsWith(reportsDir, StringComparison.OrdinalIgnoreCase) ||
                !csvPath.StartsWith(reportsDir, StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Access denied.");
            }

            if (System.IO.File.Exists(pdfPath))
            {
                var fileBytes = System.IO.File.ReadAllBytes(pdfPath);
                return File(fileBytes, "application/pdf", $"icms-report-{jobId}.pdf");
            }
            
            if (System.IO.File.Exists(csvPath))
            {
                var fileBytes = System.IO.File.ReadAllBytes(csvPath);
                return File(fileBytes, "text/csv", $"icms-report-{jobId}.csv");
            }

            return NotFound("Report not found. It may still be generating.");
        }

        /// <summary>
        /// Cancel a pending or running report job.
        /// </summary>
        [HttpDelete("{jobId}")]
        public async Task<IActionResult> Cancel(string jobId, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(jobId) || !System.Text.RegularExpressions.Regex.IsMatch(jobId, @"^[a-zA-Z0-9\-]+$"))
            {
                return BadRequest("Invalid Job ID.");
            }

            var cancelled = await reportService.CancelJobAsync(jobId, ct);
            if (!cancelled) return NotFound("Job not found or already completed.");
            return NoContent();
        }
    }
}
