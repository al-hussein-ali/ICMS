using ICMS.Domain.ValueObjects;
using ICMS.Application.DTOs.HealthAdvisory;
using ICMS.Application.Interfaces.Services;
using ICMS.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ICMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Roles.Admin + "," + Roles.VaccinationManager + "," + Roles.ReproductiveHealthManager)]
    public class HealthAdvisoriesController(IHealthAdvisoryService healthAdvisoryService) : ControllerBase
    {

        [HttpGet]
        public async Task<ActionResult<PagedResult<HealthAdvisoryReadDto>>> GetPaged(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken ct = default)
        {
            var result = await healthAdvisoryService.GetPagedAsync(pageNumber, pageSize, ct);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<HealthAdvisoryDetailsDto>> GetById(int id, CancellationToken ct = default)
        {
            var result = await healthAdvisoryService.GetByIdAsync(id, ct);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("{id}/image")]
        public async Task<IActionResult> GetImage(int id, CancellationToken ct = default)
        {
            var (content, contentType, fileName) = await healthAdvisoryService.GetImageAsync(id, ct);
            return File(content, contentType, fileName);
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

            var result = await healthAdvisoryService.CreateAndSendNowAsync(dto, userId, ct);
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

            var result = await healthAdvisoryService.CreateAsync(dto, userId, ct);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<HealthAdvisoryDetailsDto>> Update(
            int id,
            [FromBody] HealthAdvisoryCreateDto dto,
            CancellationToken ct = default)
        {
            var result = await healthAdvisoryService.UpdateAsync(id, dto, ct);
            return Ok(result);
        }

        [HttpPut("{id}/send-now")]
        public async Task<ActionResult<HealthAdvisoryDetailsDto>> UpdateAndSendNow(int id, [FromBody] HealthAdvisoryCreateDto dto, CancellationToken ct)
        {
            var result = await healthAdvisoryService.UpdateAndSendNowAsync(id, dto, ct);
            return Ok(result);
        }

        [HttpPost("{id}/resend")]
        public async Task<ActionResult<HealthAdvisoryDetailsDto>> Resend(int id, CancellationToken ct)
        {
            var result = await healthAdvisoryService.ResendAsync(id, ct);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
        {
            await healthAdvisoryService.DeleteAsync(id, ct);
            return NoContent();
        }
    }
}
