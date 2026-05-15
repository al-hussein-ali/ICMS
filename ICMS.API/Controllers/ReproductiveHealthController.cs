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
    [Authorize]
    public class ReproductiveHealthController(IReproductiveHealthService reproductiveHealthService) : ControllerBase
    {
        [HttpPost("new-pregnancy")]
        [Authorize(Roles = Roles.Admin + "," + Roles.ReproductiveHealthManager)]
        public async Task<IActionResult> StartPregnancyAsync([FromBody] StartPregnancyDto request)
        {
            var userId = ClaimsPrincipalExtensions.GetUserId(User);
            await reproductiveHealthService.StartPregnancyAsync(request, userId);
            return Created(string.Empty, new { message = "Pregnancy started successfully." });
        }

        [HttpPost("{pregnancyId}/visits/create")]
        [Authorize(Roles = Roles.Admin + "," + Roles.ReproductiveHealthManager)]
        public async Task<IActionResult> AddAncVisitAsync([FromRoute] int pregnancyId,
            [FromBody] AddAncVisitDto request)
        {
            var userId = ClaimsPrincipalExtensions.GetUserId(User);
            await reproductiveHealthService.AddAncVisitAsync(pregnancyId, request, userId);
            return Created(string.Empty, new { message = "ANC visit recorded successfully." });
        }

        [HttpPut("{pregnancyId}/conclude")]
        [Authorize(Roles = Roles.Admin + "," + Roles.ReproductiveHealthManager)]
        public async Task<IActionResult> ConcludePregnancyAsync([FromRoute] int pregnancyId,
            [FromBody] ConcludePregnancyDto request)
        {
            var userId = ClaimsPrincipalExtensions.GetUserId(User);
            await reproductiveHealthService.ConcludePregnancyAsync(pregnancyId, request, userId);
            return Ok(new { message = "Pregnancy concluded successfully." });
        }

        [HttpGet]
        [Authorize(Roles = Roles.Admin + "," + Roles.ReproductiveHealthManager)]
        public async Task<IActionResult> GetAllPregnantWomen([FromQuery] PaginationParams paginationParams,
            CancellationToken ct)
        {
            var result = await reproductiveHealthService.GetAllPregnantWomenAsync(paginationParams, ct);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = Roles.Admin + "," + Roles.ReproductiveHealthManager + "," + Roles.PregnantWoman)]
        public async Task<IActionResult> GetPregnantWomanById(int id, CancellationToken ct)
        {
            var result = await reproductiveHealthService.GetPregnantWomanByIdAsync(id, ct);
            return Ok(result);
        }

        [HttpGet("{id}/details")]
        [Authorize(Roles = Roles.Admin + "," + Roles.ReproductiveHealthManager + "," + Roles.PregnantWoman)]
        public async Task<IActionResult> GetPregnantWomanDetails(int id, CancellationToken ct)
        {
            var result = await reproductiveHealthService.GetPregnantWomanDetailsAsync(id, ct);
            return Ok(result);
        }

        [HttpGet("{id}/pregnancies")]
        [Authorize(Roles = Roles.Admin + "," + Roles.ReproductiveHealthManager + "," + Roles.PregnantWoman)]
        public async Task<IActionResult> GetPregnancyHistory([FromRoute] int id, CancellationToken ct)
        {
            var result = await reproductiveHealthService.GetPregnancyHistoryAsync(id, ct);
            return Ok(result);
        }

        [HttpGet("pregnancies/{id}")]
        [Authorize(Roles = Roles.Admin + "," + Roles.ReproductiveHealthManager + "," + Roles.PregnantWoman)]
        public async Task<IActionResult> GetPregnancyById([FromRoute] int id, CancellationToken ct)
        {
            var result = await reproductiveHealthService.GetPregnancyByIdAsync(id, ct);
            return Ok(result);
        }

        [HttpGet("{pregnancyId}/visits")]
        [Authorize(Roles = Roles.Admin + "," + Roles.ReproductiveHealthManager + "," + Roles.PregnantWoman)]
        public async Task<IActionResult> GetVisits([FromRoute] int pregnancyId, CancellationToken ct)
        {
            var result = await reproductiveHealthService.GetVisitsAsync(pregnancyId, ct);
            return Ok(result);
        }

        [HttpGet("visits/{id}")]
        [Authorize(Roles = Roles.Admin + "," + Roles.ReproductiveHealthManager + "," + Roles.PregnantWoman)]
        public async Task<IActionResult> GetVisitById([FromRoute] int id, CancellationToken ct)
        {
            var result = await reproductiveHealthService.GetVisitByIdAsync(id, ct);
            return Ok(result);
        }

        [HttpPost("create")]
        [Authorize(Roles = Roles.Admin + "," + Roles.ReproductiveHealthManager)]
        public async Task<IActionResult> CreatePregnantWoman([FromBody] PregnantWomanCreateDto request,
            CancellationToken ct)
        {
            var result = await reproductiveHealthService.CreatePregnantWomanAsync(request, ct);
            return CreatedAtAction(nameof(GetPregnantWomanById), new { id = result.Id }, result);
        }

        [HttpPut("update/{id}")]
        [Authorize(Roles = Roles.Admin + "," + Roles.ReproductiveHealthManager)]
        public async Task<IActionResult> UpdatePregnantWoman(int id, [FromBody] PregnantWomanCreateDto request,
            CancellationToken ct)
        {
            await reproductiveHealthService.UpdatePregnantWomanAsync(id, request, ct);
            return NoContent();
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = Roles.Admin + "," + Roles.ReproductiveHealthManager)]
        public async Task<IActionResult> DeletePregnantWoman(int id, CancellationToken ct)
        {
            await reproductiveHealthService.DeletePregnantWomanAsync(id, ct);
            return NoContent();
        }

        [HttpPost("{id}/account")]
        [Authorize(Roles = Roles.Admin + "," + Roles.ReproductiveHealthManager)]
        public async Task<IActionResult> GenerateAccount([FromRoute] int id)
        {
            var result = await reproductiveHealthService.GenerateAccountAsync(id);
            return Ok(result);
        }

        [HttpGet("my-id")]
        [Authorize(Roles = Roles.PregnantWoman)]
        public async Task<IActionResult> GetMyWomanId()
        {
            var userId = ClaimsPrincipalExtensions.GetUserId(User);
            var result = await reproductiveHealthService.GetWomanIdByUserIdAsync(userId);
            return Ok(new { id = result });
        }

        [HttpPut("pregnancies/{id}")]
        [Authorize(Roles = Roles.Admin + "," + Roles.ReproductiveHealthManager)]
        public async Task<IActionResult> UpdatePregnancy(int id, [FromBody] UpdatePregnancyDto request,
            CancellationToken ct)
        {
            await reproductiveHealthService.UpdatePregnancyAsync(id, request, ct);
            return NoContent();
        }

        [HttpDelete("pregnancies/{id}")]
        [Authorize(Roles = Roles.Admin + "," + Roles.ReproductiveHealthManager)]
        public async Task<IActionResult> DeletePregnancy(int id, CancellationToken ct)
        {
            await reproductiveHealthService.DeletePregnancyAsync(id, ct);
            return NoContent();
        }

        [HttpPut("visits/{id}")]
        [Authorize(Roles = Roles.Admin + "," + Roles.ReproductiveHealthManager)]
        public async Task<IActionResult> UpdateVisit(int id, [FromBody] AddAncVisitDto request, CancellationToken ct)
        {
            await reproductiveHealthService.UpdateVisitAsync(id, request, ct);
            return NoContent();
        }

        [HttpDelete("visits/{id}")]
        [Authorize(Roles = Roles.StaffRoles)]
        public async Task<IActionResult> DeleteVisit(int id, CancellationToken ct)
        {
            await reproductiveHealthService.DeleteVisitAsync(id, ct);
            return NoContent();
        }

        [HttpGet("statistics")]
        [Authorize(Roles = Roles.StaffRoles)]
        public async Task<IActionResult> GetStatistics(CancellationToken ct)
        {
            var result = await reproductiveHealthService.GetStatisticsAsync(ct);
            return Ok(result);
        }
    }
}
