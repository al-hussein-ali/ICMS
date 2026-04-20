using ICMS.Application.DTOs.Schedules;
using ICMS.Application.Interfaces.Services;
using ICMS.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ICMS.Api.Controllers
{
    [Route("api/schedules")]
    [ApiController]
    [Authorize(Roles = Roles.Admin + "," + Roles.VaccinationManager)]
    public class SchedulesController(ISchedulesService schedulesService, ILogger<SchedulesController> logger) : ControllerBase
    {

        [HttpGet("individual/{individualId}")]
        public async Task<IActionResult> GetIndividualSchedules(int individualId, CancellationToken ct)
        {
            var scheduleDtos = await schedulesService.GetIndividualSchedulesAsync(individualId, ct);
            return Ok(scheduleDtos);
        }

        [HttpGet("missed")]
        public async Task<IActionResult> GetMissedSchedules(
            [FromQuery(Name = "fromDate")] DateOnly fromDate, 
            [FromQuery(Name = "toDate")] DateOnly? toDate, 
            CancellationToken ct)
        {
            var query = new MissedScheduleQueryDto { FromDate = fromDate, ToDate = toDate };
            
            logger.LogInformation("Querying missed schedules: from {FromDate} to {ToDate}", 
                query.FromDate, query.ToDate ?? DateOnly.FromDateTime(DateTime.UtcNow.AddHours(3)));

            var missedSchedules = await schedulesService.GetMissedSchedulesDetailedAsync(query, ct);
            return Ok(missedSchedules);
        }
    }
}
