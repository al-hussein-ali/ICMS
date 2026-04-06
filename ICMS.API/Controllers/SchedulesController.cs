using ICMS.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace ICMS.Api.Controllers
{
    [Route("api/schedules")]
    [ApiController]
    public class SchedulesController : ControllerBase
    {
        private readonly ISchedulesService _schedulesService;

        public SchedulesController(ISchedulesService schedulesService)
        {
            _schedulesService = schedulesService;
        }

        [HttpGet("individual/{individualId}")]
        public async Task<IActionResult> GetIndividualSchedules(int individualId, CancellationToken ct)
        {
            var scheduleDtos = await _schedulesService.GetIndividualSchedulesAsync(individualId, ct);
            return Ok(scheduleDtos);
        }
    }
}
