using ICMS.Application.DTOs.Maternal;
using ICMS.Application.DTOs.Pagination;
using ICMS.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace ICMS.API.Controllers
{
    [Route("api/reproductive")]
    [ApiController]
    public class ReproductiveHealthController : ControllerBase
    {
        private readonly IReproductiveHealthService _reproductiveHealthService;

        public ReproductiveHealthController(IReproductiveHealthService reproductiveHealthService)
        {
            _reproductiveHealthService = reproductiveHealthService;
        }

        [HttpPost("new-pregnancy")]
        public async Task<IActionResult> StartPregnancyAsync([FromBody] StartPregnancyDto request)
        {
            await _reproductiveHealthService.StartPregnancyAsync(request);
            return Created(string.Empty, new { message = "Pregnancy started successfully." });
        }

        [HttpPost("{pregnancyId}/visits/create")]
        public async Task<IActionResult> AddAncVisitAsync([FromRoute] int pregnancyId, [FromBody] AddAncVisitDto request)
        {
            await _reproductiveHealthService.AddAncVisitAsync(pregnancyId, request);
            return Created(string.Empty, new { message = "ANC visit recorded successfully." });
        }

        [HttpPut("{pregnancyId}/conclude")]
        public async Task<IActionResult> ConcludePregnancyAsync([FromRoute] int pregnancyId, [FromBody] ConcludePregnancyDto request)
        {
            await _reproductiveHealthService.ConcludePregnancyAsync(pregnancyId, request);
            return Ok(new { message = "Pregnancy concluded successfully." });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPregnantWomen([FromQuery] PaginationParams paginationParams, CancellationToken ct)
        {
            var result = await _reproductiveHealthService.GetAllPregnantWomenAsync(paginationParams, ct);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPregnantWomanById(int id, CancellationToken ct)
        {
            var result = await _reproductiveHealthService.GetPregnantWomanByIdAsync(id, ct);
            return Ok(result);
        }

        [HttpGet("{id}/details")]
        public async Task<IActionResult> GetPregnantWomanDetails(int id, CancellationToken ct)
        {
            var result = await _reproductiveHealthService.GetPregnantWomanDetailsAsync(id, ct);
            return Ok(result);
        }

        [HttpGet("/{id}/pregnancies")]
        public async Task<IActionResult> GetPregnancyHistory([FromRoute] int id, CancellationToken ct)
        {
            var result = await _reproductiveHealthService.GetPregnancyHistoryAsync(id, ct);
            return Ok(result);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreatePregnantWoman([FromBody] PregnantWomanCreateDto request, CancellationToken ct)
        {
            var result = await _reproductiveHealthService.CreatePregnantWomanAsync(request, ct);
            return CreatedAtAction(nameof(GetPregnantWomanById), new { id = result.Id }, result);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdatePregnantWoman(int id, [FromBody] PregnantWomanCreateDto request, CancellationToken ct)
        {
            await _reproductiveHealthService.UpdatePregnantWomanAsync(id, request, ct);
            return NoContent();
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeletePregnantWoman(int id, CancellationToken ct)
        {
            await _reproductiveHealthService.DeletePregnantWomanAsync(id, ct);
            return NoContent();
        }

        [HttpPost("{id}/account")]
        public async Task<IActionResult> GenerateAccount([FromRoute] int id)
        {
            var result = await _reproductiveHealthService.GenerateAccountAsync(id);
            return Ok(result);
        }
    }
}
