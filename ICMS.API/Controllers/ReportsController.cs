using FluentValidation;
using ICMS.Application.DTOs.Reports;
using ICMS.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.RateLimiting;

namespace ICMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    [EnableRateLimiting("stricter")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly IValidator<ReportRequestDto> _validator;

        public ReportsController(IReportService reportService, IValidator<ReportRequestDto> validator)
        {
            _reportService = reportService;
            _validator = validator;
        }

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
            var validation = await _validator.ValidateAsync(dto, ct);
            if (!validation.IsValid)
                return ValidationProblem(new ValidationProblemDetails(
                    validation.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())));

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out var userId))
                return Unauthorized();

            var result = await _reportService.EnqueueReportAsync(dto, userId, ct);
            return Accepted(result);
        }

        /// <summary>
        /// Polling fallback: check report status by jobId.
        /// Returns status + download URL if ready.
        /// </summary>
        [HttpGet("{jobId}")]
        public async Task<ActionResult<ReportStatusDto>> GetStatus(string jobId, CancellationToken ct = default)
        {
            var status = await _reportService.GetStatusAsync(jobId, ct);
            if (status == null) return NotFound();
            return Ok(status);
        }

        /// <summary>
        /// Download the generated PDF report.
        /// </summary>
        [HttpGet("download/{jobId}")]
        public IActionResult Download(string jobId)
        {
            var filePath = Path.Combine("wwwroot", "reports", $"{jobId}.pdf");
            if (!System.IO.File.Exists(filePath))
                return NotFound("Report not found. It may still be generating.");

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "application/pdf", $"icms-report-{jobId}.pdf");
        }
    }
}
