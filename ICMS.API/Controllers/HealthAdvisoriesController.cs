using ICMS.Domain.ValueObjects;
using ICMS.Application.DTOs.HealthAdvisory;
using ICMS.Application.Interfaces.Services;
using ICMS.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace ICMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class HealthAdvisoriesController : ControllerBase
    {
        private readonly IHealthAdvisoryService _healthAdvisoryService;

        public HealthAdvisoriesController(IHealthAdvisoryService healthAdvisoryService)
        {
            _healthAdvisoryService = healthAdvisoryService;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<HealthAdvisoryReadDto>>> GetPaged(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken ct = default)
        {
            var result = await _healthAdvisoryService.GetPagedAsync(pageNumber, pageSize, ct);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<HealthAdvisoryDetailsDto>> GetById(int id, CancellationToken ct = default)
        {
            var result = await _healthAdvisoryService.GetByIdAsync(id, ct);
            return Ok(result);
        }

        [HttpPost("send-now")]
        public async Task<ActionResult<HealthAdvisoryDetailsDto>> SendNow(
            [FromBody] HealthAdvisoryCreateDto dto,
            CancellationToken ct = default)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out var userId))
            {
                return Unauthorized();
            }

            var result = await _healthAdvisoryService.CreateAndSendNowAsync(dto, userId, ct);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPost("schedule")]
        public async Task<ActionResult<HealthAdvisoryDetailsDto>> Schedule(
            [FromBody] HealthAdvisoryCreateDto dto,
            CancellationToken ct = default)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out var userId))
            {
                return Unauthorized();
            }

            var result = await _healthAdvisoryService.CreateAsync(dto, userId, ct);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
        {
            await _healthAdvisoryService.DeleteAsync(id, ct);
            return NoContent();
        }
    }
}
