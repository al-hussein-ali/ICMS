using ICMS.Application.DTOs.FieldVisit;
using ICMS.Application.DTOs.Pagination;
using ICMS.Application.Interfaces.Services;
using ICMS.Domain.Constants;
using ICMS.Domain.ValueObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ICMS.Api.Controllers
{
    [Route("api/field-visits")]
    [ApiController]
    [Authorize(Roles = Roles.StaffRoles)]
    public class FieldVisitsController(IFieldVisitService fieldVisitService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<PagedResult<FieldVisitReadDto>>> GetAllAsync(
            [FromQuery] PaginationParams paginationParams, CancellationToken ct)
        {
            var fieldVisits = await fieldVisitService.GetAllAsync(paginationParams, ct);
            return Ok(fieldVisits);
        }

        [HttpGet("{id}", Name = "GetFieldVisitById")]
        public async Task<ActionResult<FieldVisitDetailsDto>> GetByIdAsync(
            [FromRoute] int id, CancellationToken ct)
        {
            var fieldVisit = await fieldVisitService.GetByIdAsync(id, ct);
            return Ok(fieldVisit);
        }

        [HttpPost("create")]
        public async Task<IActionResult> AddAsync(
            [FromBody] FieldVisitCreateDto dto, CancellationToken ct)
        {
            var created = await fieldVisitService.AddAsync(dto, ct);
            return CreatedAtRoute("GetFieldVisitById", new { id = created.Id }, created);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateAsync(
            [FromRoute] int id, [FromBody] FieldVisitCreateDto dto, CancellationToken ct)
        {
            await fieldVisitService.UpdateAsync(id, dto, ct);
            return NoContent();
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteAsync(
            [FromRoute] int id, CancellationToken ct)
        {
            await fieldVisitService.DeleteAsync(id, ct);
            return NoContent();
        }

        [HttpPost("{id}/complete")]
        public async Task<IActionResult> MarkCompletedAsync(
            [FromRoute] int id, CancellationToken ct)
        {
            await fieldVisitService.MarkCompletedAsync(id, ct);
            return NoContent();
        }
    }
}
