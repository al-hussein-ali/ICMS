using ICMS.Application.DTOs.Schedules;
using ICMS.Application.Interfaces.Services;
using ICMS.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ICMS.Api.Controllers
{
    [Route("api/schedules")]
    [ApiController]
    [Authorize]
    public class SchedulesController(ISchedulesService schedulesService, ILogger<SchedulesController> logger) : ControllerBase
    {

        [HttpGet("individual/{individualId}")]
        [Authorize(Roles = Roles.Admin + "," + Roles.VaccinationManager + "," + Roles.VaccinatedIndividual)]
        public async Task<IActionResult> GetIndividualSchedules(int individualId, CancellationToken ct)
        {
            var scheduleDtos = await schedulesService.GetIndividualSchedulesAsync(individualId, ct);
            return Ok(scheduleDtos);
        }

        [HttpGet("missed")]
        [Authorize(Roles = Roles.Admin + "," + Roles.VaccinationManager + "," + Roles.FieldVisitWorker)]
        public async Task<IActionResult> GetMissedSchedules(
            [FromQuery(Name = "fromDate")] DateOnly fromDate, 
            [FromQuery(Name = "toDate")] DateOnly? toDate, 
            [FromQuery(Name = "subNeighborhoodId")] int? subNeighborhoodId,
            CancellationToken ct)
        {
            var query = new MissedScheduleQueryDto { FromDate = fromDate, ToDate = toDate, SubNeighborhoodId = subNeighborhoodId };
            
            logger.LogInformation("Querying missed schedules: from {FromDate} to {ToDate} for SubNeighborhood {SubNeighborhoodId}", 
                query.FromDate, query.ToDate ?? DateOnly.FromDateTime(DateTime.UtcNow.AddHours(3)), query.SubNeighborhoodId);

            var missedSchedules = await schedulesService.GetMissedSchedulesDetailedAsync(query, ct);
            return Ok(missedSchedules);
        }
    }
}
