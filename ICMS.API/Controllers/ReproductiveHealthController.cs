using ICMS.Application.DTOs.Maternal;
using ICMS.Application.DTOs.Pagination;
using ICMS.Application.Interfaces.Services;
using ICMS.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ICMS.API.Extensions;


namespace ICMS.API.Controllers
{
    [Route("api/reproductive")]
    [ApiController]
    [Authorize(Roles = Roles.Admin + "," + Roles.ReproductiveHealthManager)]
    public class ReproductiveHealthController(IReproductiveHealthService reproductiveHealthService) : ControllerBase
    {

        [HttpPost("new-pregnancy")]
        public async Task<IActionResult> StartPregnancyAsync([FromBody] StartPregnancyDto request)
        {
            var userId = ClaimsPrincipalExtensions.GetUserId(User);
            await reproductiveHealthService.StartPregnancyAsync(request, userId);
            return Created(string.Empty, new { message = "Pregnancy started successfully." });
        }

        [HttpPost("{pregnancyId}/visits/create")]
        public async Task<IActionResult> AddAncVisitAsync([FromRoute] int pregnancyId, [FromBody] AddAncVisitDto request)
        {
            var userId = ClaimsPrincipalExtensions.GetUserId(User);
            await reproductiveHealthService.AddAncVisitAsync(pregnancyId, request, userId);
            return Created(string.Empty, new { message = "ANC visit recorded successfully." });
        }

        [HttpPut("{pregnancyId}/conclude")]
        public async Task<IActionResult> ConcludePregnancyAsync([FromRoute] int pregnancyId, [FromBody] ConcludePregnancyDto request)
        {
            var userId = ClaimsPrincipalExtensions.GetUserId(User);
            await reproductiveHealthService.ConcludePregnancyAsync(pregnancyId, request, userId);
            return Ok(new { message = "Pregnancy concluded successfully." });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPregnantWomen([FromQuery] PaginationParams paginationParams, CancellationToken ct)
        {
            var result = await reproductiveHealthService.GetAllPregnantWomenAsync(paginationParams, ct);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPregnantWomanById(int id, CancellationToken ct)
        {
            var result = await reproductiveHealthService.GetPregnantWomanByIdAsync(id, ct);
            return Ok(result);
        }

        [HttpGet("{id}/details")]
        public async Task<IActionResult> GetPregnantWomanDetails(int id, CancellationToken ct)
        {
            var result = await reproductiveHealthService.GetPregnantWomanDetailsAsync(id, ct);
            return Ok(result);
        }

        [HttpGet("{id}/pregnancies")]
        public async Task<IActionResult> GetPregnancyHistory([FromRoute] int id, CancellationToken ct)
        {
            var result = await reproductiveHealthService.GetPregnancyHistoryAsync(id, ct);
            return Ok(result);
        }

        [HttpGet("pregnancies/{id}")]
        public async Task<IActionResult> GetPregnancyById([FromRoute] int id, CancellationToken ct)
        {
            var result = await reproductiveHealthService.GetPregnancyByIdAsync(id, ct);
            return Ok(result);
        }

        [HttpGet("{pregnancyId}/visits")]
        public async Task<IActionResult> GetVisits([FromRoute] int pregnancyId, CancellationToken ct)
        {
            var result = await reproductiveHealthService.GetVisitsAsync(pregnancyId, ct);
            return Ok(result);
        }

        [HttpGet("visits/{id}")]
        public async Task<IActionResult> GetVisitById([FromRoute] int id, CancellationToken ct)
        {
            var result = await reproductiveHealthService.GetVisitByIdAsync(id, ct);
            return Ok(result);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreatePregnantWoman([FromBody] PregnantWomanCreateDto request, CancellationToken ct)
        {
            var result = await reproductiveHealthService.CreatePregnantWomanAsync(request, ct);
            return CreatedAtAction(nameof(GetPregnantWomanById), new { id = result.Id }, result);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdatePregnantWoman(int id, [FromBody] PregnantWomanCreateDto request, CancellationToken ct)
        {
            await reproductiveHealthService.UpdatePregnantWomanAsync(id, request, ct);
            return NoContent();
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeletePregnantWoman(int id, CancellationToken ct)
        {
            await reproductiveHealthService.DeletePregnantWomanAsync(id, ct);
            return NoContent();
        }

        [HttpPost("{id}/account")]
        public async Task<IActionResult> GenerateAccount([FromRoute] int id)
        {
            var result = await reproductiveHealthService.GenerateAccountAsync(id);
            return Ok(result);
        }

        [HttpPut("pregnancies/{id}")]
        public async Task<IActionResult> UpdatePregnancy(int id, [FromBody] UpdatePregnancyDto request, CancellationToken ct)
        {
            await reproductiveHealthService.UpdatePregnancyAsync(id, request, ct);
            return NoContent();
        }

        [HttpDelete("pregnancies/{id}")]
        public async Task<IActionResult> DeletePregnancy(int id, CancellationToken ct)
        {
            await reproductiveHealthService.DeletePregnancyAsync(id, ct);
            return NoContent();
        }

        [HttpPut("visits/{id}")]
        public async Task<IActionResult> UpdateVisit(int id, [FromBody] AddAncVisitDto request, CancellationToken ct)
        {
            await reproductiveHealthService.UpdateVisitAsync(id, request, ct);
            return NoContent();
        }

        [HttpDelete("visits/{id}")]
        public async Task<IActionResult> DeleteVisit(int id, CancellationToken ct)
        {
            await reproductiveHealthService.DeleteVisitAsync(id, ct);
            return NoContent();
        }
    }
}
