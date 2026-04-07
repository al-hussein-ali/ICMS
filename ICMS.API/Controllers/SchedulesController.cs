using ICMS.Application.DTOs.Schedules;
using ICMS.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace ICMS.Api.Controllers
{
    [Route("api/schedules")]
    [ApiController]
    public class SchedulesController : ControllerBase
    {
        private readonly ISchedulesService _schedulesService;
        private readonly ILogger<SchedulesController> _logger;

        public SchedulesController(ISchedulesService schedulesService, ILogger<SchedulesController> logger)
        {
            _schedulesService = schedulesService;
            _logger = logger;
        }

        [HttpGet("individual/{individualId}")]
        public async Task<IActionResult> GetIndividualSchedules(int individualId, CancellationToken ct)
        {
            var scheduleDtos = await _schedulesService.GetIndividualSchedulesAsync(individualId, ct);
            return Ok(scheduleDtos);
        }

        [HttpGet("missed")]
        public async Task<IActionResult> GetMissedSchedules(
            [FromQuery(Name = "fromDate")] DateOnly fromDate, 
            [FromQuery(Name = "toDate")] DateOnly? toDate, 
            CancellationToken ct)
        {
            var query = new MissedScheduleQueryDto { FromDate = fromDate, ToDate = toDate };
            
            _logger.LogInformation("Querying missed schedules: from {FromDate} to {ToDate}", 
                query.FromDate, query.ToDate ?? DateOnly.FromDateTime(DateTime.UtcNow.AddHours(3)));

            var missedSchedules = await _schedulesService.GetMissedSchedulesDetailedAsync(query, ct);
            return Ok(missedSchedules);
        }
    }
}
