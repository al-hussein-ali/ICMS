using ICMS.Application.DTOs.ImmunizationRecord;
using ICMS.Application.DTOs.Pagination;
using ICMS.Application.Interfaces.Services;
using ICMS.Domain.Constants;
using ICMS.Domain.ValueObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using ICMS.API.Extensions;

namespace ICMS.API.Controllers
{
    [Route("api/immunization-records")]
    [ApiController]
    [Authorize(Roles = Roles.Admin + "," + Roles.VaccinationManager)]
    public class ImmunizationRecordsController(IImmunizationRecordService immunizationRecordService) : ControllerBase
    {

        [HttpGet()]
        public async Task<ActionResult<PagedResult<ImmunizationRecordReadDto>>> GetAllAsync([FromQuery] PaginationParams paginationParams)
        {
            var immnuizationRecords = await immunizationRecordService.GetAllAsync(paginationParams);

            return Ok(immnuizationRecords);
        }

        [HttpGet("{id}", Name = "GetImmunizationRecordById")]
        public async Task<ActionResult<ImmunizationRecordReadDto>> GetByIdAsync([FromRoute] Guid id)
        {
            var immunizationRecord = await immunizationRecordService.GetByIdAsync(id);

            return Ok(immunizationRecord);
        }

        [HttpPost("create")]
        public async Task<IActionResult> AddAsync([FromBody] ImmunizationRecordCreateDto dto)
        {
            var userId = ClaimsPrincipalExtensions.GetUserId(User);
            var record = await immunizationRecordService.AddAsync(dto, userId);

            return CreatedAtRoute("GetImmunizationRecordById", new { id = record.Id }, record);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] ImmunizationRecordCreateDto dto)
        {
            await immunizationRecordService.UpdateAsync(id, dto);

            return NoContent();
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
        {
            await immunizationRecordService.DeleteAsync(id);

            return NoContent();
        }

    }
}
