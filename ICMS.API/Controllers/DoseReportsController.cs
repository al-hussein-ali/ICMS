using ICMS.API.Extensions;
using ICMS.Application.DTOs.DoseReport;
using ICMS.Application.DTOs.Pagination;
using ICMS.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ICMS.API.Controllers
{
    [ApiController]
    [Route("api/dose-reports")]
    [Authorize]
    public class DoseReportsController(IDoseReportService doseReportService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] PaginationParams paginationParams)
        {
            var result = await doseReportService.GetAllAsync(paginationParams);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddAsync([FromBody] DoseReportCreateDto dto)
        {
            var userId = User.GetUserId();
            var result = await doseReportService.AddAsync(dto, userId);
            return Ok(result);
        }
    }
}
